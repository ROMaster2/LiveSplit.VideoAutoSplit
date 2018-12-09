﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.VAS.Models.Delta;

namespace LiveSplit.VAS.VASL
{
    public class VASLScript
    {
        public ExpandoObject Vars { get; private set; }

        private readonly string _GameVersion = "";
        private readonly bool _UsesGameTime;
        private bool _InitCompleted;

        private readonly VASLSettings _Settings;
        private ITimerModel _Timer;
        private readonly MethodList _Methods;

        public event EventHandler<DeltaOutput> ScriptUpdateFinished;

        public VASLScript(string rawScript)
        {
            _Methods = ParseScript(rawScript);

            _Settings = new VASLSettings();
            Vars = new ExpandoObject();

            if (!_Methods.start.IsEmpty) _Settings.AddBasicSetting("start");
            if (!_Methods.split.IsEmpty) _Settings.AddBasicSetting("split");
            if (!_Methods.reset.IsEmpty) _Settings.AddBasicSetting("reset");

            _UsesGameTime = !_Methods.isLoading.IsEmpty || !_Methods.gameTime.IsEmpty;
        }

        // @TODO: Implement the functionality instead of throwing new NotImplementedException
        public void UpdateGameVersion(string gameVersion)
        {
            throw new NotImplementedException("todo lol");
        }

        // Update the script
        public void Update(LiveSplitState state, DeltaOutput d)
        {
            if (_Timer == null) _Timer = new TimerModel() { CurrentState = state };

            if (!_InitCompleted) DoInit(state, d);
            else DoUpdate(state, d);

            ScriptUpdateFinished?.Invoke(this, d);
        }

        // Run startup and return settings defined in VASL script.
        public VASLSettings RunStartup(LiveSplitState state)
        {
            Debug("Running startup");
            RunNoProcessMethod(_Methods.startup, state, true);
            return _Settings;
        }

        public void RunShutdown(LiveSplitState state)
        {
            Debug("Running shutdown");
            RunMethod(_Methods.shutdown, state, null);
        }

        // This is executed each time after connecting to the game (usually just once,
        // unless an error occurs before the method finishes).
        private void DoInit(LiveSplitState state, DeltaOutput d)
        {
            Debug("Initializing");
            RunMethod(_Methods.init, state, d);

            _InitCompleted = true;
            Debug("Init completed, running main methods");
        }

        private void DoExit(LiveSplitState state)
        {
            Debug("Running exit");
            RunNoProcessMethod(_Methods.exit, state);
        }

        // This is executed repeatedly as long as the game is connected and initialized.
        private void DoUpdate(LiveSplitState state, DeltaOutput d)
        {
            if (!(RunMethod(_Methods.update, state, d) ?? true))
            {
                // If Update explicitly returns false, don't run anything else
                return;
            }

            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                if (_UsesGameTime)
                {
                    if (!state.IsGameTimeInitialized) _Timer.InitializeGameTime();

                    var isPaused = RunMethod(_Methods.isLoading, state, d);

                    if (isPaused != null)
                    {
                        var prevPauseState = state.IsGameTimePaused;

                        state.IsGameTimePaused = isPaused;

                        if (prevPauseState != isPaused)
                        {
                            var offsetTime = d.History[d.FrameIndex].ProcessDuration;

                            if (isPaused) state.GameTimePauseTime -= offsetTime;
                            else state.GameTimePauseTime += offsetTime;
                        }
                    }

                    var gameTime = RunMethod(_Methods.gameTime, state, d);
                    if (gameTime != null) state.SetGameTime(gameTime);
                }

                if (RunMethod(_Methods.reset, state, d) ?? false && _Settings.GetBasicSettingValue("reset"))
                {
                    _Timer.Reset();
                }
                else if (_Settings.GetBasicSettingValue("split"))
                {
                    var offset = RunMethod(_Methods.split, state, d);

                    // The below will be added once LiveSplit supports offseting
                    // Could add it now, but that would require a version update,
                    // which I'm avoiding for now for user simplicity.
                    /*
                    if (offset != null)
                    {
                        if (offset is TimeSpan)
                            Timer.Split(offset);
                        else if (offset is bool && offset == true)
                            if (dm.SplitIndex != null)
                            {
                                var now = Timer.CurrentState.CurrentTime.RealTime.Value;
                                var diff = d.History[dm.FrameIndex].FrameEnd.Ticks - d.SplitTime.Value.Ticks;
                                var test = now - diff;
                                Timer.Split(d.SplitTime - Timer.CurrentState.CurrentTime.RealTime);
                            }
                            else
                                Timer.Split(); // Legacy
                    }
                    */

                    if (offset)
                    {
                        _Timer.Split();
                    }

                    // @TODO: Add undoSplit;
                }
            }
            else if (state.CurrentPhase == TimerPhase.NotRunning
                && ((RunMethod(_Methods.start, state, d) ?? false)
                && _Settings.GetBasicSettingValue("start")))
            {
                _Timer.Start();
            }
        }

