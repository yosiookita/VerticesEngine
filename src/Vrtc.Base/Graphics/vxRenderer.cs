using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using VerticesEngine.Diagnostics;
using VerticesEngine.Input;
using VerticesEngine.Utilities;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// This component renders a scene given the camera
    /// </summary>
    public class vxRenderer : vxGameObject
    {
        #region Engine Constants
        /// <summary>
        /// Collection of Engine Passes
        /// </summary>
        public static class Passes
        {
            public const string OpaquePass = "OpaquePass";

            public const string TransparencyPass = "TransparencyPass";

            public const string ParticlePrePass = "ParticlePrePass";

            public const string ParticlePostPass = "ParticlePostPass";
        }
        #endregion

        /// <summary>
        /// The camera which is using this Renderer
        /// </summary>
        public vxCamera Camera { get; private set; }

        /// <summary>
        /// A list of rendering passes to be applied to by this camera
        /// </summary>
        public List<vxIRenderPass> RenderingPasses = new List<vxIRenderPass>();


        /// <summary>
        /// The index list of items to draw from the culling list
        /// </summary>
        internal int[] drawList = new int[MaxNumberOfItems];

        const int MaxNumberOfItems = 2048;

        public int totalItemsToDraw = 0;

        #region -- Render Quad Fields --

        /// <summary>
        /// The quad renderer vertices buffer.
        /// </summary>
        VertexPositionTexture[] quadRendererVerticesBuffer = null;

        /// <summary>
        /// The quad renderer index buffer.
        /// </summary>
        short[] quadRendererIndexBuffer = null;

        #endregion


        #region -- Render Targets --

        // Public Render Targets
        // **********************************************

        /// <summary>
        /// The Main Scene Render Target. This holds all of the Scene which all of the Post Processes are eventually applied to.
        /// </summary>
        //public RenderTarget2D MainSceneResult;


        /// <summary>
        /// Normal Render Target.
        /// </summary>
        public RenderTarget2D NormalMap;

        /// <summary>
        /// Depth Render Target.
        /// </summary>
        public RenderTarget2D DepthMap;

        
        // Internal Render Targets
        // **********************************************

        /// <summary>
        ///Render Target which holds Surface Data such as Specular Power, Intensity as well as Shadow Factor.
        /// </summary>
        public RenderTarget2D SurfaceDataMap;


        /// <summary>
        /// This render target holds mask information for different entities (i.e. do edge detection? do motion blur, etc...)
        /// </summary>
        public RenderTarget2D EntityMaskValues;

        /// <summary>
        /// Encoded Index Result. This is needed only internally
        /// </summary>
        internal RenderTarget2D EncodedIndexResult;


        /// <summary>
        /// Auz Depth Map
        /// </summary>
        public RenderTarget2D AuxDepthMap;


        public RenderTarget2D BlurredScene;

        #region - Temp Targets -

        /// <summary>
        /// The post process temp targets collection.
        /// </summary>
        public RenderTarget2D[] PostProcessTargets;// = new List<RenderTarget2D>();

        // Temp Targets Index
        int tempTargetIndex = 0;

        /// <summary>
        /// The number of temp targets used.
        /// </summary>
        public int TempTargetsUsed = 0;

        public int TempTargetCount = 8;

        #endregion

        #endregion

        /// <summary>
        /// The current scene
        /// </summary>
        protected vxGameplaySceneBase CurrentScene;

        /// <summary>
        /// Light Direction. In 2D only 'x' and 'y' are taken
        /// </summary>
        public Vector3 LightDirection;

        public vxRenderer(vxCamera camera)
        {
            this.Camera = camera;

            CurrentScene = Engine.GetCurrentScene<vxGameplaySceneBase>();

            // quad renderer vert buffer
            quadRendererVerticesBuffer = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(0,0,0), new Vector2(1,1)),
                new VertexPositionTexture(new Vector3(0,0,0), new Vector2(0,1)),
                new VertexPositionTexture(new Vector3(0,0,0), new Vector2(0,0)),
                new VertexPositionTexture(new Vector3(0,0,0), new Vector2(1,0))
            };

            // quad renderer index buffer
            quadRendererIndexBuffer = new short[] { 0, 1, 2, 2, 3, 0 };
        }


        


        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();

            //MainSceneResult = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8 );
#if __ANDROID__
            DepthMap = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
            AuxDepthMap = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);

