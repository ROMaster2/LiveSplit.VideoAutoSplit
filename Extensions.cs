using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LiveSplit.VAS
{
    public static partial class Extensions
    {
        public static double GetTransparencyRate(this ImageMagick.IMagickImage mi)
        {
            if (!mi.HasAlpha) return 0d;
            var bytes = mi.Separate(ImageMagick.Channels.Alpha).First().GetPixels().GetValues();
            return (byte.MaxValue - bytes.Average(x => (double)x)) / byte.MaxValue;
        }

        public static System.Windows.Point ToWindows(this System.Drawing.Point point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }

        public static System.Drawing.Point ToDrawing(this System.Windows.Point point, RoundingType rounding = RoundingType.Round)
        {
            var x = Round(point.X, rounding);
            var y = Round(point.Y, rounding);
            return new System.Drawing.Point(x, y);
        }

        public static System.Windows.Size ToWindows(this System.Drawing.Size size)
        {
            return new System.Windows.Size(size.Width, size.Height);
        }

        public static System.Drawing.Size ToDrawing(this System.Windows.Size size, RoundingType rounding = RoundingType.Round)
        {
            var width  = Round(size.Width,  rounding);
            var height = Round(size.Height, rounding);
            return new System.Drawing.Size(width, height);
        }

        // Population, not sample
        public static double StdDev(this IEnumerable<double> values)
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

        public static T Clone<T>(this T controlToClone) where T : System.Windows.Forms.Control
        {
            PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            T instance = Activator.CreateInstance<T>();

            foreach (PropertyInfo propInfo in controlProperties)
            {
                if (propInfo.CanWrite)
                {
                    if (propInfo.Name != "WindowTarget")
                        propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
                }
            }

            return instance;
        }

        public static int Round(double num, RoundingType rounding = RoundingType.Round)
        {
            const double rounder = 0.4999999999; // Difficult to be precise without posible error.
            int result;

            switch (rounding)
            {
                case RoundingType.Ceiling:
                    result = (int)Math.Ceiling(num);
                    break;
                case RoundingType.Up:
                    result = (int)Math.Round(num + (Math.Sign(num) > 0 ? rounder : -rounder));
                    break;
                case RoundingType.Round:
                    result = (int)Math.Round(num);
                    break;
                case RoundingType.Down:
                    result = (int)Math.Round(num + (Math.Sign(num) < 0 ? rounder : -rounder));
                    break;
                case RoundingType.Floor:
                    result = (int)Math.Floor(num);
                    break;
                default:
                    throw new ArgumentException();
            }

            return result;
        }

        public enum RoundingType
        {
            Ceiling,
            Up,
            Round,
            Down,
            Floor
        }
    }
}
