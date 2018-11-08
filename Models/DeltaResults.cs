using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveSplit.Model;
using LiveSplit.VAS.Models;

namespace LiveSplit.VAS.Models
{
    public delegate void DeltaResultsHandler(DeltaResults e);

    public class DeltaResults : EventArgs
    {
        public readonly int Index;
        public readonly TimeStamp FrameStart;
        public readonly TimeStamp FrameEnd;
        public readonly TimeStamp ScanEnd;
        public readonly TimeStamp WaitEnd;
        public readonly double[] Deltas;

        public DeltaResults(int index, TimeStamp frameStart, TimeStamp frameEnd, TimeStamp scanEnd, TimeStamp waitEnd, double[] deltas)
        {
            Index = index;
            FrameStart = frameStart;
            FrameEnd = frameEnd;
            ScanEnd = scanEnd;
            WaitEnd = waitEnd;
            Deltas = deltas;
        }

        public DeltaResults(int index, Scan scan, TimeStamp scanEnd, TimeStamp waitEnd, double[] deltas)
        {
            Index = index;
            FrameStart = scan.PreviousFrame.TimeStamp;
            FrameEnd = scan.CurrentFrame.TimeStamp;
            ScanEnd = scanEnd;
            WaitEnd = waitEnd;
            Deltas = deltas;
        }

        public DeltaResults()
        {
            Index = -1;
            FrameStart = FrameEnd = ScanEnd = WaitEnd = TimeStamp.Now;
            Deltas = new double[Scanner.FEATURE_COUNT_LIMIT];
        }

        public TimeSpan FrameDuration()
        {
            return FrameEnd - FrameStart;
        }

        public TimeSpan ScanDuration()
        {
            return ScanEnd - FrameEnd;
        }

        public TimeSpan WaitDuration()
        {
            return WaitEnd - ScanEnd;
        }

        public TimeSpan ProcessDuration()
        {
            return WaitEnd - FrameEnd;
        }
    }
}