#else
            
            DepthMap = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Single, DepthFormat.None);
            AuxDepthMap = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Single, DepthFormat.None);

            //AuxDepthMap = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Single, DepthFormat.None);
#endif
            NormalMap = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
            SurfaceDataMap = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            EncodedIndexResult = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            BlurredScene = new RenderTarget2D(GraphicsDevice, Camera.Viewport.Width, Camera.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);


            EncodedIndexPixels = new Color[EncodedIndexResult.Width * EncodedIndexResult.Height];

            var tempTrgts = new List<RenderTarget2D>();

#if __ANDROID__
            //var w = Engine.Game.GraphicsDeviceManager.PreferredBackBufferWidth;
            //var h = Engine.Game.GraphicsDeviceManager.PreferredBackBufferHeight;
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            //Console.WriteLine("CREATING: " + w + "," + h);

            for (int i = 0; i < TempTargetCount; i++)
            {
                tempTrgts.Add(new RenderTarget2D(GraphicsDevice, vxScreen.Width, vxScreen.Height, false,
                                                      pp.BackBufferFormat, pp.DepthStencilFormat));
            }

            //PostProcessTargets[i] = new RenderTarget2D(GraphicsDevice, w, h, false,
                                                  //pp.BackBufferFormat, pp.DepthStencilFormat);
#else
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            for (int i = 0; i < TempTargetCount; i++)
            {
                tempTrgts.Add(vxGraphics.GetNewRenderTarget(Camera.Viewport.Width, Camera.Viewport.Height));
            }
