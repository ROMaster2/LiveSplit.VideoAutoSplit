using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math;
using Accord.Video;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveSplit.VAS;
using LiveSplit.VAS.Models;


using Screen = LiveSplit.VAS.Models.Screen;

namespace LiveSplit.VAS.UI
{
    public partial class Aligner : Form
    {
        private const double DEFAULT_MOVE_DISTANCE = 1;
        private const double ALT_DISTANCE_MODIFIER = 10;
        private const double SHIFT_DISTANCE_MODIFIER = 10;
        private const double CONTROL_DISTANCE_MODIFIER = 0.1;

        private const Gravity STANDARD_GRAVITY = Gravity.Northwest;
        private const FilterType DEFAULT_SCALE_FILTER = FilterType.Lanczos;
        private static readonly MagickColor EXTENT_COLOR = MagickColor.FromRgba(255, 0, 255, 127);

        private static readonly Geometry MIN_VALUES = new Geometry(-Scanner.VideoGeometry.Width, -Scanner.VideoGeometry.Height, 4, 4);
        private static readonly Geometry MAX_VALUES = new Geometry(
            Scanner.VideoGeometry.Width,
            Scanner.VideoGeometry.Height,
            Scanner.VideoGeometry.Width * 2,
            Scanner.VideoGeometry.Height * 2);

        private static Bitmap CurrentFrame = new Bitmap(1, 1);
        private static DateTime LastUpdate = DateTime.UtcNow;
        /*
        public double CropX { get; set; }      = Scanner.CropGeometry.X;
        public double CropY { get; set; }      = Scanner.CropGeometry.Y;
        public double CropWidth { get; set; }  = Scanner.CropGeometry.Width;
        public double CropHeight { get; set; } = Scanner.CropGeometry.Height;
        */
        public Aligner()
        {
            InitializeComponent();
            SetAllNumValues(Scanner.CropGeometry);
            FillDdlWatchZone();
            Scanner.SubscribeToFrameHandler(HandleNewFrame);
            RefreshThumbnail();
        }

        private void Aligner_FormClosing(object sender, FormClosingEventArgs e)
        {
            Scanner.UnsubscribeFromFrameHandler(HandleNewFrame);/*
            CropX      = Scanner.CropGeometry.X;
            CropY      = Scanner.CropGeometry.Y;
            CropWidth  = Scanner.CropGeometry.Width;
            CropHeight = Scanner.CropGeometry.Height;*/
        }

        private void Aligner_ResizeEnd(object sender, EventArgs e)
        {
            if (ThumbnailBox.Image != null)
            {
                RefreshThumbnail();
            }
        }

