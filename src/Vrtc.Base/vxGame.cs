#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using VerticesEngine.Community;
using VerticesEngine.Diagnostics;
using VerticesEngine.Input;
using VerticesEngine.Localization;
using VerticesEngine.Mobile.Ads;
using VerticesEngine.Plugins;
using VerticesEngine.Screens.Async;
using VerticesEngine.UI;
using VerticesEngine.UI.Menus;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.Utilities;


#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Ads;
#endif

#endregion

namespace VerticesEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class vxGame : Game
    {
        #region Fields

        /// <summary>
        /// The reference to the main Vertices Engine.
        /// </summary>
        private vxEngine Engine;

        /// <summary>
        /// The graphics device manager.
        /// </summary>
        GraphicsDeviceManager GraphicsDeviceManager;


        /// <summary>
        /// Gets a value indicating whether this <see cref="T:VerticesEngine.vxGame"/> has been updated since last launch
        /// Check this whether to show a custom update screen.
        /// </summary>
        /// <value><c>true</c> if has game updated; otherwise, <c>false</c>.</value>
        public bool IsGameUpdated
        {
            get { return _isGameUpdated; }
        }
        bool _isGameUpdated = false;
        

        /// <summary>
        /// Tells whether or not the Game has loaded it's Specific Assets.
        /// </summary>
        internal bool IsGameContentLoaded = false;

        #endregion

        #region -- Private Fields --


        /// <summary>
        /// The game config object. This is set at launch and can not be changed
        /// once it's been initialised.
        /// </summary>
        public readonly vxGameConfig Config;


        /// <summary>
        /// Is this the first update
        /// </summary>
        private bool IsFirstUpdate = true;

        #endregion 

        #region Initialization


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.vxGame"/> class.
        /// </summary>
        public vxGame()
        {

            this.Config = GetGameConfig();

            GraphicsDeviceManager = new GraphicsDeviceManager(this);

            // Set the Games Root Directory
            Content = new ContentManager(this.Services)
            {
                RootDirectory = "Content"
            };

            // Create the Admob Overlay for specific mobile platforms
            vxAdManager.Init(this, new Vector2(0, 0));

            // Create the engine component.
            vxEngine.Instance.Init(this);
            Engine = vxEngine.Instance;
        }


        /// <summary>
        /// returns the games config info. Override this function in the main 
        /// game class to provide your own settings.
        /// </summary>
        /// <returns></returns>
        protected virtual vxGameConfig GetGameConfig()
        {
            vxGameConfig gameConfig = new vxGameConfig(
                "default_gamename",
                vxGameEnviromentType.TwoDimensional | vxGameEnviromentType.TwoDimensional,
                vxGameConfigFlags.AudioSettings |
                vxGameConfigFlags.ControlsSettings |
                vxGameConfigFlags.GraphicsSettings |
                vxGameConfigFlags.LanguageSettings);

            return gameConfig;
        }


        /// <summary>
        /// Gets the name of the game.
        /// </summary>
        /// <value>The name of the game.</value>
        public string GameName
        {
            get { return Config.GameName; }
        }

        /// <summary>
        /// Gets the game version.
        /// </summary>
        /// <returns>The game version.</returns>
        public virtual Version GetGameVersion()
        {
            return new Version(1, 0, 0, 0);
        }

        /// <summary>
        /// The App ID which is used by a large number of different platforms
        /// </summary>
        public string AppID
        {
            get { return GetAppID(); }
        }

        protected virtual string GetAppID()
        {
            throw new Exception("App ID Is not set for this game");
        }


        /// <summary>
        /// Initialise any and all overlays. This is often used for adding Admob views in iOS and Android
        /// as well as for handling Steam Overlay code.
        /// </summary>
        protected virtual void InitOverlays()
        {

        }

        /// <summary>
        /// Called at the start to get basic permissions for mobile apps. External Storage Read/Write is called here,
        /// but override if you want to add any more.
        /// </summary>
        public virtual void RequestPermissions()
        {
#if __ANDROID__
            Android.Support.V4.App.ActivityCompat.RequestPermissions(Game.Activity, new String[] { Android.Manifest.Permission.ReadExternalStorage }, 0);
            Android.Support.V4.App.ActivityCompat.RequestPermissions(Game.Activity, new String[] { Android.Manifest.Permission.WriteExternalStorage }, 0);
#endif
            vxTitleScreen.IsWaitingOnPermissions = false;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Load Engine 
            Engine.LoadContent();

            vxPluginManager.LoadPlugins();

            vxAdManager.Initalise(Engine);

            // Now Initialise any other Overlays
            InitOverlays();

            // Load GUI Content
            LoadGUITheme();

            vxSceneManager.AddScene(new vxTitleScreen(), null);
        }

        protected virtual string GetEngineLicense()
        {
            return "0000-0000-0000-0000-0000";
        }

        internal string CheckEngineLicense()
        {
            return GetEngineLicense();
        }

        /// <summary>
        /// Loads Global Content Specific for the Game. This is called by 'vxLoadAssetsScreen.Load(...'
        /// in the 'MainEntryPoint()' method.
        /// </summary>
        protected internal virtual void LoadGlobalContent()
        {
            // Let the Inpurt Manager Draw
            vxInput.IsCusorInitialised = true;
        }



        /// <summary>
        /// Override this to add more options to the settings screens.
        /// </summary>
        public virtual void AddSettingsScreens(vxSettingsMenuScreen SettingsMenuScreen)
        {

        }

        /// <summary>
        /// Loads GUI Theme, override to load a new theme
        /// </summary>
        public virtual void LoadGUITheme()
        {
            vxConsole.WriteLine("Loading GUI");

        }
        /// <summary>
        /// Creates the default langauge pack.
        /// </summary>
        public virtual vxLanguagePack CreateDefaultLangaugePack()
        {
            return new vxLanguagePack(vxLanguage.English);
        }
        /// <summary>
        /// Gets the supported languages. Override this to provide your own.
        /// </summary>
        /// <returns>The supported languages.</returns>
        public virtual vxLanguage[] GetSupportedLanguages()
        {
            return new vxLanguage[] { GetDefaultLanguage() };
        }

        /// <summary>
        /// Gets the default language.
        /// </summary>
        /// <returns>The default language.</returns>
        public virtual vxLanguage GetDefaultLanguage()
        {
            return vxLanguage.English;
        }

        /// <summary>
        /// Loads the Language Packs. Override this method to add your custom packs.
        /// </summary>
        public virtual void LoadLanguagePacks()
        {
            vxConsole.WriteLine("Starting Language Manager");

            // Load the default language (default is English)
            if (Engine.Languages.Count == 0)
                Engine.Languages.Add(CreateDefaultLangaugePack());

            // Now get all languages supported by the game
            foreach (var language in GetSupportedLanguages())
            {
                if (language != GetDefaultLanguage())
                    Engine.Languages.Add(vxLanguagePack.Load(Engine, language));
            }
        }
        /// <summary>
        /// Sets the language for this game. Override this method to apply your own specific localization 
        /// code such as loading specific font files, textures, etc...
        /// </summary>
        public virtual void SetLanguage(string newlanguage)
        {
            foreach (vxLanguagePack lang in Engine.Languages)
                if (lang.LanguageName == newlanguage)
                {
                    Engine.Language = lang;
                    vxSettings.Language = newlanguage;
                    Engine.Settings.Save(Engine);

                    foreach (vxBaseScene screen in vxSceneManager.GetScreens())
                        screen.OnLocalisationChanged();
                }
            LoadFonts();
        }

        public virtual void LoadFonts()
        {

        }

        /// <summary>
        /// Override this Method to have your DLC content loaded here.
        /// </summary>
        protected internal virtual string[] GetDLCPaths()
        {
            return new string[0];
        }


        /// <summary>
        /// This is the Main Entry point for the game external to the Engine. Override this and call StartGame(...)
        /// </summary>
        public virtual void OnGameStart()
        {
            throw new Exception("OnGameStart must be overriden to call StartGame(...);");
        }

        /// <summary>
        /// Loads the Player Profile Data
        /// </summary>
        public virtual void LoadPlayerProfile()
        {

        }

        /// <summary>
        /// Starts the game with the specefied Backgound Scene and Start menu. Override if you want anything more than just the
        /// basic 'background screen with menu over top.'
        /// </summary>
        public void OnGameStart(vxBaseScene BackgroundScene, vxBaseScene StartMenuScreen)
        {
            // start sign in at this point
            if (Config.HasProfileSupport)
                Engine.PlayerProfile.SignIn();

            if (vxSettings.Language == "" && Config.HasLanguageSettings)
                vxLoadAssetsScreen.Load(Engine, true, null, BackgroundScene, StartMenuScreen, GetLocalizationScreen());
            else
                vxLoadAssetsScreen.Load(Engine, true, null, BackgroundScene, StartMenuScreen);
        }

        /// <summary>
        /// Gets the localization screen. Override this to implement your own custom Localization screen.
        /// </summary>
        /// <returns>The localization screen.</returns>
        public virtual vxBaseScene GetLocalizationScreen()
        {
            return new vxLocalizationMenuScreen(true);
        }


        public virtual void InitDebugCommands()
        {

#if DEBUG
            // dumps or packs the current level
            // ******************************************************************
            vxDebug.CommandUI.RegisterCommand(
                "sbx",              // Name of command
                "Sandbox File Commands.",     // Description of command
                delegate (IDebugCommandHost host, string command, IList<string> args)
                {
                    if (args.Count == 0)
                        args.Add("-help");

                    switch (args[0])
                    {
                        case "-dump":
                            Engine.CurrentScene.DumpFile();
                            break;
                        case "-pack":
                            Engine.CurrentScene.PackFile();
                            break;
                        case "-help":
                            host.Echo("");
                            host.Echo("Sandbox File Command Help");
                            host.Echo("==============================================================================");
                            host.Echo("     -dump           Dumps the current sandbox components to it's uncompressed parts.");
                            host.Echo("     -pack           Packs dumped files into a Vertices 'sbx' file.");
                            host.Echo("");
                            break;
                    }
                });

#endif
        }

        #endregion

        #region Draw


        protected override void Update(GameTime gameTime)
        {
            // First thing is to update the time
            vxTime.Update(gameTime);

            if (IsFirstUpdate)
            {
                IsFirstUpdate = false;

                // Check if version is greater or not
                Version GameAssemblyVersion = new Version(Engine.GameVersion);

                if (GameAssemblyVersion > vxSettings.GameVersion.ToSystemVersion())
                {
                    vxSettings.GameVersion = new Serilization.vxSerializableVersion(GameAssemblyVersion);
                    Engine.Settings.Save(Engine);

                    _isGameUpdated = true;
                }
            }

            base.Update(gameTime);
            Engine.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the engine component.
            base.Draw(gameTime);

            // Draw the Engine
            Engine.Draw(gameTime);
        }


        #endregion

        protected internal virtual vxIPlugin GetMainGameContentPack()
        {
            throw new Exception("No Main Content Pack Provided.");
        }

        /// <summary>
        /// This method is called when the user clicks on the 'get mod sdk' button.
        /// </summary>
        public virtual void OnGetModSDK()
        {

        }


        /// <summary>
        /// This is called when the user wants to upload a mod
        /// </summary>
        public virtual void OnUploadMod()
        {

        }

        /// <summary>
        /// This is called when a workshop item is opened
        /// </summary>
        /// <param name="item"></param>
        public virtual void OnWorkshopItemOpen(vxIWorkshopItem item)
        {

        }

        /// <summary>
        /// Opens the credits page for this game in the web browser.
        /// </summary>
        public virtual void OnShowCreditsPage()
        {


        }

        /// <summary>
        /// Opens the review page for this games platform in the web browser.
        /// </summary>
        public virtual void OnShowReviewPrompt()
        {

        }



        /// <summary>
        /// Opens the store page for this game given the app id info in the Config.
        /// </summary>
        public virtual void OpenStorePage(string appid)
        {
#if __ANDROID__
            Engine.PlayerProfile.OpenStorePage(appid);
#elif __IOS__
            Console.WriteLine("OpenStorePage()");
            UIKit.UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(@"itms-apps://itunes.apple.com/app/id"+appid));
#else
            //System.Diagnostics.Process.Start(url);
            Engine.PlayerProfile.OpenStorePage(appid);
#endif
        }

        public virtual void ShowUpdateInfoScreen()
        {
            vxMessageBox msg = new vxMessageBox("Your Game has been updated!", "Updated!");
            vxSceneManager.AddScene(msg);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Console.WriteLine("Exiting ...");
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();

            vxEngine.Instance.UnloadContent();

            Console.WriteLine("Unloading Content...");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(true);

            Console.WriteLine("Disposing...");

            Engine.Dispose();

            if (vxEngine.PlatformOS == vxPlatformOS.Windows ||
                vxEngine.PlatformOS == vxPlatformOS.OSX ||
               vxEngine.PlatformOS == vxPlatformOS.Linux)
                System.Diagnostics.Process.GetCurrentProcess().Kill();
        }



#if __ANDROID__


        protected virtual void OnStart()
        {

        }

        protected virtual void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {

        }

        protected virtual void OnStop()
        {

        }
#endif

        #region -- Debug Methods --

        [vxDebugMethod("whoami", "Returns the current profile User Display Name if logged in")]
        static void WhoAmI(vxEngine engine)
        {
            if (engine.PlayerProfile.IsSignedIn)
                vxConsole.WriteLine(engine.PlayerProfile.DisplayName);
            else
                vxConsole.WriteLine("Player Not Signed In...");
        }



        #endregion

    }
}

