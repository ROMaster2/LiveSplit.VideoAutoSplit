using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace LiveSplit.VAS
{
    public static class Utilities
    {
        public static void Error(int id)
        {
            DialogResult dr = MessageBox.Show(
                "This shouldn't happen. Error code: " +
                    id.ToString() +
                    "\r\n" +
                    "If you could, report this to [Link] with details on how you got this, it'd really help.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.None
                );
        }

        public static void Flicker(Control form, int milliseconds, Color color)
        {
            var origColor = SystemColors.Window;
            form.BackColor = color;
            var t = new System.Windows.Forms.Timer()
            {
                Interval = milliseconds
            };
            t.Tick += (a, b) => form.BackColor = origColor;
            t.Start();
        }

        public static string PrefixNumber(decimal number, int precision = 2, string specifier = "G")
        {
            number = Math.Round(number, precision);
            var str = number.ToString(specifier, CultureInfo.CurrentCulture);
            return number >= 0 ? "+" + str : "-" + str;
        }

        public static bool GetDiskSpace(string directory, out long totalSpace, out long freeSpace)
        {
            var directoryRoot = Directory.GetDirectoryRoot(directory);
            bool isAvailable = false;
            totalSpace = -1L;
            freeSpace = -1L;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.Name == directoryRoot)
                {
                    totalSpace = drive.TotalSize;

                    if (drive.IsReady)
                    {
                        freeSpace = drive.AvailableFreeSpace;
                        isAvailable = true;
                    }
                    break;
                }
            }
            return isAvailable;
        }

        /// There's no need to worry about specific CPUs.
        /// <summary>
        /// Set the number of CPUs the program is allowed to use.
        /// </summary>
        /// <param name="processorLimit"></param>
        public static void SetCPULimit(int processorLimit)
        {
            if (processorLimit < 0)
            {
                processorLimit = Environment.ProcessorCount + processorLimit;
            }

            if (processorLimit <= 0 || processorLimit > Environment.ProcessorCount)
            {
                processorLimit = Environment.ProcessorCount;
            }

            processorLimit = (int)Math.Pow(processorLimit, 2);

            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)processorLimit;
        }

        /// <summary>
        /// Set the priority of the program.
        /// </summary>
        /// <param name="processorLimit"></param>
        public static void SetPriority(ProcessPriorityClass priority)
        {
            Process.GetCurrentProcess().PriorityClass = priority;
        }

        public static double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Any())
            {
                //Compute the Average
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together
                ret = Math.Sqrt(sum / (values.Count() - 1));
            }
            return ret;
        }

        public static decimal DivideString(string str)
        {
            var s = str.Split('/');
            decimal i = decimal.Parse(s[0]);
            for (var n = 1; n < s.Length; n++)
            {
                i /= decimal.Parse(s[n]);
            }
            return i;
        }

        public static bool ValidateTimeOCR(string text)
        {
            return Regex.IsMatch(text, @"^-?((((\d?\d:)?[0-5])?\d:)?[0-5])?\d(\.\d\d?)?$");
        }

        public static TimeSpan TimeStringToTimeSpan(string text, bool require = false)
        {
            if (!ValidateTimeOCR(text))
            {
                if (require)
                {
                    throw new ArgumentException("String is not a time.");
                }
                else
                {
                    return TimeSpan.MinValue;
                }
            }

            var isNegative = false;
            var v = new int[5] { 0, 0, 0, 0, 0 };

            if (text.Contains('-'))
            {
                isNegative = true;
                text = text.Replace("-", "");
            }

            if (text.Contains('.'))
            {
                v[4] = int.Parse(text.Split('.')[1].PadRight(3, '0'));
                text = text.Split('.')[0];
            }

            text = "000:00:00".Substring(0, 9 - text.Length) + text;

            v[3] = int.Parse(text.Substring(7, 2));
            v[2] = int.Parse(text.Substring(4, 2));
            v[1] = int.Parse(text.Substring(0, 3));
            // TimeSpan can't accept times over 24 hours, so turn the excess into days.
            v[0] = v[1] / 24;
            v[1] %= 24;

            TimeSpan result = new TimeSpan(v[0], v[1], v[2], v[3], v[4]);

            if (isNegative)
            {
                result = result.Negate();
            }
            return result;
        }

        public static int LeastCommonMultiple(int a, int b)
        {
            int num1, num2;
            if (a > b)
            {
                num1 = a; num2 = b;
            }
            else
            {
                num1 = b; num2 = a;
            }

            for (int i = 1; i < num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }

        private static int GreatestCommonDivisor(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }

            return a == 0 ? b : a;
        }

        public static string ToFraction(int a, int b, bool shrink = false)
        {
            if (shrink)
            {
                int gcd = GreatestCommonDivisor(a, b);
                a /= gcd;
                b /= gcd;
            }
            return a.ToString() + "/" + b.ToString();
        }

        public static byte[] FloatToBytes(float num)
        {
            var bytes = new byte[2];
            bytes[0] = (byte)Math.Max(Math.Min(Math.Floor(num), 255), 0);
            bytes[1] = (byte)Math.Floor(Math.Max(Math.Min(num - bytes[0], 255f / 256f), 0) * 256);
            return bytes;
        }

        public static float BytesToFloat(byte num1, byte num2)
        {
            return num1 + (num2 / 256f);
        }

        public static void LightSleep(Func<bool> func, double timeoutLimit = 5000d, int millisecondsTimeout = 1)
        {
            var breakTime = DateTime.UtcNow.AddMilliseconds(timeoutLimit);

            while (!func() && DateTime.UtcNow < breakTime)
            {
                Thread.Sleep(millisecondsTimeout);
            }
        }
    }

    [Flags]
    public enum ThreadAccess : int
    {
        NONE = 0,
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200)
    }

    public static class ProcessExtension
    {
        public static double StdDev(this IEnumerable<int> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }

        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        private static extern int ResumeThread(IntPtr hThread);

        public static void Suspend(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
            }
        }

        public static void Resume(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(pOpenThread);
            }
        }
    }
}
