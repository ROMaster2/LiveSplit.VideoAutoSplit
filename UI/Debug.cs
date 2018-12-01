using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public partial class DebugUI : UserControl
    {
        private readonly VASComponent ParentComponent;

        public DebugUI(VASComponent parentComponent)
        {
            InitializeComponent();

            ParentComponent = parentComponent;
        }
    }
}
