//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using VerticesEngine;
//using VerticesEngine.Utilities;

//namespace VerticesEngine.Graphics
//{
//    public class vxRenderTargetsCollection : vxGameObject
//    {
//        /// <summary>
//		/// The Main Scene Render Target. This holds all of the Scene which all of the Post Processes are eventually applied to.
//		/// </summary>
//		public RenderTarget2D MainSceneResult;


//        public RenderTarget2D EncodedIndexResult;

//        /// <summary>
//        /// This render target holds mask information for different entities (i.e. do edge detection? do motion blur, etc...)
//        /// </summary>
//        public RenderTarget2D EntityMaskValues;

//        // Defferred Rendering
//        // **********************************************

//        /// <summary>
//        ///Render Target which holds Surface Data such as Specular Power, Intensity as well as Shadow Factor.
//        /// </summary>
//        public RenderTarget2D SurfaceDataMap;

//        /// <summary>
//        /// Normal Render Target.
//        /// </summary>
//        public RenderTarget2D NormalMap;

//        /// <summary>
//        /// Depth Render Target.
//        /// </summary>
//        public RenderTarget2D DepthMap;

//        public RenderTarget2D AuxDepthMap;

//        public RenderTarget2D CubeReflcMap;

//        /// <summary>
//        /// Light Map Render Target.
//        /// </summary>
//        public RenderTarget2D LightMap;





//        // Blur Render Targets
//        // **********************************************

//        /// <summary>
//        /// Utility Blurred Scene for use in Post Processing (i.e. Depth of Field, Menu Back ground blurring etc...).
//        /// </summary>
//        public RenderTarget2D BlurredSceneResult;




//        // Crepuscular Rays Code
//        // **********************************************

//        /// <summary>
//        /// Render target which holds the mask map for Crepuscular Rays.
//        /// </summary>
//        public RenderTarget2D GodRaysMaskMap;




//        // Distortion Render Targets
//        // **********************************************

//        /// <summary>
//        /// The distorted scene Render Target.
//        /// </summary>
//        //public RenderTarget2D RT_DistortedScene;

//        /// <summary>
//        /// The distortion map Render Target.
//        /// </summary>
//        public RenderTarget2D DistortionMap;



//        // Screen Space Render Targets
//        // **********************************************





//        // Shadow Render Targets
//        // **********************************************


//        /// <summary>
//        /// Gets the renderer.
//        /// </summary>
//        /// <value>The renderer.</value>
//        //public vxRendererBaseClass Renderer;



//        //public int ShadowMapSize
//        //{
//        //    get { return Renderer.ShadowMapSize; }
//        //    set { Renderer.ShadowMapSize = value; }
//        //}


//        public int ShadowMapSize
//        {
//            get { return _shadowMapSize; }
//            set { _shadowMapSize= value; }
//        }
//        int _shadowMapSize = 1024;

//        public float size = 256;



//        public vxRenderTargetsCollection(vxRenderer Render) : base(Render.Engine)
//        {
//            //this.Renderer = Render;
//        }


//        public override void Dispose()
//        {
//            base.Dispose();

//            MainSceneResult.Dispose();
//        }


//        public Color GetEncodedIndex(int x, int y)
//        {
//            var i = (y * EncodedIndexResult.Width) + x;
//            EncodedIndexResult.GetData<Color>(EncodedIndexPixels);

//            return EncodedIndexPixels[i];
//        }

//        Color[] EncodedIndexPixels;// = new Color[rt.Width * rt.Height];

//        public void InitialiseRenderTargetsAll()
//        {
//            // Create two custom rendertargets.
//            PresentationParameters pp = GraphicsDevice.PresentationParameters;

//            InitialiseRenderTargetsForMain(pp);
//            InitialiseRenderTargetsForBlur(pp);
//            //InitialiseRenderTargetsForDistortion(pp);

//            EncodedIndexPixels = new Color[EncodedIndexResult.Width * EncodedIndexResult.Height];

//            //mAlphaBlendState = new BlendState();
//            //mAlphaBlendState.ColorSourceBlend = Blend.SourceAlpha;
//            //mAlphaBlendState.AlphaSourceBlend = Blend.SourceAlpha;
//            //mAlphaBlendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
//            //mAlphaBlendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;

//            InitialiseRenderTargetsForShadowMaps();
//        }

//        public void InitialiseRenderTargetsForMain(PresentationParameters pp)
//        {
//            MainSceneResult = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                               false, pp.BackBufferFormat, pp.DepthStencilFormat);

//            EncodedIndexResult = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                               false, pp.BackBufferFormat, pp.DepthStencilFormat);

//            EntityMaskValues = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                               false, pp.BackBufferFormat, pp.DepthStencilFormat);

//            SurfaceDataMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                               false, pp.BackBufferFormat, pp.DepthStencilFormat);

//            NormalMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                           false, SurfaceFormat.Color, DepthFormat.None);

//            #if __ANDROID__
//            DepthMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                          false, SurfaceFormat.Color, DepthFormat.None);

//            AuxDepthMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                             false, SurfaceFormat.Color, DepthFormat.None);

//#else
//            DepthMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, 
//                                          false,SurfaceFormat.Single, DepthFormat.None);

//            AuxDepthMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                          false, SurfaceFormat.Single, DepthFormat.None);
//#endif
//            CubeReflcMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                               false, pp.BackBufferFormat, pp.DepthStencilFormat);

//            DistortionMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                               false, pp.BackBufferFormat, pp.DepthStencilFormat);


//            LightMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
//                                               false, pp.BackBufferFormat, pp.DepthStencilFormat);




//            GodRaysMaskMap = new RenderTarget2D(GraphicsDevice,
//                                                   pp.BackBufferWidth / 2, pp.BackBufferHeight / 2, false,
//                                                   pp.BackBufferFormat, pp.DepthStencilFormat);


//            //Then Set the info for the Post Processors
//            //foreach (vxPostProcessor pstprcsr in Renderer.postprocessors)
//            //    pstprcsr.ongraphicsrefresh();
//        }



//        public void InitialiseRenderTargetsForBlur(PresentationParameters pp)
//        {
//            //Blur Render Targets
//            int width = pp.BackBufferWidth;
//            int height = pp.BackBufferHeight;

//            width /= (5 - (int)Engine.Settings.Graphics.Blur.Quality);
//            height /= (5 - (int)Engine.Settings.Graphics.Blur.Quality);

//            // Create a texture for rendering the main scene, prior to applying bloom.
//            BlurredSceneResult = new RenderTarget2D(GraphicsDevice, width / 2, height / 2, false,
//                                                   pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount,
//                                                   RenderTargetUsage.DiscardContents);
//        }


//        public void InitialiseRenderTargetsForShadowMaps()
//        {
           
//		}

//	}
//}
