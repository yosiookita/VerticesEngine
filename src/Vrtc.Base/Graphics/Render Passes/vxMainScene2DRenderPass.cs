using Microsoft.Xna.Framework.Graphics;
using System;
using VerticesEngine.Diagnostics;
using VerticesEngine;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// This is the main scene render pass for a 3D scene. This first draws all meshes' with opaque and then performs a alpha pass
    /// </summary>
    public class vxMainScene2DRenderPass : vxRenderPass, vxIRenderPass
    {

        vxGameplayScene2D CurrentScene;

        public override string RenderPass
        {
            get { return vxRenderer.Passes.OpaquePass; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxMainScene2DRenderPass"/> class.
        /// </summary>
        /// <param name="Renderer">Renderer.</param>
        public vxMainScene2DRenderPass(vxRenderer Renderer) :
        base(Renderer, "Main Scene Post Process", vxInternalAssets.Shaders.MainShader)
        {
            CurrentScene = Engine.GetCurrentScene<vxGameplayScene2D>();
        }


        public void Prepare()
        {
            // TODO: reorder items based on distance
        }


        public void Apply()
        {
            /*
            // draw the main scene
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Renderer.Camera.BackBufferColour, 1.0f, 0);
            CurrentScene.DrawBackground();
            CurrentScene.PreDraw();

            SpriteBatch.Begin("RenderPass.Main", 0, null, null, null, null, null, Renderer.Camera.View);

            // draw all of th entities
            for (int i = 0; i < Renderer.totalItemsToDraw; i++)
                CurrentScene.Entities[Renderer.drawList[i]].Draw(Renderer.Camera, 0);

            SpriteBatch.End();
            CurrentScene.PostDraw();

            if (shouldDumpCurrentDrawList)
            {
                shouldDumpCurrentDrawList = false;

                for (int i = 0; i < Renderer.totalItemsToDraw; i++)
                    Console.WriteLine(string.Format("{0}: {1}", i, CurrentScene.Entities[Renderer.drawList[i]].Id));
            }
            */
            SpriteBatch.Begin("Main Scene 2D Draw", 0, null, SamplerState.AnisotropicWrap, null, null, null, Renderer.Camera.View);



            // Draw the Particle System
            CurrentScene.ParticleSystem.DrawUnderlayItems(Renderer.Camera);

            foreach (var entity in CurrentScene.Entities)
                entity.PreDraw(RenderPass);

            // draw all of th entities
            foreach (var entity in CurrentScene.Entities)
                entity.Draw(Renderer.Camera, RenderPass);

            foreach (var entity in CurrentScene.Entities)
                entity.PostDraw(RenderPass);

            // Draws the Particles that are infront
            CurrentScene.ParticleSystem.DrawOverlayItems(Renderer.Camera);


            // draw animations on top
            //foreach (vxEntity2D entity in Entities)
            //  entity.DrawAnimation();

            SpriteBatch.End();
            CurrentScene.PostDraw();
        }
        /*
        static bool shouldDumpCurrentDrawList = false;

        [vxDebugMethodAttribute("drawlist", "Dumps out the most recent draw list")]
        static void DumpDrawList()
        {
            shouldDumpCurrentDrawList = true;
        }
        */

    }
}