using Microsoft.Xna.Framework.Graphics;
using System;
using VerticesEngine.Diagnostics;
using VerticesEngine;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// This is the main scene render pass for a 3D scene. This first draws all meshes' with opaque and then performs a alpha pass
    /// </summary>
    public class vxMainScene3DRenderPass : vxRenderPass, vxIRenderPass
    {
        public RenderTarget2D AlbedoPass;

        vxGameplaySceneBase CurrentScene;

        public override string RenderPass
        {
            get { return vxRenderer.Passes.OpaquePass; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxMainScene3DRenderPass"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        public vxMainScene3DRenderPass(vxRenderer Renderer) :
        base(Renderer, "Main Scene Post Process", vxInternalAssets.Shaders.MainShader)
        {
            CurrentScene = Engine.GetCurrentScene<vxGameplaySceneBase>();

            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            AlbedoPass = new RenderTarget2D(GraphicsDevice, Renderer.Camera.Viewport.Width, Renderer.Camera.Viewport.Height , false, pp.BackBufferFormat, pp.DepthStencilFormat);
        }


        public void Prepare()
        {
            // TODO: reorder items based on distance
        }


        public void Apply()
        {
            AlbedoPass = Renderer.GetNewTempTarget("Albedo Pass");

            GraphicsDevice.SetRenderTarget(AlbedoPass);
            // draw the main scene
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.Clear(Renderer.Camera.BackBufferColour);

            // draw all of th entities
            for (int i = 0; i < Renderer.totalItemsToDraw; i++)
            {
                CurrentScene.Entities[Renderer.drawList[i]].Draw(Renderer.Camera, vxRenderer.Passes.OpaquePass);
            }

            for (int i = 0; i < Renderer.totalItemsToDraw; i++)
            {
                CurrentScene.Entities[Renderer.drawList[i]].Draw(Renderer.Camera, vxRenderer.Passes.TransparencyPass);
            }

            if (shouldDumpCurrentDrawList)
            {
                shouldDumpCurrentDrawList = false;

                for (int i = 0; i < Renderer.totalItemsToDraw; i++)
                    Console.WriteLine(string.Format("{0}: {1}", i, CurrentScene.Entities[Renderer.drawList[i]].Id));
            }

        }

        static bool shouldDumpCurrentDrawList = false;

        [vxDebugMethodAttribute("drawlist", "Dumps out the most recent draw list")]
        static void DumpDrawList()
        {
            shouldDumpCurrentDrawList = true;
        }

    }
}