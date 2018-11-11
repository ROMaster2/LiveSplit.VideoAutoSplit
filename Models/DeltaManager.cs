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
        public const int HISTORY_SIZE        = 256;
        public const int FEATURE_COUNT_LIMIT =  64; // Todo: Make static and set as the compiled feature count.
        public const int DEFAULT_OFFSET      = 100; // 100 milliseconds. Todo: Allow VASL script to set this themselves.

        public static DeltaResults[] History = new DeltaResults[HISTORY_SIZE];

        private readonly int OriginalIndex;
        public readonly int FrameIndex;
        public readonly double FrameRate;

        private int? FeatureIndex = null;

        internal DeltaManager(int index, double frameRate)
        {
            OriginalIndex = index;
            FrameIndex = index % HISTORY_SIZE;
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
                    _MillisecondOffsetLimit = (int)Math.Ceiling((HISTORY_SIZE - 1) / FrameRate * 1000);
                }
                return _MillisecondOffsetLimit;
            }
        }

        private void ResetFeatureIndex()
        {
            FeatureIndex = null;
        }

        private int FrameOffset(int milliseconds)
        {
            if (milliseconds < 0)
                throw new ArgumentOutOfRangeException("Offset cannot be negative.");

            if (milliseconds > MillisecondOffsetLimit)
                throw new ArgumentOutOfRangeException(
                    "Offset cannot exceed the history's count, which is currently " +
                    HISTORY_SIZE.ToString() +
                    ", and this is trying to access previous frame #" +
                    Math.Round(FrameRate * milliseconds / 1000d).ToString() +
                    ".");

            return Math.Max(1, (int)Math.Round(FrameRate * milliseconds / 1000d));
        }
        // ^ v These names are questionable
        private int IndexFromOffset(int offset)
        {
            return (OriginalIndex - offset) % HISTORY_SIZE;
        }

        #region VASL Syntax

        public DeltaManager this[int i]
        {
            get
            {
                if (i >= FEATURE_COUNT_LIMIT || i < 0)
                    throw new ArgumentOutOfRangeException();
                FeatureIndex = i;
                return this;
            }
        }

        public DeltaManager this[string str]
        {
            get
            {

                if (!CompiledFeatures.IndexNames.TryGetValue(str, out int i))
                    throw new ArgumentNullException();
                if (i < 0)
                    throw new ArgumentException("This name is shared between more than one feature. Identify it more specifically.");
                FeatureIndex = i;
                return this;
            }
        }

        public double current
        {
            get
            {
                if (FeatureIndex == null)
                    throw new InvalidOperationException("Feature index has not been set. Declare such with [n] first.");

                var featureIndex = FeatureIndex.Value;
                ResetFeatureIndex();
                return History[FrameIndex].Deltas[featureIndex];
            }
        }

        public double old(int milliseconds = DEFAULT_OFFSET)
        {
            if (FeatureIndex == null)
                throw new InvalidOperationException("Feature index has not been set. Declare such with [n] first.");

            var featureIndex = FeatureIndex.Value;
            ResetFeatureIndex();
            var prevFrameIndex = IndexFromOffset(FrameOffset(milliseconds));
            return History[prevFrameIndex].Deltas[featureIndex];
        }

        #endregion VASL Syntax

        internal static void AddResult(int index, TimeStamp frameStart, TimeStamp frameEnd, TimeStamp scanEnd, double[] deltas)
        {
            const int BREAK_POINT = 3000;
            const int SAFETY_THRESHOLD = 30;
            var currIndex = index % HISTORY_SIZE;
            var prevIndex = (index - 1) % HISTORY_SIZE;

            var dr = new DeltaResults(index, frameStart, frameEnd, scanEnd, deltas);
            History[currIndex] = dr;

            if (index > SAFETY_THRESHOLD)
            {
                int i = 0;
                while (History[prevIndex] == null || History[prevIndex].Index != index - 1)
                {
                    if (i >= BREAK_POINT)
                    {
                        // Maybe instead just skip the frame and log it as an error?
                        //throw new Exception("Previous frame could not be processed or is taking too long to process.");
                        break;
                    }
                    i++;
                    Thread.Sleep(1);
                }
            }

            dr.WaitEnd = TimeStamp.Now;
        }

        internal static void AddResult(int index, Scan scan, TimeStamp scanEnd, double[] deltas) 
            => AddResult(index, scan.PreviousFrame.TimeStamp, scan.CurrentFrame.TimeStamp, scanEnd, deltas);


    }
}
