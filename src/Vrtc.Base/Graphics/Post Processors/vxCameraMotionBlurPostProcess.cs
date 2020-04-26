
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using VerticesEngine;
//using VerticesEngine.Settings;
//using VerticesEngine;

//namespace VerticesEngine.Graphics
//{
//    public class vxMotionBlurGraphicsConfig : vxGraphicsConfig
//    {
//        public float MotionBlurFactor = 2;
//    }
//    /// <summary>
//    /// Grab a scene that has already been rendered, 
//    /// and add a distortion effect over the top of it.
//    /// </summary>
//    public class vxCameraMotionBlurPostProcess : vxPostProcessor3D
//    {
//        /// <summary>
//        /// The Scene Texture.
//        /// </summary>
//        public RenderTarget2D SceneTexture
//        {
//            set { Parameters["SceneTexture"].SetValue(value); }
//        }


//        /// <summary>
//        /// Depth map of the scene.
//        /// </summary>
//        public RenderTarget2D DepthMap
//        {
//            set { SetEffectParameter("DepthMap", value); }
//        }

//        /// <summary>
//        /// Sets the view projection.
//        /// </summary>
//        /// <value>The view projection.</value>
//        public Matrix PreviousViewProjection
//        {
//            set { SetEffectParameter("PrevViewProjection", value); }
//        }

//        /// <summary>
//        /// Intensity of the edge dection.
//        /// </summary>
//        public Matrix InverseViewProjection
//        {
//            set { SetEffectParameter("InverseViewProjection", value); }
//        }

//		public float MotionBlurFactor
//		{
//			set { SetEffectParameter("BlurLength", value); }
//		}

//        public Vector3[] RAND_SAMPLES
//        {
//            set
//            {
//                if (value != null)
//                    if (Parameters["RAND_SAMPLES"] != null)
//                        Parameters["RAND_SAMPLES"].SetValue(value);
//            }
//        }

//        public vxCameraMotionBlurPostProcess(vxRenderer3D Renderer) :
//        base(Renderer, "Camera Motion Blur", vxInternalAssets.PostProcessShaders.CameraMotionBlurEffect)
//        {

//        }

//        public override void LoadContent(vxRendererConfig config)
//        {
//            base.LoadContent(config);
//            //BlurLength = 2;

//            RAND_SAMPLES = new Vector3[]
//            {
//                      new Vector3( 0.5381f, 0.1856f,-0.4319f),
//      new Vector3( 0.1379f, 0.2486f, 0.4430f),
//      new Vector3( 0.3371f, 0.5679f,-0.0057f),
//      new Vector3(-0.6999f,-0.0451f,-0.0019f),
//      new Vector3( 0.0689f,-0.1598f,-0.8547f),
//      new Vector3( 0.0560f, 0.0069f,-0.1843f),
//      new Vector3(-0.0146f, 0.1402f, 0.0762f),
//      new Vector3( 0.0100f,-0.1924f,-0.0344f),
//      new Vector3(-0.3577f,-0.5301f,-0.4358f),
//      new Vector3(-0.3169f, 0.1063f, 0.0158f),
//      new Vector3( 0.0103f,-0.5869f, 0.0046f),
//      new Vector3(-0.0897f,-0.4940f, 0.3287f),
//      new Vector3( 0.7119f,-0.0154f,-0.0918f),
//      new Vector3(-0.0533f, 0.0596f,-0.5411f),
//      new Vector3( 0.0352f,-0.0631f, 0.5460f),
//      new Vector3(-0.4776f, 0.2847f,-0.0271f)
//        };
//            }

//        public override void OnGraphicsRefresh()
//        {
//            base.OnGraphicsRefresh();
//            vxInternalAssets.PostProcessShaders.CameraMotionBlurEffect.Parameters["MatrixTransform"].SetValue(MatrixTransform);
//        }


//        public override void Apply()
//        {
//            //CameraPosition = Scene.Camera.Position;
//            if (Settings.Graphics.MotionBlur.IsEnabled == true && 
//               Scene.Camera.ViewProjection != Scene.Camera.PreviousViewProjection)
//            {
//                Engine.GraphicsDevice.SetRenderTarget(Renderer.GetNewTempTarget("Motion Blur"));
                
//                DepthMap = RenderTargets.DepthMap;
//                Engine.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

//                PreviousViewProjection = Scene.Camera.PreviousViewProjection;

//                InverseViewProjection = Matrix.Invert(Scene.Camera.ViewProjection);

//                //Viewport viewport = Engine.GraphicsDevice.Viewport;

//                Effect.CurrentTechnique = Effect.Techniques["Technique_CameraMotionBlur"];
//                Parameters["MaskTexture"].SetValue(RenderTargets.EntityMaskValues);

//                Scene.Renderer.DrawFullscreenQuad("PostProcess.CameraMotionBlur.Apply()", Renderer.GetCurrentTempTarget(),
//                    Viewport.Width, Viewport.Height, Effect);
//            }
//        }
//    }
//}
