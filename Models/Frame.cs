using System;
using System.Drawing;
using LiveSplit.Model;

namespace LiveSplit.VAS.Models
{
    public struct Frame // Bad name idea?
    {
        public readonly DateTime DateTime;
        public readonly Bitmap Bitmap;

        public Frame(DateTime dateTime, Bitmap bitmap)
        {
            DateTime = dateTime;
            Bitmap = bitmap;
        }

        public static readonly Frame Blank = new Frame(TimeStamp.CurrentDateTime.Time, new Bitmap(1, 1));

        public Frame Clone()
        {
            return new Frame(DateTime, (Bitmap)Bitmap.Clone());
        }
    }
}
