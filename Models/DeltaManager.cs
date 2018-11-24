using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveSplit.VAS;
using LiveSplit.Model;
using System.IO;
using System.Threading;

namespace LiveSplit.VAS.Models
{
    public delegate void DeltaResultsHandler(object sender, DeltaManager dm);

    public class DeltaManager
    {
        public static int HistorySize { get; set; }   = 256; // Todo: Allow VASL script to set these.
        public static int DefaultOffset { get; set; } = 100; // 100 milliseconds.

        public static DeltaResults[] History = new DeltaResults[HistorySize];

        private readonly int OriginalIndex;
        public readonly int FrameIndex;
        public readonly double FrameRate;

        private int[] _FeatureIndexes = null;
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
                if (value.Length > CompiledFeatures.FeatureCount)
                    throw new ArgumentOutOfRangeException();
                _FeatureIndexes = value;
            }
        }

        internal DeltaManager(int index, double frameRate)
        {
            OriginalIndex = index;
            FrameIndex = index % HistorySize;
            FrameRate = frameRate;
        }

        // Cache for slight performance buff...?
        private int _MillisecondOffsetLimit = -1;
        private int MillisecondOffsetLimit
        {
            get
            {
                if (_MillisecondOffsetLimit == -1)
                {
                    _MillisecondOffsetLimit = (int)Math.Ceiling((HistorySize - 1) / FrameRate * 1000);
                }
                return _MillisecondOffsetLimit;
            }
        }

        // Name is questionable
        private int FrameOffset(int milliseconds)
        {
            if (milliseconds < 0)
                throw new IndexOutOfRangeException("Offset cannot be negative.");

            if (milliseconds > MillisecondOffsetLimit)
                throw new IndexOutOfRangeException(
                    "Offset cannot exceed the history's count, which is currently " +
                    HistorySize.ToString() +
                    ", and this is trying to access previous frame #" +
                    Math.Round(FrameRate * milliseconds / 1000d).ToString() +
                    ".");

            return Math.Max(1, (int)Math.Round(FrameRate * milliseconds / 1000d));
        }

        // Name is questionable
        private int IndexFromOffset(int offset)
        {
            return (OriginalIndex - offset) % HistorySize;
        }

        public static double[,] Benchmarks
        {
            get
            {
                var featureCount = CompiledFeatures.FeatureCount;

                var results = new double[HistorySize, featureCount];
                for (int i = 0; i < HistorySize; i++)
                {
                    for (int n = 0; n < CompiledFeatures.FeatureCount; n++)
                    {
                        results[i, n] = History[i] != null ? History[i].Benchmarks[n] : double.NaN;
                    }
                }

                return results;
            }
        }

        public static double[] BenchmarkAverages
        {
            get
            {
                var featureCount = CompiledFeatures.FeatureCount;

                var allBenchmarks = Benchmarks;
                var results = new double[featureCount];
                var lists = new List<List<double>>();

                for (int i = 0; i < featureCount; i++)
                {
                    lists.Add(new List<double>());

                    for (int n = 0; n < HistorySize; n++)
                    {
                        var x = allBenchmarks[n, i];
                        if (!x.Equals(double.NaN))
                        {
                            lists[i].Add(x);
                        }
                    }

                    results[i] = lists[i].Count > 0 ? lists[i].Average() : double.NaN;
                }

                return results;
            }
        }

        public double[] GetDeltaRange(int startMilliseconds, int duration, params int[] indexes)
        {
            int startOffset;
            int endOffset;

            if (startMilliseconds <= 0)
            {
                startOffset = 0;
                startMilliseconds = 0;
            }
            else
                startOffset = FrameOffset(startMilliseconds);

            if (duration <= 0)
                endOffset = FrameOffset(startMilliseconds + DefaultOffset);
            else
                endOffset = FrameOffset(startMilliseconds + duration);

            var result = new double[endOffset - startOffset];

            if (result.Length == 0)
                return new double[] { double.NaN };

            var featureIndexes = indexes.Length == 0 ? FeatureIndexes : indexes;

            for (int i = 0; i < endOffset - startOffset; i++)
            {
                var frameIndex = IndexFromOffset(i + startOffset);
                foreach (var f in featureIndexes)
                {
                    result[i] = History[frameIndex].Deltas[f]; // Todo: Handle NaN.
                }
            }

            return result;
        }

        public double[] MinMany(int startMilliseconds, int endMilliseconds)
        {
            var featureIndexes = FeatureIndexes;
            var results = new double[featureIndexes.Length];
            for (int i = 0; i < featureIndexes.Length; i++)
            {
                results[i] = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds, featureIndexes[i]).Min();
            }
            return results;
        }

        public double[] MaxMany(int startMilliseconds, int endMilliseconds)
        {
            var featureIndexes = FeatureIndexes;
            var results = new double[featureIndexes.Length];
            for (int i = 0; i < featureIndexes.Length; i++)
            {
                results[i] = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds, featureIndexes[i]).Max();
            }
            return results;
        }

        #region VASL Syntax

        public double current // Single index only
        {
            get
            {
                return History[FrameIndex].Deltas[FeatureIndexes[0]];
            }
        }

        public double old(int milliseconds = 0) // Single index only
        {
            if (milliseconds <= 0)
                milliseconds = DefaultOffset;

            var prevFrameIndex = IndexFromOffset(FrameOffset(milliseconds));
            return History[prevFrameIndex].Deltas[FeatureIndexes[0]];
        }

        // For the below, actual timestamps will be used once splitting can be offset'd.
        public void pause(double milliseconds = 0d)
        {
            var untilDate = milliseconds > 0d ? History[FrameIndex].FrameEnd.AddMilliseconds(milliseconds) : DateTime.MaxValue;
            //var untilDate = milliseconds > 0d ? TimeStamp.CurrentDateTime.Time.AddMilliseconds(milliseconds) : DateTime.MaxValue;
            foreach (var f in FeatureIndexes)
            {
                CompiledFeatures.PauseFeature(f, untilDate);
            }
        }

        public void resume(double milliseconds = 0d)
        {
            var untilDate = milliseconds > 0d ? History[FrameIndex].FrameEnd.AddMilliseconds(milliseconds) : DateTime.MaxValue;
            foreach (var f in FeatureIndexes)
            {
                CompiledFeatures.ResumeFeature(f, untilDate);
            }
        }

        public void pauseAll()
        {
            var untilDate = DateTime.MaxValue;
            for (int i = 0; i < CompiledFeatures.FeatureCount; i++)
            {
                CompiledFeatures.PauseFeature(i, untilDate);
            }
        }

        public bool isPaused
        {
            get
            {
                var result = false;
                foreach (var f in FeatureIndexes)
                {
                    if (double.IsNaN(History[FrameIndex].Deltas[f]))
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }

        public double min(int milliseconds = 0)
        {
            return min(0, milliseconds);
        }

        public double min(int startMilliseconds, int endMilliseconds)
        {
            var range = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds);
            return range.Min();
        }

        public double max(int milliseconds = 0)
        {
            return max(0, milliseconds);
        }

        public double max(int startMilliseconds, int endMilliseconds)
        {
            var range = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds);
            return range.Max();
        }

        public double average(int milliseconds = 0)
        {
            return average(0, milliseconds);
        }

        public double average(int startMilliseconds, int endMilliseconds)
        {
            var range = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds);
            return range.Average();
        }

        public double stdev(int milliseconds = 0)
        {
            return stdev(0, milliseconds);
        }

        public double stdev(int startMilliseconds, int endMilliseconds)
        {
            var range = GetDeltaRange(startMilliseconds, endMilliseconds - startMilliseconds);
            return range.StdDev();
        }

        public double delta(int milliseconds = 0)
        {
            return delta(0, milliseconds);
        }

        public double delta(int startMilliseconds, int endMilliseconds) // Single index only
        {
            var featureIndex = FeatureIndexes[0];

            double start;
            if (startMilliseconds <= 0)
            {
                start = this[featureIndex].current;
                startMilliseconds = 0;
            }
            else
                start = this[featureIndex].old(startMilliseconds);

            return start / this[featureIndex].old(endMilliseconds - startMilliseconds);
        }

        // Incomplete
        public double dupeDelta(int milliseconds = 0) // Single index only
        {
            var featureIndex = FeatureIndexes[0];

            return this[featureIndex].min(milliseconds) / this[featureIndex].max(milliseconds, milliseconds * 2);
        }

        // DEBUGGING
        public double jitter // Single index only
        {
            get
            {
                return Benchmarks[FrameIndex, FeatureIndexes[0]] * FrameRate;
            }
        }

        public DeltaManager this[params int[] numbers]
        {
            get
            {
                FeatureIndexes = numbers;
                return this;
            }
        }

        public DeltaManager this[params string[] strings]
        {
            get
            {
                int[] numbers = new int[strings.Length];
                for (int n = 0; n < strings.Length; n++)
                {
                    int i;
                    if (!CompiledFeatures.IndexNames.TryGetValue(strings[n], out i))
                        throw new ArgumentException("This name does not exist.");
                    if (i < 0)
                        throw new ArgumentException("This name is shared between more than one feature. Identify it more specifically.");
                    numbers[n] = i;
                }
                FeatureIndexes = numbers;
                return this;
            }
        }

        // Great naming. No one will ever get them mixed up.
        public double maxMin(int milliseconds = 0)
        {
            return MinMany(0, milliseconds).Max();
        }

        public double minMax(int milliseconds = 0)
        {
            return MaxMany(0, milliseconds).Min();
        }

        #endregion VASL Syntax

        internal static void AddResult(int index, DateTime frameStart, DateTime frameEnd, DateTime scanEnd, double[] deltas, double[] benchmarks)
        {
            const int SAFETY_THRESHOLD = 30;
            var currIndex = index % HistorySize;
            var prevIndex = (index - 1) % HistorySize;

            var dr = new DeltaResults(index, frameStart, frameEnd, scanEnd, deltas, benchmarks);
            History[currIndex] = dr;

            if (index > SAFETY_THRESHOLD)
            {
                while ((History[prevIndex]?.Index ?? -1) != index - 1)
                {
                    if (Scanner.CurrentIndex - HistorySize >= index)
                    {
                        // DEBUGGING
                        var a = BenchmarkAverages;

                        var AverageFPS       = Scanner.AverageFPS;
                        var MinFPS           = Scanner.MinFPS;
                        var MaxFPS           = Scanner.MaxFPS;
                        var AverageScanTime  = Scanner.AverageScanTime;
                        var MinScanTime      = Scanner.MinScanTime;
                        var MaxScanTime      = Scanner.MaxScanTime;
                        var AverageWaitTime  = Scanner.AverageWaitTime;
                        var MinWaitTime      = Scanner.MinWaitTime;
                        var MaxWaitTime      = Scanner.MaxWaitTime;

                        var pauses = CompiledFeatures.PauseIndex;
                        //var pause1 = CompiledFeatures.CWatchZones[0].CWatches[0].CWatchImages[0].IsPaused(TimeStamp.CurrentDateTime.Time);
                        //var pause2 = CompiledFeatures.CWatchZones[3].CWatches[0].CWatchImages[0].IsPaused(TimeStamp.CurrentDateTime.Time);

                        throw new Exception("Previous frame could not be processed or is taking too long to process.");
                    }
                    Thread.Sleep(1);
                }
            }

            dr.WaitEnd = TimeStamp.CurrentDateTime.Time;
        }

        internal static void AddResult(int index, Scan scan, DateTime scanEnd, double[] deltas, double[] benchmarks) 
            => AddResult(index, scan.PreviousFrame.DateTime, scan.CurrentFrame.DateTime, scanEnd, deltas, benchmarks);


    }
}
