using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;
using System.Diagnostics;
using VerticesEngine.Utilities;

namespace VerticesEngine.Diagnostics
{
    public static class vxProfiler
    {
        static vxEngine Engine;

        [vxDebugSetting("Profiler.IsEnabled")]
        public static bool IsEnabled = false;

        [vxDebugSetting("Profiler.FPSUpdateRate")]
        public static float UpdateRate = 0.5f;

        internal static void Init(vxEngine engine)
        {
            Engine = engine;

            FPS = 0;
            sampleFrames = 0;
            stopwatch = Stopwatch.StartNew();
            SampleSpan = TimeSpan.FromSeconds(UpdateRate);
        }

        #region -- FPS --

        /// <summary>
        /// Gets current Frames Per Second
        /// </summary>
        public static float FPS { get; internal set; }

        // Stopwatch for fps measuring.
        private static Stopwatch stopwatch;

        /// <summary>
        /// Gets/Sets FPS sample duration.
        /// </summary>
        public static TimeSpan SampleSpan;

        private static int sampleFrames;

        #endregion

        #region -- Time Ruler --

        /// <summary>
        /// The Colleciton of Timers
        /// </summary>
        public static Dictionary<object, vxDebugTimerGraphSet> TimerCollection = new Dictionary<object, vxDebugTimerGraphSet>();


        public static void RegisterMark(string markID, Color color)
        {
            TimerCollection.Add(markID, new vxDebugTimerGraphSet(Engine, markID, color));
        }

        public static void BeginMark(string mark)
        {
            if (TimerCollection.ContainsKey(mark) && IsEnabled)
                TimerCollection[mark].Start();
        }

        public static void EndMark(string mark)
        {
            if (TimerCollection.ContainsKey(mark) && IsEnabled)
                TimerCollection[mark].Stop();
        }

        #endregion

        internal static void Update()
        {
            if (IsEnabled)
            {
                sampleFrames++;

                // Handle FPS here
                if (stopwatch != null)
                {
                    if (stopwatch.Elapsed > SampleSpan)
                    {
                        // Update FPS value and start next sampling period.
                        FPS = (float)sampleFrames / (float)stopwatch.Elapsed.TotalSeconds;

                        stopwatch.Reset();
                        stopwatch.Start();
                        sampleFrames = 0;

                    }
                }
            }
        }
    }
}
