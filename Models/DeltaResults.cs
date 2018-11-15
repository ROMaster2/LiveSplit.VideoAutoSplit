using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveSplit.Model;
using LiveSplit.VAS.Models;

namespace LiveSplit.VAS.Models
{
    public class DeltaResults : EventArgs
    {
        public readonly int Index;
        public readonly DateTime FrameStart;
        public readonly DateTime FrameEnd;
        public readonly DateTime ScanEnd;
        public DateTime? WaitEnd;
        public readonly double[] Deltas;
        public readonly double[] Benchmarks;

        public DeltaResults(int index, DateTime frameStart, DateTime frameEnd, DateTime scanEnd, double[] deltas, double[] benchmarks)
        {
            Index = index;
            FrameStart = frameStart;
            FrameEnd = frameEnd;
            ScanEnd = scanEnd;
            WaitEnd = null;
            Deltas = deltas;
            Benchmarks = benchmarks;
        }

        public DeltaResults(int index, Scan scan, DateTime scanEnd, double[] deltas, double[] benchmarks)
        {
            Index = index;
            FrameStart = scan.PreviousFrame.DateTime;
            FrameEnd = scan.CurrentFrame.DateTime;
            ScanEnd = scanEnd;
            WaitEnd = null;
            Deltas = deltas;
            Benchmarks = benchmarks;
        }

        public TimeSpan FrameDuration
        {
            get
            {
                return FrameEnd - FrameStart;
            }
        }

        public TimeSpan ScanDuration
        {
            get
            {
                return ScanEnd - FrameEnd;
            }
        }

        public TimeSpan? WaitDuration
        {
            get
            {
                return WaitEnd - ScanEnd;
            }
        }

        public TimeSpan? ProcessDuration
        {
            get
            {
                return WaitEnd - FrameEnd;
            }
        }
    }
}
