
#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Ads;
#endif

#endregion

namespace VerticesEngine
{

    [Flags]
    public enum vxGameConfigFlags
    {
        ControlsSettings = 1,
        GraphicsSettings = 2,
        AudioSettings = 4,
        LanguageSettings = 8,
        NetworkEnabled = 16,
        IsCursorVisible = 32,
        PlayerProfileSupport = 128,
        LeaderboardsSupport = 256,
        AchievementsSupport = 512
    }

    public sealed class vxGameConfig
    {
        /// <summary>
        /// The name of the game.
        /// </summary>
        public readonly string GameName;

		/// <summary>
		/// The type of the game.
		/// </summary>
		public readonly vxGameEnviromentType GameType;

        /// <summary>
        /// The main orientation.
        /// </summary>
        public readonly vxOrientationType MainOrientation = vxOrientationType.Landscape;

        vxGameConfigFlags LaunchFlags;

		/// <summary>
		/// The has controls settings.
		/// </summary>
		public bool HasControlsSettings
        {
            get { return LaunchFlags.HasFlag(vxGameConfigFlags.ControlsSettings); }
        }

		/// <summary>
		/// The has graphics settings.
		/// </summary>
        public bool HasGraphicsSettings
        {
            get { return LaunchFlags.HasFlag(vxGameConfigFlags.GraphicsSettings); }
        }

		/// <summary>
		/// The has audio settings.
		/// </summary>
        public bool HasAudioSettings
        {
            get { return LaunchFlags.HasFlag(vxGameConfigFlags.AudioSettings); }
        }

		/// <summary>
		/// The has language settings.
		/// </summary>
        public bool HasLanguageSettings
        {
            get { return LaunchFlags.HasFlag(vxGameConfigFlags.LanguageSettings); }
        }

		/// <summary>
		/// Does this game require the network code to be setup at start.
		/// </summary>
        public bool HasNetworkCapabilities
        {
            get { return LaunchFlags.HasFlag(vxGameConfigFlags.NetworkEnabled); }
        }

        /// <summary>
        /// The ideal screen size. This is used for GUI Scaling.
        /// </summary>
        public Point IdealScreenSize = new Point(1200, 720);


        public bool HasProfileSupport
        {
            get { return LaunchFlags.HasFlag(vxGameConfigFlags.PlayerProfileSupport); }
        }

        public bool HasLeaderboards
        {
            get { return LaunchFlags.HasFlag(vxGameConfigFlags.LeaderboardsSupport); }
        }

        public bool HasAchievements
        {
            get { return LaunchFlags.HasFlag(vxGameConfigFlags.AchievementsSupport); }
        }

        /// <summary>   
        /// The MAX number of players in this Scene. This must be set in the constructor.
        /// </summary>
        public int MaxNumberOfPlayers = 1;


        public vxGameConfig(
            string GameName,
            vxGameEnviromentType GameType, 
            vxGameConfigFlags LaunchFlags,
            vxOrientationType MainOrientation = vxOrientationType.Landscape)
        {
            this.GameName = GameName;

            this.GameType = GameType;

            this.LaunchFlags = LaunchFlags;

            this.MainOrientation = MainOrientation;
        }
    }
}

