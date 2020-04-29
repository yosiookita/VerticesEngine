
#region Using Statements
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;

#endregion

namespace VerticesEngine
{
    public partial class vxGameplayScene3D : vxGameplaySceneBase
    {
        int _totalEntityCount = 0;


        /// <summary>
        /// Is encoded index rendertarget needed for handling selection.
        /// </summary>
        public bool IsEncodedIndexNeeded = true;

        /// <summary>
        /// Gets a new entity handle for this scene.
        /// </summary>
        /// <returns>The new entity handle.</returns>
        public virtual int GetNewEntityHandle()
        {
            return _totalEntityCount++;
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void DrawScene(GameTime gameTime)
        {
            // Zero out Primitive Counts
            //Engine.GlobalPrimitiveCount = 0;
            base.DrawScene(gameTime);


            //// **********************************************************************************************

            //// first render each camera to their respective back buffers
            //foreach (vxCamera camera in Cameras)
            //{
            //    camera.Render();
            //}

            //// now combine each of the camera's to the Engine's Final Back buffer
            //GraphicsDevice.SetRenderTarget(Engine.FinalBackBuffer);
            //SpriteBatch.Begin("Scene Draw");

            //foreach (vxCamera camera in Cameras)
            //{
            //    //    camera.Render();
            //    SpriteBatch.Draw(camera.Renderer.Finalise(), camera.Viewport.Bounds, Color.White);
            //}
            //SpriteBatch.End();

            //ViewportManager.ResetViewport();

            /*
            if (vxEngine.PlatformOS == vxPlatformOS.Android)
            {
                Engine.GraphicsDevice.SetRenderTarget(RenderTargets.MainSceneResult);
                Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                Engine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, SkyColour, 1.0f, 0);

                foreach (vxCamera3D camera in Cameras)
                {
                    camera.SetAsActiveCamera();
                    foreach (vxEntity3D entity in Entities)
                        entity.RenderMesh(camera);
                }

            }
            else
            {
                #region Prep Passes

                // Draw Shadow Map
                // **********************************************************************************************
                foreach (vxCamera3D camera in Cameras)
                {
                    camera.SetAsActiveCamera();
                    GenerateShadowMaps(gameTime, camera);
                }

                // Prepare all Post Processes
                Renderer.Prepare();

                #endregion

                // Draw Main Render Passes
                // **********************************************************************************************
                Engine.GraphicsDevice.SetRenderTarget(RenderTargets.MainSceneResult);
                Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                Engine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, SkyColour, 1.0f, 0);

                if (SceneDebugDisplayMode == vxEnumSceneDebugMode.Default)
                {
                    foreach (vxCamera3D camera in Cameras)
                    {
                        camera.SetAsActiveCamera();
                        RenderMainScene(gameTime);
                    }
                }
                WorkingPlane.DrawPlane(Camera);

                // Generate Light Map
                /****************************************************************************
                Renderer.DefferredRenderPostProcess.GenerateLightMap(Cameras);

                ViewportManager.ResetViewport();

                // Apply Post Processing Effects
                //**********************************************************************************************
                Renderer.ApplyPostProcessors();

                SpriteBatch.Begin("UI.HUD");
                //SpriteBatch.Draw(Engine.FinalSceneRenderTarget, Viewport.Bounds, Color.White);


                DrawHUD();

                if (PauseAlpha > 0)
                {
                    float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, PauseAlpha);
                    Engine.SpriteBatch.Draw(RenderTargets.BlurredSceneResult, Viewport.Bounds, Color.White * alpha);
                }
                SpriteBatch.End();

        }
                */
        }


        /// <summary>
        /// Draws the HUD Once the entire scene has been rendered.
        /// </summary>
        public override void DrawHUD()
        {
            base.DrawHUD();
            DrawViewportSplitters();
        }

        public override void DrawOverlayItems()
        {
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            for (int c = 0; c < Cameras.Count; c++)
            {
                for (int i = 0; i < EditorEntities.Count; i++)
                {
                    EditorEntities[i].RenderOverlayMesh((vxCamera3D)Cameras[c]);
                }
            }

        }

        public override void DrawViewportSplitters()
        {
            // Draw Split line(s)

            // Always draw the Horizontal Line if the Viewport Count is greater than 1
            if (ViewportManager.NumberOfViewports > 1)
            {
                Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank,
                                        new Rectangle(
                                            0,
                                            Cameras[0].Viewport.Bounds.Bottom - 1,
                                            ViewportManager.MainViewport.Width,
                                            2),
                                        Color.Black);
                // Now draw the Verticle Line if the Viewport Count is greater than 2
                if (ViewportManager.NumberOfViewports > 2)
                {

                    Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank,
                                            new Rectangle(
                                                Cameras[0].Viewport.Bounds.Right - 1,
                                                0,
                                                2,
                                               ViewportManager.MainViewport.Height),
                                            Color.Black);
                }
            }
        }

        /*
        /// <summary>
        /// This is the Render Call for the Main Scene which will have all post processes applied to it.
        /// Override this Method if you want to add or change the main scene which is rendered.
        /// </summary>
        /// <param name="gameTime"></param>
		public virtual void RenderMainScene(GameTime gameTime)
        {
            // Render Main Pass
            //**********************************************************************************************
            foreach (vxEntity3D entity in Entities)
                entity.RenderMesh(Camera);

            
            // Now Draw Water Entities
            //**********************************************************************************************
            foreach (vxWaterEntity water in WaterEntities)
                water.DrawWater(Camera);


            if (SandboxCurrentState == vxEnumSandboxStatus.EditMode)
            {
                Engine.GraphicsDevice.RasterizerState = rs_wire;
                foreach (vxEntity3D entity in SelectedItems)
                    entity.RenderDebugWire(Camera, true);

                Engine.GraphicsDevice.RasterizerState = rs_solid;
            }
        }
        */
    }
}