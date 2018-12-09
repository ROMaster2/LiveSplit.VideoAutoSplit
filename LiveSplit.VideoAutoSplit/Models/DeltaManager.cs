﻿using System;
using System.Threading;
using LiveSplit.Model;

namespace LiveSplit.VAS.Models.Delta
{
    public class DeltaManager
    {
        internal CompiledFeatures CompiledFeatures { get; private set; }

        public DeltaHistory History { get; private set; }

        // Todo: Let scripts modify this.
        public int DefaultOffset { get; set; } = 100; // 100 milliseconds.

        public DeltaManager(CompiledFeatures compiledFeatures, int capacity)
        {
            CompiledFeatures = compiledFeatures;
            History = new DeltaHistory(capacity);
        }

        public bool AddResult(
            int index,
            DateTime frameStart,
            DateTime frameEnd,
            DateTime scanEnd,
            double[] deltas,
            double[] benchmarks)
        {
            if (index >= History.Count)
            {
                int curIndex = index % History.Count;
                int prevIndex = (index - 1) % History.Count;
                int i = 0;
                while (History[prevIndex].IsBlank || History[prevIndex].Index != index - 1)
                {
                    if (curIndex - History.Count >= index || i >= 5000)
                    {
                        return false;
                        //throw new Exception("Previous frame could not be processed or is taking too long to process.");
                    }
                    Thread.Sleep(1);
                    i++;
                }
            }

            var waitEnd = TimeStamp.CurrentDateTime.Time;

            History.AddResult(index, frameStart, frameEnd, scanEnd, waitEnd, deltas, benchmarks);
            return true;
        }

        public void AddResult(int index, Scan scan, DateTime scanEnd, double[] deltas, double[] benchmarks)
            => AddResult(index, scan.PreviousFrame.DateTime, scan.CurrentFrame.DateTime, scanEnd, deltas, benchmarks);
    }
}
