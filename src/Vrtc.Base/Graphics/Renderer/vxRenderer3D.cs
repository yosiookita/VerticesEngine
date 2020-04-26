
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
//    public class vxGraphicsConfig
//    {
//        public vxGraphicsConfig() { }

//        public ConsoleColor ConfigConsoleColour = ConsoleColor.DarkCyan;
//        public virtual void VerboseOutput(int depth) { }
//    }
//    /// <summary>
//    /// A Configuration Class to be passed to a vxRenderer Class
//    /// </summary>
//    public class vxRendererConfig : vxGraphicsConfig
//    {
//        public vxBloomGraphicsConfig Bloom = new vxBloomGraphicsConfig();
//        public vxBlurUtilityGraphicsConfig Blur = new vxBlurUtilityGraphicsConfig();
//        public vxDepthofFieldGraphicsConfig DepthOfField = new vxDepthofFieldGraphicsConfig();
//        public vxFogGraphicsConfig Fog = new vxFogGraphicsConfig();
//        public vxEdgeDetectGraphicsConfig EdgeDetect = new vxEdgeDetectGraphicsConfig();
//        public vxGodRaysGraphicsConfig GodRays = new vxGodRaysGraphicsConfig();
//        public vxSSAOGraphicsConfig SSAO = new vxSSAOGraphicsConfig();
//        public vxSSRGraphicsConfig SSR = new vxSSRGraphicsConfig();
//        public vxAAGraphicsConfig AA = new vxAAGraphicsConfig();

//        public vxRendererConfig() { }

//        public override void VerboseOutput(int depth)
//        {
//            vxConsole.WriteLine("Starting 3D Rendering Engine With Following Settings", ConfigConsoleColour);
//            Bloom.VerboseOutput(depth + 1);
//            DepthOfField.VerboseOutput(depth + 1);
//            Fog.VerboseOutput(depth + 1);
//            EdgeDetect.VerboseOutput(depth + 1);
//            GodRays.VerboseOutput(depth + 1);
//        }
//    }
//    /// <summary>
//    /// A simple renderer. Contains methods for rendering standard XNA models, as well as
//    /// instanced rendering (see class InstancedModel), and rendering of selected triangles. 
//    /// The scene light position can be set with the property lightPosition.
//    /// </summary>
//    public partial class vxRenderer3D : vxRendererBaseClass
//    {
//        /// <summary>
//        /// The scene.
//        /// </summary>
//        public new vxGameplayScene3D Scene
//        {
//            get { return (vxGameplayScene3D)base.Scene; }
//        }

//        /// <summary>
//        /// Gets the light position of the scene.
//        /// </summary>
//        /// <value>The light position.</value>
//        public Vector3 LightPosition;



//        /// <summary>
//        /// The SSLR Post process.
//        /// </summary>
//        public vxSSRPostProcess SSRPostProcess;

//        public vxSSAOPostProcess SSAOPostProcess;

//        /// <summary>
//        /// The screen space post processes.
//        /// </summary>
//        public vxScreenSpacePostProcess ScreenSpacePostProcesses;

//        /// <summary>
//        /// The defferred render post process.
//        /// </summary>
//        public vxDefferredRenderPostProcess DefferredRenderPostProcess;


//        /// <summary>
//        /// The distortion post process.
//        /// </summary>
//        public vxDistortionPostProcess DistortionPostProcess;

//        /// <summary>
//        /// The gaussian blur scene post process.
//        /// </summary>
//        public vxBlurScenePostProcess BlurScenePostProcess;


//        /// <summary>
//        /// The final scene post process.
//        /// </summary>
//        public vxFinalScenePostProcess FinalScenePostProcess;

//        /// <summary>
//        /// The FXAA Post process.
//        /// </summary>
//        public vxAntiAliasPostProcess FXAAPostProcess;

//        /// <summary>
//        /// The camera motion blur post process.
//        /// </summary>
//        public vxCameraMotionBlurPostProcess CameraMotionBlurPostProcess;


//        /// <summary>
//        /// The render targets collection.
//        /// </summary>
//        //public vxRenderTargetsCollection RenderTargets;
        

//        public Color FogColour
//        {
//            get { return DefferredRenderPostProcess.FogColor; }
//            set { DefferredRenderPostProcess.FogColor = value; }
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxRenderer"/> class.
//        /// </summary>
//        /// <param name="Scene">Scene.</param>
//        public vxRenderer3D(vxGameplayScene3D Scene) : this(Scene, new vxRendererConfig())
//        {
            
//        }

//        vxRendererConfig StartupConfig;

//        /// <summary>        
//        /// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxRenderer"/> class.
//        /// </summary>
//        /// <param name="Scene">The scene which this renderer will be rendering</param>
//        /// <param name="config">Startup Configuration for this renderer</param>
//        public vxRenderer3D(vxGameplayScene3D Scene, vxRendererConfig config) : base(Scene)
//        {
//            StartupConfig = config;
//        }

//        /// <summary>
//        /// Loads the content.
//        /// </summary>
//        public override void LoadContent()
//        {
//            base.LoadContent();


//            InitialiseRenderTargetsAll();

//        }

//        public override void InitialiseRenderTargetsAll()
//        {
//            // Create two custom rendertargets.
//            //PresentationParameters pp = GraphicsDevice.PresentationParameters;
//            RenderTargets.InitialiseRenderTargetsAll();
//        }

//        public override void OnGraphicsRefresh()
//        {
//            foreach(var postprocessor in PostProcessors)
//                postprocessor.OnGraphicsRefresh();
//        }


//        /// <summary>
//        /// Applies the Post Processors
//        /// </summary>
//        public override void ApplyPostProcessors()
//        {
//            PostProcessTargets[0] = RenderTargets.MainSceneResult;
//            PostProcessTargets[0].Name = "Main Scene Pass";

//            foreach (var pstprcsr in PostProcessors)
//                pstprcsr.Apply();
//        }
//    }
//}