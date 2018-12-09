using System;
using System.Linq;

namespace LiveSplit.VAS.Models.Delta
{
    public class DeltaOutput
    {
        private readonly DeltaManager Manager;

        public DeltaHistory History => Manager.History;
        public int HistorySize => History.Count;

        public int OriginalIndex { get; private set; }
        public int FrameIndex { get; private set; }
        public double FrameRate { get; private set; }

        private int[] _FeatureIndexes;

        private int[] FeatureIndexes
        {
            get
            {
                var featureIndexes = _FeatureIndexes;
                _FeatureIndexes = null;
                return featureIndexes;
            }
            set
            {
                _FeatureIndexes = value;
            }
        }

        internal DeltaOutput(DeltaManager manager, int index, double frameRate)
        {
            Manager = manager;
            OriginalIndex = index;
            FrameIndex = index % Manager.History.Count;
            FrameRate = frameRate;

            _FeatureIndexes = null;
        }

        // Name is questionable
        private int IndexFromOffset(int offset)
        {
            return (OriginalIndex - offset) % HistorySize;
        }

        // Name is questionable
        private int FrameOffset(int milliseconds)
        {
            try
            {
                return Math.Max(1, (int)Math.Round(FrameRate * milliseconds / 1000d));
            }
            catch (Exception e)
            {
                var millisecondOffsetLimit = (int)Math.Ceiling((HistorySize - 1) / FrameRate * 1000);
                if (milliseconds < 0)
                {
                    throw new IndexOutOfRangeException("Offset cannot be negative.");
                }
                else if (milliseconds > millisecondOffsetLimit)
                {
                    throw new IndexOutOfRangeException(
                        "Offset cannot exceed the history's count, which is currently " +
                        HistorySize.ToString() +
                        ", and this is trying to access previous frame #" +
                        Math.Round(FrameRate * milliseconds / 1000d).ToString() +
                        ".");
                }
                else
                {
                    throw;
                }
            }
        }

        private void FrameOffsets(int startMilliseconds, int duration, out int startOffset, out int endOffset)
        {
            if (startMilliseconds <= 0)
            {
                startOffset = 0;
                startMilliseconds = 0;
            }
            else
            {
                startOffset = FrameOffset(startMilliseconds);
            }

            if (duration <= 0)
            {
                endOffset = FrameOffset(startMilliseconds + Manager.DefaultOffset);
            }
            else
            {
                endOffset = FrameOffset(startMilliseconds + duration);
            }
        }

        private double[] GetDeltaRange(int startMilliseconds, int duration, params int[] indexes)
        {
            FrameOffsets(startMilliseconds, duration, out var startOffset, out var endOffset);

            var featureIndexes = indexes.Length == 0 ? FeatureIndexes : indexes;

            var result = new double[(endOffset - startOffset) * featureIndexes.Length];

            if (result.Length == 0)
            {
                return new double[] { double.NaN };
            }

            for (int i = 0; i < endOffset - startOffset; i++)
            {
                var frameIndex = IndexFromOffset(i + startOffset);
                for (int n = 0; n < featureIndexes.Length; n++)
                {
                    var t = n + (i * featureIndexes.Length);
                    result[t] = Manager.History[frameIndex, featureIndexes[n]]; // Todo: Handle NaN.
                }
            }

            return result;
        }

        private double[] GetDeltaRange(Func<double[], double> func, int startMilliseconds, int duration, params int[] indexes)
        {
            FrameOffsets(startMilliseconds, duration, out var startOffset, out var endOffset);

            var featureIndexes = indexes.Length == 0 ? FeatureIndexes : indexes;
            var featureArray = new double[featureIndexes.Length];

            var offsetDiff = endOffset - startOffset;

            if (offsetDiff <= 0)
            {
                return new double[] { double.NaN };
            }

            var result = new double[offsetDiff];

            for (int i = 0; i < offsetDiff; i++)
            {
                var frameIndex = IndexFromOffset(i + startOffset);
                for (int n = 0; n < featureIndexes.Length; n++)
                {
                    featureArray[n] = Manager.History[frameIndex, featureIndexes[n]]; // Todo: Handle NaN.
                }
                result[i] = func(featureArray);
            }

            return result;
        }

        private double[] GetDeltaRangeInverse(Func<double[], double> func, int startMilliseconds, int duration, params int[] indexes)
        {
            FrameOffsets(startMilliseconds, duration, out var startOffset, out var endOffset);

            var featureIndexes = indexes.Length == 0 ? FeatureIndexes : indexes;
            var result = new double[featureIndexes.Length];

            var offsetDiff = endOffset - startOffset;

            if (offsetDiff <= 0)
            {
                return new double[] { double.NaN };
            }

            var indexArray = new double[offsetDiff];

            for (int i = 0; i < featureIndexes.Length; i++)
            {
                for (int n = 0; n < offsetDiff; n++)
                {
                    var frameIndex = IndexFromOffset(n + startOffset);
                    indexArray[n] = Manager.History[frameIndex, featureIndexes[i]];
                }
                result[i] = func(indexArray);
            }

            return result;
        }

        public double[] MinMany(int startMilliseconds, int endMilliseconds)
        {
            var featureIndexes = FeatureIndexes;
            return GetDeltaRange((x) => x.Min(), startMilliseconds, endMilliseconds - startMilliseconds, featureIndexes);
        }

        public double[] MinManyInverse(int startMilliseconds, int endMilliseconds)
        {
            var featureIndexes = FeatureIndexes;
            return GetDeltaRangeInverse((x) => x.Min(), startMilliseconds, endMilliseconds - startMilliseconds, featureIndexes);
        }

