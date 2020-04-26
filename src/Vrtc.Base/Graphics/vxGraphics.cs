using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// Graphical Variables like the Main Sprite Sheet and Getting New Render Textures
    /// </summary>
    public static class vxGraphics
    {

        private static RasterizerState wireframeRasterizerState;
        private static RasterizerState solidRasterizerState;

        internal static void Init()
        {
            wireframeRasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame, };
            solidRasterizerState = new RasterizerState() { FillMode = FillMode.Solid, };
        }

        /// <summary>
        /// The current Engine Graphics Device
        /// </summary>
        public static GraphicsDevice GraphicsDevice
        {
            get { return vxEngine.Instance.Game.GraphicsDevice; }
        }

        /// <summary>
        /// The Current Presentation Parameters
        /// </summary>
        public static PresentationParameters PresentationParameters
        {
            get { return vxEngine.Instance.Game.GraphicsDevice.PresentationParameters; }
        }



        /// <summary>
        /// The main render target whithout any GUI and Overlay Items.
        /// </summary>
        public static RenderTarget2D FinalBackBuffer;




        /// <summary>
        /// The global sprite sheet which all draw calls should reference for effeciency.
        /// </summary>
        public static Texture2D MainSpriteSheet;

        /// <summary>
        /// Creates a new render target using the current resolution
        /// </summary>
        /// <returns></returns>
        public static RenderTarget2D GetNewRenderTarget()
        {
            return GetNewRenderTarget(1);
        }

        public static RenderTarget2D GetNewRenderTarget(float scale)
        {
            return GetNewRenderTarget((int)(PresentationParameters.BackBufferWidth * scale), (int)(PresentationParameters.BackBufferHeight * scale));
        }

        /// <summary>
        /// Creates a new render target using the current resolution with the specified width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mipmap"></param>
        /// <returns></returns>
        public static RenderTarget2D GetNewRenderTarget(int width, int height, bool mipmap = false)
        {
            return new RenderTarget2D(GraphicsDevice, PresentationParameters.BackBufferWidth, PresentationParameters.BackBufferHeight,
                mipmap, PresentationParameters.BackBufferFormat, PresentationParameters.DepthStencilFormat);
        }


        /// <summary>
        /// Inits the main rendertarget which is the final screen which is drawn.
        /// </summary>
        public static void InitMainBuffer()
        {
#if __IOS__
            if (Game.Config.MainOrientation == vxEnumMainOrientation.Portrait)
            {
                FinalSceneRenderTarget = new RenderTarget2D(GraphicsDevice,
                        Game.GraphicsDeviceManager.PreferredBackBufferHeight,
                                                          Game.GraphicsDeviceManager.PreferredBackBufferWidth,
                        false,
                        SurfaceFormat.Color,
                        DepthFormat.Depth24Stencil8);
            }

            else
            {
                FinalSceneRenderTarget = new RenderTarget2D(GraphicsDevice,
                        Game.GraphicsDeviceManager.PreferredBackBufferWidth,
                                                          Game.GraphicsDeviceManager.PreferredBackBufferHeight,
                        false,
                        SurfaceFormat.Color,
                        DepthFormat.Depth24Stencil8);
            }
#else

            FinalBackBuffer = new RenderTarget2D(GraphicsDevice, vxScreen.Width, vxScreen.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
#endif

        }

        /// <summary>
        /// Finalises the Frame 
        /// </summary>
        public static void Finalise()
        {
            // Get the Final Scene
            vxEngine.Instance.Game.GraphicsDevice.SetRenderTarget(null);

            vxEngine.Instance.SpriteBatch.Begin("Engine.DrawScenesToBackBuffer");
            vxEngine.Instance.SpriteBatch.Draw(FinalBackBuffer, GraphicsDevice.Viewport.Bounds, Color.White);
        }



        /// <summary>
        /// Sets the rasterizer state which allows for switching between Wireframe and Solid
        /// </summary>
        /// <param name="fillMode"></param>
        public static void SetRasterizerState(FillMode fillMode)
        {
            vxEngine.Instance.GraphicsDevice.RasterizerState = fillMode == FillMode.WireFrame ? wireframeRasterizerState : solidRasterizerState;
        }
    }
}
