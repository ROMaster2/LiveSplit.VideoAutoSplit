/*using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;*/
using ImageMagick;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Configuration;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LiveSplit.VAS;
using LiveSplit.VAS.Models;

using Accord;
using Accord.Video;
using Accord.Video.DirectShow;
using Accord.Imaging;
using Accord.Vision;
using Accord.Vision.Detection;
using Accord.Imaging.Filters;
using Accord.Vision.Detection.Cascades;
using Accord.Math;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

using Screen = LiveSplit.VAS.Models.Screen;
using Blend = Accord.Imaging.Filters.Blend;
using Point = System.Windows.Point;

namespace LiveSplit.VAS
{
    class Test6
    {
        public static void Run2()
        {
            string filePath = @"C:\Users\Administrator\Pictures\VID\pkmnsnap\game_autofit.png";
            var mi = new MagickImage(filePath);
            var d = new DistortSettings { Bestfit = true };
            //d.Viewport = new MagickGeometry(0, 0, 750, 562) { IgnoreAspectRatio = true };
            //d.Scale = 689.1892 / 750;


            mi.FilterType = FilterType.Triangle;
            mi.Alpha(AlphaOption.On);
            //mi.Distort(DistortMethod.ScaleRotateTranslate, d, new double[] { 19.3243, 11.016, 689.1892 / 750 , 455.4331 / 562, 0 });
            mi.Distort(DistortMethod.ScaleRotateTranslate, d, new double[] { 0, 0, 691.50001d / 750d, 455.4331d / 562d, 0, 0, 0 });
            mi.Write(@"E:\memes9.png");

            // 690 = 692, 690.1 = 692, 690.5 = 693, 690.9 = 693, 691 = 693, 691.1 = 693, 691.5 = 693, 691.9 = 694, 692 = 694
            // y = Math.Round(x) + 2

        }

        public static void Run()
        {
            string filePath = @"C:\Users\Administrator\Pictures\VID\pkmnsnap_features\pkmnsnap.zip";
            var archive = ZipFile.OpenRead(filePath);
            var entries = archive.Entries;
            var xmlFiles = entries.Where(z => z.Name.Contains(".xml"));
            if (xmlFiles.Count() == 0)
            {
                throw new Exception("Game Profile XML is missing");
            }
            else if (xmlFiles.Count() > 1)
            {
                throw new Exception("Multiple XML files found, we only need one.");
            }
            var xmlFile = xmlFiles.First();
            var stream = xmlFile.Open();

            GameProfile gp = null;

            using (StreamReader reader = new StreamReader(stream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameProfile));
                GameProfile gp2 = null;

                // Todo: ACTUAL exception handling.
                try
                {
                    gp2 = (GameProfile)serializer.Deserialize(reader);
                    gp2.ReSyncRelationships();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Game Profile failed to load.");
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }

                gp = gp2;
            }

