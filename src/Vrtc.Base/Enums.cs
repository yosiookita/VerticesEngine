using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VerticesEngine
{
    /// <summary>
    /// Selection state, useful for Sandbox items. 
    /// </summary>
    public enum vxSelectionState
    {
        /// <summary>
        /// Item is unseleced and is not hovered.
        /// </summary>
        None,

        /// <summary>
        /// Item is being hovered.
        /// </summary>
        Hover,

        /// <summary>
        /// The Item is selected.
        /// </summary>
        Selected
    }
}


/// <summary>
/// Main orientation for Mobile Games.
/// </summary>
public enum vxOrientationType
{
    Portrait,

    Landscape
}



/// <summary>
/// Is the game a 2D or 3D Game. Is it VR? etc...
/// </summary>
[Flags]
public enum vxGameEnviromentType
{
    TwoDimensional,
    ThreeDimensional,
    VR,
}

/// <summary>
/// Is the game a Local game, or is it networked.
/// </summary>
public enum vxNetworkGameType
{
    /// <summary>
    /// The game is a local game.
    /// </summary>
    Local,

    /// <summary>
    /// The game is a network game.
    /// </summary>
    Networked
}

/*****************************************************************************/
/*							Cascade Shadow Mapping  						 */
/*****************************************************************************/

/// <summary>
/// Shadow map overlay mode.
/// </summary>
public enum vxEnumShadowMapOverlayMode
{
    None,
    ShadowFrustums,
    ShadowMap,
    ShadowMapAndViewFrustum
};

/// <summary>
/// Virtual camera mode.
/// </summary>
public enum vxEnumVirtualCameraMode
{
    None,
    ViewFrustum,
    ShadowSplits
};

/// <summary>
/// Scene shadow mode.
/// </summary>
public enum vxEnumSceneDebugMode
{
    Default,
    EncodedIndex,
    SplitColors,
    /*
    MeshTessellation,
    TexturedWireFrame,
    WireFrame,
    NormalMap,
    DepthMap,
    LightMap,
    SSAO,
    SSRUVs,
    BlankShadow,
    SplitColors,
    BlockPattern,
    */
    PhysicsDebug,
};
