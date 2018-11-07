using System;
using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(Factory))]

namespace LiveSplit.UI.Components
{
    public class Factory : IComponentFactory
    {
        public string ComponentName => "Video Feature Monitor";
        public string Description =>
            "Monitors a video feed for features and returns a comparison rate. Best used with Scriptable Auto Splitter.";
        public ComponentCategory Category => ComponentCategory.Control;
        public Version Version => Version.Parse("1.7.5");

        public string UpdateName => ComponentName;
        public string UpdateURL => "http://livesplit.org/update/";
        public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.VideoFeatureMonitor.xml";

        public IComponent Create(LiveSplitState state) => new VFMComponent(state);
        public IComponent Create(LiveSplitState state, string script) => new VFMComponent(state, script);
    }
}
