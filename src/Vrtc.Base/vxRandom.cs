using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VerticesEngine
{
    /// <summary>
    /// Random Value Generator
    /// </summary>
    public static class vxRandom
    {
        static Random random = new Random();

        internal static void Init(int seed)
        {
            random = new Random(seed);
        }

        /// <summary>
        /// Returns a random float between 0.0 and 1.0
        /// </summary>
        /// <returns></returns>
        public static float GetRandomValue()
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// Returns a random value between the min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float GetRandomValue(int min, int max)
        {
            return (float)(random.Next(min, max) * 100)/100.0f;
        }



        public static Color GetRandomColour()
        {
            return new Color((int)GetRandomValue(0, 256), (int)GetRandomValue(0, 256), (int)GetRandomValue(0, 256));
        }
    }
}
