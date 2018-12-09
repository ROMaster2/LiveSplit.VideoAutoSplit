using System;
using System.Drawing;
using LiveSplit.Model;

namespace LiveSplit.VAS.Models
{
    public struct Frame
    {
        public static readonly Frame Blank = new Frame(TimeStamp.CurrentDateTime.Time, null);

        public readonly DateTime DateTime;
        public readonly Bitmap Bitmap;
        public readonly bool IsBlank;

        public Frame(DateTime dateTime, Bitmap bitmap)
        {
            DateTime = dateTime;
            Bitmap = bitmap;
            IsBlank = (bitmap == null);
        }
    }
}
