using System.Threading;
using Microsoft.Xna.Framework;
using VerticesEngine.Utilities;
using VerticesEngine.Net;
using Lidgren.Network;
using System;
using VerticesEngine.Net.Events;
using VerticesEngine.Mathematics;
using System.Collections.Generic;

namespace VerticesEngine
{
    public sealed partial class vxEngine
    {

        /// <summary>
        /// Gets the type of the build config. This is set internally but can be changed by launching with specific launch options.
        /// </summary>
        /// <value>The type of the build config.</value>
        public static vxBuildType BuildType
        {
            get { return _buildConfigType; }
        }
        static private vxBuildType _buildConfigType;


        /// <summary>
        /// Gets the OS type of the platform.
        /// </summary>
        /// <value>The type of the platorm.</value>
        public static vxPlatformOS PlatformOS
        {
            get { return _platformOS; }
        }
        static vxPlatformOS _platformOS;


        /// <summary>
        /// Specifies whether this game is running on Desktop, Console, Mobile or Web.
        /// </summary>
        public static vxPlatformType PlatformType
        {
            get { return _platformType; }
        }
        static vxPlatformType _platformType;

        /// <summary>
        /// Which platform is this version meant to released on, i.e. Steam, ItchIO, Android etc...
        /// </summary>
        public static vxReleasePlatformType ReleasePlatformType
        {
            get
            {
                //Set Location of Content Specific too Platform
#if __ITCH_IO__
                return vxReleasePlatformType.Itchio;
#elif __STEAM__
                return vxReleasePlatformType.Steam;
#elif __ANDROID__
                return vxReleasePlatformType.Android;
#elif __IOS__
                return vxReleasePlatformType.iOS;
#else
                return vxReleasePlatformType.General;
#endif
            }
        }


        /// <summary>
        /// Gets the type graphical backend that's being used (i.e. OpenGL, DirectX etc...)
        /// </summary>
        public static vxGraphicalBackend GraphicalBackend
        {
            get
            {
                //Set Location of Content Specific too Platform
#if VRTC_PLTFRM_XNA
                return vxGraphicalBackend.XNA;
#elif VRTC_PLTFRM_GL
                return vxGraphicalBackend.OpenGL;
#elif __ANDROID__
                return vxGraphicalBackend.Android;
#elif __IOS__
                return vxGraphicalBackend.iOS;
#endif
            }
        }

    }
}