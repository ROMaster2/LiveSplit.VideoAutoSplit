using System;

namespace LiveSplit.VAS.Models.Delta
{
    public struct DeltaResult
    {
        public int Index { get; private set; }
        public DateTime FrameStart { get; private set; }
        public DateTime FrameEnd { get; private set; }
        public DateTime ScanEnd { get; private set; }
        public DateTime WaitEnd { get; private set; }
        public double[] Deltas { get; private set; }
        public double[] Benchmarks { get; private set; }

        public DeltaResult(
            int index,
            DateTime frameStart,
            DateTime frameEnd,
            DateTime scanEnd,
            DateTime waitEnd,
            double[] deltas,
            double[] benchmarks)
        {
            Index = index;
            FrameStart = frameStart;
            FrameEnd = frameEnd;
            ScanEnd = scanEnd;
            WaitEnd = waitEnd;
            Deltas = deltas;
            Benchmarks = benchmarks;
        }

        public static readonly DeltaResult Blank =
            new DeltaResult(-1, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, null, null);

        public bool IsBlank => Index == -1;

        public TimeSpan FrameDuration => FrameEnd - FrameStart;
        public TimeSpan ScanDuration => ScanEnd - FrameEnd;
        public TimeSpan WaitDuration => WaitEnd - ScanEnd;
        public TimeSpan ProcessDuration => WaitEnd - FrameEnd;

        // Todo: Add method(s) for getting Frame timestamps rounded to the framerate.
    }
}
