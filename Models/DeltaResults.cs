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
        public readonly TimeStamp FrameStart;
        public readonly TimeStamp FrameEnd;
        public readonly TimeStamp ScanEnd;
        public TimeStamp WaitEnd;
        public readonly double[] Deltas;

        public DeltaResults(int index, TimeStamp frameStart, TimeStamp frameEnd, TimeStamp scanEnd, double[] deltas)
        {
            Index = index;
            FrameStart = frameStart;
            FrameEnd = frameEnd;
            ScanEnd = scanEnd;
            WaitEnd = null;
            Deltas = deltas;
        }

        public DeltaResults(int index, Scan scan, TimeStamp scanEnd, double[] deltas)
        {
            Index = index;
            FrameStart = scan.PreviousFrame.TimeStamp;
            FrameEnd = scan.CurrentFrame.TimeStamp;
            ScanEnd = scanEnd;
            WaitEnd = null;
            Deltas = deltas;
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

        // No handling WaitEnd != null for now.
        public TimeSpan WaitDuration
        {
            get
            {
                return WaitEnd - ScanEnd;
            }
        }

        public TimeSpan ProcessDuration
        {
            get
            {
                return WaitEnd - FrameEnd;
            }
        }
    }
}
