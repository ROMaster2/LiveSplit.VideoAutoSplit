using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.VAS.Models;

namespace LiveSplit.VAS.VASL
{
    public class VASLScript
    {
        public class Methods : IEnumerable<VASLMethod>
        {
            private static VASLMethod no_op = new VASLMethod("");

            public VASLMethod startup = no_op;
            public VASLMethod shutdown = no_op;
            public VASLMethod init = no_op;
            public VASLMethod exit = no_op;
            public VASLMethod update = no_op;
            public VASLMethod start = no_op;
            public VASLMethod split = no_op;
            public VASLMethod reset = no_op;
            public VASLMethod isLoading = no_op;
            public VASLMethod gameTime = no_op;

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
                    gameTime
                };
            }

            public IEnumerator<VASLMethod> GetEnumerator() => ((IEnumerable<VASLMethod>)GetMethods()).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetMethods().GetEnumerator();
        }

        public event EventHandler<double> RefreshRateChanged;
        public event EventHandler<string> GameVersionChanged;

        private string _game_version = string.Empty;
        public string GameVersion
        {
            get { return _game_version; }
            set
            {
                if (value != _game_version)
                    GameVersionChanged?.Invoke(this, value);
                _game_version = value;
            }
        }

        private double _refresh_rate = 1000 / 15d;
        public double RefreshRate // per sec
        {
            get { return _refresh_rate; }
            set
            {
                if (Math.Abs(value - _refresh_rate) > 0.01)
                    RefreshRateChanged?.Invoke(this, value);
                _refresh_rate = value;
            }
        }

        // public so other components (VASLVarViewer) can access
        public VASLState State { get; private set; }
        public VASLState OldState { get; private set; }
        public ExpandoObject Vars { get; }

        private bool _uses_game_time;
        private bool _init_completed;

        private VASLSettings _settings;

        private TimerModel _timer;

        private Dictionary<string, List<VASLState>> _states;

        private Methods _methods;

        public VASLScript(Methods methods)
        {
            _methods = methods;

            _settings = new VASLSettings();
            Vars = new ExpandoObject();

            if (!_methods.start.IsEmpty)
                _settings.AddBasicSetting("start");
            if (!_methods.split.IsEmpty)
                _settings.AddBasicSetting("split");
            if (!_methods.reset.IsEmpty)
                _settings.AddBasicSetting("reset");

            _uses_game_time = !_methods.isLoading.IsEmpty || !_methods.gameTime.IsEmpty;
        }

        // Update the script
        public void Update(LiveSplitState state, DeltaManager dm)
        {
            if (!Scanner.IsVideoSourceRunning())
            {
                //TryConnect(state);
                //DoExit(state); // This'll probably change
            }
            else
            {
                if (_timer == null)
                    _timer = new TimerModel() { CurrentState = state };
                if (!_init_completed)
                    DoInit(state, dm);
                else
                    DoUpdate(state, dm);
            }
        }

        // Run startup and return settings defined in VASL script.
        public VASLSettings RunStartup(LiveSplitState state)
        {
            Debug("Running startup");
            RunNoProcessMethod(_methods.startup, state, true);                                                         
            return _settings;
        }

        public void RunShutdown(LiveSplitState state)
        {
            Debug("Running shutdown");
            RunMethod(_methods.shutdown, state, null);
        }


        // This is executed each time after connecting to the game (usually just once,
        // unless an error occurs before the method finishes).
        private void DoInit(LiveSplitState state, DeltaManager dm)
        {
            Debug("Initializing");

            GameVersion = string.Empty;

            // Fetch version from init-method
            var ver = string.Empty;
            RunMethod(_methods.init, state, dm, ref ver);

            _init_completed = true;
            Debug("Init completed, running main methods");
        }

        private void DoExit(LiveSplitState state)
        {
            Debug("Running exit");
            RunNoProcessMethod(_methods.exit, state);
        }

        // This is executed repeatedly as long as the game is connected and initialized.
        private void DoUpdate(LiveSplitState state, DeltaManager dm)
        {
            if (!(RunMethod(_methods.update, state, dm) ?? true))
            {
                // If Update explicitly returns false, don't run anything else
                return;
            }

            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                if (_uses_game_time && !state.IsGameTimeInitialized)
                    _timer.InitializeGameTime();

                var is_paused = RunMethod(_methods.isLoading, state, dm);
                if (is_paused != null)
                    state.IsGameTimePaused = is_paused;

                var game_time = RunMethod(_methods.gameTime, state, dm);
                if (game_time != null)
                    state.SetGameTime(game_time);

                if (RunMethod(_methods.reset, state, dm) ?? false)
                {
                    if (_settings.GetBasicSettingValue("reset"))
                        _timer.Reset();
                }
                else if (RunMethod(_methods.split, state, dm) ?? false)
                {
                    if (_settings.GetBasicSettingValue("split"))
                        _timer.Split();
                }
            }

            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                if (RunMethod(_methods.start, state, dm) ?? false)
                {
                    if (_settings.GetBasicSettingValue("start"))
                        _timer.Start();
                }
            }
        }

        private dynamic RunMethod(VASLMethod method, LiveSplitState state, DeltaManager dm, ref string version)
        {
            var refresh_rate = RefreshRate;
            var result = method.Call(state, Vars, ref version, ref refresh_rate, _settings.Reader,
                OldState?.Data, State?.Data, dm);
            RefreshRate = refresh_rate;
            return result;
        }

        private dynamic RunMethod(VASLMethod method, LiveSplitState state, DeltaManager dm)
        {
            var version = GameVersion;
            return RunMethod(method, state, dm, ref version);
        }

        // Run method without counting on being connected to the game (startup/shutdown).
        private void RunNoProcessMethod(VASLMethod method, LiveSplitState state, bool is_startup = false)
        {
            var refresh_rate = RefreshRate;
            var version = GameVersion;
            method.Call(state, Vars, ref version, ref refresh_rate,
                is_startup ? _settings.Builder : (object)_settings.Reader);
            RefreshRate = refresh_rate;
        }

        private void Debug(string output, params object[] args)
        {
            Log.Info(String.Format("[VASL/{1}] {0}",
                String.Format(output, args),
                this.GetHashCode()));
        }
    }
}
