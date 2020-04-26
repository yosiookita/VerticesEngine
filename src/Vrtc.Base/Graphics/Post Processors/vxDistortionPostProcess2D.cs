
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VerticesEngine;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// Grab a scene that has already been rendered, 
    /// and add a distortion effect over the top of it.
    /// </summary>
    public class vxDistortionPostProcess2D : vxRenderPass, vxIRenderPass
    {

        vxGameplayScene2D CurrentScene;

        vxMainScene2DRenderPass mainPass;

        /// <summary>
        /// The Scene Texture.
        /// </summary>
        public RenderTarget2D SceneTexture
        {
            set { Parameters["SceneTexture"].SetValue(value); }
        }

        /// <summary>
        /// The Distortion Map
        /// </summary>
        public RenderTarget2D DistortionMap
        {
            get { return _distortionMap; }
            set
            {
                _distortionMap = value;
                Parameters["DistortionMap"].SetValue(value);
            }
        }
        public RenderTarget2D _distortionMap;

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


        public vxDistortionPostProcess2D(vxRenderer Renderer) :
        base(Renderer, "Distortion", vxInternalAssets.PostProcessShaders.DistortSceneEffect)
        {
            //Engine.Assets.PostProcessShaders.DistortSceneEffect.Parameters["ZeroOffset"].SetValue(0.5f / 255.0f);
            //DoDistortionBlur = false;
            CurrentScene = Engine.GetCurrentScene<vxGameplayScene2D>();
            mainPass = Renderer.GetRenderingPass<vxMainScene2DRenderPass>();
        }


        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();
            //vxInternalAssets.PostProcessShaders.DistortSceneEffect.Parameters["MatrixTransform"].SetValue(MatrixTransform);

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            DistortionMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                                              false, pp.BackBufferFormat, pp.DepthStencilFormat);

        }

        public void Prepare()
        {

        }

        public void Apply()
        {
            // in mobile, draw a different type 
            if (vxEngine.PlatformType == vxPlatformType.Mobile)
            {
                SpriteBatch.Begin("PostProcess.Distortion2D.Prepare()", 0, null, null, null, null, null, Renderer.Camera.View);

                foreach (var entity in CurrentScene.Entities)
                    ((vxEntity2D)entity).DrawDistoriton();
                SpriteBatch.End();
            }
            else
            {
                // Draw Distortion Stuff
                Engine.GraphicsDevice.SetRenderTarget(_distortionMap);

                Engine.GraphicsDevice.Clear(Color.Transparent);

                Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;


                SpriteBatch.Begin("PostProcess.Distortion2D.Prepare()", 0, null, null, null, null, null, Renderer.Camera.View);

                foreach (var entity in CurrentScene.Entities)
                    ((vxEntity2D)entity).DrawDistoriton();

                SpriteBatch.End();


                // pass in the RMA map and light map and apply it to the scene
                GraphicsDevice.SetRenderTarget(Renderer.GetNewTempTarget("Distortion Pass"));

                Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                //Engine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                Parameters["SceneTexture"].SetValue(Renderer.GetCurrentTempTarget());
                Parameters["DistortionMap"].SetValue(_distortionMap);

                Effect.CurrentTechnique = Effect.Techniques["Distort"];
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Renderer.RenderQuad(Vector2.One * -1, Vector2.One);
                }
            }
            /*
            // if we want to show the distortion map, then the backbuffer is done.
            // if we want to render the scene distorted, then we need to resolve the
            // backbuffer as the distortion map and use it to distort the scene
            Engine.GraphicsDevice.SetRenderTarget(Renderer.GetNewTempTarget("Distort Pass"));

            // draw the scene image again, distorting it with the distortion map
            DistortionMap = _distortionMap;

            Effect.CurrentTechnique = Effect.Techniques["Distort"];

            DrawFullscreenQuad("PostProcess.Disortion2D.Apply()", Renderer.GetCurrentTempTarget(),
                Viewport.Width, Viewport.Height, Effect);
            */
        }
    }
}
