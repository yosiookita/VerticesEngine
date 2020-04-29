#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using VerticesEngine.Audio;
using VerticesEngine;
using VerticesEngine.Commands;
using VerticesEngine.Diagnostics;

using VerticesEngine.Particles;
using VerticesEngine.Input;
using VerticesEngine.Input.Events;
using VerticesEngine.Profile;
using VerticesEngine.Screens.Async;
using VerticesEngine.Serilization;
using VerticesEngine.UI;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Menus;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.Utilities;
using VerticesEngine.Graphics;
//using Microsoft.Xna.Framework.GamerServices;

#endregion

namespace VerticesEngine
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class vxGameplaySceneBase : vxBaseScene
    {
        /// <summary>
        /// The cameras collection.
        /// </summary>
        public List<vxCamera> Cameras = new List<vxCamera>();

        /// <summary>
        /// The particle system.
        /// </summary>
        public vxParticleSystem ParticleSystem;



        #region Fields

        public vxIPlayerProfile PlayerProfile
        {
            get { return Engine.PlayerProfile; }
        }

        /// <summary>
        /// The content manager for this scene. This is unloaded at the end of the scene.
        /// </summary>
        public ContentManager SceneContent;

        /// <summary>
        /// The type of the game, whether its a Local game or Networked.
        /// </summary>
        public vxNetworkGameType GameType = vxNetworkGameType.Local;

        /// <summary>
        /// The Scene UI Manager
        /// </summary>
        public vxGUIManager UIManager;


        /// <summary>
        /// Is this level the start background
        /// </summary>
        public bool IsStartBackground = false;

        /// <summary>
        /// The Level Title
        /// </summary>
        public string Title
        {
            get { return SandBoxFile.LevelTitle; }
            set { SandBoxFile.LevelTitle = value; }
        }

        /// <summary>
        /// The Level Description
        /// </summary>
        public string Description
        {
            get { return SandBoxFile.LevelDescription; }
            set { SandBoxFile.LevelDescription = value; }
        }

        /// <summary>
        /// The current game time.
        /// </summary>
        //public GameTime CurrentGameTime;

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        /// <value>The elapsed time.</value>
        //public float ElapsedTime = 0.0167f;


        /// <summary>
        /// Should the GUI be shown?
        /// </summary>
        public bool IsGUIVisible = true;


        /// <summary>
        /// Whether or not to dim the scene when it's covered by another screen.
        /// </summary>
        public bool IsSceneDimmedOnCover = true;


        /// <summary>
        /// The number of players in this Scene. This must be set in the constructor.
        /// </summary>
        public readonly int NumberOfPlayers;

        /// <summary>
        /// Gets the default texture.
        /// </summary>
        /// <value>The default texture.</value>
        public Texture2D DefaultTexture
        {
            get { return vxInternalAssets.Textures.Blank; }
        }


        /// <summary>
        /// List of Entities to draw as with Alpha less than 1 and which don't cast shadows.
        /// </summary>
        public List<vxEntity> AlphaEntities = new List<vxEntity>();


        /// <summary>
        /// This is the Pause Alpha Amount based off of the poisition the screen is in terms of
        /// transitioning too a new screen.
        /// </summary>
        public float PauseAlpha;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pausable.
        /// </summary>
        /// <value><c>true</c> if this instance is pausable; otherwise, <c>false</c>.</value>
        public bool IsPausable = true;

        /// <summary>
        /// The command manager to handle undo redos.
        /// </summary>
        public vxCommandManager CommandManager;


        /// <summary>
        /// The entity collection for this Scene.
        /// </summary>
        public List<vxEntity> Entities = new List<vxEntity>();


        /// <summary>
        /// Is the Sandbox In Testing Mode
        /// </summary>
        public vxEnumSandboxStatus SandboxCurrentState;


        public vxStartGameMode SandboxStartGameType
        {
            get { return _sandboxStartGameType; }
            set
            {
                _sandboxStartGameType = value;
            }
        }
        public vxStartGameMode _sandboxStartGameType = vxStartGameMode.GamePlay;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.vxSceneBase"/> class.
        /// </summary>
        /// <param name="sandboxStartGameType">Sandbox start game type.</param>
        /// <param name="FilePath">File path.</param>
        /// <param name="NumberOfPlayers">Number of players.</param>
        public vxGameplaySceneBase(vxStartGameMode sandboxStartGameType = vxStartGameMode.GamePlay,
                           string FilePath = "",
                           int NumberOfPlayers = 1)
        {
            vxConsole.WriteLine("");
            vxConsole.WriteLine("Starting Scene - " + this.GetType());
            vxConsole.WriteLine("---------------------------------------------------");

            this.NumberOfPlayers = NumberOfPlayers;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // Initiaise the File Structure
            SandBoxFile = InitSaveFile();

            this._sandboxStartGameType = sandboxStartGameType;

            this.FilePath = FilePath;
        }


        /// <summary>
        /// Initialises the physics engine for this scene.
        /// </summary>
        public virtual void InitialisePhysics() { }


        /// <summary>
        /// Initialises the cameras for this scene.
        /// </summary>
        public virtual void InitialiseCameras() { }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            DebugMethodCall("LoadContent", ConsoleColor.Magenta);

            Engine.CurrentScene = this;

            UIManager = new vxGUIManager(Engine);

            ParticleSystem = new vxParticleSystem();

            if (SceneContent == null)
                SceneContent = new ContentManager(Engine.Game.Services, "Content");


            CommandManager = new vxCommandManager();


            InitialisePhysics();

            // Set up the Camera
            InitialiseCameras();

            LoadParticlePools();
        }


        protected virtual void LoadParticlePools() { }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            for (int i = 0; i < Entities.Count;)
                Entities[i].Dispose();

            Entities.Clear();

            ParticleSystem.Dispose();

            vxGameObject.NameRegister.Clear();

            if (SceneContent != null)
                SceneContent.Unload();

            DebugMethodCall("UnloadContent", ConsoleColor.DarkMagenta);


            GC.Collect();
        }

        public void DebugMethodCall(string method, ConsoleColor ConsoleColor = ConsoleColor.Yellow)
        {
            vxConsole.WriteLine(this.ToString().Substring(this.ToString().LastIndexOf('.') + 1) + "." + method + "()", ConsoleColor);
        }
        #endregion

        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();


            foreach (vxCamera camera in Cameras)
                camera.OnGraphicsRefresh();
        }

        #region IO

        public virtual vxFileInfo GetFileInfo()
        {
            var fileInfo = new vxFileInfo();
            fileInfo.Initialise(Engine);

            // Set current version and info
            fileInfo.SandboxFileInfo.Title = Title;
            fileInfo.SandboxFileInfo.Description = Description;

            fileInfo.WorkshopFileInfo.Author = Engine.PlayerProfile.DisplayName;
            fileInfo.WorkshopFileInfo.id = WorkshopID;

            return fileInfo;
        }
        /// <summary>
        /// The workshop identifier. This is set only 
        /// </summary>
        public ulong WorkshopID = 0;


        //[XmlAttribute("wrkshpID")]
        //public ulong WorkshopID = 0;
        public virtual vxSandboxFileLoadResult LoadFile(string FilePath)
        {
            vxSandboxFileLoadResult loadResult = new vxSandboxFileLoadResult();
            // Only load a file if the File Path isn't empty
            if (FilePath != "")
            {
                DateTime startTime = DateTime.Now;

                vxConsole.WriteLine("");
                vxConsole.WriteLine("=====================================================");
                vxConsole.WriteLine("Loading File: " + FilePath);
                vxConsole.WriteLine("-----------------------------------------------------");

                FileName = Path.GetFileName(FilePath);
                FileName = FileName.Substring(0, FileName.LastIndexOf('.'));

                // First clear the temp folder
                vxIO.ClearTempDirectory();



                // Decompress The file
                try
                {
                    vxIO.DecompressToDirectory(FilePath, vxIO.PathToTempFolder, null,
                                               (SandboxStartGameType == vxStartGameMode.GamePlay));
                }
                catch
                {
                    // OSX .app files put them in a different path
                    // TODO; move sbx files into content reader class
                    string macContentPath = "../Resources/" + FilePath;
                    vxIO.DecompressToDirectory(macContentPath, vxIO.PathToTempFolder, null,
                                               (SandboxStartGameType == vxStartGameMode.GamePlay));
                }

                // First load the file info
                var fileInfo = new vxFileInfo();
                fileInfo.Load(vxIO.PathToTempFolder);

                OnFileInfoLoad(fileInfo);

                // Future Use, Handle IO Version Differences Here.
                loadResult = LoadFile(FilePath, fileInfo.Version);


                DateTime endTime = DateTime.Now;

                vxConsole.WriteIODebug(string.Format("Finished. File Loaded in: {0} ms", (float)(endTime - startTime).Milliseconds / 1000));
                vxConsole.WriteIODebug(string.Format("     File Version: {0}.", fileInfo.Version));
                vxConsole.WriteLine("=====================================================");
            }
            return loadResult;
        }

        /// <summary>
        /// When File Info is Loaded. This is called before LoadFile(...) is called.
        /// </summary>
        /// <param name="fileInfo">File info.</param>
        public virtual void OnFileInfoLoad(vxFileInfo fileInfo)
        {

        }

        public virtual vxSandboxFileLoadResult LoadFile(string FilePath, int version)
        {
            vxMessageBox ErrorMsgBox = new vxMessageBox(
                "This file version is not supported.\nMake sure you have the most up to date version\nand try again!",
                "Uncompatible File Version.");

            vxSceneManager.AddScene(ErrorMsgBox);
            Console.WriteLine("File Version is not supported.");

            return new vxSandboxFileLoadResult(FilePath);
        }


        /// <summary>
        /// Saves the current Sandbox File.
        /// </summary>
        /// <param name="takeScreenshot"></param>
        public virtual void SaveFile(bool takeScreenshot, bool DoDump = false)
        {
            // first check that there's a file name
            if (FileName == "")
                SaveFileAs();
            else
            {
                //takeScreenshot = true;
                if (takeScreenshot)
                {
                    IsGUIVisible = false;
                    ThumbnailImage = vxScreen.TakeScreenshot();// LastFrame;
                    IsGUIVisible = true;
                }
                IsDumping = DoDump;
                //SerializeFile();
                vxSceneManager.AddScene(GetAsyncSaveScreen());
            }
        }
        public bool IsDumping = false;

        /// <summary>
        /// Gets the async save screen. Override this to provide a custom save screeen.
        /// </summary>
        /// <returns>The async save screen.</returns>
        public virtual vxSaveBusyScreen GetAsyncSaveScreen()
        {
            return new vxSaveBusyScreen(this);
        }



        /// <summary>
        /// Saves the support files such as as thumbnail and img. Override to add your own files.
        /// </summary>
        public virtual void SaveSupportFiles()
        {
            //First Check, if the Items Directory Doesn't Exist, Create It
            string ExtractionPath = vxIO.PathToTempFolder;

            // clear the temp directory

            string path = vxIO.PathToSandbox;

            if (IsDumping)
                ExtractionPath = path + "/dump";

            //First Check, if the Items Directory Doesn't Exist, Create It
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            if (Directory.Exists(ExtractionPath) == false)
                Directory.CreateDirectory(ExtractionPath);

            if (ThumbnailImage == null)
                ThumbnailImage = DefaultTexture;

            var size = OnGetPreviewImageSize();

            Texture2D img = ThumbnailImage.Resize(Engine, size.X, size.Y);

            img.SaveToDisk(ExtractionPath + "/img.png");

            img.Resize(Engine, 96, 96).SaveToDisk(ExtractionPath + "/thumbnail.png");

        }

        public virtual Point OnGetPreviewImageSize()
        {
            return new Point(1280, 720);
        }

        /// <summary>
        /// This is a Verbose Debug info for which game saved the file.
        /// </summary>
        /// <returns></returns>
        public virtual string GetExporterInfo()
        {
            return "Saved by the Default Engine Exporter! v." + Engine.EngineVersion;
        }

        protected List<vxEntity> DisposalQueue = new List<vxEntity>();
        public virtual void AddForDisposal(vxEntity entity)
        {
            DisposalQueue.Add(entity);
        }

        /// <summary>
        /// File Format
        /// </summary>
        public vxSerializableSceneBaseData SandBoxFile;

        /// <summary>
        /// Initialises the Save File. If the XML Save file uses a different type, then
        /// this can be overridden.
        /// </summary>
        /// <returns></returns>
        public virtual vxSerializableSceneBaseData InitSaveFile()
        {
            return new vxSerializableSceneBaseData();
        }


        /// <summary>
        /// Returns a Deserializes the File. If you want to use a different type to Serialise than the base 'vxSerializableScene3DData'
        /// then you must override this or it will throw an error.
        /// </summary>
        public virtual vxSerializableSceneBaseData DeserializeFile(string path)
        {
            vxSerializableSceneBaseData file;
            XmlSerializer deserializer = new XmlSerializer(typeof(vxSerializableSceneBaseData));
            TextReader reader = new StreamReader(path);
            file = (vxSerializableSceneBaseData)deserializer.Deserialize(reader);
            reader.Close();

            return file;
        }

        /// <summary>
        /// Place holder for the texture for taking a fullscreenshot.
        /// </summary>
        public Texture2D ThumbnailImage;


        public string FilePath = "";

        public string FileName = "";

        /// <summary>
        /// An in game Open File Dialog too access files from specific directories.
        /// </summary>
        public vxOpenSandboxFileDialog OpenFileDialog;

        /// <summary>
        /// Save As Message Box.
        /// </summary>
        vxMessageBoxSaveAs SaveAsMsgBox;


        /// <summary>
        /// Start a New File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_NewFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            vxMessageBox NewFile = new vxMessageBox("Are you sure you want to Start a New File,\nAll Unsaved Work will be Lost", "quit?");
            vxSceneManager.AddScene(NewFile, ControllingPlayer);
            NewFile.Accepted += Event_NewFile_Accepted;
        }

        /// <summary>
        /// When to do when the New File button is clicked. This must be overridden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_NewFile_Accepted(object sender, PlayerIndexEventArgs e)
        {
            vxLoadingScreen.Load(Engine, true, PlayerIndex.One, OnNewSandbox());
        }

        /// <summary>
        /// Called when a New Sandbox file is called. Override this to provide your base class.
        /// </summary>
        /// <returns>The new sandbox.</returns>
        public virtual vxGameplaySceneBase OnNewSandbox()
        {
            return new vxGameplaySceneBase();
        }





        /// <summary>
        /// Event for Opening a File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_OpenFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            OpenFileDialog = new vxOpenSandboxFileDialog( "sbx");
            vxSceneManager.AddScene(OpenFileDialog, ControllingPlayer);
            OpenFileDialog.Accepted += Event_OpenFileDialog_Accepted;
        }

        public virtual void Event_OpenFileDialog_Accepted(object sender, vxSandboxFileSelectedEventArgs e)
        {
            vxMessageBoxSaveBeforeQuit saveBeforeCloseCheck = new vxMessageBoxSaveBeforeQuit("Are you sure you want to close without Saving?\nAll un-saved work will be lost", "Close Without Saving?");
            saveBeforeCloseCheck.Apply += Event_SaveBeforeCloseCheck_Save;
            saveBeforeCloseCheck.Accepted += Event_SaveBeforeCloseCheck_DontSave;
            vxSceneManager.AddScene(saveBeforeCloseCheck, ControllingPlayer);
        }

        /// <summary>
        /// Called when a New Sandbox file is called. Override this to provide your base class.
        /// </summary>
        /// <returns>The new sandbox.</returns>
        public virtual vxGameplaySceneBase OnOpenSandboxFile(string filePath)
        {
            return new vxGameplaySceneBase();
        }

        void LoadFileFromOpenDialog()
        {
            vxLoadingScreen.Load(Engine, true, PlayerIndex.One, OnOpenSandboxFile(OpenFileDialog.SelectedItem));
        }


        public virtual void Event_SaveBeforeCloseCheck_Save(object sender, PlayerIndexEventArgs e)
        {
            SaveFile(true);
            LoadFileFromOpenDialog();
        }

        public virtual void Event_SaveBeforeCloseCheck_DontSave(object sender, PlayerIndexEventArgs e)
        {
            LoadFileFromOpenDialog();
        }

        public virtual void SaveFileAs(string saveAsMsg = "Save the current file as...")
        {
            // Capture the Thumbnail before the Message Box Pops up.
            IsGUIVisible = false;
            ThumbnailImage = vxScreen.TakeScreenshot();
            IsGUIVisible = true;

            SaveAsMsgBox = new vxMessageBoxSaveAs(saveAsMsg, "Save As", FileName);
            vxSceneManager.AddScene(SaveAsMsgBox, ControllingPlayer);
            SaveAsMsgBox.Accepted += Event_SaveAsMsg_Accepted;
        }

        /// <summary>
        /// Event for Saving the Current File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_SaveFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SaveFile(true);
        }


        /// <summary>
        /// Event for Saving As the Current File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Event_SaveAsFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SaveFileAs();
        }

        public virtual void Event_SaveAsMsg_Accepted(object sender, PlayerIndexEventArgs e)
        {
            //FileName = SaveAsMsgBox.Textbox.Text;
            SaveAs(SaveAsMsgBox.Textbox.Text);
        }

        void SaveAs(string filename)
        {
            FileName = filename;
            if (File.Exists(Path.Combine(vxIO.PathToSandbox, filename + ".sbx")))
            {
                var confirmOverriteMsgBox = new vxMessageBox("Overwrite File '" + filename + ".sbx'?", "Overwrite?");

                confirmOverriteMsgBox.Accepted += delegate {
                    vxConsole.WriteIODebug("Saving: " + filename);
                    SaveFile(false);
                };
                vxSceneManager.AddScene(confirmOverriteMsgBox);
            }
            else
            {
                vxConsole.WriteIODebug("Saving: " + filename);
                SaveFile(false);
            }
        }

        public virtual void DumpFile()
        {
            string compFile = vxIO.PathToSandbox + "/dump/" + FileName + ".sbx";

            vxConsole.WriteLine(string.Format("Dumping file '{0}'...", compFile));

            //SaveFile(true, true);
            IsDumping = true;
            var saveScreen = GetAsyncSaveScreen();
            saveScreen.StartSave();

            vxConsole.WriteLine(string.Format("Files Dumped"));
        }

        public virtual void PackFile()
        {
            vxConsole.WriteLine(string.Format("Packing Files from '{0}'", vxIO.PathToSandbox + "/dump/"));

            string compFile = vxIO.PathToSandbox + "/dump/" + FileName + ".sbx";

            if (File.Exists(compFile))
                File.Delete(compFile);

            vxIO.CompressDirectory(vxIO.PathToSandbox + "/dump", compFile, null);

            vxConsole.WriteLine(string.Format("Files packed into '{0}'", compFile));
        }


        #endregion

        #region Particles 


        /// <summary>
        /// Spawns a new particle using the specified key from the Particle System.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="emitter"></param>
        public void SpawnParticle(object key, vxGameObject emitter)
        {
            ParticleSystem.SpawnParticle(key, emitter);
        }

        #endregion

        #region Sandbox


        protected Dictionary<string, Type> TypeRegister = new Dictionary<string, Type>();


        protected virtual void OnRegisterNewEntity(vxSandboxEntityRegistrationInfo entityDefinition)
        {
            if (TypeRegister.ContainsKey(entityDefinition.Key))
            {
                vxConsole.WriteError("Key '" + entityDefinition.Key + "' already exits.");
                return;
            }

            TypeRegister.Add(entityDefinition.Key, entityDefinition.Type);
        }
        #endregion

        #region Update and Draw

        public virtual void SimulationStart() { }

        public virtual void SimulationStop() { }

        /// <summary>
        /// Updates the state of the game. This method checks the vxGameBaseScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Handle Disposing of Items
            if (DisposalQueue.Count > 0)
            {
                // Loop through entire queue
                foreach (var entity in DisposalQueue)
                    entity.Dispose();

                // now clear it
                DisposalQueue.Clear();
            }

            // Calculate GameTime variables
            //CurrentGameTime = gameTime;
            //ElapsedTime = (float)CurrentGameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            //Only update the GUI if the Current Screen is Active
            if (IsActive)
            {
                UIManager.Update();
            }

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen && IsSceneDimmedOnCover)
                PauseAlpha = Math.Min(PauseAlpha + 1f / 32, 1);
            else
                PauseAlpha = Math.Max(PauseAlpha - 1f / 32, 0);

            if (IsActive || IsPausable == false)
            {
                //UpdateScene (gameTime, otherScreenHasFocus,coveredByOtherScreen);
            }

            // Update Camera
            //**********************************************************************************************
            
            for (int c = 0; c < Cameras.Count; c++)
                Cameras[c].Update(gameTime);

            ParticleSystem.Update(gameTime);
        }



        /// <summary>
        /// Main Game Update Loop which is accessed outside of the vxEngine
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public virtual void UpdateScene(GameTime gameTime, bool otherScreenHasFocus,
            bool coveredByOtherScreen)
        {

        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput()
        {

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            if (vxInput.IsPauseGame())
            {
                if (SandboxStartGameType != vxStartGameMode.GamePlay &&
                    SandboxCurrentState == vxEnumSandboxStatus.Running)
                    SimulationStop();
                else
                    ShowPauseScreen();
            }
            else
            {
                HandleInputBase();
            }
        }

        /// <summary>
        /// Handles the input base.
        /// </summary>
        /// <param name="input">Input.</param>
        public virtual void HandleInputBase()
        {

        }


        /// <summary>
        /// This Method Loads the Engine Base Pause Screen (PauseMenuScreen()), but 
        /// more items might be needed to be added. Override to
        /// load your own Pause Screen.
        /// </summary>
        /// <example> 
        /// This sample shows how to override the <see cref="ShowPauseScreen"/> method. 'MyGamesCustomPauseScreen()' inheirts
        /// from the <see cref="VerticesEngine.UI.Menus.vxMenuBaseScreen"/> Class.
        /// <code>
        /// //This Allows to show your own custom pause screen.
        /// public override void ShowPauseScreen()
        /// {
        ///     vxSceneManager.AddScreen(new MyGamesCustomPauseScreen(), ControllingPlayer);
        /// }
        /// </code>
        /// </example>
        public virtual void ShowPauseScreen()
        {
            vxSceneManager.AddScene(new vxPauseMenuScreen(), ControllingPlayer);
        }


        /// <summary>
        /// The color to clear the back buffer with.
        /// </summary>
        public Color BackBufferClearColor = Color.CornflowerBlue;


        /// <summary>
        /// Draw's the background before proceeding with other drawing. This is useful for Skyboxes and backgrounds as a whole
        /// </summary>
        public virtual void DrawBackground()
        {

        }
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.SetRenderTarget(SetInitialRenderTarget());

            GraphicsDevice.Clear(ClearOptions.Target, BackBufferClearColor, 0, 0);

            // Draw the scene
            DrawScene(gameTime);


            vxProfiler.BeginMark("GUI");
            // if the GUI should be shown, then show it.
            if (IsGUIVisible)
            {
                Engine.SpriteBatch.Begin("UI.MainGUI");
                DrawGUIItems();
                Engine.SpriteBatch.End();
            }
            vxProfiler.EndMark("GUI");
            CommandManager.Draw();


            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || PauseAlpha > 0)
                DrawTransition(MathHelper.Lerp(1f - TransitionAlpha, 1f, PauseAlpha * PauseAlphaFactor));

            DrawDebug(gameTime);
        }

        float PauseAlphaFactor = 0.5f;
        public virtual void DrawTransition(float transitionPosition)
        {
            vxSceneManager.FadeBackBufferToBlack(transitionPosition);
        }


        public virtual void DrawDebug(GameTime gameTime)
        {

        }


        /// <summary>
        /// Draws the GUII tems. this is pre-faced by a SpriteBatch.Begin() call.
        /// </summary>
        public virtual void DrawGUIItems()
        {
            UIManager.DrawByOwner();

        }



        /// <summary>
        /// This is called before the main sprite draw, but after the SpriteBatch.Begin() call. Overload this
        /// if you want to do some drawing before the main sprite draws, but within the same SpriteBatch call
        /// for efficinency.
        /// </summary>
        public virtual void PreDraw() { }

        /// <summary>
        /// This is called after the main sprite draw, but before the SpriteBatch.End() call. Overload this
        /// if you want to do some drawing after the main sprite draws, but within the same SpriteBatch call
        /// for efficinency.
        /// </summary>
        public virtual void PostDraw() { }

        /// <summary>
        /// Called after the full scene is drawn and all post processes are applied.
        /// </summary>
        public virtual void OnPostRender() { }


        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public virtual void DrawScene(GameTime gameTime)
        {

            // Zero out Primitive Counts
            Engine.GlobalPrimitiveCount = 0;

            // **********************************************************************************************

            // first render each camera to their respective back buffers
            for (int c = 0; c < Cameras.Count; c++)
            {
                Cameras[c].Render();
            }

            // now combine each of the camera's to the Engine's Final Back buffer
            GraphicsDevice.SetRenderTarget(vxGraphics.FinalBackBuffer);
            SpriteBatch.Begin("Scene Draw");

            for (int c = 0; c < Cameras.Count; c++) 
            {
                //    camera.Render();
                SpriteBatch.Draw(Cameras[c].Renderer.Finalise(), Cameras[c].Viewport.Bounds, Color.White);

            }
            SpriteBatch.End();

            for (int c = 0; c < Cameras.Count; c++)
            {
                // draw any debug stuff
                Cameras[c].Renderer.DrawDebug();
            }
            IsGUIVisible = (SandboxCurrentState == vxEnumSandboxStatus.EditMode);

            // Debug Rendering & Draw any inherited or overriden code
            //**********************************************************************************************
            DrawGameplayScreen(gameTime);

            // Draw Overlay items such as 3D Sandbox Cursor and HUD
            //**********************************************************************************************
            DrawOverlayItems();

            DrawHUD();

            //ViewportManager.ResetViewport();
        }

        public virtual void DrawOverlayItems() { }

        public virtual void DrawHUD() { }

        /// <summary>
        /// Main Gameplay Draw Loop which is accessed outside of the Engine.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void DrawGameplayScreen(GameTime gameTime)
        {


        }


        /// <summary>
        /// Draws the viewport splitters. This is called during the HUD draw call, therefore
        /// will be scooped up into that Spritebatch Call.
        /// </summary>
        public virtual void DrawViewportSplitters()
        {

        }

        #endregion

        #region Debug Drawing

        /// <summary>
        /// Draws the physics debug overlay for the individual systems.
        /// </summary>
        public virtual void DrawPhysicsDebug(vxCamera camera) { }

        #endregion
    }
}
