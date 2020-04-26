using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VerticesEngine
{
    /// <summary>
    /// A static class which holds a number of different classes for handling time
    /// </summary>
    public static class vxTime
    {
        /// <summary>
        /// The amount of time in seconds elapsed since the last frame 
        /// </summary>
        public static float ElapsedTime { get; private set; }

        /// <summary>
        /// The total amount of time in seconds elapsed since the game started.
        /// </summary>
        public static float TotalElapsedTime { get; private set; }

        /// <summary>
        /// The number of frames since the start of the game
        /// </summary>
        public static int FrameCount { get; private set; }


        /// <summary>
        /// Updates the time variables internally in the engine
        /// </summary>
        /// <param name="gameTime"></param>
        internal static void Update(GameTime gameTime)
        {
            ElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds * (vxEngine.PlatformOS == vxPlatformOS.Windows && vxScreen.IsFullScreen == false ? 2 : 1);

            TotalElapsedTime = (float)gameTime.TotalGameTime.TotalSeconds;

            FrameCount++;
        }
    }
}
