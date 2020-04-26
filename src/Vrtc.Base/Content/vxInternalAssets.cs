
using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.ContentManagement;
using VerticesEngine.Graphics;
using VerticesEngine.Utilities;

namespace VerticesEngine
{
    /// <summary>
    /// Class for holding all Internal Engine Assets
    /// </summary>
    public static class vxInternalAssets
    {
        static vxEngine Engine;

        public static class Fonts
        {
            public static SpriteFont MenuTitleFont;
            public static SpriteFont MenuFont;
            public static SpriteFont BaseFont { get; internal set; }
            public static SpriteFont DebugFont;
            public static SpriteFont ViewerFont;
        };

        public static class Textures
        {
            public static Texture2D Blank;
            public static Texture2D Gradient;
            public static Texture2D RandomValues;
            public static Texture2D Arrow_Left, Arrow_Right;

            public static Texture2D Texture_WaterEnvr;
            public static Texture2D Texture_WaterWaves;
            public static Texture2D Texture_WaterDistort;

            public static Texture2D Texture_Cube_Null;

            public static Texture2D DefaultDiffuse;
            public static Texture2D DefaultNormalMap;
            public static Texture2D DefaultSurfaceMap;

            public static Texture2D Texture_Sun_Glow;

            public static Texture2D Texture_ArrowDown;
            public static Texture2D Texture_ArrowRight;

            public static Texture2D TreeMesh;
            public static Texture2D TreeModel, TreeRoot;

        };


        public static class SoundEffects
        {
            public static SoundEffect MenuClick;
            public static SoundEffect MenuConfirm;
            public static SoundEffect MenuError;
        };

        /// <summary>
        /// Internal Engine Sound Effects.
        /// </summary>
        //public AssetSoundEffects SoundEffects = new AssetSoundEffects();



        public static class Models
        {
            public static vxModel UnitArrow;
            public static vxModel UnitTorus;
            public static vxModel UnitBox;
            public static vxModel UnitCylinder;
            public static vxModel UnitSphere;
            public static vxModel UnitPlane;
            public static vxModel UnitPlanePan;

            /// <summary>
            ///  The Sphere used for Point Lights. Note this is a simple Model, not a vxModel.
            /// </summary>
            public static Model PointLightSphere;
            public static vxModel WaterPlane;

            //public vxModel Sun_Mask;

            // The Model Viewer Plane
            public static vxModel ViewerPlane;
        };

        /// <summary>
        /// The models.
        /// </summary>
        //public AssetModels Models = new AssetModels();




        public static class Shaders
        {
            public static Effect MainShader;
            public static Effect CascadeShadowShader;
            public static Effect PrepPassShader;
            public static Effect DebugShader;
            public static Effect IndexEncodedColourShader;
            public static Effect OutlineShader;
            public static Effect CartoonShader;
            //public Effect DistortionShader;
            public static Effect WaterReflectionShader;
            public static Effect HeightMapTerrainShader;
        };

        /// <summary>
        /// Model Shaders.
        /// </summary>
        //public AssetShaders Shaders = new AssetShaders();







        public static class PostProcessShaders
        {

            //Deffered Rendering
            public static Effect DrfrdRndrClearGBuffer;
            public static Effect DrfrdRndrCombineFinal;
            public static Effect LightingCombine;
            public static Effect DrfrdRndrDirectionalLight;
            public static Effect DrfrdRndrPointLight;

            //public Effect CartoonEdgeDetection;
            public static Effect BloomExtractEffect;
            public static Effect BloomCombineEffect;
            public static Effect GaussianBlurEffect;

            public static Effect SceneBlurEffect;


            public static Effect CartoonEdgeDetect;

            /// <summary>
            /// An Effect Which applies Post Processes once the scene has been lit in the
            /// defferred renderer. 
            /// The Post Processes are SSR and God Rays.
            /// </summary>
            public static Effect ScreenSpaceCombine;

            /// <summary>
            /// This Effect Draws the Lights to be masked for the Post Lighting Effect
            /// </summary>
            public static Effect MaskedSunEffect;

            public static Effect SSAOEffect;

            /// <summary>
            /// This Post Process preforms a Screen Space Reflection on the Scene given the Depth and Normal Maps.
            /// It creates a UV Map of the location of the reflected pixels.
            /// </summary>
            public static Effect ScreenSpaceReflectionEffect;

            public static Effect DistortSceneEffect;

            public static Effect CameraMotionBlurEffect;

            public static Effect FXAA;
        };

		/// <summary>
		/// Loads the sprite font.
		/// </summary>
		/// <returns>The sprite font.</returns>
		/// <param name="path">Path.</param>
		public static SpriteFont LoadInternalSpriteFont(string path)
		{
            SpriteFont font = vxContentManager.Instance.Load<SpriteFont>(path);
			return font;
		}