            foreach(var s in gp.Screens)
            {
                var search = s.Name.ToLower() + "_autofit.png";
                var files = entries.Where(z => z.Name.ToLower().Contains(search));
                if (files.Count() == 0)
                {
                    continue;
                }
                else if (files.Count() > 1)
                {
                    throw new Exception("Multiple autofit images found for " + s.Name + ", only one per screen is currently supported.");
                }
                var file = files.First();
                var imageStream = file.Open();
                s.Autofitter.Image = new Bitmap(imageStream);
            }
            foreach (var wi in gp.WatchImages)
            {
                var search = wi.FilePath.Replace('\\', '/').ToLower();
                var files = entries.Where(z => z.FullName.ToLower().Contains(search));
                if (files.Count() == 0)
                {
                    throw new Exception("Image for " + search + " not found.");
                }
                else if (files.Count() > 1)
                {
                    throw new Exception("Multiple images found for " + search + ", somehow.");
                }
                var file = files.First();
                var imageStream = file.Open();
                wi.Image = new Bitmap(imageStream);
            }


        }
    }

    class Test5
    {
        public static void Run()
        {
            var gp = Scanner.GameProfile;

            foreach (var s in gp.Screens)
            {
                s.CropGeometry = Scanner.CropGeometry;
            }

            //CompiledFeatures.Compile(gp);

            /*
            var x = new MagickReadSettings()
            {
                ExtractArea = new MagickGeometry(100, 200, 300, 400), // <-- USE THIS!!!!!!
                ColorSpace = ColorSpace.HCL // Maybe if global
            };

            var i = new Bitmap(@"E:\0\10\1.png");
            var mi3 = new MagickImage(@"E:\0\10\1.png");
            var mi4 = new MagickImage(@"E:\0\10\1.png", x);
            var ss = new FileStream(@"E:\0\10\1.png", FileMode.Open, FileAccess.Read);

            var s1 = Stopwatch.StartNew();
            mi4 = new MagickImage(@"E:\0\10\1.png", x);
            s1.Stop();
            var t1 = s1.ElapsedMilliseconds;

            var s2 = Stopwatch.StartNew();
            mi3 = new MagickImage(@"E:\0\10\1.png");
            s2.Stop();
            var t2 = s2.ElapsedMilliseconds;

            var mi5 = new MagickImage(i);

            var s3 = Stopwatch.StartNew();
            mi5 = new MagickImage(i);
            s3.Stop();
            var t3 = s3.ElapsedMilliseconds;

            var s4 = Stopwatch.StartNew();
            mi5 = new MagickImage(ss, x);
            s4.Stop();
            var t4 = s4.ElapsedMilliseconds;

            //var mi2 = CompiledFeatures.GetComposedImage(mi, ColorSpace.RGB, 0);
            mi3.Write(@"E:\0\10\2.png");
            */
        }
    }

    class Test4
    {
        public static void Run()
        {
            /*
            using (var mi = new MagickImage(@"E:\0\_Original.png"))
            {
                Parallel.ForEach((ColorSpace[])Enum.GetValues(typeof(ColorSpace)), (cs) =>
                {
                    using (var mi1 = mi.Clone())
                    {
                        mi1.ColorSpace = cs;
                        var mi2 = mi1.Separate();
                        for (int i = 0; i < mi2.Count(); i++)
                        {
                            using (var mi3 = mi2.ToList()[i])
                            {
                                //mi3.FilterType = FilterType.Triangle;
                                //mi3.Resize(new Percentage(50));
                                mi3.Write(@"E:\0\8\" + cs.ToString() + "_" + (i + 1).ToString() + ".png");
                            }
                        }
                    }
                });
            }
            */

            DirectoryInfo d = new DirectoryInfo(@"E:\0\9");
            FileInfo[] files = d.GetFiles("*.png");

            var b = new ConcurrentBag<Bag4>();

            Parallel.For(0, files.Count(), (i1) =>
            {
                var name1 = files[i1].Name;
                using (var mi1 = new MagickImage(files[i1].FullName))
                {
                    Parallel.For(0, files.Count(), (i2) =>
                    {
                        var name2 = files[i2].Name;
                        if (i1 < i2)
                        {
                            using (var mi2 = new MagickImage(files[i2].FullName))
                            {
                                var v = mi1.Compare(mi2, ErrorMetric.MeanSquared);
                                b.Add(new Bag4(name1, name2, v));
                            }
                        }
                    });
                }
            });

            var c1 = b.ToList().OrderByDescending(x => x.Value).ToList();
            var d1 = c1.First();

            var c2 = b.ToList().OrderBy(x => x.Value).ToList();
            var d2 = c2.First();

            var c3 = b.ToList().Where(x => x.Value < 0.001).OrderBy(x => x.Name1).ToList();
            var d3 = c3.First();

            string s = "";
            foreach(var e in c2)
            {
                s += e.Name1 + "\t" + e.Name2 + "\t" + e.Value.ToString() + "\r\n";
            }

        }

        public struct Bag4
        {
            public string Name1;
            public string Name2;
            public double Value;
            public Bag4(string name1, string name2, double value)
            {
                Name1 = name1;
                Name2 = name2;
                Value = value;
            }
        }

    }

    class Test3
    {
        public static void Wsg()
        {
            var a = new MagickImageCollection();
            var b = new MagickImage();

            //a.Combine(ColorSpace.HCL);

            //a.Evaluate(EvaluateOperator.Abs);

            var c = new QuantizeSettings
            {
                ColorSpace = ColorSpace.HCL,
                DitherMethod = DitherMethod.FloydSteinberg
            };

            //a.Map(b, c);

            foreach (MagickImage e in a)
            {

            }

        }


        public static void Math1()
        {
            var mi1 = new MagickImage(@"E:\7\1.png") { ColorSpace = ColorSpace.HCL };
            var mi2 = new MagickImage(@"E:\7\2.png") { ColorSpace = ColorSpace.HCL };
            mi1.Crop(512, 200, 1390, 805); // (512, 200, 1390, 805)
            mi2.Crop(512, 200, 1390, 805);
            mi1.RePage();
            mi2.RePage();
            mi1.Equalize();
            mi2.Equalize();
            mi1.RePage();
            mi2.RePage();
            mi1.ColorSpace = ColorSpace.HCL;
            mi2.ColorSpace = ColorSpace.HCL;
            mi1.RePage();
            mi2.RePage();

            var mi5 = (MagickImage)mi1.Clone();
            var mi6 = (MagickImage)mi2.Clone();

            mi5.Write(@"E:\7\5.png");
            mi6.Write(@"E:\7\6.png");
            mi5 = (MagickImage)mi5.Separate().First();
            mi6 = (MagickImage)mi6.Separate().First();
            mi5.Write(@"E:\7\7.png");
            mi6.Write(@"E:\7\8.png");
            var b01 = mi5.ToByteArray(MagickFormat.Rgb);
            var b02 = mi6.ToByteArray(MagickFormat.Rgb);
            var len0 = b01.Length;
            var b03 = new byte[b01.Length];
            for (int i = 0; i < b01.Length; i += 3)
            {
                b03[i] =
                b03[i + 1] =
                b03[i + 2] = Math7(b01[i], b02[i]);
            }
            var avg =  (b03.Average(q => (double)q)) / 255d;
            var e = new PixelStorageSettings(1390, 805, StorageType.Char, PixelMapping.RGB);
            var mii = new MagickImage(b03, e);
            mii.Write(@"E:\7\9.png");


            var start1 = DateTime.UtcNow;
            var b11 = mi1.GetPixelsUnsafe().ToByteArray(PixelMapping.RGB);
            var b12 = mi2.GetPixelsUnsafe().ToByteArray(PixelMapping.RGB);
            var len1 = b11.Length;
            double b13 = 0;
            //Parallel.For(0, b1.Length / 3, (n) =>
            for (int i = 0; i < b11.Length; i += 3)
            {
                b13 += Math6(b11[i], b11[i + 1], b11[i + 2], b12[i], b12[i + 1], b12[i + 2]);
                //int i = n * 3;
                //b3 += Math6(b1[i], b1[i + 1], b1[i + 2], b2[i], b2[i + 1], b2[i + 2]);
            }
            //});
            double delta1 = b13 / len1;
            var duration1 = DateTime.UtcNow.Subtract(start1).Milliseconds * 60;

            var mi3 = (MagickImage)mi1.Clone();
            var mi4 = (MagickImage)mi2.Clone();

            var start2 = DateTime.UtcNow;
            mi3 = (MagickImage)mi3.Separate().First();
            mi4 = (MagickImage)mi4.Separate().First();
            var b21 = mi3.ToByteArray(MagickFormat.Gray);
            var b22 = mi4.ToByteArray(MagickFormat.Gray);
            var len2 = b21.Length;
            uint b23 = 0;
            for (int i = 0; i < b21.Length; i++)
            {
                b23 += Math5(b21[i], b22[i]);
            }
            double delta2 = (double)b23 / (double)len2 / 255d / 3d;
            var duration2 = DateTime.UtcNow.Subtract(start2).Milliseconds * 60;

            var start3 = DateTime.UtcNow;
            mi1 = (MagickImage)mi1.Separate().First();
            mi2 = (MagickImage)mi2.Separate().First();
            double delta3 = mi1.Compare(mi2, ErrorMetric.PeakSignalToNoiseRatio);
            var duration3 = DateTime.UtcNow.Subtract(start3).Milliseconds * 60;
            mi1.Composite(mi2, CompositeOperator.Difference);
            mi1.RePage();
            mi1.Write(@"E:\7\3.png");

            //var mi3 = new MagickImage(b3, x);
            //mi3.Write(@"E:\7\3.png");
            A();
        }
        public static void A()
        {
            var mi4 = new MagickImage(@"E:\7\1.png") { ColorSpace = ColorSpace.Rec709YCbCr };
            var mi5 = new MagickImage(@"E:\7\2.png") { ColorSpace = ColorSpace.Rec709YCbCr };
            mi4.Crop(512, 200, 1390, 805);
            mi5.Crop(512, 200, 1390, 805);
            mi4.RePage();
            mi5.RePage();
            mi4.Equalize();
            mi5.Equalize();
            mi4.RePage();
            mi5.RePage();
            mi4.ColorSpace = ColorSpace.Rec709YCbCr;
            mi5.ColorSpace = ColorSpace.Rec709YCbCr;
            mi4.RePage();
            mi5.RePage();

            var start = DateTime.UtcNow;
            mi4 = (MagickImage)mi4.Separate().Last();
            mi5 = (MagickImage)mi5.Separate().Last();
            double a = mi4.Compare(mi5, ErrorMetric.PeakSignalToNoiseRatio);
            var duration = DateTime.UtcNow.Subtract(start).Milliseconds * 60;

            mi4.Composite(mi5, CompositeOperator.Difference);
            mi4.RePage();
            mi4.Write(@"E:\7\4.png");
        }
        public static uint Math5(byte x, byte y)
        {
            uint X = Math.Max(x, y);
            uint Y = Math.Min(x, y);
            uint z = Math.Min(X - Y, 256 - (X - Y)) * 2;
            //if (z >= 256)
            //    return 255;
            //else
                return z;
        }
        public static double Math6(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
        {
            double x = RBG2Hue(r1, g1, b1);
            double y = RBG2Hue(r2, g2, b2);
            double X = Math.Max(x, y);
            double Y = Math.Min(x, y);
            double z = Math.Min(X - Y, 1 - (X - Y));
            //if (z >= 256)
            //    return 255;
            //else
            return z;
        }
        public static byte Math7(byte x, byte y)
        {
            byte X = Math.Max(x, y);
            byte Y = Math.Min(x, y);
            int z = Math.Min(X - Y, 256 - (X - Y)) * 2;
            if (z >= 256)
                return 255;
            else
                return (byte)z;
        }
        public static double RBG2Hue(byte r, byte g, byte b)
        {
            double max = Math.Max(Math.Max(r, g), b);
            double range = max - Math.Min(Math.Min(r, g), b);

            if (range == 0d) return 0d;

            double d = max / 2d;
            double R = (((max - r) / 6d) + d) / max;
            double G = (((max - g) / 6d) + d) / max;
            double B = (((max - b) / 6d) + d) / max;

            double H = 0;

                 if (R == max) H =             B - G;
            else if (G == max) H = (1d / 3d) + R - B;
            else               H = (2d / 3d) + B - R;
            
            return H;
        }

        public static void Run()
        {
            Parallel.For(1, 5, (n) =>
            {
                using (var mi = new MagickImage(@"E:\" + n.ToString() + @".png"))
                {
                    mi.Equalize();
                    foreach (ColorSpace cs in (ColorSpace[])Enum.GetValues(typeof(ColorSpace)))
                    {
                        using (var mi1 = mi.Clone())
                        {
                            mi1.ColorSpace = cs;
                            var mi2 = mi1.Separate();
                            for (int i = 0; i < mi2.Count(); i++)
                            {
                                using (var mi3 = mi2.ToList()[i])
                                {
                                    //mi3.FilterType = FilterType.Triangle;
                                    //mi3.Resize(new Percentage(50));
                                    mi3.Write(@"E:\" + n.ToString() + @"\" + cs.ToString() + "_" + (i + 1).ToString() + ".png");
                                }
                            }
                        }
                    }
                }
            });
            
            DirectoryInfo d = new DirectoryInfo(@"E:\1");
            FileInfo[] files = d.GetFiles("*.png");

            var b = new ConcurrentBag<Bag3>();

            Parallel.For(0, files.Count(), (i) =>
            {
                var name = files[i].Name;
                var mc = new MagickImageCollection();
                var mi1 = new MagickImage(@"E:\1\" + name);
                var mi2 = new MagickImage(@"E:\2\" + name);
                var mi3 = new MagickImage(@"E:\3\" + name);
                var mi4 = new MagickImage(@"E:\4\" + name);
                mi1.Crop(1409, 940, Gravity.Southeast);
                mi2.Crop(1409, 940, Gravity.Southeast);
                mi3.Crop(1409, 940, Gravity.Southeast);
                mi4.Crop(1409, 940, Gravity.Southeast);
                mi1.RePage(); mi1.Equalize(); mi1.RePage();
                mi2.RePage(); mi2.Equalize(); mi2.RePage();
                mi3.RePage(); mi3.Equalize(); mi3.RePage();
                mi4.RePage(); mi4.Equalize(); mi4.RePage();
                mc.Add(mi1);
                mc.Add(mi2);
                mc.Add(mi3);
                mc.Add(mi4);
                var mc1 = mc.Clone();
                var mc2 = mc.Clone();
                var mi5 = mc1.Evaluate(EvaluateOperator.Min);
                var mi6 = mc2.Evaluate(EvaluateOperator.Max);
                var v = mi5.Compare(mi6, ErrorMetric.MeanSquared);
                b.Add(new Bag3(name, v));
                mi5.Composite(mi6, CompositeOperator.Difference);
                mi5.Write(@"E:\6\" + name);
            });

            var c1 = b.ToList().OrderByDescending(x => x.Value);
            var d1 = b.First();

            var c2 = b.ToList().OrderBy(x => x.Value);
            var d2 = b.First();

        }
    }

    public struct Bag3
    {
        public string Name;
        public double Value;
        public Bag3(string name, double value)
        {
            Name = name;
            Value = value;
        }
    }

    class Test2
    {


        public static Geometry Run(Bitmap needle, Bitmap haystack, double retryThreshold = 1, int retryLimit = 10)
        {
            IntPoint[] harrisPoints1;
            IntPoint[] harrisPoints2;
            IntPoint[] correlationPoints1;
            IntPoint[] correlationPoints2;
            MatrixH homography;

            var mi1 = new MagickImage(needle);   mi1.Equalize(); needle   = mi1.ToBitmap();
            var mi2 = new MagickImage(haystack); mi2.Equalize(); haystack = mi2.ToBitmap();
            //MagickImage mi1;
            //MagickImage mi2;

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

            // Concatenate the two images in a single image (just to show on screen)
            Concatenate concat = new Concatenate(needle);
            Bitmap img3 = concat.Apply(haystack);

            // Show the marked correlations in the concatenated image
            PairsMarker pairs = new PairsMarker(
                inliers1, // Add image1's width to the X points to show the markings correctly
                inliers2.Apply(p => new IntPoint(p.X + needle.Width, p.Y)));

            var Image = pairs.Apply(img3);
            Image.Save(@"E:\ayy.png");

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

            mi1 = new MagickImage(needle);
            mi2 = new MagickImage(haystack);
            mi1.Resize(new MagickGeometry((int)Math.Round(width), (int)Math.Round(height)) { IgnoreAspectRatio = true });
            var mg = new MagickGeometry((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height)) { IgnoreAspectRatio = true };
            mi2.Extent(mg, Gravity.Northwest, MagickColor.FromRgba(0, 0, 0, 0));

            double delta = mi1.Compare(mi2, ErrorMetric.NormalizedCrossCorrelation);

            Geometry outGeo = new Geometry(x, y, width, height);

            if (delta < retryThreshold && retryLimit > 0)
            {
                retryLimit--;
                outGeo = Run(needle, haystack, delta, retryLimit);
            }

            return outGeo;

            /*
            var q = new MagickImage(needle);
            var geo1 = new MagickGeometry((int)Math.Round(width), (int)Math.Round(height)) { IgnoreAspectRatio = true };
            q.Resize(geo1);
            q.RePage();
            q.Write(@"E:\test2.png");

            var w = new MagickImage(haystack);
            var geo2 = new MagickGeometry((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height)) { IgnoreAspectRatio = true };
            w.Extent(geo2, Gravity.Northwest, MagickColor.FromRgba(0, 0, 0, 0));
            w.RePage();
            w.Write(@"E:\test3.png");

            var cropGeo = new MagickGeometry(
                (int)Math.Round(width * 0.1),
                (int)Math.Round(height * 0.1),
                (int)Math.Round(width * 0.8),
                (int)Math.Round(height * 0.8)) { IgnoreAspectRatio = true };

            w.Composite(q, CompositeOperator.CopyAlpha);
            q.Composite(w, CompositeOperator.CopyAlpha);
            q.Alpha(AlphaOption.Set);
            w.Alpha(AlphaOption.Set);
            q.ColorAlpha(MagickColor.FromRgb(255, 0, 255));
            w.ColorAlpha(MagickColor.FromRgb(255, 0, 255));

            q.Crop(cropGeo, Gravity.Northwest);
            q.Equalize();
            q.RePage();
            q.Write(@"E:\test4.png");
            w.Crop(cropGeo, Gravity.Northwest);
            w.Equalize();
            w.RePage();
            w.Write(@"E:\test5.png");


            var outcome = q.Compare(w, ErrorMetric.MeanSquared);

            q.Composite(w, CompositeOperator.Difference);
            q.Write(@"E:\test6.png");
            */

        }

    }

    /*
    class Test
    {
        public const double frameRate = 60;
        public const long waitTicks = (long)(10000000 / frameRate);
        public const int loopCount = 10;
        public const int frameCount = (int)frameRate * loopCount;
        public const double timeIndexMultiplier = 1000d / 60d;

        public static int curIndex = 0;
        public ConcurrentBag<Bag> DeltaBag = new ConcurrentBag<Bag>();
        public List<Bag2> UntitledBag = new List<Bag2>();
        public static readonly MagickImage deltaImage = new MagickImage(@"C:\Users\Administrator\Desktop\Untitled.bmp");
        public double total = 0;
        public double total2 = 0;
        public double total3 = 0;

        public Geometry Geo;
        public bool UseInterlace;

        public bool isActive = true;

        public void CopyFromScreen(Geometry geo, bool useInterlace = false)
        {
            Geo = geo;
            UseInterlace = useInterlace;
            isActive = true;
            //Test1();
            //Test2();
            var t = new Stopwatch();
            t.Start();
            //Parallel.Invoke(() => { Test2(); }, () => { Test1(); });
            deltaImage.Equalize();
            Parallel.Invoke(() => { Test2(); }, () => { CopyFromDevice(); });
            t.Stop();
            var a = (double)t.ElapsedMilliseconds / loopCount / 1000;
            var b = total / loopCount / 1000;
            var c = total2 / loopCount / 1000;

            var d = total3 / loopCount / 1000;
            var e = d / c;
            isActive = false;
        }

        public void CopyFromDevice()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
            videoSource.Start();
        }

        public int counttest = 0;
        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = eventArgs.Frame;

            if (counttest % 300 == 0)
            {
                bitmap.Save(@"E:\test.png");
            }

            UntitledBag.Add(new Bag2((Bitmap)bitmap.Clone(), counttest));
            counttest++;

        }

        public struct Bag2
        {
            public Bitmap Bitmap;
            public int Index;
            public Bag2(Bitmap bitmap, int i)
            {
                Bitmap = bitmap;
                Index = i;
            }
            // Hacky
            public bool IsDisposed()
            {
                try
                {
                    var x = Bitmap.Height;
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        public void Test1()
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < frameCount; i++)
            {
                var timer2 = DateTime.UtcNow;
                Bitmap b = new Bitmap((int)Geo.Width, (int)Geo.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.CopyFromScreen((int)Geo.X, (int)Geo.Y, 0, 0, Geo.Size.ToDrawing(), CopyPixelOperation.SourceCopy);
                }
                UntitledBag.Add(new Bag2(b, i));
                var t3 = DateTime.UtcNow.Subtract(timer2);
                total += t3.TotalMilliseconds;
                if (t3.Ticks < waitTicks)
                {
                    Thread.Sleep(TimeSpan.FromTicks(waitTicks - t3.Ticks));
                }
            }
            timer.Stop();
            isActive = false;
        }

        public void Test2()
        {
            do
            {

                var bagCount = UntitledBag.Count;
                var t = new Stopwatch();
                t.Start();
                Parallel.For(curIndex, bagCount, (i) =>
                //for(int i = curIndex; i < bagCount; i++)
                {
                    var t3 = DateTime.UtcNow;
                    var bag = UntitledBag[i];
                    if (!bag.IsDisposed())
                    {
                        using (var fileImageCompare = new MagickImage(bag.Bitmap))
                        {
                            fileImageCompare.Trim();
                            if (deltaImage.HasAlpha)
                            {
                                fileImageCompare.Composite(deltaImage, CompositeOperator.CopyAlpha);
                            }
                            fileImageCompare.Equalize();
                            var delta = (float)deltaImage.Compare(fileImageCompare, ErrorMetric.NormalizedCrossCorrelation);

                            if (i % 300 == 0)
                            {
                                fileImageCompare.Write(@"E:\test2.png");
                                deltaImage.Write(@"E:\test3.png");
                            }

                            int id = (int)Math.Round(bag.Index * timeIndexMultiplier);
                            DeltaBag.Add(new Bag(id, delta, null));
                            bag.Bitmap.Dispose();
                        }
                    }
                    var t4 = DateTime.UtcNow.Subtract(t3);
                    total3 += t4.TotalMilliseconds;
                //}
                });
                t.Stop();
                if (bagCount > curIndex)
                    total2 += t.ElapsedMilliseconds;
                curIndex = bagCount;
                // So that it doesn't hog a whole core when waiting for more images.
                Thread.Sleep(1);
            } while (isActive || curIndex != UntitledBag.Count);
        }

    }*/
}
