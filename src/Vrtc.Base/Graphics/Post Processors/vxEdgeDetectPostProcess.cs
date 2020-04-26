using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VerticesEngine.Graphics
{
    public class vxEdgeDetectPostProcess : vxRenderPass, vxIRenderPass
    {

        vxMainScene3DRenderPass mainPass;

        //vxScenePrepRenderingPass prepPass;

        public RenderTarget2D NormalMap
        {
            set { Parameters["NormalTexture"].SetValue(value); }
        }

        /// <summary>
        /// Normal Map.
        /// </summary>
        public RenderTarget2D SceneTexture
        {
            set { Parameters["SceneTexture"].SetValue(value); }
        }

        /// <summary>
        /// Depth map of the scene.
        /// </summary>
        public RenderTarget2D DepthMap
        {
            set { Parameters["DepthTexture"].SetValue(value); }
        }

        /// <summary>
        /// Edge Width for edge detection.
        /// </summary>
        public float EdgeWidth
        {
            set { Parameters["EdgeWidth"].SetValue(value); }
        }

        /// <summary>
        /// Intensity of the edge dection.
        /// </summary>
        public float EdgeIntensity
        {
            set { Parameters["EdgeIntensity"].SetValue(value); }
        }





        public float NormalThreshold
        {
            set { Parameters["NormalThreshold"].SetValue(value); }
        }


        public float DepthThreshold
        {
            set { Parameters["DepthThreshold"].SetValue(value); }
        }

        public float NormalSensitivity
        {
            set { Parameters["NormalSensitivity"].SetValue(value); }
        }


        public float DepthSensitivity
        {
            set { Parameters["DepthSensitivity"].SetValue(value); }
        }

        public vxEdgeDetectPostProcess(vxRenderer renderer) :base(renderer, "Edge Detect", vxInternalAssets.PostProcessShaders.CartoonEdgeDetect)
        {

            // Set Edge Settings
            EdgeWidth = 1;// 0.5f;
            EdgeIntensity = 1;


            // How sensitive should the edge detection be to tiny variations in the input data?
            // Smaller settings will make it pick up more subtle edges, while larger values get
            // rid of unwanted noise.
            NormalThreshold = 0.5f;
            DepthThreshold = 0.005f;

            // How dark should the edges get in response to changes in the input data?
            NormalSensitivity = 10.0f;
            DepthSensitivity = 1000;

            mainPass = renderer.GetRenderingPass<vxMainScene3DRenderPass>();
        }
        [vxGraphicalSettings("IsEdgeDetectionEnabled")]
        public static bool IsEdgeDetectionEnabled = true;

        public void Prepare() { }

        public void Apply()
        {

            if (IsEdgeDetectionEnabled)
            {

                //Set Render Target
                GraphicsDevice.SetRenderTarget(Renderer.GetNewTempTarget("Edge Pass"));
                Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                // Pass in the Normal Map
                SceneTexture = Renderer.GetCurrentTempTarget();
                NormalMap = Renderer.NormalMap;


                // Pass in the Depth Map
                DepthMap = Renderer.DepthMap;


                // Activate the appropriate effect technique.
                Effect.CurrentTechnique = Effect.Techniques["EdgeDetect"];

                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Renderer.RenderQuad(Vector2.One * -1, Vector2.One);
                }

            }
        }
    }
}