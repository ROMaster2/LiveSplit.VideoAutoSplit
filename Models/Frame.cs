using System;
using System.Drawing;
using LiveSplit.Model;

namespace LiveSplit.VAS.Models
{
    public struct Frame // Bad name idea?
    {
        public readonly TimeStamp TimeStamp;
        public readonly Bitmap Bitmap;

        public Frame(TimeStamp timeStamp, Bitmap bitmap)
        {
            TimeStamp = timeStamp;
            Bitmap = bitmap;
        }

        public static readonly Frame Blank = new Frame(TimeStamp.Now, new Bitmap(1, 1));

        public Frame Clone()
        {
            return new Frame(TimeStamp, (Bitmap)Bitmap.Clone());
        }
    }
}
