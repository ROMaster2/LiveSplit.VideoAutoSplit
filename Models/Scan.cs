using System;

namespace LiveSplit.VAS.Models
{
    public struct Scan
    {
        public readonly Frame CurrentFrame;
        public readonly Frame PreviousFrame;
        public bool IsDisposed;

        public Scan(Frame currentFrame, Frame previousFrame)
        {
            CurrentFrame = currentFrame;
            PreviousFrame = previousFrame;
            IsDisposed = false;
        }

        public static readonly Scan Blank = new Scan(Frame.Blank, Frame.Blank);

        public TimeSpan TimeDelta()
        {
            return CurrentFrame.TimeStamp - PreviousFrame.TimeStamp;
        }

        public void Dispose()
        {
            IsDisposed = true;
            CurrentFrame.Bitmap.Dispose();
            PreviousFrame.Bitmap.Dispose();
        }
    }
}