        public double[] MaxMany(int startMilliseconds, int endMilliseconds)
        {
            var featureIndexes = FeatureIndexes;
            return GetDeltaRange((x) => x.Max(), startMilliseconds, endMilliseconds - startMilliseconds, featureIndexes);
        }

        public double[] MaxManyInverse(int startMilliseconds, int endMilliseconds)
        {
            var featureIndexes = FeatureIndexes;
            return GetDeltaRangeInverse((x) => x.Max(), startMilliseconds, endMilliseconds - startMilliseconds, featureIndexes);
        }

        #region VASL Syntax

        public double Current // Single index only
        {
            get
            {
                return Manager.History[FrameIndex, FeatureIndexes[0]];
            }
        }

        public double Old(int milliseconds = 0) // Single index only
        {
            if (milliseconds <= 0)
            {
                milliseconds = Manager.DefaultOffset;
            }

            var prevFrameIndex = IndexFromOffset(FrameOffset(milliseconds));
            return Manager.History[prevFrameIndex, FeatureIndexes[0]];
        }

        // For the below, actual timestamps will be used once splitting can be offset'd.
        public void Pause(double milliseconds = 0d)
        {
            var untilDate = milliseconds > 0d ? Manager.History[FrameIndex].FrameEnd.AddMilliseconds(milliseconds) : DateTime.MaxValue;
            //var untilDate = milliseconds > 0d ? TimeStamp.CurrentDateTime.Time.AddMilliseconds(milliseconds) : DateTime.MaxValue;
            foreach (var f in FeatureIndexes)
            {
                Manager.CompiledFeatures.PauseFeature(f, untilDate);
            }
        }

        public void Resume(double milliseconds = 0d)
        {
            var untilDate = milliseconds > 0d ? Manager.History[FrameIndex].FrameEnd.AddMilliseconds(milliseconds) : DateTime.MaxValue;
            foreach (var f in FeatureIndexes)
            {
                Manager.CompiledFeatures.ResumeFeature(f, untilDate);
            }
        }

        public void PauseAll()
        {
            var untilDate = DateTime.MaxValue;
            for (int i = 0; i < Manager.CompiledFeatures.FeatureCount; i++)
            {
                Manager.CompiledFeatures.PauseFeature(i, untilDate);
            }
        }

        public bool IsPaused
        {
            get
            {
                var result = false;
                foreach (var f in FeatureIndexes)
                {
                    if (double.IsNaN(Manager.History[FrameIndex, f]))
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }

        public double Min(int milliseconds = 0)
        {
            return Min(0, milliseconds);
        }

        public double Min(int startMilliseconds, int endMilliseconds)
        {
            var range = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds);
            return range.Min();
        }

        public double Max(int milliseconds = 0)
        {
            return Max(0, milliseconds);
        }

        public double Max(int startMilliseconds, int endMilliseconds)
        {
            var range = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds);
            return range.Max();
        }

        public double Average(int milliseconds = 0)
        {
            return Average(0, milliseconds);
        }

        public double Average(int startMilliseconds, int endMilliseconds)
        {
            var range = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds);
            return range.Average();
        }

        public double Stdev(int milliseconds = 0)
        {
            return Stdev(0, milliseconds);
        }

        public double Stdev(int startMilliseconds, int endMilliseconds)
        {
            var range = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds);
            return range.StdDev();
        }

        public double Delta(int milliseconds = 0)
        {
            return Delta(0, milliseconds);
        }

        public double Delta(int startMilliseconds, int endMilliseconds) // Single index only
        {
            var featureIndex = FeatureIndexes[0];

            double start;
            if (startMilliseconds <= 0)
            {
                start = this[featureIndex].Current;
                startMilliseconds = 0;
            }
            else
            {
                start = this[featureIndex].Old(startMilliseconds);
            }

            return start / this[featureIndex].Old(endMilliseconds - startMilliseconds);
        }

        // Incomplete
        public double DupeDelta(int milliseconds = 0) // Single index only
        {
            var featureIndex = FeatureIndexes[0];

            return this[featureIndex].Min(milliseconds) / this[featureIndex].Max(milliseconds, milliseconds * 2);
        }

        /*
        // DEBUGGING
        public double jitter // Single index only
        {
            get
            {
                return Benchmarks[FrameIndex, FeatureIndexes[0]] * FrameRate;
            }
        }
        */

        public DeltaOutput this[params int[] numbers]
        {
            get
            {
                FeatureIndexes = numbers;
                return this;
            }
        }

        public DeltaOutput this[params string[] strings]
        {
            get
            {
                int[] numbers = new int[strings.Length];
                for (int n = 0; n < strings.Length; n++)
                {
                    if (!Manager.CompiledFeatures.IndexNames.TryGetValue(strings[n], out var i))
                    {
                        throw new ArgumentException("This name does not exist.");
                    }

                    if (i < 0)
                    {
                        throw new ArgumentException("This name is shared between more than one feature. Identify it more specifically.");
                    }

                    numbers[n] = i;
                }
                FeatureIndexes = numbers;
                return this;
            }
        }

        // Great naming. No one will ever get them mixed up.
        public double MaxMin(int milliseconds = 0)
        {
            return MinMany(0, milliseconds).Max();
        }

        public double MinMax(int milliseconds = 0)
        {
            return MaxMany(0, milliseconds).Min();
        }

        public double MaxMinInverse(int milliseconds = 0)
        {
            return MinManyInverse(0, milliseconds).Max();
        }

        public double MinMaxInverse(int milliseconds = 0)
        {
            return MaxManyInverse(0, milliseconds).Min();
        }

        #endregion VASL Syntax

    }
}
