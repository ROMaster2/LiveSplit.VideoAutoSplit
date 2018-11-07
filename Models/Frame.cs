using System;
using System.Drawing;

namespace LiveSplit.VAS.Models
{
    public struct Frame // Bad name idea?
    {
        public readonly long Timestamp;
        public Bitmap Bitmap;

        public Frame(long timestamp, Bitmap bitmap)
        {
            Timestamp = timestamp;
            Bitmap = bitmap;
        }

        public static readonly Frame Blank = new Frame(0, new Bitmap(1, 1));

        public Frame Clone()
        {
            return new Frame(Timestamp, (Bitmap)Bitmap.Clone());
        }
    }
}
