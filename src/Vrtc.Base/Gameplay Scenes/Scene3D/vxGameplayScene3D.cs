
#region Using Statements
//Virtex vxEngine Declaration

using BEPUphysics;
using BEPUphysicsDrawer.Models;
using BEPUutilities.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using VerticesEngine.Audio;
using VerticesEngine;

using VerticesEngine.Util;
using VerticesEngine.Graphics;
using VerticesEngine.Input;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.Utilities;

#endregion

namespace VerticesEngine
{
    /// <summary>
    /// The vxGameplayScene3D class implements the actual game logic for 3D Games.
    /// </summary>
    public partial class vxGameplayScene3D : vxGameplaySceneBase
    {
        #region Fields

        /// <summary>
        /// Editor Entities
        /// </summary>
        public List<vxEntity3D> EditorEntities = new List<vxEntity3D>();

        /// <summary>
        /// Sky box
        /// </summary>
        public vxSkyBoxEntity SkyBox;

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public vxSettings Settings
        {
            get { return Engine.Settings; }
        }



        /// <summary>
        /// Collection of Lights in the Level.
        /// </summary>
        public List<vxLightEntity> LightItems = new List<vxLightEntity>();

        /// <summary>
        /// The main BEPU Physics Simulation Space used in the game.
        /// </summary>
        public Space PhyicsSimulation;

        /// <summary>
        /// This is the multithreaded parrallel looper class used by BEPU too
        /// multi-thread the physics engine.
        /// </summary>
        private ParallelLooper BEPUParallelLooper;

        /// <summary>
        /// Model Drawer for debugging the phsyics system
        /// </summary>
        public ModelDrawer PhysicsDebugViewer;

        /// <summary>
        /// Manages the Sun Class.
        /// </summary>
        public vxSunEntity SunEmitter;

        /// <summary>
        /// Gets or sets the light positions.
        /// </summary>
        /// <value>The light positions.</value>
		public Vector3 LightPositions
        {
            get { return SunEmitter.SunWorldPosition; }
            set
            {
                _lightPositions = value;

                for (int c = 0; c < Cameras.Count; c++)
                    Cameras[c].Renderer.LightDirection = _lightPositions;
            }
        }
        private Vector3 _lightPositions;


        /// <summary>
        /// The viewport manager.
        /// </summary>
        public vxViewportManager ViewportManager;



        float pauseAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor which starts the game as a Normal (Non-Networked) game.
        /// </summary>
        public vxGameplayScene3D(vxStartGameMode SandboxStartMode, string FileToOpen = "", int NumberOfPlayers = 1)
            : base(SandboxStartMode, FileToOpen, NumberOfPlayers)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);


            // Inisitalise the Selected Items Collection
            SelectedItems = new List<vxEntity3D>();

            // Set the Hover Index to negative 1
            SelectedIndex = -1;

            // Default is Edit Mode
            SandboxCurrentState = vxEnumSandboxStatus.EditMode;

            ConnectedMatrix = Matrix.Identity;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            SandboxEditMode = vxEnumSanboxEditMode.SelectItem;



            this.SandboxStartGameType = SandboxStartMode;

            if (SandboxStartMode == vxStartGameMode.GamePlay)
                SandboxCurrentState = vxEnumSandboxStatus.Running;
            //else if (FileToOpen == "")
            //  SandboxStartGameType = vxStartGameMode.NewSandbox;
            else
                SandboxCurrentState = vxEnumSandboxStatus.EditMode;
              //  SandboxStartGameType = vxStartGameMode.OpenSandbox;

