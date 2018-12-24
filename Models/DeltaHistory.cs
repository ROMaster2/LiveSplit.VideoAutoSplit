using System;
using System.Collections;
using System.Collections.Generic;

namespace LiveSplit.VAS.Models.Delta
{
    public class DeltaHistory : IReadOnlyList<DeltaResult>
    {
        private DeltaResult[] History;

        public int Count => History.Length;

        public DeltaResult this[int index] => History[index];

        public double this[int index, int featureIndex] => History[index].Deltas[featureIndex];

        public DeltaHistory(int capacity)
        {
            History = new DeltaResult[capacity];
            for (int i = 0; i < capacity; i++)
            {
                History[i] = DeltaResult.Blank;
            }
        }

        internal void AddResult(
            int index,
            DateTime frameStart,
            DateTime frameEnd,
            DateTime scanEnd,
            DateTime waitEnd,
            double[] deltas,
            double[] benchmarks)
        {
            var currIndex = index % Count;
            var prevIndex = (index - 1) % Count;

            var dr = new DeltaResult(index, frameStart, frameEnd, scanEnd, waitEnd, deltas, benchmarks);
            History[currIndex] = dr;
        }

        public IEnumerator<DeltaResult> GetEnumerator()
        {
            foreach (var result in History)
            {
                yield return result;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
