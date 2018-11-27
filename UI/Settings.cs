using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.VAS.UI
{
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }
    }

    /// <summary>
    /// TreeView with fixed double-clicking on checkboxes.
    /// </summary>
    ///
    /// See also:
    /// http://stackoverflow.com/questions/17356976/treeview-with-checkboxes-not-processing-clicks-correctly
    /// http://stackoverflow.com/questions/14647216/c-sharp-treeview-ignore-double-click-only-at-checkbox
    class NewTreeView : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x203) // identified double click
            {
                var local_pos = PointToClient(Cursor.Position);
                var hit_test_info = HitTest(local_pos);

                if (hit_test_info.Location == TreeViewHitTestLocations.StateImage)
                {
                    m.Msg = 0x201; // if checkbox was clicked, turn into single click
                }

                base.WndProc(ref m);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
