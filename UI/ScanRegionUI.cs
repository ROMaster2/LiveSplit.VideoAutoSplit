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
using Accord.Video;
using Accord.Video.DirectShow;
using ImageMagick;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.UI;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    public partial class ScanRegionUI : AbstractUI
    {
        private GameProfile GameProfile { get { return Component.GameProfile; } }
        private Scanner Scanner => Component.Scanner;

        // Temporary. Remove later.
        private Geometry CropGeometry { get { return Component.CropGeometry; } set { Component.CropGeometry = value; } }

        private Geometry VideoGeometry => Scanner.VideoGeometry;

        private const Gravity STANDARD_GRAVITY = Gravity.Northwest;
        private const FilterType DEFAULT_SCALE_FILTER = FilterType.Box;
        private static readonly MagickColor EXTENT_COLOR = MagickColor.FromRgba(255, 0, 255, 127);

        private DateTime NextUpdate = DateTime.UtcNow;

        private bool RenderingFrame = false;

        private Geometry minGeometry
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
        private Geometry MAX_VALUES
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

        public ScanRegionUI(VASComponent component) : base(component)
        {
            InitializeComponent();
        }

        override public void Rerender()
        {
            SetAllNumValues(CropGeometry, false);
            FillboxPreviewFeature();
            Scanner.SubscribeToFrameHandler(HandleNewFrame);
        }

        override public void Derender()
        {
            Scanner.UnsubscribeFromFrameHandler(HandleNewFrame);
            boxPreviewFeature.Items.Clear();
            pictureBox.Image = new Bitmap(1,1);
        }

        override internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {

        }

        private void SetAllNumValues(Geometry geo, bool updateGeo = true)
        {
            numX.Value      = (decimal)geo.X;
            numY.Value      = (decimal)geo.Y;
            numWidth.Value  = (decimal)geo.Width;
            numHeight.Value = (decimal)geo.Height;
            if (updateGeo) UpdateCropGeometry(geo);
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
            CropGeometry = newGeo.Min(MAX_VALUES).Max(minGeometry);
        }

        private void FillboxPreviewType()
        {
            boxPreviewType.Items.Add("Full Frame");
            boxPreviewType.Items.Add("Optimal Frame Crop"); // TrueCropGeometry
            boxPreviewType.Items.Add("Screen*");
            boxPreviewType.Items.Add("Feature*");
            boxPreviewType.SelectedIndex = 0;
        }

        private void FillboxPreviewFeature()
        {
            boxPreviewFeature.Items.Add("<None>");
            if (GameProfile != null)
            {
                foreach (var wi in GameProfile.WatchImages)
                {
                    wi.SetName(wi.Screen, wi.WatchZone, wi.Watcher);
                    boxPreviewFeature.Items.Add(wi);
                }
            }
            boxPreviewFeature.SelectedIndex = 0;
        }

        // Would be better if something like this was in Geometry.cs
        private Geometry GetScaledGeometry(Geometry refGeo)
        {
            var referenceWidth = refGeo.IsBlank ? CropGeometry.Width : refGeo.Width;
            var referenceHeight = refGeo.IsBlank ? CropGeometry.Height : refGeo.Height;

            // Tried to not have hardcoded, but Microsoft disagreed.
            var parent = pictureBox.Parent;
            var parentWidth = parent.Width;
            var parentHeight = parent.Height - 100;
            var xMargin = pictureBox.Margin.Left + pictureBox.Margin.Right;
            var yMargin = pictureBox.Margin.Top + pictureBox.Margin.Bottom;
            var xRatio = (parentWidth - xMargin) / referenceWidth;
            var yRatio = (parentHeight - yMargin) / referenceHeight;

            var ratio = Math.Min(Math.Min(1, xRatio), yRatio);
            var width = Math.Max(referenceWidth * ratio, 4);
            var height = Math.Max(referenceHeight * ratio, 4);
            return new Geometry(width, height);
        }

        private void HandleNewFrame(object sender, NewFrameEventArgs e)
        {
            if (!RenderingFrame)
            {
                RenderingFrame = true;
                var frame = (Bitmap)e.Frame.Clone();
                Task.Run(() => RefreshThumbnail(frame));
            }
        }

        private void RefreshThumbnail(Bitmap frame)
        {
            int featureIndex = -1;
            WatchImage feature = null;
            boxPreviewFeature.Invoke((MethodInvoker)delegate
            {
                featureIndex = boxPreviewFeature.SelectedIndex;
                if (featureIndex > 0)
                    feature = (WatchImage)boxPreviewFeature.SelectedItem;
            });
            RefreshThumbnailAsync(frame, featureIndex, feature);
        }

        private async void RefreshThumbnailAsync(Bitmap frame, int featureIndex, WatchImage feature)
        {
            await Task.Delay(0);
            Geometry minGeo = Geometry.Min(CropGeometry, GetScaledGeometry(Geometry.Blank));

            MagickImage mi = new MagickImage(frame);
            var tmp = VideoGeometry;

            if (!VideoGeometry.Contains(CropGeometry))
            {
                mi.Extent(CropGeometry.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);
            }
            else
            {
                mi.Crop(CropGeometry.ToMagick(), STANDARD_GRAVITY);
            }
            mi.RePage();

            if (featureIndex > 0)
            {
                var wi = feature;
                var tGeo = wi.WatchZone.CropGeometry;
                tGeo.Update(-CropGeometry.X, -CropGeometry.Y);

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
                    overlay.Composite(baseM, new PointD(baseMGeo.X, baseMGeo.Y), CompositeOperator.Alpha);
                    mi.Composite(overlay, CompositeOperator.Atop);
                }
                mi.RePage();

                minGeo = minGeo.Min(GetScaledGeometry(tGeo));

                if (ckbShowComparison.Checked)
                {
                    using (var deltaImage = wi.MagickImage.Clone())
                    {
                        mi.ColorSpace = wi.Watcher.ColorSpace;
                        deltaImage.ColorSpace = wi.Watcher.ColorSpace;
                        mi.Crop(baseMGeo, STANDARD_GRAVITY);
                        mi.Alpha(AlphaOption.Off); // Why is this necessary? It wasn't necessary before.
                        if (wi.Watcher.Equalize)
                        {
                            deltaImage.Equalize();
                            mi.Equalize();
                        }
                        deltaImage.RePage();
                        mi.RePage();
                        lblDelta.Text = mi.Compare(deltaImage, ErrorMetric.PeakSignalToNoiseRatio).ToString("0.####");
                        mi.Composite(deltaImage, CompositeOperator.Difference);
                    }

                    minGeo = minGeo.Min(GetScaledGeometry(wi.WatchZone.CropGeometry));
                }
            }

            var drawingSize = minGeo.Size.ToDrawing();
            if (mi.Width > drawingSize.Width || mi.Height > drawingSize.Height)
            {
                var mGeo = minGeo.ToMagick();
                mGeo.IgnoreAspectRatio = false;
                //mi.ColorSpace = ColorSpace.HCL;
                mi.FilterType = DEFAULT_SCALE_FILTER;
                mi.Resize(mGeo);
            }
            UpdatepictureBox(drawingSize, mi.ToBitmap(System.Drawing.Imaging.ImageFormat.MemoryBmp));
        }

        private void UpdatepictureBox(Size minGeo, Bitmap bitmap)
        {
            pictureBox.Invoke((MethodInvoker)delegate
            {
                pictureBox.Size = minGeo;
                pictureBox.Image = bitmap;
                RenderingFrame = false;
            });
        }

    }
}
