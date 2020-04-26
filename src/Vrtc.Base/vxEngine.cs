#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VerticesEngine.Audio;
using VerticesEngine.ContentManagement;
using VerticesEngine.Diagnostics;
using VerticesEngine.Graphics;
using VerticesEngine.Input;
using VerticesEngine.Localization;
using VerticesEngine.Media;
using VerticesEngine.Net;
using VerticesEngine.Plugins;
using VerticesEngine.Profile;
using VerticesEngine;
using VerticesEngine.UI;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Themes;
using VerticesEngine.Utilities;


//Android Libraries
#if __ANDROID__
using Android.Views;
using Android.Content;
#endif

#endregion

namespace VerticesEngine
{
    /// <summary>
    /// The vxEngine is a component which manages one or more vxGameBaseScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public sealed partial class vxEngine : IDisposable
    {
        public static vxEngine Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new vxEngine();
                }

                return _instance;
            }
        }

        private static vxEngine _instance;

        #region -- Public Fields --



        #endregion

        #region Fields

        /// <summary>
        /// The Game reference. Note: this hides but also inherites the original Component game reference.
        /// </summary>
        public vxGame Game
        {
            get { return _game; }
        }
        private vxGame _game;

        /// <summary>
        /// Gets or sets the name of the game, this needs to be set by each program which uses this library.
        /// </summary>
        /// <value>The name of the game.</value>
        public string GameName        
        {
            get { return _gameName; }
}
        private string _gameName = "default_gamename";

        public vxContentManager ContentManager
        {
            get { return vxContentManager.Instance; }
        }

/// <summary>
/// A default SpriteBatch shared by all the screens. This saves
/// each screen having to bother creating their own local instance.
/// </summary>
public vxSpriteBatch SpriteBatch;

        /// <summary>
        /// Whether or not the engine has been initliased or not.
        /// </summary>
        internal bool IsEngineInitialised = false;

        /// <summary>
        /// The global primitive count.
        /// </summary>
        public int GlobalPrimitiveCount = 0;

        /// <summary>
        /// The loading text.
        /// </summary>
        public static string LoadingText = "";


        /// <summary>
        /// Line Batch Manager which draw's a number of 2D Lines to the screen.
        /// </summary>
        /// <value>The line batch.</value>
        public vxLineBatch LineBatch;

        /// <summary>
        /// Should the Physics Engine be multithreaded, if available.
        /// </summary>
        public bool MultiThreadPhysics = true;

        /// <summary>
        /// Gets or sets the game version.
        /// </summary>
        /// <value>The game version.</value>
        public string GameVersion = "v. 0.0.0.0";

        /// <summary>
        /// Gets or sets the vxEngine version.
        /// </summary>
        /// <value>The vxEngine version.</value>
        public string EngineVersion = "v. 0.0.0.0";

        /// <summary>
        /// The cmd line arguments.
        /// </summary>
        public readonly string[] CMDLineArgs;


        /// <summary>
        /// Gets the CMDL ine arguments as string.
        /// </summary>
        /// <returns>The CMDL ine arguments as string.</returns>
        public string CMDLineArgsToString()
        {
            string[] args = CMDLineArgs;

            string argoutput = "";
            for (int argIndex = 1; argIndex < args.Length; argIndex++)
            {
                argoutput += args[argIndex] + " ";
            }
            return argoutput;
        }


        /// <summary>
        /// The Current Gameplay Screen that is being run.
        /// </summary>
        public vxGameplaySceneBase CurrentScene;


        /// <summary>
        /// Bool value letting the Renderer know if the render targets need to be resized.
        /// </summary>
        public bool IsBackBufferInvalidated = false;


        /// <summary>
        /// Gets or sets the vxGUItheme use by this game.
        /// </summary>
        public vxGUITheme GUITheme;
        


        /// <summary>
        /// Gets or sets the splash screen.
        /// </summary>
        /// <value>The splash screen.</value>
        public Texture2D SplashScreen;


        #endregion

#if __ANDROID__
        public Android.App.Activity Activity;
