using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace VerticesEngine.Graphics
{
    public class vxScenePrepRenderingPass : vxRenderPass, vxIRenderPass
    {
        public RenderTarget2D CubeReflcMap;
        public RenderTarget2D DistortionMap;

        vxCascadeShadowRenderPass shadowPass;


        [vxGraphicalSettingsAttribute("NormalsQuality")]
        public static vxEnumQuality NormalsQuality = vxEnumQuality.Medium;

        [vxGraphicalSettingsAttribute("DoDistortion")]
        public static bool DoDistortion = true;

        /// <summary>
        /// Entity Mask
        /// </summary>
        RenderTarget2D EntityMaskValues;

        public vxScenePrepRenderingPass(vxRenderer Renderer) :
        base(Renderer, "Scene Prep Rendering Pass", vxInternalAssets.Shaders.PrepPassShader)
        {

            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            CubeReflcMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);

            DistortionMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);

            EntityMaskValues = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);

            shadowPass = Renderer.GetRenderingPass<vxCascadeShadowRenderPass>();
        }

        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();
        }

        public override void RegisterRenderTargetsForDebug()
        {
            base.RegisterRenderTargetsForDebug();

            Renderer.RegisterDebugRenderTarget("RMA Map", Renderer.SurfaceDataMap);
            Renderer.RegisterDebugRenderTarget("Normal Map", Renderer.NormalMap);
            Renderer.RegisterDebugRenderTarget("Depth Map", Renderer.DepthMap);
            Renderer.RegisterDebugRenderTarget("AuxDepthMap Map", Renderer.AuxDepthMap);
            Renderer.RegisterDebugRenderTarget("Distortion Map", DistortionMap);
            Renderer.RegisterDebugRenderTarget("EncodedIndexResult Map", Renderer.EncodedIndexResult);
            Renderer.RegisterDebugRenderTarget("EntityMaskValues Map", EntityMaskValues);
        }

        public void Prepare()
        {
            GeneratePrePass();
            GenerateDataMaskPass();
        }

        void GeneratePrePass()
        {
            // Set Multi Rendertargets
            Engine.GraphicsDevice.SetRenderTargets(
                Renderer.SurfaceDataMap,
                Renderer.NormalMap,
                Renderer.DepthMap);


            //Setup initial graphics states for prep pass            
            Engine.GraphicsDevice.BlendState = BlendState.Opaque;
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            //Reset Appropriate Values for Rendertargets
            vxInternalAssets.PostProcessShaders.DrfrdRndrClearGBuffer.Techniques[0].Passes[0].Apply();
            Renderer.RenderQuad(Vector2.One * -1, Vector2.One);

            //Set the Depth Buffer appropriately
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            for (int i = 0; i < Renderer.totalItemsToDraw; i++)
            {
                vxEntity3D entity = (vxEntity3D)Engine.CurrentScene.Entities[Renderer.drawList[i]];
                if (entity.IsEntityDrawable)
                {
                    Matrix TempWorld = entity.WorldTransform;// * Renderer.Camera.ViewProjection;
                    Matrix TempWVP = entity.WorldTransform * Renderer.Camera.ViewProjection;

                    for (int mi = 0; mi < entity.Model.Meshes.Count; mi++)
                    {
                        if (entity.Model.Meshes[mi].Material.IsDefferedRenderingEnabled)
                        {
                            entity.Model.Meshes[mi].Material.UtilityEffect.CurrentTechnique = entity.Model.Meshes[mi].Material.UtilityEffect.Techniques["Technique_PrepPass"];
                            entity.Model.Meshes[mi].Material.UtilityEffect.World = TempWorld;
                            entity.Model.Meshes[mi].Material.UtilityEffect.WVP = TempWVP;

                            //ShadowBrightness = 0.5f;

                            if (vxCascadeShadowRenderPass.ShadowQaulity > vxEnumQuality.None)
                            {
                                // Shadow Parameters
                                //entity.Model.Meshes[mi].Material.UtilityEffect.ShadowBrightness = 0.25f;
                                entity.Model.Meshes[mi].Material.UtilityEffect.ShadowBlurStart = 2;
                                entity.Model.Meshes[mi].Material.UtilityEffect.ShadowMap = shadowPass.ShadowMap;
                                entity.Model.Meshes[mi].Material.UtilityEffect.ShadowTransform = shadowPass.ShadowSplitProjectionsWithTiling;
                                entity.Model.Meshes[mi].Material.UtilityEffect.TileBounds = shadowPass.ShadowSplitTileBounds;
                            }

                            entity.Model.Meshes[mi].Draw(entity.Model.Meshes[mi].Material.UtilityEffect);
                        }
                    }
                }
            }
        }

        void GenerateDataMaskPass()
        {
            // Set Multi Rendertargets
            Engine.GraphicsDevice.SetRenderTargets(
                    Renderer.EncodedIndexResult,
                    EntityMaskValues,
                    Renderer.AuxDepthMap,
                DistortionMap);

            Engine.GraphicsDevice.BlendState = BlendState.Opaque;
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.Clear(Color.TransparentBlack);

            //Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            //Reset Appropriate Values for Rendertargets
            //vxInternalAssets.PostProcessShaders.DrfrdRndrClearGBuffer.Techniques[0].Passes[0].Apply();
            //Renderer.RenderQuad(Vector2.One * -1, Vector2.One);

            //Set the Depth Buffer appropriately
            //Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            for (int i = 0; i < Renderer.totalItemsToDraw; i++)
            {
                DrawEntity((vxEntity3D)Engine.CurrentScene.Entities[Renderer.drawList[i]]);
                //   vxEntity3D entity = (vxEntity3D)Engine.CurrentScene.Entities[Renderer.drawList[i]];
                //if (entity.IsEntityDrawable)
                //{
                //    Matrix TempWorld = entity.WorldTransform;// * Renderer.Camera.ViewProjection;
                //    Matrix TempWVP = entity.WorldTransform * Renderer.Camera.ViewProjection;

                //    for (int mi = 0; mi < entity.Model.Meshes.Count; mi++)
                //    {
                //            entity.Model.Meshes[mi].Material.UtilityEffect.CurrentTechnique = entity.Model.Meshes[mi].Material.UtilityEffect.Techniques["Technique_DataMaskPass"];

                //            entity.Model.Meshes[mi].Material.UtilityEffect.Parameters["IndexEncodedColour"].SetValue(entity.IndexEncodedColour.ToVector4());
                //            entity.Model.Meshes[mi].Material.UtilityEffect.World = TempWorld;
                //            entity.Model.Meshes[mi].Material.UtilityEffect.WVP = TempWVP;

                //            entity.Model.Meshes[mi].Draw(entity.Model.Meshes[mi].Material.UtilityEffect);
                //    }
                //}

                // if it's in sandbox mode, re draw the editor items
                //}

            }
            /*
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            ;

            */
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            foreach (var entity in Engine.GetCurrentScene<vxGameplayScene3D>().EditorEntities)
            {
                DrawEntity(entity);
            }
        }

        void DrawEntity(vxEntity3D entity)
        {
            if (entity.IsEntityDrawable && entity.IsVisible)
            {
                Matrix TempWorld = entity.WorldTransform;// * Renderer.Camera.ViewProjection;
                Matrix TempWVP = entity.WorldTransform * Renderer.Camera.ViewProjection;

                for (int mi = 0; mi < entity.Model.Meshes.Count; mi++)
                {
                    entity.Model.Meshes[mi].Material.UtilityEffect.CurrentTechnique = entity.Model.Meshes[mi].Material.UtilityEffect.Techniques["Technique_DataMaskPass"];

                    entity.Model.Meshes[mi].Material.UtilityEffect.Parameters["IndexEncodedColour"].SetValue(entity.IndexEncodedColour.ToVector4());
                    entity.Model.Meshes[mi].Material.UtilityEffect.World = TempWorld;
                    entity.Model.Meshes[mi].Material.UtilityEffect.WVP = TempWVP;

                    entity.Model.Meshes[mi].Draw(entity.Model.Meshes[mi].Material.UtilityEffect);
                }
            }
        }

        public void Apply()
        {
            //
        }
    }
}
