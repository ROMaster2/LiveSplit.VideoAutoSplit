namespace LiveSplit.VFM.Models
{
    public struct Scan
    {
        public Frame CurrentFrame;
        public Frame PreviousFrame;
        public bool IsCleaned;
        public long TimeDelta;

        public Scan(Frame currentFrame, Frame previousFrame)
        {
            CurrentFrame = currentFrame;
            PreviousFrame = previousFrame;
            IsCleaned = false;
            TimeDelta = CurrentFrame.Timestamp - PreviousFrame.Timestamp;
        }

        public static readonly Scan Blank = new Scan(Frame.Blank, Frame.Blank);

        public void Update(Frame newFrame)
        {
            PreviousFrame.Bitmap.Dispose();
            PreviousFrame = CurrentFrame;
            CurrentFrame = newFrame;
        }

        public void Clean()
        {
            IsCleaned = true;
            CurrentFrame.Bitmap.Dispose();
            PreviousFrame.Bitmap.Dispose();
        }
    }
}
