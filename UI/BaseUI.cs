using System.Windows.Forms;
using LiveSplit.VAS.VASL;

namespace LiveSplit.UI.Components
{
    public abstract class AbstractUI : UserControl
    {
        private readonly VASComponent ParentComponent;
        public TabPage PageParent => (TabPage)Parent;
        public TabControl TabParent => (TabControl)PageParent.Parent;
        abstract public void Rerender();
        abstract public void Derender();
        abstract internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded);
    }
}
