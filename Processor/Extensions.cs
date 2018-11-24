using System;
using System.Linq;
using System.Collections.Generic;

namespace LiveSplit.VAS
{
    public static partial class Extensions
    {
        public static double TransparencyRate(this ImageMagick.IMagickImage mi)
        {
            if (!mi.HasAlpha) return 0;
            var bytes = mi.Separate(ImageMagick.Channels.Alpha).First().GetPixels().GetValues();
            return (255d - bytes.Average(x => (double)x)) / 255d;
        }

        public static System.Windows.Point ToWindows(this System.Drawing.Point point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }

        public static System.Drawing.Point ToDrawing(this System.Windows.Point point, int rounding = 0)
        {
            const double rounder = 0.4999999999; // Difficult to be precise without posible error.
            int x;
            int y;

            switch (rounding)
            {
                case 2: // Ceiling
                    x = (int)Math.Ceiling(point.X);
                    y = (int)Math.Ceiling(point.Y);
                    break;
                case 1: // Roundup
                    x = (int)Math.Round(point.X + (Math.Sign(point.X) > 0 ? rounder : -rounder));
                    y = (int)Math.Round(point.Y + (Math.Sign(point.Y) > 0 ? rounder : -rounder));
                    break;
                case 0: // Round
                default:
                    x = (int)Math.Round(point.X);
                    y = (int)Math.Round(point.Y);
                    break;
                case -1: // Rounddown
                    x = (int)Math.Round(point.X + (Math.Sign(point.X) < 0 ? rounder : -rounder));
                    y = (int)Math.Round(point.Y + (Math.Sign(point.Y) < 0 ? rounder : -rounder));
                    break;
                case -2: // Floor
                    x = (int)Math.Floor(point.X);
                    y = (int)Math.Floor(point.Y);
                    break;
            }

            return new System.Drawing.Point(x, y);
        }

        public static System.Windows.Size ToWindows(this System.Drawing.Size size)
        {
            return new System.Windows.Size(size.Width, size.Height);
        }

        public static System.Drawing.Size ToDrawing(this System.Windows.Size size, int rounding = 0)
        {
            const double rounder = 0.4999999999; // Difficult to be precise without posible error.
            int x;
            int y;

            switch (rounding)
            {
                case 2: // Ceiling
                    x = (int)Math.Ceiling(size.Width);
                    y = (int)Math.Ceiling(size.Height);
                    break;
                case 1: // Roundup
                    x = (int)Math.Round(size.Width + (Math.Sign(size.Width) > 0 ? rounder : -rounder));
                    y = (int)Math.Round(size.Height + (Math.Sign(size.Height) > 0 ? rounder : -rounder));
                    break;
                case 0: // Round
                default:
                    x = (int)Math.Round(size.Width);
                    y = (int)Math.Round(size.Height);
                    break;
                case -1: // Rounddown
                    x = (int)Math.Round(size.Width + (Math.Sign(size.Width) < 0 ? rounder : -rounder));
                    y = (int)Math.Round(size.Height + (Math.Sign(size.Height) < 0 ? rounder : -rounder));
                    break;
                case -2: // Floor
                    x = (int)Math.Floor(size.Width);
                    y = (int)Math.Floor(size.Height);
                    break;
            }

            return new System.Drawing.Size(x, y);
        }

        // Population, not sample
        private static double StdDev(this IEnumerable<double> values)
        {
            double result = 0;
            int count = values.Count();
            if (count > 0)
            {
                double avg = values.Average();
                double sum = values.Sum(x => Math.Pow(x - avg, 2));
                result = Math.Sqrt(sum / count);
            }
            return result;
        }

    }
}
