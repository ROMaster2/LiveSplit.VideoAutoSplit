using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Accord.Video.DirectShow;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.UI;
using LiveSplit.VAS.VASL;

namespace LiveSplit.UI.Components
{
    public partial class ScanRegion : UserControl
    {
        public Geometry CropGeometry;

        public ScanRegion()
        {
            InitializeComponent();
        }
    }
}