            //Set File name
            FilePath = FileToOpen;
            FileName = FileToOpen.GetFileNameFromPath();

        }

        /// <summary>
        /// Load graphics content for the game. Note the base.LoadContent() must be called at the
        /// top of any override function so that all systems are properly set up before loading your code.
        /// The calling order is:
        /// <para />InitialiseRenderingEngine(); 
        /// <para />InitialisePhysics();
        /// <para />InitialiseCameras();
        /// <para />InitialiseViewportManager();
        /// <para />InitialiseSky();
        /// <para />InitialiseAudioManager();
        /// </summary>
        public override void LoadContent()
        {
            // Call the base Method
            base.LoadContent();

            // Initialise the Viewport Manager
            InitialiseViewportManager();

            // Initialise the Sky
            InitialiseSky();
            
            // Setup GUI Items
            SetupSandboxGUIItems();

            SandboxEditMode = vxEnumSanboxEditMode.SelectItem;

            if (SandboxCurrentState == vxEnumSandboxStatus.EditMode)
                vxInput.IsCursorVisible = true;

            IsPausable = !(GameType == vxNetworkGameType.Networked);

            //Load the files if it isn't a new 
            if (FilePath != "")
            {
                LoadFile(FilePath);
            }
        }

        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();

            foreach(var camera in Cameras)
            {
                camera.OnGraphicsRefresh();
                camera.FieldOfView = vxCamera.DefaultFieldOfView * MathHelper.Pi / 180;
            }

            
            SetCameraViewports();
            ViewportManager.ResetMainViewport();


        }



        protected override void OnFirstUpdate()
        {
            base.OnFirstUpdate();

            //LeftClick();

            // The Main Treeview
            TreeControl = new vxTreeControl(Engine, new Vector2(0, 140)); ;
            //UIManager.Add(treeView);
            RootTreeNode = new vxTreeNode(Engine, TreeControl, "Root", vxInternalAssets.Textures.TreeRoot);
            TreeControl.RootNode = RootTreeNode;
            RootTreeNode.IsExpanded = true;
            //UIManager.Add(TreeControl);
            RootTreeNode.Add(new vxTreeNode(Engine, TreeControl, "Cameras"));
            ResetTree();

        }


        /// <summary>
        /// This Initalises the Physics System
        /// </summary>
        public override void InitialisePhysics()
        {
            // Next Initialise the Physics Engine
            if (Engine.MultiThreadPhysics)
            {
                //Starts BEPU with multiple Cores
                BEPUParallelLooper = new ParallelLooper();
                if (Environment.ProcessorCount > 1)
                {
                    for (int i = 0; i < Environment.ProcessorCount; i++)
                    {
                        BEPUParallelLooper.AddThread();
                    }
                }

                PhyicsSimulation = new Space(BEPUParallelLooper);
                vxConsole.WriteLine("Starting Physics Engine using " + PhyicsSimulation.ParallelLooper.ThreadCount + " Processors");
            }
            else
            {
                // Start BEPU without Multi Threading
                PhyicsSimulation = new Space();
                vxConsole.WriteLine("Starting Physics Engine using 1 Processor");
            }
            PhyicsSimulation.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
            vxConsole.WriteLine("        Allow Multithreading: " + PhyicsSimulation.ForceUpdater.AllowMultithreading);
            vxConsole.WriteLine("        Iteration Limit: " + PhyicsSimulation.Solver.IterationLimit);
            vxConsole.WriteLine("        Gravity: " + PhyicsSimulation.ForceUpdater.Gravity);

            PhysicsDebugViewer = new ModelDrawer(Engine.Game);


            // an Item Increment Count to give unqiue names to items.
            IncrementalItemCount = 0;

            // Initalise the Cursor
            Gizmo = new vxGimbal(this);

            //Sets The Outof sifht Position
            OutofSight = Vector3.One * 100000;

            // Setup the Terrain Manager
            TerrainManager = new vxTerrainManager();

            

            // Now Initialise all fo the Sandbox Adding/Deleting items code.
            NewSandboxItemDialog = new vxSandboxNewItemDialog( this);

            WorkingPlane = new vxWorkingPlane(this, vxInternalAssets.Models.UnitPlane, new Vector3(0, 0, 0));
            ConnectedMatrix = Matrix.Identity;
            CurrentlySelectedKey = "";


        }

        
        /// <summary>
        /// Initialises the cameras for this Scene. The number of Cameras used is based on the number of Players
        /// specified in the optional constructor argument. If you want multiply players but only one Camera, then overload this 
        /// method.
        /// </summary>
        public override void InitialiseCameras()
        {
            // Setup Cameras
            for (int i = 0; i < NumberOfPlayers; i++)
            {
                Cameras.Add(new vxCamera3D(this, CameraType.Orbit, new Vector3(0, 15, 0)));
            }
            SetCameraViewports();
        }

        /// <summary>
        /// Sets the Camera Viewports
        /// </summary>
        public void SetCameraViewports()
        {
            for (int i = 0; i < NumberOfPlayers; i++)
            {
                Viewport thisCamerasViewport = Engine.GraphicsDevice.Viewport;

                switch (NumberOfPlayers)
                {
                    case 1:
                        // Do nothing
                        break;

                    case 2:
                        thisCamerasViewport = new Viewport(0, thisCamerasViewport.Height / 2 * i,
                                                           thisCamerasViewport.Width, thisCamerasViewport.Height / 2);
                        break;


                    case 3:
                    case 4:
                        int j = ((i) % 2);
                        thisCamerasViewport = new Viewport(thisCamerasViewport.Width / 2 * (i % 2), thisCamerasViewport.Height / 2 * (i / 2),
                                                           thisCamerasViewport.Width / 2, thisCamerasViewport.Height / 2);
                        break;
                }

                Cameras[i].Viewport = thisCamerasViewport;
            }
        }


        /// <summary>
        /// Initialises the Viewport Manager.
        /// </summary>
		public virtual void InitialiseViewportManager()
        {
            ViewportManager = new vxViewportManager(this);
        }

        /// <summary>
        /// Initialises the SkyBoxes and the Sun Entity
        /// </summary>
        public virtual void InitialiseSky()
        {
            //Setup Sun
            SunEmitter = new vxSunEntity(this);
            SkyBox = OnSkyboxInit();
        }

        /// <summary>
        /// This returns the skybox. Override this to provide your own skybox
        /// </summary>
        /// <returns></returns>
        protected virtual vxSkyBoxEntity OnSkyboxInit()
        {
            return new vxSkyBoxEntity(this);
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            vxGameObject.NameRegister.Clear();


            for (int i = 0; i < Entities.Count; i++)
                Entities[i] = null;

            Entities.Clear();

            SceneContent.Unload();

            CanRemoveCompletely = true;

            foreach (var camera in Cameras)
                camera.Dispose();

            base.UnloadContent();
        }



        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the vxGameBaseScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public sealed override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                            bool coveredByOtherScreen)
        {
            //Update Audio Manager
            //**************************************
            for (int c = 0; c < Cameras.Count; c++)
            {
                vxAudioManager.Listener.Position = Cameras[c].Position / 10;
                vxAudioManager.Listener.Forward = Cameras[c].View.Forward;
                vxAudioManager.Listener.Up = Cameras[c].View.Up;
                vxAudioManager.Listener.Velocity = ((vxCamera3D)Cameras[c]).Velocity;//.View.Forward;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);


            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen && IsPausable)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive || IsPausable == false)
            {

                // Update Debug Code
                UpdateDebug();



                // Update Physics
                //**********************************************************************************************
                // Start measuring time for "Physics".
                //Engine.PhysicsDebugTimer.Start();

                //Update the Physics System.
                //BEPUPhyicsSpace.Update(ElapsedTime);
                PhyicsSimulation.Update();

                vxConsole.WriteInGameDebug(this, "ElapsedTime: " + vxTime.ElapsedTime);

                // Stop measuring time for "Physics".
                //Engine.PhysicsDebugTimer.Stop();


                // Update the Scene
                //**********************************************************************************************
                UpdateScene(gameTime, otherScreenHasFocus, false);



                // Update Scene Entities
                //**********************************************************************************************
                for (int i = 0; i < Entities.Count; i++)
                {

                    Entities[i].Update(gameTime);

                    if (Entities[i].KeepUpdating == false)
                        Entities.RemoveAt(i);
                }

                // Update Particle System
                //**********************************************************************************************
                //ParticleSystem.Update(gameTime);



                //Engine.Renderer.SetLightPosition(-LightPositions);

                // Tell the lensflare component where our camera is positioned.
                //lensFlare.LightDir = SunEmitter.LightDirection;
                //lensFlare.View = Camera.View;
                //lensFlare.Projection = Camera.Projection;
            }
            else
                vxInput.IsCursorVisible = true;
        }


        /// <summary>
        /// Main Game Update Loop which is accessed outside of the vxEngine
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus,
                                          bool coveredByOtherScreen)
        {
            //ParentEntityPlaceHolder = null;

            TerrainManager.Update();

            if (IsActive)
            {
                // What to update if it's in Edit mode
                if (SandboxCurrentState == vxEnumSandboxStatus.EditMode)
                {
                    HoveredSnapBoxWorld = Matrix.Identity;
                    HoveredSnapBox = null;
                    //Reset to 'MouseOver' each loop
                    IsMouseOverSnapBox = false;
                    AddMode = vxEnumAddMode.OnPlane;

                    // Edit mode is always in one viewport mode.
                    MouseRay = vxGeometryHelper.CalculateCursorRay(Engine, Cameras[0].Projection, Cameras[0].View);

                    Gizmo.Update(gameTime, MouseRay);

                    if (Gizmo.IsMouseHovering == false && IsRaySelectionEnabled)
                        HandleMouseRay(MouseRay);


                    //If Index still equals -1, then it isn't over any elements, and a new element can be added.
                    PreviousIntersection = Intersection;

                    if (SandboxCurrentState == vxEnumSandboxStatus.EditMode)
                    {
                        if (MouseRay.Intersects(WorkingPlane.WrknPlane) != null)
                        {
                            Intersection = (float)MouseRay.Intersects(WorkingPlane.WrknPlane) * MouseRay.Direction + MouseRay.Position;
                        }

                        if (TempPart != null)
                        {
                            if (AddMode == vxEnumAddMode.OnSurface && TempPart.CanBePlacedOnSurface == true)
                                TempPart.WorldTransform = HoveredSnapBoxWorld;
                            else if (IsMouseOverSnapBox)
                                TempPart.WorldTransform = HoveredSnapBoxWorld;
                            else
                            {
                                TempPart.Position = IsGridSnap ? Intersection.ToIntValue() : Intersection;
                                TempPart.OnSetTransform();
                            }
                        }
                    }
                    else
                    {
                        //Get it WAYYYY out of the scene
                        Intersection = OutofSight;
                    }
                }
            }

            IsGUIVisible = (SandboxCurrentState == vxEnumSandboxStatus.EditMode);

        }



        #endregion
    }
}