        private dynamic RunMethod(VASLMethod method, LiveSplitState state, DeltaOutput d)
        {
            return method.Call(state, Vars, _GameVersion, _Settings.Reader, d);
        }

        // Run method without counting on being connected to the game (startup/shutdown).
        private void RunNoProcessMethod(VASLMethod method, LiveSplitState state, bool isStartup = false)
        {
            method.Call(state, Vars, _GameVersion, isStartup ? _Settings.Builder : (object)_Settings.Reader, null);
        }

        private void Debug(string output, params object[] args)
        {
            Log.Info(String.Format("[VASL/{1}] {0}",
                String.Format(output, args),
                GetHashCode()));
        }

        public static MethodList ParseScript(string code)
        {
            var grammar = new VASLGrammar();
            var parser = new Parser(grammar);
            var tree = parser.Parse(code);

            if (tree.HasErrors())
            {
                var errorMsg = new StringBuilder("VASL parse error(s):");
                foreach (var msg in parser.Context.CurrentParseTree.ParserMessages)
                {
                    var loc = msg.Location;
                    errorMsg.Append("\nat Line ").Append(loc.Line + 1).Append(", Col ").Append(loc.Column + 1).Append(": ").Append(msg.Message);
                }

                throw new Exception(errorMsg.ToString());
            }

            var methodsNode = tree.Root.ChildNodes.First(x => x.Term.Name == "methodList");

            // @TODO: Aliasing

            var methods = new MethodList();

            foreach (var method in methodsNode.ChildNodes[0].ChildNodes)
            {
                var body = (string)method.ChildNodes[2].Token.Value;
                var methodName = (string)method.ChildNodes[0].Token.Value;
                var line = method.ChildNodes[2].Token.Location.Line + 1;
                var script = new VASLMethod(body, methodName, line)
                {
                    ScriptMethods = methods
                };
                switch (methodName)
                {
                    case "init": methods.init = script; break;
                    case "exit": methods.exit = script; break;
                    case "update": methods.update = script; break;
                    case "start": methods.start = script; break;
                    case "split": methods.split = script; break;
                    case "isLoading": methods.isLoading = script; break;
                    case "gameTime": methods.gameTime = script; break;
                    case "reset": methods.reset = script; break;
                    case "startup": methods.startup = script; break;
                    case "shutdown": methods.shutdown = script; break;
                    case "undoSplit": methods.undoSplit = script; break;
                }
            }

            return methods;
        }

        public class MethodList : IEnumerable<VASLMethod>
        {
            private static readonly VASLMethod noOp = new VASLMethod("");

            public VASLMethod startup = noOp;
            public VASLMethod shutdown = noOp;
            public VASLMethod init = noOp;
            public VASLMethod exit = noOp;
            public VASLMethod update = noOp;
            public VASLMethod start = noOp;
            public VASLMethod split = noOp;
            public VASLMethod reset = noOp;
            public VASLMethod isLoading = noOp;
            public VASLMethod gameTime = noOp;
            public VASLMethod undoSplit = noOp;

            public VASLMethod[] GetMethods()
            {
                return new VASLMethod[]
                {
                    startup,
                    shutdown,
                    init,
                    exit,
                    update,
                    start,
                    split,
                    reset,
                    isLoading,
                    gameTime,
                    undoSplit
                };
            }

            public IEnumerator<VASLMethod> GetEnumerator() => ((IEnumerable<VASLMethod>)GetMethods()).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetMethods().GetEnumerator();
        }
    }
}
