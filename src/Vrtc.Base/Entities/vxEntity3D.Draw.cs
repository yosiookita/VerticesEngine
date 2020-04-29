
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


//Virtex vxEngine Declaration
using VerticesEngine;
using VerticesEngine.Graphics;
using VerticesEngine.Utilities;

namespace VerticesEngine
{
    public partial class vxEntity3D : vxEntity
    {
        #region -- Rendering Fields --

        /// <summary>
        /// This bool flag is set each update loop for whether or not the the entity should be drawn.
        /// </summary>
        public bool IsEntityDrawable = true;

        public List<vxMaterial> GetMaterials()
        {
            List<vxMaterial> materials = new List<vxMaterial>();

            foreach (var mesh in Model.Meshes)
            {
                materials.Add(mesh.Material);
            }

            return materials;
        }

        public List<T> GetMaterials<T>() where T : vxMaterial
        {
            List<T> materials = new List<T>();

            foreach(var mesh in Model.Meshes)
            {
                if(mesh.Material.GetType() == typeof(T))
                {
                    materials.Add((T)mesh.Material);
                }
            }


            return materials;
        }


        /// <summary>
        /// The vxModel model which holds are graphical, shader and vertices data to be shown.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.ModelProperties)]
        public vxModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                if (_model != null)
                {
                    // Recalculate the Bounding Box
                    if (_model.ModelMain != null)
                        _boundingBox = vxGeometryHelper.GetModelBoundingBox(_model.ModelMain);
                    else
                        _boundingBox = _model.BoundingBox;


                    // Get the Bounding Shadpw used for Camera Frustrum Culling.
                    _boundingSphere = GetBoundingShape();

                    // Set the Model Center as the Bounding Shape Center.
                    ModelCenter = _boundingSphere.Center;

                    // Now Initialise Shaders
                    InitShaders();
                }
            }
        }
        vxModel _model;


        /// <summary>
        /// The Colour used for certain shaders (i.e. Highliting, and Plain Color)
        /// </summary>
        public Color PlainColor = Color.White;

        public bool IsAlphaNoShadow = false;

        #endregion


        public override void Draw(vxCamera Camera, string renderpass)
        {
            TempWVP = WorldTransform * Camera.ViewProjection;
            if (IsVisible)
            {
                Draw(WorldTransform, Camera.View, Camera.Projection, renderpass);
            }
        }

        /// <summary>
        /// Renders the mesh using a speciefed Position, View and Projection.
        /// </summary>
        /// <param name="world">World.</param>
        /// <param name="view">View.</param>
        /// <param name="projection">Projection.</param>
        public virtual void Draw(Matrix world, Matrix view, Matrix projection, string renderpass)
        {
            foreach (vxModelMesh mesh in Model.Meshes)
            {
                if (renderpass == mesh.Material.MaterialRenderPass)
                {
                    mesh.Material.World = world;
                    mesh.Material.WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(world));
                    mesh.Material.WVP = world * view * projection;
                    mesh.Material.View = view;
                    mesh.Material.Projection = projection;
                    mesh.Draw();
                }
            }
        }

        Color debgColor = Color.DeepPink;



        public bool IsMotionBlurEnabled
        {
            get { return _isMotionBlurEnabled; }
            set
            {
                _isMotionBlurEnabled = value;
                OnSetMotionBlurMask();
            }
        }
        bool _isMotionBlurEnabled = true;

        public float MotionBlurFactor
        {
            get { return _motionBlurFactor; }
            set
            {
                _motionBlurFactor = value;
                OnSetMotionBlurMask();
            }
        }
        float _motionBlurFactor = 1;


        void OnSetMotionBlurMask()
        {
            MaskPropertiesColor.G = (byte)(_isMotionBlurEnabled ? 0 : 255 * MotionBlurFactor);
        }

        /// <summary>
        /// This color holds Mask Properties for different Post Processes.
        /// R: TODO
        /// G: Camera Motion Blur Factor (0: Do Motion Blur. 1: None),
        /// B: TODO
        /// </summary>
        public Color MaskPropertiesColor = Color.Black;


        /// <summary>
        /// Renders the overlay mesh.
        /// </summary>
        /// <param name="Camera">Camera.</param>
        public virtual void RenderOverlayMesh(vxCamera3D Camera)
        {

        }

        //float timer = 0;
        Matrix TempWorld;
        Matrix TempWVP;
        /// <summary>
        /// Renders the utility render targets in a single-pass-multi-target pass. These render targets include
        /// the Normal Map, Depth Map, Distortion Map and the Surface Detail/Lighting Map.
        /// </summary>
        //public virtual void RenderPrepPass(vxCamera3D Camera)
        //{
        //    TempWVP = TempWorld * Camera.ViewProjection;

        //    timer += 0.0167f;


        //    //if (Model != null  && IsInCameraViewFrustum == true && ShouldDraw == true)
        //    if (IsEntityDrawable && DoPrepPassRenders)
        //    {

        //        LightDirection = Vector3.Normalize(Scene.Renderer.LightPosition);

        //        if (vxEngine.BuildType == vxBuildType.Debug)
        //            Engine.GlobalPrimitiveCount += Model.TotalPrimitiveCount;

        //        // Now draw the model
        //        foreach (vxModelMesh mesh in Model.Meshes)
        //        {
        //            mesh.UtilityEffect.CurrentTechnique = mesh.UtilityEffect.Techniques["Technique_PrepPass"];

        //            mesh.UtilityEffect.World = TempWorld;

        //            mesh.UtilityEffect.WVP = TempWVP;

        //            //ShadowBrightness = 0.5f;

        //            if (Engine.Settings.Graphics.Shadows.Quality > vxEnumQuality.None)
        //            {
        //                // Shadow Parameters
        //                mesh.UtilityEffect.ShadowMap = RenderTargets.ShadowMap;
        //                mesh.UtilityEffect.ShadowTransform = Scene.Renderer.ShadowSplitProjectionsWithTiling;
        //                //mesh.UtilityEffect.ShadowBrightness = 0;

        //                //mesh.UtilityEffect.Parameters["fFresnelBias"].SetValue(0.025f);
        //                //mesh.UtilityEffect.Parameters["fFresnelPower"].SetValue(6.0f);
        //                mesh.UtilityEffect.Parameters["CameraPos"].SetValue(Camera.WorldMatrix.Translation);
        //            }

        //            mesh.Draw(mesh.UtilityEffect);
        //        }
        //    }
        //}

        //public bool DoPrepPassRenders = true;
        ///// <summary>
        ///// This renders to a set of render targets for handling index encoding, motion blur masking, etc...
        ///// </summary>
        ///// <param name="Camera"></param>
        //public virtual void RenderDataMaskPass(vxCamera3D Camera)
        //{
        //    TempWorld = ScaleMatrix * WorldTransform;
        //    TempWVP = TempWorld * Camera.ViewProjection;

        //    timer += 0.0167f;

        //    //if (Model != null  && IsInCameraViewFrustum == true && ShouldDraw == true)
        //    if (IsEntityDrawable && DoPrepPassRenders)
        //    {
        //        LightDirection = Vector3.Normalize(Scene.Renderer.LightPosition);

        //        if (vxEngine.BuildType == vxBuildType.Debug)
        //            Engine.GlobalPrimitiveCount += Model.TotalPrimitiveCount;

        //        // Now draw the model
        //        foreach (vxModelMesh mesh in Model.Meshes)
        //        {
        //            mesh.UtilityEffect.CurrentTechnique = mesh.UtilityEffect.Techniques["Technique_DataMaskPass"];

        //            mesh.UtilityEffect.WVP = TempWVP;

        //            mesh.UtilityEffect.Parameters["IndexEncodedColour"].SetValue(IndexEncodedColour.ToVector4());
        //            mesh.UtilityEffect.Parameters["MaskPropertiesColor"].SetValue(MaskPropertiesColor.ToVector4());


        //            mesh.Draw(mesh.UtilityEffect);
        //        }
        //    }
        //}
        /*
        /// <summary>
        /// Renders the utility render targets in a single-pass-multi-target pass. These render targets include
        /// the Normal Map, Depth Map, Distortion Map and the Surface Detail/Lighting Map.
        /// </summary>
        public virtual void RenderEncodedIndex(vxCamera3D Camera)
        {
            Effect effect = vxInternalAssets.Shaders.IndexEncodedColourShader;

            if (IsEntityDrawable)
            {
                //vxConsole.WriteInGameDebug(this.Id + " index:" + this._handleID);
                // Now draw the model
                foreach (vxModelMesh mesh in Model.Meshes)
                {
                    effect.Parameters["WorldViewProjection"].SetValue((ScaleMatrix * WorldTransform * Camera.ViewProjection));
                    effect.Parameters["IndexEncodedColour"].SetValue(IndexEncodedColour.ToVector4());

                    mesh.Draw(effect);
                }
            }
        }
        */
        //#region Debug Methods

        //public virtual void RenderDebugWire(vxCamera3D Camera, bool DoDebugWireFrame)
        //{
        //    float color = 0.55f;
        //    RenderDebugWire(Camera, DoDebugWireFrame, new Color(color, color, color, 1));
        //}

        //public virtual void RenderDebugWire(vxCamera3D Camera, bool DoDebugWireFrame, Color WireColour)
        //{
        //    RenderDebugWire(Camera, DoDebugWireFrame, false, WireColour);
        //}

        //public virtual void RenderDebugWire(vxCamera3D Camera, bool DoDebugWireFrame, bool DoTexture)
        //{
        //    RenderDebugWire(Camera, DoDebugWireFrame, true, Color.Black);
        //}

        //private void RenderDebugWire(vxCamera3D Camera, bool DoDebugWireFrame, bool DoTexture, Color WireColour)
        //{
        //    switch (SelectionState)
        //    {
        //        case vxSelectionState.Selected:
        //            WireColour = Color.DeepSkyBlue;
        //            break;
        //        case vxSelectionState.Hover:
        //            WireColour = Color.DeepSkyBlue;
        //            break;
        //        case vxSelectionState.None:
        //            //WireColour = Color.Black;
        //            break;
        //    }

        //    if (IsEntityDrawable)
        //    {

        //        if (DoDebugWireFrame == false)
        //        {
        //            if (vxEngine.BuildType == vxBuildType.Debug)
        //                Engine.GlobalPrimitiveCount += Model.TotalPrimitiveCount;
        //        }

        //        foreach (vxModelMesh mesh in Model.Meshes)
        //        {
        //            mesh.DebugEffect.CurrentTechnique = mesh.DebugEffect.Techniques["Technique_Debug"];

        //            mesh.DebugEffect.World = TempWorld;
        //            mesh.DebugEffect.WVP = TempWVP;
        //            mesh.DebugEffect.DoDebugWireFrame = DoDebugWireFrame;
        //            mesh.DebugEffect.DoTexture = DoTexture;
        //            mesh.DebugEffect.WireColour = WireColour;
        //            //mesh.DebugEffect.DiffuseTexture = vxInternalAssets.Textures.Texture_Diffuse_Null;

        //            mesh.Draw(mesh.DebugEffect);
        //        }
        //    }
        //    SetRenderEndState();
        //}


        //public virtual void RenderMeshShadowDebug(vxCamera3D Camera, bool DoBlockTexture)
        //{
        //    RenderMeshShadowDebug(Camera, DoBlockTexture, false);
        //}

        //public virtual void RenderMeshShadowDebug(vxCamera3D Camera, bool DoBlockTexture, bool DoOnlyShadow)
        //{
        //    //if (Model != null && IsInCameraViewFrustum == true && ShouldDraw == true)
        //    if (IsEntityDrawable)
        //    {
        //        if (vxEngine.BuildType == vxBuildType.Debug)
        //            Engine.GlobalPrimitiveCount += Model.TotalPrimitiveCount;

        //        foreach (vxModelMesh mesh in Model.Meshes)
        //        {
        //            mesh.UtilityEffect.CurrentTechnique = mesh.UtilityEffect.Techniques["Technique_ShadowDebug"];

        //            //mesh.DebugEffect.Parameters["DoOnlyShadow"].SetValue(DoOnlyShadow);
        //            mesh.UtilityEffect.World = TempWorld;
        //            mesh.UtilityEffect.WVP = TempWVP;
        //            mesh.UtilityEffect.Parameters["ShowShadowDebugColours"].SetValue(DoOnlyShadow);
        //            mesh.UtilityEffect.LightDirection = LightDirection;
        //            mesh.Draw(mesh.UtilityEffect);

        //        }
        //    }
        //}

        //public virtual void RenderMeshOutline(vxCamera3D Camera, Color color)
        //{
        //    //if (Model != null && IsInCameraViewFrustum == true && ShouldDraw == true)
        //    if (IsEntityDrawable)
        //    {
        //        foreach (vxModelMesh mesh in Model.Meshes)
        //        {
        //            //mesh.UtilityEffect.CurrentTechnique = mesh.UtilityEffect.Techniques["Technique_ShadowDebug"];

        //            //mesh.DebugEffect.Parameters["DoOnlyShadow"].SetValue(DoOnlyShadow);
        //            //mesh.OutlineEffect.World = TempWorld;
        //            mesh.OutlineEffect.WVP = TempWVP;
        //            mesh.OutlineEffect.SelectionColour = color;
        //            mesh.OutlineEffect.LineThickness = 0.002f;
        //            //mesh.UtilityEffect.LightDirection = LightDirection;
        //            mesh.Draw(mesh.OutlineEffect);

        //        }
        //    }
        //}

        //#endregion
    }
}