
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VerticesEngine.Graphics
{

    /// <summary>
    /// The base class for creating a Post Processor for the Vertices Engine
    /// </summary>
    public class vxRenderPass : vxGameObject
    {

        /// <summary>
        /// This represents a Render pass int which can be overrid to pas a custom designed
        /// enum for specifing the rendering pass.
        /// </summary>
        public virtual string RenderPass
        {
            get { return "renderpass_" + this.ToString(); }
        }

        /// <summary>
        /// The name of this post processor.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// The main effect for this Post Process.
        /// </summary>
        public Effect Effect { get; set; }

        /// <summary>
        /// The parameters for the main effect.
        /// </summary>
        public EffectParameterCollection Parameters;

        /// <summary>
        /// The renderering engine which will be using this Post Process.
        /// </summary>
        public vxRenderer Renderer;


        public Vector2 ScreenResolution
        {
            set
            {
                if(Parameters["ScreenResolution"] != null)
                    Parameters["ScreenResolution"].SetValue(value);
            }
        }

        public Matrix OrthogonalProjection;

        public Matrix HalfPixelOffset;

        //[vxPostProcessingPropertyAttribute(Title = "Half Pixel Size")]
        public Vector2 HalfPixel
        {
            get { return _halfPixel; }
            set
            {
                _halfPixel = value;
                if (Parameters["HalfPixel"] != null)
                    Parameters["HalfPixel"].SetValue(value); }
        }
        public Vector2 _halfPixel;

        public Matrix MatrixTransform
        {
            get { return _matrixTransform; }
            set
            {
                _matrixTransform = value;
                if (Parameters["MatrixTransform"] != null)
                    Parameters["MatrixTransform"].SetValue(value);
            }
        }
        Matrix _matrixTransform;


        public vxRenderPass(vxRenderer Renderer, string Name, Effect Effect)
        {
            //Get a Reference to the Engine
            this.Name = Name;
            this.Renderer = Renderer;

            this.Effect = Effect;
            this.Parameters = Effect.Parameters;

        }

        /// <summary>
        /// Register any render targets with the debug system here
        /// </summary>
        public virtual void RegisterRenderTargetsForDebug()
        {

        }

        public virtual void LoadContent()
        {
            OnGraphicsRefresh();
        }

        public virtual void RefreshSettings()
        {

        }



        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();

            ScreenResolution = new Vector2(Engine.GraphicsDevice.Viewport.Width, Engine.GraphicsDevice.Viewport.Height);

            OrthogonalProjection = Matrix.CreateOrthographicOffCenter(0,
                Engine.GraphicsDevice.Viewport.Width,
                Engine.GraphicsDevice.Viewport.Height,
                0, 0, 1);
            
            HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            
            HalfPixel = new Vector2(.5f / (float)Engine.GraphicsDevice.Viewport.Width, .5f / (float)Engine.GraphicsDevice.Viewport.Height);

            MatrixTransform = HalfPixelOffset * OrthogonalProjection;
        }


        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        public void DrawRenderTargetIntoOther(string tag, Texture2D texture, RenderTarget2D renderTarget, Effect effect)
        {
            Renderer.GraphicsDevice.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(tag, texture,
                               renderTarget.Width, renderTarget.Height,
                               effect);
        }


        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="effect"></param>
        public void DrawFullscreenQuad(string tag, Texture2D texture, int width, int height, Effect effect)
        {
            SpriteBatch.Begin(tag, 0, BlendState.Opaque, null, null, null, effect);
            SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            SpriteBatch.End();
        }

        /// <summary>
        /// Draws With No Effect Used
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void DrawFullscreenQuad(string tag, Texture2D texture, int width, int height)
        {
            SpriteBatch.Begin(tag);
            SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            SpriteBatch.End();
        }


        public void SetEffectParameter(string param, float value)
        {
            if (Parameters[param] != null)
                Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, Vector2 value)
        {
            if (Parameters[param] != null)
                Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, Matrix value)
        {
            if (Parameters[param] != null)
                Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, Texture2D value)
        {
            if (Parameters[param] != null)
                Parameters[param].SetValue(value);
        }
    }

}