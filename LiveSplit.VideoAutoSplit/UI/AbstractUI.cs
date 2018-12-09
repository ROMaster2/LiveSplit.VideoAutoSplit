﻿using System.Windows.Forms;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    // if DEBUG is included for the WinForms Designer.
#if DEBUG
    public class AbstractUI : UserControl
#else
    public abstract class AbstractUI : UserControl
#endif
    {
        internal VASComponent Component { get; }

        public TabPage PageParent => (TabPage)Parent;
        public TabControl TabParent => (TabControl)Parent.Parent;

        public AbstractUI(VASComponent component) : base()
        {
            Component = component;
        }

#if DEBUG
        public AbstractUI() : base() { }

        virtual public void Rerender() { }
        virtual public void Derender() { }
        virtual internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded) { }
#else
        abstract public void Rerender();
        abstract public void Derender();
        abstract internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded);
#endif
    }
}
