﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageMagick;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    public partial class ScanRegionUI : AbstractUI
    {
        private const Gravity STANDARD_GRAVITY = Gravity.Northwest;
        private const FilterType DEFAULT_SCALE_FILTER = FilterType.Box;

        // Semi-Transparent Magenta
        private static readonly MagickColor EXTENT_COLOR =         MagickColor.FromRgba(255,   0, 255, 127);
        // 2/3 Transparent Yellow
        private static readonly MagickColor SCREEN_COLOR =         MagickColor.FromRgba(255, 255,   0,  85);
        // 2/3 Transparent Cyan
        private static readonly MagickColor WATCHZONE_COLOR =      MagickColor.FromRgba(  0, 255, 255,  85);
        // Mostly Opaque Dark Grey
        private static readonly MagickColor PREVIEW_EXTENT_COLOR = MagickColor.FromRgba(170, 170, 170, 223);

        private GameProfile _GameProfile => _Component.GameProfile;
        private Scanner _Scanner => _Component.Scanner;
        private CompiledFeatures _CompiledFeatures => _Component.Scanner.CompiledFeatures;
        //private IDictionary<string, Geometry> _CropGeometries => _Component.CropGeometries;
        private Geometry _CropGeometry => _Component.CropGeometry;
        private Geometry _TrueCropGeometry => _Component.Scanner.TrueCropGeometry;
        private Geometry _VideoGeometry => _Scanner.VideoGeometry;

        private MagickImage _ScreenOverlay;
        private MagickImage _WatchZoneOverlay;

        private bool _RenderingFrame = false;

        private Point _SelectionStart = new Point(0, 0);
        private bool _Selecting = false;

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
            CreateOverlays(_CropGeometry, _TrueCropGeometry);
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

        private void btnReset_Click(object sender, EventArgs e) => ResetNumValues();

        private void FillboxPreviewType()
        {
            boxPreviewType.Items.Add("Full Frame");
            boxPreviewType.Items.Add("Frame Crop");
            boxPreviewType.Items.Add("Features");
            boxPreviewType.SelectedIndex = 0;
        }

        private void FillboxPreviewFeature()
        {
            boxPreviewFeature.Items.Add("<None>");
            if (_CompiledFeatures != null)
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

        private void boxPreviewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            boxPreviewFeature.Enabled =
            ckbShowComparison.Enabled = _ActivePreviewType == PreviewType.Features;

            pictureBox.Cursor = _ActivePreviewType == PreviewType.FullFrame ? Cursors.Cross : Cursors.No;
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

        // @TODO: Eventually use distort overlay instead, for increased precision.
        private void CreateOverlays(Geometry cropGeo, Geometry trueCropGeo)
        {
            var cropRect = cropGeo.ToRectangle();
            var trueCropRect = trueCropGeo.ToRectangle();

            _ScreenOverlay = new MagickImage(SCREEN_COLOR, cropRect.Width, cropRect.Height);
            _WatchZoneOverlay = new MagickImage(MagickColors.Transparent, trueCropRect.Width, trueCropRect.Height);

            if (_CompiledFeatures != null)
            {
                foreach (var wz in _CompiledFeatures.CWatchZones)
                {
                    var wzg = wz.Geometry;
                    var wzmg = wzg.ToMagick();
                    using (var wzmi = new MagickImage(WATCHZONE_COLOR, wzmg.Width, wzmg.Height))
                    using (var wzmia = new MagickImage(MagickColors.Transparent, wzmg.Width, wzmg.Height))
                    {
                        var xOffsetWZ = (int)Math.Round(wzg.X - trueCropGeo.X);
                        var yOffsetWZ = (int)Math.Round(wzg.Y - trueCropGeo.Y);
                        var xOffsetS = (int)Math.Round(wzg.X - cropGeo.X);
                        var yOffsetS = (int)Math.Round(wzg.Y - cropGeo.Y);
                        _WatchZoneOverlay.Composite(wzmi, xOffsetWZ, yOffsetWZ, CompositeOperator.Over);
                        _ScreenOverlay.Composite(wzmia, xOffsetS, yOffsetS, CompositeOperator.Copy);
                    }
                }
                var xOffset = (int)Math.Round(trueCropGeo.X - cropGeo.X);
                var yOffset = (int)Math.Round(trueCropGeo.Y - cropGeo.Y);
                _ScreenOverlay.Composite(_WatchZoneOverlay, xOffset, yOffset, CompositeOperator.Over);
            }
        }

        #region Thumbnail Rendering

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

        // @TODO: Break into separate methods?
        private async void RefreshThumbnailAsync(Scan scan, PreviewType previewType, PreviewFeature feature)
        {
            await Task.Delay(0);

            try
            {
                Geometry minGeo;
                MagickImage mi;
                string confidence = string.Empty;
                Geometry cropGeo = _CropGeometry;

                if (previewType == PreviewType.FullFrame)
                {
                    minGeo = Geometry.Min(_VideoGeometry, GetScaledGeometry(_VideoGeometry));
                    mi = new MagickImage(scan.CurrentFrame.Bitmap);

                    if (!_Selecting)
                    {
                        var roundGeo = cropGeo.Round();
                        mi.Composite(_ScreenOverlay, (int)roundGeo.X, (int)roundGeo.Y, CompositeOperator.Over);
                    }
                    else
                    {
                        var numRect = NumGeometry.ToRectangle();
                        using (var overlay = new MagickImage(SCREEN_COLOR, numRect.Width, numRect.Height))
                        {
                            mi.Composite(overlay, numRect.X, numRect.Y, CompositeOperator.Over);
                        }
                    }
                }
                else if (previewType == PreviewType.FrameCrop)
                {
                    minGeo = Geometry.Min(cropGeo, GetScaledGeometry(cropGeo));
                    if (!_VideoGeometry.Contains(cropGeo))
                    {
                        mi = new MagickImage(scan.CurrentFrame.Bitmap);
                        mi.Extent(cropGeo.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);
                    }
                    else
                    {
                        mi = new MagickImage(scan.CurrentFrame.Bitmap.Clone(cropGeo.ToRectangle(), PixelFormat.Format24bppRgb));
                    }

                    if (!_Selecting)
                    {
                        var roundGeo = cropGeo.Round();
                        var roundTrueGeo = _TrueCropGeometry.Round();
                        int xOffset = (int)(roundTrueGeo.X - cropGeo.X);
                        int yOffset = (int)(roundTrueGeo.Y - cropGeo.Y);
                        mi.Composite(_WatchZoneOverlay, xOffset, yOffset, CompositeOperator.Over);
                    }
                }
                else if (previewType == PreviewType.Features && feature == null)
                {
                    var trueGeo = _TrueCropGeometry;

                    minGeo = Geometry.Min(trueGeo, GetScaledGeometry(trueGeo));
                    if (!_VideoGeometry.Contains(trueGeo))
                    {
                        mi = new MagickImage(scan.CurrentFrame.Bitmap);
                        mi.Extent(trueGeo.ToMagick(), STANDARD_GRAVITY, EXTENT_COLOR);
                    }
                    else
                    {
                        mi = new MagickImage(scan.CurrentFrame.Bitmap.Clone(trueGeo.ToRectangle(), PixelFormat.Format24bppRgb));
                    }

                    if (!_Selecting)
                    {
                        mi.Composite(_WatchZoneOverlay, CompositeOperator.Over);
                    }
                }
                else if (previewType == PreviewType.Features && feature != null)
                {
                    var wzGeo = feature.WatchZone.Geometry;

                    if (!ckbShowComparison.Checked)
                    {
                        var baseMGeo = new MagickGeometry(64, 64, (int)Math.Round(wzGeo.Width), (int)Math.Round(wzGeo.Height));

                        wzGeo.Adjust(-64, -64, 128, 128);

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
                            MagickColors.Transparent,
                            baseMGeo.Width,
                            baseMGeo.Height))
                        using (var overlay = new MagickImage(
                                PREVIEW_EXTENT_COLOR,
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
                    throw new InvalidEnumArgumentException("PreviewType out of bounds? This shouldn't happen.");
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
                Log.Error(e, "Thumbnail failed to render for the Scan Region.");
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

        #endregion Thumbnail Rendering

        #region Image Click Logic

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_ActivePreviewType == PreviewType.FullFrame)
            {
                var vGeo = _VideoGeometry;
                var widthMultiplier = pictureBox.Width <= 0 ? 1 : (decimal)vGeo.Width / pictureBox.Width;
                var heightMultiplier = pictureBox.Height <= 0 ? 1 : (decimal)vGeo.Height / pictureBox.Height;
                numX.Value = _SelectionStart.X * widthMultiplier;
                numY.Value = _SelectionStart.Y * heightMultiplier;
                numWidth.Value = 0m;
                numHeight.Value = 0m;

                _Selecting = true;

                _SelectionStart = e.Location;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Selecting) Click_Resize_Preview(e.Location);
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (_ActivePreviewType == PreviewType.FullFrame)
            {
                Click_Resize_Preview(e.Location);
                _Selecting = false;
                UpdateCropGeometry();
            }
        }

        private void Click_Resize_Preview(Point newPoint)
        {
            var vGeo = _VideoGeometry;
            decimal screenWidth = (decimal)vGeo.Width;
            decimal screenHeight = (decimal)vGeo.Height;
            decimal widthMultiplier =   pictureBox.Width <= 0 ? 1 : screenWidth / pictureBox.Width;
            decimal heightMultiplier = pictureBox.Height <= 0 ? 1 : screenHeight / pictureBox.Height;

            int mouseX = Math.Min(Math.Max(0, newPoint.X), (int)Math.Round(screenWidth / widthMultiplier));
            int mouseY = Math.Min(Math.Max(0, newPoint.Y), (int)Math.Round(screenHeight / heightMultiplier));

            numX.Value = Math.Max(0m, Math.Min(_SelectionStart.X, mouseX) * widthMultiplier);
            numY.Value = Math.Max(0m, Math.Min(_SelectionStart.Y, mouseY) * heightMultiplier);
            numWidth.Value =
                Math.Min(screenWidth - numX.Value, Math.Abs(_SelectionStart.X - mouseX) * widthMultiplier);
            numHeight.Value =
                Math.Min(screenHeight - numY.Value, Math.Abs(_SelectionStart.Y - mouseY) * heightMultiplier);
        }

        #endregion Image Click Logic

        #region Private Classes

        private enum PreviewType
        {
            FullFrame,
            FrameCrop, // CropGeometry
            Features // WatchZone
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

        #endregion Private Classes
    }
}
