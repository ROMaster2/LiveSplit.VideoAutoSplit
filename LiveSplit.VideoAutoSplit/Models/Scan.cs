namespace LiveSplit.VAS.Models
{
    public struct Scan
    {
        public readonly Frame CurrentFrame;
        public readonly Frame PreviousFrame;
        public readonly bool HasPreviousFrame;
        public static readonly Scan Blank = new Scan(Frame.Blank, Frame.Blank, false);

        public Scan(Frame currentFrame, Frame previousFrame, bool usePreviousFrame)
        {
            CurrentFrame = currentFrame;
            PreviousFrame = previousFrame;
            HasPreviousFrame = usePreviousFrame || !previousFrame.IsBlank;
        }

        public void Dispose()
        {
            PreviousFrame.Bitmap?.Dispose();
        }
    }
}