#endif

        /// <summary>
        /// The screen recorder.
        /// </summary>
        public vxIScreenRecorder ScreenRecorder;

        /// <summary>
        /// A List Containing all Language Packs for this 
        /// </summary>
        public List<vxLanguagePack> Languages;

        /// <summary>
        /// The Currently Selected Language
        /// </summary>
        public vxLanguagePack Language;

        /// <summary>
        /// Default Settings
        /// </summary>
        public vxSettings Settings = new vxSettings();

        #region Properties

        /// <summary>
        /// The current player profile. This holds platform spricif values such as Steam User info, Google Play info or Game Center stuff.
        /// </summary>
        public vxIPlayerProfile PlayerProfile { get; private set; }




        public GraphicsDevice GraphicsDevice
        {
            get { return Game.GraphicsDevice; }
        }



        #endregion

        #region Initialization

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        private vxEngine()
        {
#if __ANDROID__
            _platformOS = vxPlatformOS.Android;
            _platformType = vxPlatformType.Mobile;
#elif __IOS__
            _platformOS = vxPlatformOS.iOS;
            _platformType = vxPlatformType.Mobile;
#else

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _platformOS = vxPlatformOS.Windows;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                _platformOS = vxPlatformOS.OSX;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                _platformOS = vxPlatformOS.Linux;

            _platformType = vxPlatformType.Desktop;
#endif

            // Get the CMD line args for this game.
            CMDLineArgs = System.Environment.GetCommandLineArgs();

            HasLicense = LicenseCheck();

        }

        internal void Init(vxGame Game)
        { 
            _game = Game;

            _gameName = Game.Config.GameName;

            GameVersion = Game.GetGameVersion().ToString();

            this.Game.IsMouseVisible = true;


            #region Setup Fundamental Systems

            vxRandom.Init(DateTime.Now.Millisecond);

            vxIO.Init(this);

            vxAudioManager.Init(this);

            #endregion


#if __STEAM__
            PlayerProfile = new vxPlayerProfileSteam(this);
#else
            PlayerProfile = new vxPlayerProfile(this);
#endif

            if (Game.Config.HasProfileSupport)
                PlayerProfile.Initialise();

            IsEngineInitialised = true;
        }

        private readonly bool HasLicense = false;
        private bool LicenseCheck()
        {
            return true;
            // TO Decode
            //int numPrev = 0;
            //string result = "";
            //foreach (var s in str)
            //{
            //    char ch = (char)(256 + numPrev - int.Parse(s));

            //    numPrev = (int)(ch);

            //    result += ch;
            //}

            //string n = Game.Config.GameName;

            //string r = "";

            //int prevMin = 0;
            //foreach (char c in n)
            //{
            //    int num = (256 + prevMin - (int)c);
            //    prevMin = (int)c;
            //    string output = num.ToString();
            //    r += output;

            //}

            //if (r == "" || n == "")
            //    return false;

            //if (Game.CheckEngineLicense() == r)
            //    return true;
            //else
            //    return false;
        }



        /// <summary>
        /// Sets the build config.
        /// </summary>
        /// <param name="configType">Config type.</param>
        internal void SetBuildType(vxBuildType configType)
        {
            vxConsole.WriteLine("Setting Build Config too: " + configType.ToString());
            _buildConfigType = configType;
        }

        private Texture2D Logo;
        /*
        public void InitIO()
        {
            vxIO.CheckDirExist();

            // Setup and Load Settings at the start
            vxSettings.Init();
        }
        */
        /// <summary>
        /// Load your graphics content.
        /// </summary>
        internal void LoadContent()
        {
#if DEBUG
            _buildConfigType = vxBuildType.Debug;
#else
            _buildConfigType = vxBuildType.Release;
#endif

            // Initialise the Engine Speciality Content Manager.
            vxContentManager.Instance.Init();

            // Setup and Load Settings at the start
            vxSettings.Init();

            // Setup the Debug Systems as the Singletons and Class after will use it
            #region Initialise Debug Systems

            //Get the vxEngine Version through Reflection
            EngineVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


            // Initialize the debug system
            vxDebug.Init(this);

            vxConsole.Initialize(this);

            // Setup the Profiler
            vxProfiler.Init(this);

            vxProfiler.RegisterMark("Draw", Color.Red);
            vxProfiler.RegisterMark("Update", Color.Yellow);
            vxProfiler.RegisterMark("Physics", Color.LimeGreen);
            vxProfiler.RegisterMark("GUI", Color.DeepSkyBlue);

            Game.InitDebugCommands();

            //vxSystemProfiler.Init();
            #endregion

            vxGraphics.Init();

            // Setup the Screen Manager
            vxScreen.Init(this);

            // Setup the Input Manager
            vxInput.Init(this);

            // Start the Plugin Manager
            vxPluginManager.Init(this);

            // Setup the Scene Manager
            vxSceneManager.Init(this);

            vxAudioManager.Init(this);

            LineBatch = new vxLineBatch(GraphicsDevice);
            SpriteBatch = new vxSpriteBatch(GraphicsDevice);

            GUITheme = new vxGUITheme(this);
            GUITheme.SetDefaultTheme();
            Logo = vxInternalAssets.LoadInternal<Texture2D>("Textures/logo/logo_72");

            // Setup the Network Manager
            vxNetworkManager.Init(this);

            #region Load Base Language

            Languages = new List<vxLanguagePack>();
            this.Game.LoadLanguagePacks();

            //Default is English
            this.Language = Languages[0];

            #endregion

            //Load in Settings Data
            try
            {
                Settings.Load(this);
            }
            catch
            {
                vxConsole.WriteLine("UNABLE TO LOAD SETTINGS");
            }
            // TODO: Add Language handleing from Player Proile APIs

            //Once all items are Managers and Engine classes are intitalised, then apply any and all passed cmd line arguments
            ApplyCommandLineArgs();

            //Set Initial Graphics Settings
            vxScreen.RefreshGraphics();

            // finally apply the language
            Game.SetLanguage(vxSettings.Language);

            // Call this once the Game and graphics Device Object have been created.
            if (Game.Config.HasProfileSupport)
                PlayerProfile.InitialisePlayerInfo();

            vxSceneManager.LoadContent();

            vxGraphics.InitMainBuffer();

            ScreenRecorder = new vxScreenRecorder();
        }



        public void OnGraphicsRefresh()
        {
            vxLayout.SetLayoutScale(GraphicsDevice.Viewport.Bounds.Size, Game.Config.IdealScreenSize);
            vxSceneManager.OnGraphicsRefresh();

        }




        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        public void UnloadContent()
        {
            vxIO.ClearTempDirectory();

            vxSceneManager.UnloadContent();
        }

        public void Dispose()
        {
            vxNetworkManager.Dispose();

            vxConsole.DumpLog();
        }

        #endregion

        #region Update and Draw

        public bool Pause = false;

        public bool AlwaysRun = true;

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            vxProfiler.Update();

            vxProfiler.BeginMark("Update");

            // Reset Batch Caller
            SpriteBatch.StartNewFrame();

            PlayerProfile.Update();

            if (Game.IsActive && Pause == false || AlwaysRun)
            {
                vxNotificationManager.Update();

                //If the Debug Console is Open, Then don't update or take input
                if (!vxDebug.CommandUI.Focused)
                {
                    // Read the keyboard and gamepad.
                    vxInput.Update(gameTime);

                    // Now update all scenes 
                    vxSceneManager.Update(gameTime);
                }
            }

            // update audio manager
            vxAudioManager.Update(gameTime);

            // Stop measuring time for "Update".
            vxProfiler.EndMark("Update");

            vxDebug.UpdateTools(gameTime);
        }



        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            // Start measuring time for "Draw".
            vxProfiler.BeginMark("Draw");
            
            // draw all scenes
            vxSceneManager.Draw(gameTime);

            // finalise the Frame
            vxGraphics.Finalise();

            vxNotificationManager.Draw();


            DrawDebugFlag();

            // Draw required textures for the Input Manager
            vxInput.Draw();

            
            vxConsole.WriteInGameDebug("ClearCount: ", GraphicsDevice.Metrics.ClearCount);
            vxConsole.WriteInGameDebug("DrawCount: ", GraphicsDevice.Metrics.DrawCount);
            vxConsole.WriteInGameDebug("PrimitiveCount: ", GraphicsDevice.Metrics.PrimitiveCount);
            vxConsole.WriteInGameDebug("VertexShaderCount: ", GraphicsDevice.Metrics.VertexShaderCount);
            vxConsole.WriteInGameDebug("PixelShaderCount: ", GraphicsDevice.Metrics.PixelShaderCount);

            // Draw the Debug Console
            vxConsole.Draw();

            if (HasLicense == false)
            {
                Vector2 pos = new Vector2(48, GraphicsDevice.Viewport.Height - 48);
                string txt = "VERTICES ENGINE \nv." + EngineVersion + "\n" + GraphicalBackend;
                
                // Draw the Trial Text
                SpriteBatch.DrawString(vxInternalAssets.Fonts.ViewerFont, txt, pos + Vector2.One, Color.Black);
                SpriteBatch.DrawString(vxInternalAssets.Fonts.ViewerFont, txt, pos, Color.White);
                var rect = new Rectangle(
                5, GraphicsDevice.Viewport.Height - 48, 72 / 2, 72 / 2);
                rect.Location += new Point(1);
                SpriteBatch.Draw(Logo, rect, Color.Black * 0.5f);
                rect.Location -= new Point(1);
                SpriteBatch.Draw(Logo, rect, Color.White);
            }

            SpriteBatch.End();

            vxProfiler.EndMark("Draw");
            // Now draw the Time Ruler Graph outside of the Sprite Batch
            //DebugSystem.DebugTimer.DrawGraph();
            vxDebug.Draw(gameTime);
        }


        #endregion

        #region Utilty Functions


        /// <summary>
        /// Returns the current scene 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCurrentScene<T>() where T : vxGameplaySceneBase
        {
            return (T)CurrentScene;
        }

        #endregion
    }
}