        // If the window is mazimized or restored, do thing.
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0112 && (m.WParam == new IntPtr(0xF030) || m.WParam == new IntPtr(0xF120)))
            {
                RefreshThumbnail();
            }
        }

        private Geometry GetScaledGeometry(Geometry refGeo)
        {
            var referenceWidth = refGeo.IsBlank ? Scanner.CropGeometry.Width : refGeo.Width;
            var referenceHeight = refGeo.IsBlank ? Scanner.CropGeometry.Height : refGeo.Height;

            // Tried to not have hardcoded, but Microsoft diagreed.
            var parent = (TableLayoutPanel)ThumbnailBox.Parent;
            var parentWidth = parent.Width - 200;
            var parentHeight = parent.Height;
            var xMargin = ThumbnailBox.Margin.Left + ThumbnailBox.Margin.Right;
            var yMargin = ThumbnailBox.Margin.Top + ThumbnailBox.Margin.Bottom;
            var xRatio = (parentWidth - xMargin) / referenceWidth;
            var yRatio = (parentHeight - yMargin) / referenceHeight;

            var ratio = Math.Min(Math.Min(1, xRatio), yRatio);
            var width = Math.Max(referenceWidth * ratio, 1);
            var height = Math.Max(referenceHeight * ratio, 1);
            return new Geometry(width, height);
        }

        private void HandleNewFrame(object sender, NewFrameEventArgs e)
        {
            // Heisenbug
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    if (DateTime.UtcNow.Subtract(LastUpdate) > TimeSpan.FromSeconds(1))
                    {
                        LastUpdate = DateTime.UtcNow;
                        CurrentFrame = (Bitmap)e.Frame.Clone();
                        RefreshThumbnail();
                    }
                });
            }
            catch (Exception) { }
        }

        private void RefreshThumbnail()
        {
            Geometry minGeo = Geometry.Min(Scanner.CropGeometry, GetScaledGeometry(Geometry.Blank));

            MagickImage mi = new MagickImage(CurrentFrame);

            if (!Scanner.VideoGeometry.Contains(Scanner.CropGeometry))
            {
                mi.Extent(Scanner.CropGeometry.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);
            }
            else
            {
                mi.Crop(Scanner.CropGeometry.ToMagick(), STANDARD_GRAVITY);
            }
            mi.RePage();

            if (DdlWatchZone.SelectedIndex > 0)
            {
                var wi = (WatchImage)DdlWatchZone.SelectedItem;
                var tGeo = wi.WatchZone.CropGeometry;
                tGeo.Update(-Scanner.CropGeometry.X, -Scanner.CropGeometry.Y);

                var baseMGeo = new MagickGeometry(100, 100, (int)Math.Round(tGeo.Width), (int)Math.Round(tGeo.Height));

                tGeo.Update(-100, -100, 200, 200);
                mi.Extent(tGeo.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);

                using (var baseM = new MagickImage(
                    MagickColor.FromRgba(0, 0, 0, 0),
                    baseMGeo.Width,
                    baseMGeo.Height))
                using (var overlay = new MagickImage(
                        MagickColor.FromRgba(170, 170, 170, 223),
                        baseMGeo.Width + 200,
                        baseMGeo.Height + 200))
                {
                    baseM.ColorSpace = ColorSpace.RGB;
                    overlay.ColorSpace = ColorSpace.RGB;
                    overlay.Composite(baseM, new PointD(baseMGeo.X, baseMGeo.Y), CompositeOperator.Alpha);
                    mi.Composite(overlay, CompositeOperator.Atop);
                }
                mi.RePage();

                minGeo = minGeo.Min(GetScaledGeometry(tGeo));

                if (CkbViewDelta.Checked)
                {
                    using (var deltaImage = wi.MagickImage.Clone())
                    {
                        mi.ColorSpace = wi.Watcher.ColorSpace;
                        deltaImage.ColorSpace = wi.Watcher.ColorSpace;
                        mi.Crop(baseMGeo, STANDARD_GRAVITY);
                        //mi.Write(@"E:\fuck0.png");
                        //deltaImage.Write(@"E:\fuck1.png");
                        mi.Alpha(AlphaOption.Off); // Why is this necessary? It wasn't necessary before.
                        //mi.Write(@"E:\fuck2.png");
                        if (wi.Watcher.Equalize)
                        {
                            deltaImage.Equalize();
                            mi.Equalize();
                        }
                        deltaImage.RePage();
                        mi.RePage();
                        //mi.Write(@"E:\fuck3.png");
                        //deltaImage.Write(@"E:\fuck4.png");
                        LblDeltas.Text =
                            mi.Compare(deltaImage, ErrorMetric.PeakSignalToNoiseRatio).ToString("0.####") + "\r\n" +
                            mi.Compare(deltaImage, ErrorMetric.NormalizedCrossCorrelation).ToString("0.####") + "\r\n" +
                            mi.Compare(deltaImage, ErrorMetric.Absolute).ToString("0.####") + "\r\n" +
                            mi.Compare(deltaImage, ErrorMetric.Fuzz).ToString("0.####") + "\r\n" +
                            mi.Compare(deltaImage, ErrorMetric.MeanAbsolute).ToString("0.####") + "\r\n" +
                            mi.Compare(deltaImage, ErrorMetric.MeanSquared).ToString("0.####") + "\r\n" +
                            mi.Compare(deltaImage, ErrorMetric.StructuralDissimilarity).ToString("0.####") + "\r\n" +
                            mi.Compare(deltaImage, ErrorMetric.StructuralSimilarity).ToString("0.####");
                        mi.Composite(deltaImage, CompositeOperator.Difference);
                        //mi.Write(@"E:\fuck5.png");
                        //deltaImage.Write(@"E:\fuck6.png");
                    }

                    minGeo = minGeo.Min(GetScaledGeometry(wi.WatchZone.CropGeometry));
                }
            }

            if (mi.Width > minGeo.Size.ToDrawing().Width || mi.Height > minGeo.Size.ToDrawing().Height)
            {
                var mGeo = minGeo.ToMagick();
                mGeo.IgnoreAspectRatio = false;
                mi.ColorSpace = ColorSpace.Lab;
                mi.FilterType = DEFAULT_SCALE_FILTER;
                mi.Resize(mGeo);
            }
            ThumbnailBox.Size = minGeo.Size.ToDrawing();
            ThumbnailBox.Image = mi.ToBitmap(System.Drawing.Imaging.ImageFormat.MemoryBmp);
        }

        private void BtnRefreshFrame_Click(object sender, EventArgs e) => RefreshThumbnail();

        private void BtnTryAutoAlign_Click(object sender, EventArgs e)
        {
            if (Scanner.GameProfile != null)
            {
                using (var haystack = (Bitmap)Scanner.CurrentFrame.Bitmap.Clone())
                using (var needle = (Bitmap)Scanner.GameProfile.Screens[0].Autofitter.Image.Clone())
                {
                    var geo = AutoAlign(needle, haystack);
                    SetAllNumValues(geo.Min(MAX_VALUES).Max(MIN_VALUES));
                }
            }
        }

        #region Numeric Field Logic/Events

        private void UpdateCropGeometry(Geometry? geo = null)
        {
            Geometry newGeo = geo ?? Geometry.Blank;
            if (geo == null)
            {
                newGeo.X = (double)NumX.Value;
                newGeo.Y = (double)NumY.Value;
                newGeo.Width = (double)NumWidth.Value;
                newGeo.Height = (double)NumHeight.Value;
            }
            Scanner.CropGeometry = newGeo.Min(MAX_VALUES).Max(MIN_VALUES);
            Scanner.UpdateCropGeometry();
            RefreshThumbnail();
        }

        private void SetAllNumValues(Geometry geo)
        {
            NumX.Value = (decimal)geo.X;
            NumY.Value = (decimal)geo.Y;
            NumWidth.Value = (decimal)geo.Width;
            NumHeight.Value = (decimal)geo.Height;
            UpdateCropGeometry(geo);
        }

        // Validated triggers when the user manually the value, rather than anytime it changes.
        private void NumX_Validated(object sender, EventArgs e) => UpdateCropGeometry();
        private void NumY_Validated(object sender, EventArgs e) => UpdateCropGeometry();
        private void NumWidth_Validated(object sender, EventArgs e) => UpdateCropGeometry();
        private void NumHeight_Validated(object sender, EventArgs e) => UpdateCropGeometry();

        #endregion Numeric Field Logic/Events

        #region DPad Logic

        private int GetDPadID(object sender)
        {
            // Technically hacky but it makes for less methods.
            var name = ((PictureBox)sender).Name;
            return int.Parse(name.Substring(name.Length - 1, 1));
        }

        private void ShowSelectedDPad(PictureBox sender, bool selected, bool reverse)
        {
            int i = GetDPadID(sender);
            // Todo: Un-hardcode...somehow
            string imgName = "DPad" + (reverse ? "N" : null) + (selected ? "S" : null) + i.ToString();
            sender.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(imgName);
        }

        private double GetDistanceModifier()
        {
            double dist = DEFAULT_MOVE_DISTANCE; // 1 pixel
            if (ModifierKeys.HasFlag(Keys.Alt) && ModifierKeys.HasFlag(Keys.Shift))
                dist *= ALT_DISTANCE_MODIFIER * SHIFT_DISTANCE_MODIFIER; // 100 pixels
            else if (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Shift))
                dist *= CONTROL_DISTANCE_MODIFIER / SHIFT_DISTANCE_MODIFIER; // 0.01 pixels
            else if (ModifierKeys.HasFlag(Keys.Control))
                dist *= CONTROL_DISTANCE_MODIFIER; // 0.1 pixels
            else if (ModifierKeys.HasFlag(Keys.Shift))
                dist *= SHIFT_DISTANCE_MODIFIER; // 10 pixels
            return dist;
        }

        // I can't think of good names for what's used in the offsets/resizers. I can hardly explain them.
        // This could probably go in Geometry as a method anyway.
        private void AdjustCrop(PictureBox sender, double o1, double o2, double r1, double r2)
        {
            int i = GetDPadID(sender);
            var geo = Scanner.CropGeometry;

            switch (i)
            {
                case 1: geo.Offset(o1, o1); geo.Resize(r1, r1); break;
                case 2: geo.Offset(0, o1); geo.Resize(0, r1); break;
                case 3: geo.Offset(o2, o1); geo.Resize(r2, r1); break;
                case 4: geo.Offset(o1, 0); geo.Resize(r1, 0); break;
                case 6: geo.Offset(o2, 0); geo.Resize(r2, 0); break;
                case 7: geo.Offset(o1, o2); geo.Resize(r1, r2); break;
                case 8: geo.Offset(0, o2); geo.Resize(0, r2); break;
                case 9: geo.Offset(o2, o2); geo.Resize(r2, r2); break;
                default: break;
            }

            SetAllNumValues(geo.Min(MAX_VALUES).Max(MIN_VALUES));
        }

        #endregion DPad Logic

        #region DPad Events

        private void MoveDPad_MouseUp(object sender, MouseEventArgs e) => ShowSelectedDPad((PictureBox)sender, false, false);
        private void GrowDPad_MouseUp(object sender, MouseEventArgs e) => ShowSelectedDPad((PictureBox)sender, false, false);
        private void ShrinkDPad_MouseUp(object sender, MouseEventArgs e) => ShowSelectedDPad((PictureBox)sender, false, true);

        private void MoveDPad_MouseDown(object sender, MouseEventArgs e)
        {
            double dist = GetDistanceModifier();
            ShowSelectedDPad((PictureBox)sender, true, false);
            AdjustCrop((PictureBox)sender, -dist, dist, 0, 0);
        }

        private void GrowDPad_MouseDown(object sender, MouseEventArgs e)
        {
            double dist = GetDistanceModifier();
            ShowSelectedDPad((PictureBox)sender, true, false);
            AdjustCrop((PictureBox)sender, -dist, 0, dist, dist);
        }

        private void ShrinkDPad_MouseDown(object sender, MouseEventArgs e)
        {
            double dist = GetDistanceModifier();
            ShowSelectedDPad((PictureBox)sender, true, true);
            AdjustCrop((PictureBox)sender, dist, 0, -dist, -dist);
        }

        #endregion DPad Events

        private void BtnResetRegion_Click(object sender, EventArgs e)
        {
            var geo = Scanner.ResetCropGeometry();
            SetAllNumValues(geo);
        }

        private void FillDdlWatchZone()
        {
            DdlWatchZone.Items.Add("<None>");
            if (Scanner.GameProfile != null)
            {
                foreach (var wi in Scanner.GameProfile.WatchImages)
                {
                    wi.SetName(wi.Screen, wi.WatchZone, wi.Watcher);
                    DdlWatchZone.Items.Add(wi);
                }
            }
            DdlWatchZone.SelectedIndex = 0;
        }

        private void DdlWatchZone_SelectedIndexChanged(object sender, EventArgs e) => RefreshThumbnail();

        private void CkbViewDelta_CheckedChanged(object sender, EventArgs e) => RefreshThumbnail();

        private void CkbUseExtraPrecision_CheckedChanged(object sender, EventArgs e)
        {
            RefreshThumbnail();
        }

        public static Geometry AutoAlign(Bitmap needle, Bitmap haystack, double retryThreshold = 1, int retryLimit = 10)
        {
            IntPoint[] harrisPoints1;
            IntPoint[] harrisPoints2;
            IntPoint[] correlationPoints1;
            IntPoint[] correlationPoints2;
            MatrixH homography;

            var mi1 = new MagickImage(needle); mi1.Equalize(); needle = mi1.ToBitmap();
            var mi2 = new MagickImage(haystack); mi2.Equalize(); haystack = mi2.ToBitmap();

            HarrisCornersDetector harris = new HarrisCornersDetector(0.04f, 20000f);
            harrisPoints1 = harris.ProcessImage(needle).ToArray();
            harrisPoints2 = harris.ProcessImage(haystack).ToArray();

            CorrelationMatching matcher = new CorrelationMatching(9, needle, haystack);
            IntPoint[][] matches = matcher.Match(harrisPoints1, harrisPoints2);

            correlationPoints1 = matches[0];
            correlationPoints2 = matches[1];

            RansacHomographyEstimator ransac = new RansacHomographyEstimator(0.001, 0.999);
            homography = ransac.Estimate(correlationPoints1, correlationPoints2);

            IntPoint[] inliers1 = correlationPoints1.Get(ransac.Inliers);
            IntPoint[] inliers2 = correlationPoints2.Get(ransac.Inliers);

            Concatenate concat = new Concatenate(needle);
            Bitmap img3 = concat.Apply(haystack);

            PairsMarker pairs = new PairsMarker(
                inliers1,
                inliers2.Apply(p => new IntPoint(p.X + needle.Width, p.Y)));

            var Image = pairs.Apply(img3);
            Image.Save(@"C:\AutoAlignDebug.png");

            var pointCount = inliers1.Length;

            int[] xList1 = new int[pointCount];
            int[] yList1 = new int[pointCount];
            int[] xList2 = new int[pointCount];
            int[] yList2 = new int[pointCount];

            for (int n = 0; n < pointCount; n++)
            {
                xList1[n] = inliers1[n].X;
                yList1[n] = inliers1[n].Y;
                xList2[n] = inliers2[n].X;
                yList2[n] = inliers2[n].Y;
            }

            var f = new double[8] { xList1.Min(), yList1.Min(), xList1.Max(), yList1.Max(), xList2.Min(), yList2.Min(), xList2.Max(), yList2.Max() };

            double distFromX1 = f[0] / needle.Width;
            double distFromX2 = f[2] / needle.Width;
            double leftRatio = f[0] / (f[2] - f[0]);
            double rightRatio = (needle.Width - f[2]) / (f[2] - f[0]);
            double distFromY1 = f[1] / needle.Height;
            double distFromY2 = f[3] / needle.Height;
            double topRatio = f[1] / (f[3] - f[1]);
            double bottomRatio = (needle.Height - f[3]) / (f[3] - f[1]);

            double leftDist = (f[6] - f[4]) * leftRatio;
            double rightDist = (f[6] - f[4]) * rightRatio;
            double topDist = (f[7] - f[5]) * topRatio;
            double bottomDist = (f[7] - f[5]) * bottomRatio;

            double x = f[4] - leftDist;
            double y = f[5] - topDist;
            double width = leftDist + (f[6] - f[4]) + rightDist;
            double height = topDist + (f[7] - f[5]) + bottomDist;

            mi1.Resize(new MagickGeometry((int)Math.Round(width), (int)Math.Round(height)) { IgnoreAspectRatio = true });
            var mg = new MagickGeometry((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height)) { IgnoreAspectRatio = true };
            mi2.Extent(mg, Gravity.Northwest, MagickColor.FromRgba(0, 0, 0, 0));

            double delta = mi1.Compare(mi2, ErrorMetric.NormalizedCrossCorrelation);

            Geometry outGeo = new Geometry(x, y, width, height);

            if (delta < retryThreshold && retryLimit > 0)
            {
                retryLimit--;
                outGeo = AutoAlign(needle, haystack, delta, retryLimit);
            }

            return outGeo;
        }
    }
}
