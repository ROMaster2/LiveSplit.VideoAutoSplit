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
using ImageMagick;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.VAS;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.UI;
using LiveSplit.VAS.VASL;

namespace LiveSplit.UI.Components
{
    public partial class ScanRegion : UserControl
    {
        private const double DEFAULT_MOVE_DISTANCE = 1;
        private const double ALT_DISTANCE_MODIFIER = 10;
        private const double SHIFT_DISTANCE_MODIFIER = 10;
        private const double CONTROL_DISTANCE_MODIFIER = 0.1;

        private const Gravity STANDARD_GRAVITY = Gravity.Northwest;
        private const FilterType DEFAULT_SCALE_FILTER = FilterType.Lanczos;
        private static readonly MagickColor EXTENT_COLOR = MagickColor.FromRgba(255, 0, 255, 127);

        public Geometry CropGeometry { get { return Scanner.CropGeometry; } set { Scanner.CropGeometry = value; } }
        public static Geometry VideoGeometry { get { return Scanner.VideoGeometry; } }

        private static Geometry MIN_VALUES
        {
            get
            {
                var geo = VideoGeometry;
                if (geo.IsBlank)
                    return new Geometry(-8192, -8192, 4, 4);
                else
                    return new Geometry(-VideoGeometry.Width, -VideoGeometry.Height, 4, 4);
            }
        }
        private static Geometry MAX_VALUES
        {
            get
            {
                var geo = VideoGeometry;
                if (geo.IsBlank)
                    return new Geometry(8192, 8192, 8192, 8192);
                else
                    return new Geometry(VideoGeometry.Width, VideoGeometry.Height, VideoGeometry.Width * 2, VideoGeometry.Height * 2);
            }
        }

        public ScanRegion()
        {
            InitializeComponent();
        }

        public void Rerender()
        {
            SetAllNumValues(CropGeometry);
        }

        private void SetAllNumValues(Geometry geo)
        {
            numX.Value      = (decimal)geo.X;
            numY.Value      = (decimal)geo.Y;
            numWidth.Value  = (decimal)geo.Width;
            numHeight.Value = (decimal)geo.Height;
            UpdateCropGeometry(geo);
        }

        private void UpdateCropGeometry(Geometry? geo = null)
        {
            Geometry newGeo = geo ?? Geometry.Blank;
            if (geo == null)
            {
                newGeo.X      = (double)numX.Value;
                newGeo.Y      = (double)numY.Value;
                newGeo.Width  = (double)numWidth.Value;
                newGeo.Height = (double)numHeight.Value;
            }
            CropGeometry = newGeo.Min(MAX_VALUES).Max(MIN_VALUES);
            //RefreshThumbnail();
        }


    }
}
