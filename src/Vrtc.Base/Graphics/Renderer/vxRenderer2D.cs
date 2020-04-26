
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
//    public partial class vxRenderer2D : vxRendererBaseClass
//    {
//        /// <summary>
//        /// The scene.
//        /// </summary>
//        public new vxGameplayScene2D Scene
//        {
//            get { return (vxGameplayScene2D)base.Scene; }
//        }

//        /// <summary>
//        /// The distortion post process.
//        /// </summary>
//        public vxDistortionPostProcess2D DistortionPostProcess;

        

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxRenderer"/> class.
//        /// </summary>
//        /// <param name="Scene">Scene.</param>
//        public vxRenderer2D(vxGameplayScene2D Scene) : this(Scene, new vxRendererConfig())
//        {
            
//        }

//        vxRendererConfig StartupConfig;

//        /// <summary>        
//        /// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxRenderer"/> class.
//        /// </summary>
//        /// <param name="Scene">The scene which this renderer will be rendering</param>
//        /// <param name="config">Startup Configuration for this renderer</param>
//        public vxRenderer2D(vxGameplayScene2D Scene, vxRendererConfig config) : base(Scene)
//        {
//            StartupConfig = config;
//        }

//        /// <summary>
//        /// Loads the content.
//        /// </summary>
//        public override void LoadContent()
//        {
//            base.LoadContent();

//            // Now distort any reflections and the main lit scene
//            DistortionPostProcess = new vxDistortionPostProcess2D(this);

//#if !__MOBILE__
//           PostProcessors.Add(DistortionPostProcess);
//#endif

//            RandomTexture2D = vxInternalAssets.Textures.RandomValues;

//            foreach (var pstprcs in PostProcessors)
//                pstprcs.LoadContent(StartupConfig);


//            InitialiseRenderTargetsAll();

//        }

//        public override void InitialiseRenderTargetsAll()
//        {
//            OnGraphicsRefresh();
//        }

//        public override void OnGraphicsRefresh()
//        {
//            base.OnGraphicsRefresh();

//            foreach(var postprocessor in PostProcessors)
//                postprocessor.OnGraphicsRefresh();
//        }


//        /// <summary>
//        /// Applies the Post Processors
//        /// </summary>
//        public override void ApplyPostProcessors()
//        {
//            PostProcessTargets[0] = Scene.MainSceneResult;;
//            PostProcessTargets[0].Name = "Main Scene Pass";

//            foreach (var pstprcsr in PostProcessors)
//                pstprcsr.Apply();

//        }
//    }
//}