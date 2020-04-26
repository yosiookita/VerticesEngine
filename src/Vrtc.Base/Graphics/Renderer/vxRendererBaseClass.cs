
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;

////Virtex vxEngine Declaration
//using VerticesEngine;
//using VerticesEngine.Utilities;
//using VerticesEngine.Cameras;
//using VerticesEngine.Settings;
//using VerticesEngine.Scenes;

//namespace VerticesEngine.Graphics
//{

//    /// <summary>
//    /// A simple renderer. Contains methods for rendering standard XNA models, as well as
//    /// instanced rendering (see class InstancedModel), and rendering of selected triangles. 
//    /// The scene light position can be set with the property lightPosition.
//    /// </summary>
//    public partial class vxRendererBaseClass : vxGameObject
//    {
//        /// <summary>
//        /// The scene.
//        /// </summary>
//        public readonly vxGameplaySceneBase Scene;


//        /// <summary>
//        /// The quad renderer vertices buffer.
//        /// </summary>
//        VertexPositionTexture[] quadRendererVerticesBuffer = null;

//        /// <summary>
//        /// The quad renderer index buffer.
//        /// </summary>
//        short[] quadRendererIndexBuffer = null;


//        /// <summary>
//        /// Collection of Post Processes in thie Renderer
//        /// </summary>
//        public List<vxPostProcessorInterface> PostProcessors = new List<vxPostProcessorInterface>();


//        /// <summary>
//        /// The main passes performed during the main render 'Draw' call
//        /// </summary>
//        public List<vxPostProcessorInterface> SceneRenderPasses = new List<vxPostProcessorInterface>();

//        /// <summary>
//        /// The render targets collection.
//        /// </summary>
//        public vxRenderTargetsCollection RenderTargets;

//        /// <summary>
//        /// The post process temp targets collection.
//        /// </summary>
//        public RenderTarget2D[] PostProcessTargets;// = new List<RenderTarget2D>();

//        // Temp Targets Index
//        int tempTargetIndex = 0;

//        /// <summary>
//        /// The number of temp targets used.
//        /// </summary>
//        public int TempTargetsUsed = 0;

//        public int TempTargetCount = 8;



//        /// <summary>
//        /// Gets the random texture2 d.
//        /// </summary>
//        /// <value>The random texture2 d.</value>
//        public Texture2D RandomTexture2D;



//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxRenderer"/> class.
//        /// </summary>
//        /// <param name="Scene">Scene.</param>
//        public vxRendererBaseClass(vxGameplaySceneBase Scene) : base(Scene.Engine)
//        {
//            this.Scene = Scene;
//        }


//        /// <summary>
//        /// Loads the content.
//        /// </summary>
//        public virtual void LoadContent()
//        {
//            var tempTrgts = new List<RenderTarget2D>();

//#if __ANDROID__
//            //var w = Engine.Game.GraphicsDeviceManager.PreferredBackBufferWidth;
//            //var h = Engine.Game.GraphicsDeviceManager.PreferredBackBufferHeight;
//            PresentationParameters pp = GraphicsDevice.PresentationParameters;
//            //Console.WriteLine("CREATING: " + w + "," + h);

//            for (int i = 0; i < TempTargetCount; i++)
//            {
//                tempTrgts.Add(new RenderTarget2D(GraphicsDevice, vxScreen.Width, vxScreen.Height, false,
//                                                      pp.BackBufferFormat, pp.DepthStencilFormat));
//            }

//            //PostProcessTargets[i] = new RenderTarget2D(GraphicsDevice, w, h, false,
//                                                  //pp.BackBufferFormat, pp.DepthStencilFormat);
//#else
//            PresentationParameters pp = GraphicsDevice.PresentationParameters;
//            for (int i = 0; i < TempTargetCount; i++)
//            {
//                tempTrgts.Add(vxGraphics.GetNewRenderTarget());
//            }
//#endif
//            PostProcessTargets = tempTrgts.ToArray();



//            quadRendererVerticesBuffer = new VertexPositionTexture[]
//            {
//                new VertexPositionTexture(
//                    new Vector3(0,0,0),
//                    new Vector2(1,1)),
//                new VertexPositionTexture(
//                    new Vector3(0,0,0),
//                    new Vector2(0,1)),
//                new VertexPositionTexture(
//                    new Vector3(0,0,0),
//                    new Vector2(0,0)),
//                new VertexPositionTexture(
//                    new Vector3(0,0,0),
//                    new Vector2(1,0))
//            };

//            quadRendererIndexBuffer = new short[] { 0, 1, 2, 2, 3, 0 };

//            RandomTexture2D = vxInternalAssets.Textures.RandomValues;

//            RenderTargets = new vxRenderTargetsCollection(this);
//        }

//        public T GetRenderPass<T>() where T : vxPostProcessor
//        {
//            foreach(var renderPass in PostProcessors)
//            {
//                if(renderPass.GetType() == typeof(T))
//                {
//                    return (T)renderPass;
//                }
//            }

//            return default(T);
//        }

//        /// <summary>
//        /// Gets the new temp target.
//        /// </summary>
//        /// <returns>The new temp target.</returns>
//        public RenderTarget2D GetNewTempTarget(string name)
//        {
//            PostProcessTargets[tempTargetIndex + 1].Name = name;
//            return PostProcessTargets[tempTargetIndex + 1];
//        }

