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
        public TimeStamp FrameStart;
        public TimeStamp FrameEnd;
        public TimeStamp ScanEnd;
        public double[] Deltas;

        public DeltaResults(TimeStamp frameStart, TimeStamp frameEnd, TimeStamp scanEnd, double[] deltas)
        {
            FrameStart = frameStart;
            FrameEnd = frameEnd;
            ScanEnd = scanEnd;
            Deltas = deltas;
        }

        public DeltaResults(Scan scan, TimeStamp scanEnd, double[] deltas)
        {
            FrameStart = scan.PreviousFrame.TimeStamp;
            FrameEnd = scan.CurrentFrame.TimeStamp;
            ScanEnd = scanEnd;
            Deltas = deltas;
        }

        public TimeSpan FrameDuration()
        {
            return FrameEnd - FrameStart;
        }

        public TimeSpan ScanDuration()
        {
            return ScanEnd - FrameEnd;
        }
    }
}
