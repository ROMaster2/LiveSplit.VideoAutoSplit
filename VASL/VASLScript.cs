using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using LiveSplit.Model;
using LiveSplit.VAS.Models.Delta;

namespace LiveSplit.VAS.VASL
{
    public class VASLScript
    {
        private string GameVersion;

        private readonly bool UsesGameTime;
        private readonly bool UsesCustomGameTime;
        private readonly bool UsesIsLoading;
        private bool InitCompleted;

        private VASLSettings Settings;

        private ITimerModel Timer;
        private TimerPhase PreviousPhase;
        private TimeSpan LastSplitOffset;

        private MethodList Methods;


        public ExpandoObject Vars { get; }

        public event EventHandler<DeltaOutput> ScriptUpdateFinished;

        public VASLScript(string rawScript, string gameVersion)
        {
            Methods = ParseScript(rawScript);

            GameVersion = gameVersion;

            Settings = new VASLSettings();
            Vars = new ExpandoObject();

            if (!Methods.start.IsEmpty)
                Settings.AddBasicSetting("start");
            if (!Methods.split.IsEmpty)
                Settings.AddBasicSetting("split");
            if (!Methods.reset.IsEmpty)
                Settings.AddBasicSetting("reset");

            UsesCustomGameTime = !Methods.gameTime.IsEmpty;
            UsesIsLoading = !Methods.isLoading.IsEmpty;
            UsesGameTime = UsesCustomGameTime || UsesIsLoading;
        }

        private void Timer_OnUndoSplit(object sender, EventArgs e)
        {
            if (PreviousPhase == TimerPhase.Ended)
            {
                var state = ((ITimerModel)sender).CurrentState;
                AdjustGameTime(state, LastSplitOffset);
                LastSplitOffset = TimeSpan.Zero;
            }
        }

        private void AdjustGameTime(LiveSplitState state, TimeSpan offsetTime)
        {
            state.SetGameTime(state.CurrentTime.GameTime + offsetTime);
        }

        // Update the script
        public void Update(LiveSplitState state, DeltaOutput d)
        {
            if (Timer == null)
            {
                Timer = new TimerModel() { CurrentState = state };
                PreviousPhase = state.CurrentPhase;
                Timer.OnUndoSplit += Timer_OnUndoSplit;
            }

            if (!InitCompleted)
            {
                DoInit(state, d);
            }
            else
            {
                DoUpdate(state, d);
            }
            ScriptUpdateFinished?.Invoke(this, d);
        }

        public VASLSettings RunStartup(LiveSplitState state)
        {
            Debug("Running startup");
            RunNoProcessMethod(Methods.startup, state, true);
            return Settings;
        }

        private void DoInit(LiveSplitState state, DeltaOutput d)
        {
            Debug("Initializing");

            RunMethod(Methods.init, state, d);

            InitCompleted = true;
            Debug("Init completed, running main methods");
        }

        private void DoUpdate(LiveSplitState state, DeltaOutput d)
        {
            var updateState = RunMethod(Methods.update, state, d);

            // If Update explicitly returns false, don't run anything else
            if (updateState is bool && updateState == false)
                return;

            var offsetTime = TimeStamp.CurrentDateTime.Time - d.History[d.FrameIndex].FrameEnd;

            // Enable regardless to keep track of offseting for now.
            if (!state.IsGameTimeInitialized)
                Timer.InitializeGameTime();

            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                if (UsesGameTime)
                {
                    if (UsesIsLoading)
                    {
                        var isPausedState = RunMethod(Methods.isLoading, state, d);

                        if (isPausedState is bool)
                        {
                            var prevPauseState = state.IsGameTimePaused;

                            state.IsGameTimePaused = isPausedState;

                            if (prevPauseState != isPausedState)
                            {
                                if (isPausedState)
                                    state.GameTimePauseTime -= offsetTime;
                                else
                                    state.GameTimePauseTime += offsetTime;
                            }
                        }
                    }

                    if (UsesGameTime)
                    {
                        var gameTimeState = RunMethod(Methods.gameTime, state, d);

                        if (gameTimeState is TimeSpan)
                            state.SetGameTime(gameTimeState);
                    }
                }

                if (Settings.GetBasicSettingValue("reset"))
                {
                    var resetState = RunMethod(Methods.reset, state, d);

                    if (resetState is bool && resetState == true)
                        Timer.Reset();
                }

                if (Settings.GetBasicSettingValue("split"))
                {
                    var splitState = RunMethod(Methods.split, state, d);

                    if (splitState is bool && splitState == true)
                    {
                        if (!UsesCustomGameTime)
                            AdjustGameTime(state, offsetTime.Negate());

                        Timer.Split();

                        if (!UsesCustomGameTime)
                        {
                            if (state.CurrentPhase != TimerPhase.Ended)
                                AdjustGameTime(state, offsetTime);
                            else
                                LastSplitOffset = offsetTime;
                        }
                    }
                }
                // @TODO: Add undo and skip;
            }
            else if (state.CurrentPhase == TimerPhase.NotRunning && Settings.GetBasicSettingValue("start"))
            {
                var startState = RunMethod(Methods.start, state, d);

                if ((startState is bool && startState == true) || startState is TimeSpan)
                {
                    Timer.Start();

                    if (!state.IsGameTimeInitialized)
                        Timer.InitializeGameTime();

                    LastSplitOffset = TimeSpan.Zero;

                    TimeSpan startOffset = (startState is TimeSpan) ? startState : TimeSpan.Zero;

                    if (!UsesCustomGameTime)
                        AdjustGameTime(state, startOffset + offsetTime);
                }
            }

            PreviousPhase = state.CurrentPhase;
        }

        private void DoExit(LiveSplitState state)
        {
            Debug("Running exit");
            RunNoProcessMethod(Methods.exit, state);
        }

        public void RunShutdown(LiveSplitState state)
        {
            Debug("Running shutdown");
            RunMethod(Methods.shutdown, state, new DeltaOutput());
        }

        private dynamic RunMethod(VASLMethod method, LiveSplitState state, DeltaOutput d)
        {
            var result = method.Call(state, Vars, GameVersion, Settings.Reader, d);
            return result;
        }

        // Run method without counting on being connected to the game (startup/shutdown).
        private void RunNoProcessMethod(VASLMethod method, LiveSplitState state, bool isStartup = false)
        {
            method.Call(state, Vars, GameVersion, isStartup ? Settings.Builder : (object)Settings.Reader, new DeltaOutput());
        }

        private void Debug(string output, params object[] args)
        {
            Log.Info(String.Format("[VASL/{1}] {0}",
                String.Format(output, args),
                this.GetHashCode()));
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
                    errorMsg.Append($"\nat Line {loc.Line + 1}, Col {loc.Column + 1}: {msg.Message}");
                }

                throw new Exception(errorMsg.ToString());
            }

            var methodsNode = tree.Root.ChildNodes.First(x => x.Term.Name == "methodList");

            // Todo: Aliasing

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
            private static VASLMethod noOp = new VASLMethod("");

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
