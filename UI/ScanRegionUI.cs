using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
        private const Gravity STANDARD_GRAVITY = Gravity.Northwest;
        private const FilterType DEFAULT_SCALE_FILTER = FilterType.Box;
        private static readonly MagickColor EXTENT_COLOR = MagickColor.FromRgba(255, 0, 255, 127);

        private GameProfile _GameProfile => _Component.GameProfile;
        private Scanner _Scanner => _Component.Scanner;
        private CompiledFeatures _CompiledFeatures => _Component.Scanner.CompiledFeatures;
        //private IDictionary<string, Geometry> _CropGeometries => _Component.CropGeometries;
        private Geometry _CropGeometry => _Component.CropGeometry;
        private Geometry _TrueCropGeometry => _Component.Scanner.TrueCropGeometry;
        private Geometry _VideoGeometry => _Scanner.VideoGeometry;

        private bool _RenderingFrame = false;

        public Geometry NumGeometry
        {
            get
            {
                Geometry vGeo = _VideoGeometry;
                Geometry maxGeometry;
                Geometry minGeometry;
                if (vGeo.IsBlank)
                {
                    maxGeometry = new Geometry(8192, 8192, 8192, 8192);
                    //minGeometry = new Geometry(-8192, -8192, 4, 4);
                    minGeometry = new Geometry(0, 0, 4, 4);
                }
                else
                {
                    //maxGeometry = new Geometry(vGeo.Width, vGeo.Height, vGeo.Width * 2, vGeo.Height * 2);
                    //minGeometry = new Geometry(-vGeo.Width, -vGeo.Height, 4, 4);
                    maxGeometry = new Geometry(vGeo.Width - 4, vGeo.Height - 4, vGeo.Width, vGeo.Height);
                    minGeometry = new Geometry(0, 0, 4, 4);
                }

                return new Geometry(
                    (double)numX.Value,
                    (double)numY.Value,
                    (double)numWidth.Value,
                    (double)numHeight.Value).Clamp(maxGeometry, minGeometry);
            }
            set
            {
                numX.Value      = (decimal)value.X;
                numY.Value      = (decimal)value.Y;
                numWidth.Value  = (decimal)value.Width;
                numHeight.Value = (decimal)value.Height;
            }
        }

        private PreviewType _ActivePreviewType
        {
            get
            {
                var str = ((string)boxPreviewType.SelectedItem).Replace(" ", "");
                return (PreviewType)Enum.Parse(typeof(PreviewType), str);
            }
        }

        public ScanRegionUI(VASComponent component) : base(component)
        {
            InitializeComponent();
            FillboxPreviewType();
        }

        override public void Rerender()
        {
            SetAllNumValues(_CropGeometry);
            FillboxPreviewFeature();
            _Scanner.SubscribeToFrameHandler(HandleNewScan);
        }

        override public void Derender()
        {
            _Scanner.UnsubscribeFromFrameHandler(HandleNewScan);
            boxPreviewFeature.Items.Clear();

            pictureBox?.Image?.Dispose();
            pictureBox.Image = null;
            _RenderingFrame = false;
        }

        // While the profile and scanner affect this UI and vice versa, the profile's script does not.
        override internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded) { }

        private void UpdateCropGeometry()
        {
            _Component.CropGeometry = NumGeometry;
        }

        private void SetAllNumValues(Geometry geo)
        {
            NumGeometry = geo;
            UpdateCropGeometry();
        }

        private void ResetNumValues()
        {
            NumGeometry = (_VideoGeometry.Width < 8 || _VideoGeometry.Height < 8) ? new Geometry(640, 480) : _VideoGeometry;
            UpdateCropGeometry();
        }

        // Validated triggers when the user manually changes the value, rather than anytime it changes.
        private void numX_Validated(object sender, EventArgs e)      => UpdateCropGeometry();
        private void numY_Validated(object sender, EventArgs e)      => UpdateCropGeometry();
        private void numWidth_Validated(object sender, EventArgs e)  => UpdateCropGeometry();
        private void numHeight_Validated(object sender, EventArgs e) => UpdateCropGeometry();

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetNumValues();
        }

        private void FillboxPreviewType()
        {
            boxPreviewType.Items.Add("Full Frame");
            boxPreviewType.Items.Add("Frame Crop");
            boxPreviewType.Items.Add("All Features");
            boxPreviewType.Items.Add("Feature");
            boxPreviewType.SelectedIndex = 0;
        }

        private void FillboxPreviewFeature()
        {
            boxPreviewFeature.Items.Add("<None>");
            if (!_CompiledFeatures.IsBlank)
            {
                foreach (var wz in _CompiledFeatures.CWatchZones)
                {
                    foreach (var w in wz.CWatches)
                    {
                        foreach (var wi in w.CWatchImages)
                        {
                            var name = wz.Name + "/" + w.Name + " - " + wi.Name;
                            var pf = new PreviewFeature(name, wz, w, wi);
                            boxPreviewFeature.Items.Add(pf);
                        }
                    }
                }
            }
            boxPreviewFeature.SelectedIndex = 0;
        }

        // Would be better if something like this was in Geometry.cs
        private Geometry GetScaledGeometry(Geometry refGeo)
        {
            var referenceWidth = refGeo.Width;
            var referenceHeight = refGeo.Height;

            // Wanted to not have 150 hardcoded, but Microsoft disagreed.
            var parent = pictureBox.Parent;
            var parentWidth = parent.Width;
            var parentHeight = parent.Height - 150;
            var xMargin = pictureBox.Margin.Left + pictureBox.Margin.Right;
            var yMargin = pictureBox.Margin.Top + pictureBox.Margin.Bottom;
            var xRatio = (parentWidth - xMargin) / referenceWidth;
            var yRatio = (parentHeight - yMargin) / referenceHeight;

            var ratio = Math.Min(Math.Min(1, xRatio), yRatio);
            var width = Math.Max(referenceWidth * ratio, 4);
            var height = Math.Max(referenceHeight * ratio, 4);
            return new Geometry(width, height);
        }

        private void HandleNewScan(object sender, Scan scan)
        {
            if (!_RenderingFrame)
            {
                _RenderingFrame = true;
                Task.Run(() => RefreshThumbnail(scan));
            }
        }

        private void RefreshThumbnail(Scan scan)
        {
            PreviewType previewType = PreviewType.FullFrame;
            PreviewFeature feature = null;
            if (boxPreviewFeature.Created)
            {
                boxPreviewFeature.Invoke((MethodInvoker)delegate
                {
                    previewType = _ActivePreviewType;
                    if (boxPreviewFeature.SelectedIndex > 0)
                        feature = (PreviewFeature)boxPreviewFeature.SelectedItem;
                });
            }
            RefreshThumbnailAsync(scan, previewType, feature);
        }

        private async void RefreshThumbnailAsync(Scan scan, PreviewType previewType, PreviewFeature feature)
        {
            await Task.Delay(0);

            try
            {
                Geometry minGeo;
                MagickImage mi;
                string confidence = string.Empty;

                if (previewType == PreviewType.FullFrame)
                {
                    minGeo = Geometry.Min(_VideoGeometry, GetScaledGeometry(_VideoGeometry));
                    mi = new MagickImage(scan.CurrentFrame.Bitmap);
                    // @TODO: Add preview zones for crop(s).
                }
                else if (previewType == PreviewType.FrameCrop)
                {
                    minGeo = Geometry.Min(_CropGeometry, GetScaledGeometry(_CropGeometry));
                    if (!_VideoGeometry.Contains(_CropGeometry))
                    {
                        mi = new MagickImage(scan.CurrentFrame.Bitmap);
                        mi.Extent(_CropGeometry.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);
                    }
                    else
                    {
                        mi = new MagickImage(scan.CurrentFrame.Bitmap.Clone(_CropGeometry.ToRectangle(), PixelFormat.Format24bppRgb));
                    }
                }
                else if (previewType == PreviewType.AllFeatures || (previewType == PreviewType.Feature && feature == null))
                {
                    minGeo = Geometry.Min(_TrueCropGeometry, GetScaledGeometry(_TrueCropGeometry));
                    if (!_VideoGeometry.Contains(_TrueCropGeometry))
                    {
                        mi = new MagickImage(scan.CurrentFrame.Bitmap);
                        mi.Extent(_TrueCropGeometry.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);
                    }
                    else
                    {
                        mi = new MagickImage(scan.CurrentFrame.Bitmap.Clone(_TrueCropGeometry.ToRectangle(), PixelFormat.Format24bppRgb));
                    }
                }
                else if (previewType == PreviewType.Feature && feature != null)
                {
                    var wzGeo = feature.WatchZone.Geometry;

                    if (!ckbShowComparison.Checked)
                    {
                        var baseMGeo = new MagickGeometry(64, 64, (int)Math.Round(wzGeo.Width), (int)Math.Round(wzGeo.Height));

                        wzGeo.Update(-64, -64, 128, 128);

                        minGeo = Geometry.Min(wzGeo, GetScaledGeometry(wzGeo));
                        if (!_VideoGeometry.Contains(wzGeo))
                        {
                            mi = new MagickImage(scan.CurrentFrame.Bitmap);
                            mi.Extent(wzGeo.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);
                        }
                        else
                        {
                            mi = new MagickImage(scan.CurrentFrame.Bitmap.Clone(wzGeo.ToRectangle(), PixelFormat.Format24bppRgb));
                        }

                        using (var baseM = new MagickImage(
                            MagickColor.FromRgba(0, 0, 0, 0),
                            baseMGeo.Width,
                            baseMGeo.Height))
                        using (var overlay = new MagickImage(
                                MagickColor.FromRgba(170, 170, 170, 223),
                                baseMGeo.Width + 128,
                                baseMGeo.Height + 128))
                        {
                            overlay.Composite(baseM, new PointD(baseMGeo.X, baseMGeo.Y), CompositeOperator.Alpha);
                            mi.Composite(overlay, CompositeOperator.Atop);
                        }
                    }
                    else
                    {
                        minGeo = Geometry.Min(wzGeo, GetScaledGeometry(wzGeo));
                        if (!_VideoGeometry.Contains(wzGeo))
                        {
                            mi = new MagickImage(scan.CurrentFrame.Bitmap);
                            mi.Extent(wzGeo.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);
                        }
                        else
                        {
                            mi = new MagickImage(scan.CurrentFrame.Bitmap.Clone(wzGeo.ToRectangle(), PixelFormat.Format24bppRgb));
                        }

                        // @TODO: Add previous frame stuff
                        using (var deltaImage = feature.WatchImage.MagickImage.Clone())
                        {
                            mi.ColorSpace = feature.Watcher.ColorSpace;
                            deltaImage.ColorSpace = feature.Watcher.ColorSpace;
                            if (feature.WatchImage.HasAlpha)
                            {
                                mi.Composite(feature.WatchImage.AlphaChannel, CompositeOperator.Over);
                            }
                            if (feature.Watcher.Equalize) mi.Equalize();
                            confidence = mi.Compare(deltaImage, feature.Watcher.ErrorMetric).ToString("F4");
                            mi.Composite(deltaImage, CompositeOperator.Difference);
                        }
                    }
                }
                else
                {
                    Log.Error("I like turtles.");
                    throw new Exception("How did this happen?");
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
                UpdatepictureBox(drawingSize, mi.ToBitmap(), confidence);
            }
            catch (Exception e)
            {
                Log.Error("Thumbnail failed to render for the Scan Region");
                Log.Error(e);
                scan.Dispose();
                _RenderingFrame = false;
            }
        }

        private void UpdatepictureBox(Size minGeo, Bitmap bitmap, string confidence)
        {
            this.Invoke((MethodInvoker)delegate
            {
                pictureBox.Size = minGeo;
                pictureBox.Image = bitmap;
                lblDelta.Text = confidence;
            });
            _RenderingFrame = false;
        }

        private void boxPreviewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            boxPreviewFeature.Enabled =
            ckbShowComparison.Enabled = _ActivePreviewType == PreviewType.Feature;

            pictureBox.Cursor = _ActivePreviewType == PreviewType.FullFrame ? Cursors.Cross : Cursors.No;
        }

        private enum PreviewType
        {
            FullFrame,
            FrameCrop, // CropGeometry
            AllFeatures, // TrueCropGeometry
            Feature // WatchZone
        }

        private class PreviewFeature
        {
            public string Name;
            public CWatchZone WatchZone;
            public CWatcher Watcher;
            public CWatchImage WatchImage;
            public PreviewFeature(string name, CWatchZone watchZone, CWatcher watcher, CWatchImage watchImage)
            {
                Name = name;
                WatchZone = watchZone;
                Watcher = watcher;
                WatchImage = watchImage;
            }
            public override string ToString()
            {
                return Name;
            }
        }

    }
}