#endif
            PostProcessTargets = tempTrgts.ToArray();

            foreach (var pass in RenderingPasses)
                pass.OnGraphicsRefresh();


        }

        enum RenderView
        {
            RegularScene,
            Physics
        }

        /// <summary>
        /// This renders the scene
        /// </summary>
        public void RenderScene()
        {
            tempTargetIndex = 0;
            // perform all prep work here
            for (int p = 0; p< RenderingPasses.Count; p++)
                RenderingPasses[p].Prepare();


            // now apply the rendering passes
            for (int p = 0; p < RenderingPasses.Count; p++)
                RenderingPasses[p].Apply();

        }

        public void DrawDebug()
        {
            if (vxDebug.IsDebugRenderTargetsVisible)
                DrawDebugRenderTargets();
        }


        public override void Dispose()
        {
            base.Dispose();

            foreach (var pass in RenderingPasses)
                pass.Dispose();
        }

        #region -- Utility Functions --


        /// <summary>
        /// Gets a new temp target.
        /// </summary>
        /// <returns>The new temp target.</returns>
        public RenderTarget2D GetNewTempTarget(string name)
        {
            tempTargetIndex++;
            PostProcessTargets[tempTargetIndex].Name = name;
            return PostProcessTargets[tempTargetIndex];
        }

        /// <summary>
        /// Gets the current temp target. This pushes the stack forward. Use peek if you arent' drawing with this one to the new one.
        /// </summary>
        /// <returns>The current temp target.</returns>
        public RenderTarget2D GetCurrentTempTarget()
        {
            return PostProcessTargets[tempTargetIndex-1];
        }

        public virtual RenderTarget2D Finalise()
        {
            
            TempTargetsUsed = tempTargetIndex;
            vxConsole.WriteInGameDebug(this, "Temp Targets Used: " + TempTargetsUsed);
            return PostProcessTargets[tempTargetIndex];
        }

        /// <summary>
        /// Render the specified v1 and v2.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        public void RenderQuad(Vector2 v1, Vector2 v2)
        {
            quadRendererVerticesBuffer[0].Position.X = v2.X;
            quadRendererVerticesBuffer[0].Position.Y = v1.Y;

            quadRendererVerticesBuffer[1].Position.X = v1.X;
            quadRendererVerticesBuffer[1].Position.Y = v1.Y;

            quadRendererVerticesBuffer[2].Position.X = v1.X;
            quadRendererVerticesBuffer[2].Position.Y = v2.Y;

            quadRendererVerticesBuffer[3].Position.X = v2.X;
            quadRendererVerticesBuffer[3].Position.Y = v2.Y;

            Engine.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, quadRendererVerticesBuffer, 0, 4, quadRendererIndexBuffer, 0, 2);
        }

        /// <summary>
        /// Returns a Rendering Pass that is registered to this Renderer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetRenderingPass<T>() where T : vxIRenderPass
        {
            foreach (var pass in RenderingPasses)
            {
                if (pass.GetType() == typeof(T))
                    return (T)pass;
            }
            return default(T);
        }


        public Color GetEncodedIndex(int x, int y)
        {
            var i = (y * EncodedIndexResult.Width) + x;
            EncodedIndexResult.GetData<Color>(EncodedIndexPixels);
            
            return i < EncodedIndexPixels.Length ? EncodedIndexPixels[i] : Color.Black;
        }

        Color[] EncodedIndexPixels;// = new Color[rt.Width * rt.Height];

        #endregion

        #region -- Debug --

        protected internal Vector2 debugRTPos = new Vector2(0, 0);
        protected internal int debugRTWidth = 200;
        protected internal int debugRTHeight = 200;
        protected internal int debugRTPadding = 2;
        protected internal int debugRTScale = 4;
        protected internal int rtDb_count = 0;

        class RenderTargetDebug
        {
            public string name;

            public RenderTarget2D renderTarget;

            public RenderTargetDebug(string name, RenderTarget2D renderTarget)
            {
                this.name = name;
                this.renderTarget = renderTarget;
            }
        }

        List<RenderTargetDebug> debugRenderTargets = new List<RenderTargetDebug>();

        
        public void RegisterDebugRenderTarget(string name, RenderTarget2D renderTarget)
        {
            debugRenderTargets.Add(new RenderTargetDebug(name, renderTarget));
        }
        

        void DrawDebugRenderTargets()
        {

            SpriteFont font = vxInternalAssets.Fonts.DebugFont;


            if (vxInput.IsKeyDown(Keys.OemPlus))
                debugRTPos -= Vector2.UnitX * 15;
            else if (vxInput.IsKeyDown(Keys.OemMinus))
                debugRTPos += Vector2.UnitX * 15;


            // Clamp Debug Position
            debugRTPos.X = MathHelper.Clamp(debugRTPos.X, -debugRTWidth * rtDb_count, 0);
            rtDb_count = 0;

            Engine.SpriteBatch.Begin("Debug.RenderTargetsDump", 0, BlendState.Opaque, SamplerState.PointClamp, null, null);


            Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank,
                new Rectangle(0, 0, Engine.GraphicsDevice.Viewport.Width, debugRTHeight + font.LineSpacing + 2 * debugRTPadding), Color.Black * 0.5f);


            // first draw all registered RTs
            foreach(var rt in debugRenderTargets)
            {
                DrawRT(rt.name, rt.renderTarget);
            }

            for (int i = 0; i < TempTargetsUsed; i++)
            {
                var trgt = PostProcessTargets[i];
                if (trgt != null)
                    DrawRT(trgt.Name, trgt);
            }

            Engine.SpriteBatch.End();
        }

        public void DrawRT(string name, RenderTarget2D rt)
        {
            if (rt != null)
            {
                SpriteFont font = vxInternalAssets.Fonts.DebugFont;
                debugRTHeight = Viewport.Height / debugRTScale;
                debugRTWidth = Viewport.Width / debugRTScale;//* debugRTHeight / Viewport.Height;

                Point rtDb_pos = new Point(rtDb_count * debugRTWidth, 0) + debugRTPos.ToPoint();
                Engine.SpriteBatch.Draw(rt, new Rectangle(rtDb_pos.X, rtDb_pos.Y + 16, debugRTWidth, debugRTHeight), Color.White);
                //Engine.SpriteBatch.Draw(rt, Viewport.Bounds, Color.White);
                if (name != null)
                {
                    //DrawDebugStrings();
                    var pos = new Vector2(debugRTWidth / 2 - font.MeasureString(name).X / 2,
                                          debugRTHeight + debugRTPadding) + rtDb_pos.ToVector2();


                    pos = rtDb_pos.ToVector2() + Vector2.One;
                    //Engine.SpriteBatch.Draw(DefaultTexture, new Rectangle(0,0, Viewport.Width, 24), Color.Black);

                    // Engine.SpriteBatch.DrawString(font, name, pos + Vector2.One, Color.Black);
                    Engine.SpriteBatch.DrawString(font, name, pos, Color.White);
                    rtDb_count++;
                }
            }
        }

        #endregion
    }
}