		/// <summary>
		/// Loads a texture2D.
		/// </summary>
		/// <returns>The texture2 d.</returns>
		/// <param name="path">Path.</param>
		public static Texture2D LoadInternalTexture2D(string path)
		{
            Texture2D texture = vxContentManager.Instance.Load<Texture2D>(path);
            return texture;
		}

		/// <summary>
		/// Loads the sound effect.
		/// </summary>
		/// <returns>The sound effect.</returns>
		/// <param name="path">Path.</param>
        public static SoundEffect LoadInternalSoundEffect(string path)
		{
			return vxContentManager.Instance.Load<SoundEffect>(path);
		}

        public static Effect LoadInternalEffect(string path)
		{
			return vxContentManager.Instance.Load<Effect>(path);
		}

		/// <summary>
		/// Gets the path for the internal assets and content for the engine.
		/// </summary>
		/// <value>The content of the path to engine.</value>
        internal static string PathToEngineContent
		{
			get { return "EngineContent"; }
		}

        /// <summary>
        /// Loads an Internal Asset, NOTE: this is the same as calling 'Engine.ContentManager.Load<T>(...)'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static T LoadInternal<T>(string path)
        {
            try
            {
                return vxContentManager.Instance.Load<T>(path);
            }
            catch(Exception ex)
            {
                vxConsole.WriteLine("COULDN'T LOAD FILE: " + path);
                vxConsole.WriteException("", ex);
               
                return vxContentManager.Instance.Load<T>("Shaders/Lighting/ClearGBuffer");
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.ContentManagement.vxInternalAssetManager"/> class.
		/// </summary>
		/// <param name="engine">Engine.</param>
		internal static void Init()
		{
			Engine = vxEngine.Instance;

            /********************************************************************************************/
            /*										Fonts												*/
            /********************************************************************************************/
            Fonts.MenuTitleFont = LoadInternal<SpriteFont>("Fonts/font_gui_title");
            Fonts.MenuFont = LoadInternal<SpriteFont>("Fonts/font_gui");
            Fonts.BaseFont = LoadInternal<SpriteFont>("Fonts/font_gui");
            Fonts.DebugFont = LoadInternal<SpriteFont>("Fonts/font_debug");
            Fonts.ViewerFont = LoadInternal<SpriteFont>("Fonts/font_viewer");


            /********************************************************************************************/
            /*										Textures											*/
            /********************************************************************************************/

            Textures.Blank = LoadInternal<Texture2D>("Textures/Defaults/blank");
            Textures.Gradient = LoadInternal<Texture2D>("Textures/Defaults/gradient");
            Textures.RandomValues = LoadInternal<Texture2D>("Textures/Defaults/random");

            Textures.DefaultDiffuse = LoadInternal<Texture2D>("Textures/Defaults/model_diffuse");
            Textures.DefaultNormalMap = LoadInternal<Texture2D>("Textures/Defaults/model_normalmap");
            Textures.DefaultSurfaceMap = LoadInternal<Texture2D>("Textures/Defaults/model_surfacemap");


            // Handle GUI Items
            Textures.Arrow_Left = LoadInternalTexture2D("Textures/gui/Slider/Arrow_Left");
            Textures.Arrow_Right = LoadInternalTexture2D("Textures/gui/Slider/Arrow_Right");

            Textures.Texture_ArrowDown = LoadInternalTexture2D("Textures/gui/icons/arrow_down");
            Textures.Texture_ArrowRight = LoadInternalTexture2D("Textures/gui/icons/arrow_right");

            Textures.Texture_Sun_Glow = LoadInternalTexture2D("Textures/godrays/rays0");





            /********************************************************************************************/
            /*										Sound Effects  										*/
            /********************************************************************************************/
            SoundEffects.MenuClick = LoadInternalSoundEffect("SndFx/Menu/click");
            SoundEffects.MenuConfirm = LoadInternalSoundEffect("SndFx/Menu/confirm");
            SoundEffects.MenuError = LoadInternalSoundEffect("SndFx/Menu/error");



            /********************************************************************************************/
            /*										Shaders												*/
            /********************************************************************************************/

            // Certain shaders need modifications for MonoGame, so there is an alternate version of them

            //Shader Collection
            Shaders.MainShader = LoadInternalEffect("Shaders/ModelShaders/MainModelShader");
            Shaders.CartoonShader = LoadInternalEffect("Shaders/ModelShaders/CellModelShader");
            Shaders.CascadeShadowShader = LoadInternalEffect("Shaders/Shadows/CascadeShadowShader");
            Shaders.PrepPassShader = LoadInternalEffect("Shaders/Utility/PrepPassShader");
            Shaders.DebugShader = LoadInternalEffect("Shaders/Utility/DebugShader");
            Shaders.IndexEncodedColourShader = LoadInternalEffect("Shaders/Utility/IndexEncodedColourShader");
            Shaders.OutlineShader = LoadInternalEffect("Shaders/Utility/OutlineShader");
            //vxRenderer3D.ShadowEffect = new vxShadowEffect(Shaders.CascadeShadowShader);

            //Water Shader
            Shaders.WaterReflectionShader = LoadInternalEffect("Shaders/Water/WaterShader");

            // Height Map Terrain Shader
            Shaders.HeightMapTerrainShader = LoadInternalEffect("Shaders/Terrain/TerrainHeightMap");


            //Bloom
            PostProcessShaders.BloomExtractEffect = LoadInternalEffect("Shaders/Bloom/BloomExtract");
            PostProcessShaders.BloomCombineEffect = LoadInternalEffect("Shaders/Bloom/BloomCombine");
            PostProcessShaders.GaussianBlurEffect = LoadInternalEffect("Shaders/Bloom/GaussianBlur");

            PostProcessShaders.CartoonEdgeDetect = LoadInternalEffect("Shaders/EdgeDetection/CartoonEdgeDetection");

            //Distortion Shaders
            PostProcessShaders.DistortSceneEffect = LoadInternalEffect("Shaders/Distorter/DistortScene");
            PostProcessShaders.CameraMotionBlurEffect = LoadInternalEffect("Shaders/Blur/CameraMotionBlur");

            PostProcessShaders.SceneBlurEffect = LoadInternalEffect("Shaders/Blur/SceneBlur");

            // Anti Aliasing Shaders
            PostProcessShaders.FXAA = LoadInternalEffect("Shaders/AntiAliasing/FXAA/FXAA");


            //Defferred Shading
            PostProcessShaders.DrfrdRndrClearGBuffer = LoadInternalEffect("Shaders/Lighting/ClearGBuffer");
            PostProcessShaders.DrfrdRndrCombineFinal = LoadInternalEffect("Shaders/Lighting/CombineFinal");
            PostProcessShaders.LightingCombine = LoadInternalEffect("Shaders/Lighting/LightingCombinePass");
            PostProcessShaders.DrfrdRndrDirectionalLight = LoadInternalEffect("Shaders/Lighting/DirectionalLight");
            PostProcessShaders.DrfrdRndrPointLight = LoadInternalEffect("Shaders/Lighting/PointLight");
            PostProcessShaders.MaskedSunEffect = LoadInternalEffect("Shaders/Lighting/MaskedSun");

            // Screen Space Reflections
            PostProcessShaders.SSAOEffect = LoadInternalEffect("Shaders/ScreenSpace/SSAO");
            PostProcessShaders.ScreenSpaceReflectionEffect = LoadInternalEffect("Shaders/ScreenSpace/SSR");
            PostProcessShaders.ScreenSpaceCombine = LoadInternalEffect("Shaders/ScreenSpace/ScreenSpaceCombine");

            /********************************************************************************************/
            /*										Models												*/
            /********************************************************************************************/
            //Unit Models
            Models.UnitArrow = LoadInternalModel("Models/utils/unit_arrow/unit_arrow");
            Models.UnitTorus = LoadInternalModel("Models/utils/unit_torus/unit_torus");
            Models.UnitPlanePan = LoadInternalModel("Models/utils/unit_plane/unit_plane_pan");

			Models.UnitBox = LoadInternalModel("Models/utils/unit_box/unit_box");
            Models.UnitCylinder = LoadInternalModel("Models/utils/unit_cylinder/unit_cylinder");
			Models.UnitPlane = LoadInternalModel("Models/utils/unit_plane/unit_plane");
			Models.UnitSphere = LoadInternalModel("Models/utils/unit_sphere/unit_sphere");
			Models.WaterPlane = LoadInternalModel("Models/water/water_plane", Shaders.WaterReflectionShader);
			Models.ViewerPlane = LoadInternalModel("Models/viewer/plane");
			
            Models.PointLightSphere = vxContentManager.Instance.Load<Model>("Models/lghtng/sphere");
        }
        public static vxModel LoadInternalModelWithBasicEffect(string path)
        {
            return vxContentManager.Instance.LoadModelWithBasicEffect(path, vxContentManager.Instance);
        }


		public static vxModel LoadInternalModel(string path)
		{
			return LoadInternalModel(path, Shaders.MainShader);
		}



        public static vxModel LoadInternalModel(string path, Effect MainEffect)
		{
			return vxContentManager.Instance.LoadModel(path, vxContentManager.Instance);
		}
	}
}