//        /// <summary>
//        /// Gets the current temp target. This pushes the stack forward. Use peek if you arent' drawing with this one to the new one.
//        /// </summary>
//        /// <returns>The current temp target.</returns>
//        public RenderTarget2D GetCurrentTempTarget()
//        {
//            tempTargetIndex++;
//            return PostProcessTargets[tempTargetIndex - 1];
//        }

//        /// <summary>
//        /// Peeks at current temp target.
//        /// </summary>
//        /// <returns>The at current temp target.</returns>
//        public RenderTarget2D PeekAtCurrentTempTarget()
//        {
//            return PostProcessTargets[tempTargetIndex];
//        }

//        public override void Dispose()
//        {
//            base.Dispose();

//            foreach (var pp in PostProcessors)
//                pp.Dispose();
//        }

//        public virtual void InitialiseRenderTargetsAll()
//        {

//        }


//        public override void OnGraphicsRefresh()
//        {
//            base.OnGraphicsRefresh();

//            for (int i = 0; i < PostProcessTargets.Length; i++)
//            {
//                // Android needs to handle differently
//#if __ANDROID__

//                PresentationParameters pp = GraphicsDevice.PresentationParameters;
                
//                    PostProcessTargets[i] = new RenderTarget2D(GraphicsDevice, vxScreen.Width, vxScreen.Height, false,
//                                                      pp.BackBufferFormat, pp.DepthStencilFormat);
//#else
//                PresentationParameters pp = GraphicsDevice.PresentationParameters;

//                PostProcessTargets[i] = vxGraphics.GetNewRenderTarget();

//#endif
//                //PostProcessTargets[i] = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
//                //pp.BackBufferFormat, pp.DepthStencilFormat);
//            }

//        }


//        public void Prepare()
//        {
//            tempTargetIndex = 0;
//            TempTargetsUsed = 0;


//            foreach (var pstprcsr in PostProcessors)
//                pstprcsr.Prepare();
//        }


//        /// <summary>
//        /// This is the main draw call where each entity is drawn by the renderer.
//        /// </summary>
//        public virtual void Draw()
//        {
//            foreach (var pstprcsr in SceneRenderPasses)
//                pstprcsr.Apply();
//        }

//        /// <summary>
//        /// Applies the Post Processors
//        /// </summary>
//        public virtual void ApplyPostProcessors()
//        {
//            //PostProcessTargets[0] = RenderTargets.MainSceneResult;
//            //PostProcessTargets[0].Name = "Main Scene Pass";

//            foreach (var pstprcsr in PostProcessors)
//                pstprcsr.Apply();
//        }

//        public virtual RenderTarget2D Finalise()
//        {   
//            TempTargetsUsed = tempTargetIndex+1;
//            vxConsole.WriteInGameDebug(this, "Temp Targets Used: " + TempTargetsUsed);
//            return PostProcessTargets[tempTargetIndex];
//        }

//#region Drawing Rendertarget Code

//        /// <summary>
//        /// Helper for drawing a texture into a rendertarget, using
//        /// a custom shader to apply postprocessing effects.
//        /// </summary>
//        public void DrawRenderTargetIntoOther(string tag, Texture2D texture, RenderTarget2D renderTarget, Effect effect)
//        {
//            GraphicsDevice.SetRenderTarget(renderTarget);

//            DrawFullscreenQuad(tag, texture,
//                               renderTarget.Width, renderTarget.Height,
//                               effect);
//        }


//        /// <summary>
//        /// Helper for drawing a texture into the current rendertarget,
//        /// using a custom shader to apply postprocessing effects.
//        /// </summary>
//        /// <param name="texture"></param>
//        /// <param name="width"></param>
//        /// <param name="height"></param>
//        /// <param name="effect"></param>
//        public void DrawFullscreenQuad(string tag, Texture2D texture, int width, int height,
//                                Effect effect)
//        {
//            Engine.SpriteBatch.Begin(tag, 0, BlendState.Opaque, null, null, null, effect);
//            Engine.SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
//            Engine.SpriteBatch.End();
//        }



//        /// <summary>
//        /// Draws With No Effect Used
//        /// </summary>
//        /// <param name="texture"></param>
//        /// <param name="width"></param>
//        /// <param name="height"></param>
//        public void DrawFullscreenQuad(string tag, Texture2D texture, int width, int height)
//        {
//            Engine.SpriteBatch.Begin(tag);
//            Engine.SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
//            Engine.SpriteBatch.End();
//        }



//        /// <summary>
//        /// Render the specified v1 and v2.
//        /// </summary>
//        /// <param name="v1">V1.</param>
//        /// <param name="v2">V2.</param>
//        public void RenderQuad(Vector2 v1, Vector2 v2)
//        {
//            quadRendererVerticesBuffer[0].Position.X = v2.X;
//            quadRendererVerticesBuffer[0].Position.Y = v1.Y;

//            quadRendererVerticesBuffer[1].Position.X = v1.X;
//            quadRendererVerticesBuffer[1].Position.Y = v1.Y;

//            quadRendererVerticesBuffer[2].Position.X = v1.X;
//            quadRendererVerticesBuffer[2].Position.Y = v2.Y;

//            quadRendererVerticesBuffer[3].Position.X = v2.X;
//            quadRendererVerticesBuffer[3].Position.Y = v2.Y;

//            Engine.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
//            (PrimitiveType.TriangleList, quadRendererVerticesBuffer, 0, 4, quadRendererIndexBuffer, 0, 2);
//        }

//#endregion
//    }
//}