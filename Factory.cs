using System;
using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(Factory))]

namespace LiveSplit.UI.Components
{
    public class Factory : IComponentFactory
    {
        public string ComponentName => "Video Auto Splitter";
        public string Description => "Allows scripting of splitting behavior based on events from a video feed.";
        public ComponentCategory Category => ComponentCategory.Control;
        public Version Version => Version.Parse("0.1.0");

        public string UpdateName => ComponentName;
        public string UpdateURL => "http://livesplit.org/update/";
        public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.VideoAutoSplitter.xml";

        public IComponent Create(LiveSplitState state) => new VASComponent(state);
    }
}
