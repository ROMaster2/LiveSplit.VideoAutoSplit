using System;
using System.Collections;
using System.Collections.Generic;

namespace LiveSplit.VAS.Models.Delta
{
    public class DeltaHistory : IReadOnlyList<DeltaResult>
    {
        public int Count => _History.Length;
        public double this[int index, int featureIndex] => _History[index].Deltas[featureIndex];
        public DeltaResult this[int index] => _History[index];

        private readonly DeltaResult[] _History;

        public DeltaHistory(int capacity)
        {
            _History = new DeltaResult[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _History[i] = DeltaResult.Blank;
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

            _History[currIndex] = new DeltaResult(index, frameStart, frameEnd, scanEnd, waitEnd, deltas, benchmarks);
        }

        // @TODO: Are you sure this does what you expect it to do?
        public IEnumerator<DeltaResult> GetEnumerator()
        {
            foreach (var result in _History)
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
