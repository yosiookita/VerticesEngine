
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// Grab a scene that has already been rendered, 
    /// and add a distortion effect over the top of it.
    /// </summary>
    public class vxDistortionPostProcess : vxRenderPass, vxIRenderPass
    {
        /// <summary>
        /// The Scene Texture.
        /// </summary>
        public RenderTarget2D SceneTexture
        {
            set { Parameters["SceneTexture"].SetValue(value); }
        }


        vxMainScene3DRenderPass mainPass;

        vxScenePrepRenderingPass prepPass;

        /// <summary>
        /// Depth map of the scene.
        /// </summary>
        public RenderTarget2D DepthMap
        {
            set { Parameters["DepthMap"].SetValue(value); }
        }

        /// <summary>
        /// Should the Distortion be blurred?
        /// </summary>
        public bool DoDistortionBlur
        {
            set { Parameters["distortionBlur"].SetValue(value); }
        }


        public vxDistortionPostProcess(vxRenderer Renderer) :
        base(Renderer, "Distortion", vxInternalAssets.PostProcessShaders.DistortSceneEffect)
        {
            //Engine.Assets.PostProcessShaders.DistortSceneEffect.Parameters["ZeroOffset"].SetValue(0.5f / 255.0f);

            mainPass = Renderer.GetRenderingPass<vxMainScene3DRenderPass>();
            prepPass = Renderer.GetRenderingPass<vxScenePrepRenderingPass>();
        }




        public void Prepare()
        {

        }

        public void Apply()
        {

            // pass in the RMA map and light map and apply it to the scene
            GraphicsDevice.SetRenderTarget(Renderer.GetNewTempTarget("Distortion Pass"));

            Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //Engine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Parameters["SceneTexture"].SetValue(Renderer.GetCurrentTempTarget());
            Parameters["DistortionMap"].SetValue(prepPass.DistortionMap);

            Effect.CurrentTechnique = Effect.Techniques["Distort"];
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Renderer.RenderQuad(Vector2.One * -1, Vector2.One);
            }
        }
    }
}
