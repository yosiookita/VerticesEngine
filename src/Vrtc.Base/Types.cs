using System;
using System.Collections.Generic;
using System.Text;

namespace VerticesEngine
{

    /// <summary>
    /// An enum holding the platform/backend type for this engine build.
    /// </summary>
    public enum vxGraphicalBackend
    {
        /// <summary>
        /// DirectX/MonoGame Platform tag
        /// </summary>
        DirectX,


        /// <summary>
        /// OpenGL/MonoGame Platform tag
        /// </summary>
        OpenGL,


        /// <summary>
        /// Android/MonoGame Platform tag
        /// </summary>
        Android,


        /// <summary>
        /// iOS/MonoGame Platform tag
        /// </summary>
        iOS
    }

    public enum vxPlatformOS
    {
        Windows,

        OSX,

        Linux,

        iOS,

        Android,

        Switch
    }

    public enum vxPlatformType
    {
        Desktop,
        Console,
        Mobile,
        Web
    }

    public enum vxReleasePlatformType
    {
        /// <summary>
        /// This Build is for a Steam release
        /// </summary>
        Steam,


        /// <summary>
        /// This Build is for an ItchIO release
        /// </summary>
        Itchio,


        /// <summary>
        /// This Build is for a Discord release
        /// </summary>
        Discord,


        /// <summary>
        /// Android/MonoGame Platform tag
        /// </summary>
        Android,


        /// <summary>
        /// iOS/MonoGame Platform tag
        /// </summary>
        iOS,


        /// <summary>
        /// Generic Desktop
        /// </summary>
        General,
    }

    /// <summary>
    /// enum of build configuration types. This can be used instead of compiler flags so
    /// to allows for special debug code that can still be run in a release version.
    /// </summary>
    public enum vxBuildType
    {
        /// <summary>
        /// The engine is in Debug mode. This can unclock a number of functions in th e
        /// engine for debugging. Launch the game with the '-dev' launch parameter to 
        /// set this flag in a release enviroment.
        /// </summary>
        Debug,

        /// <summary>
        /// The Release build tag. This deactivates all Debug info.
        /// </summary>
        Release
    }


}
