
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
//Virtex vxEngine Declaration
using VerticesEngine;
using VerticesEngine.Controllers;

using VerticesEngine.Util;
using VerticesEngine.Graphics;
using VerticesEngine.Serilization;
using VerticesEngine.UI;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.Utilities;

namespace VerticesEngine
{

    public enum vxEnumCameraEditMode
    {
        Fly,
        Orbit
    }

    public partial class vxGameplayScene3D
    {



        /// <summary>
        /// Current Key being used too add new entities.
        /// </summary>
        public string CurrentlySelectedKey = "";

        public vxEnumAddMode AddMode = vxEnumAddMode.OnPlane;

        /// <summary>
        /// Gets or sets the camera edit mode.
        /// </summary>
        /// <value>The camera edit mode.</value>
        vxEnumCameraEditMode CameraEditMode 
        {
            get { return _cameraEditMode; }
            set{
                _cameraEditMode = value;
                switch (_cameraEditMode)
                {
                    case vxEnumCameraEditMode.Orbit:
                        foreach(var camera in Cameras)
                            camera.CameraType = CameraType.Orbit;
                        break;
                    case vxEnumCameraEditMode.Fly:
                        foreach (var camera in Cameras)
                            camera.CameraType = CameraType.Freeroam;
                        break;
                }
            }
        }
        vxEnumCameraEditMode _cameraEditMode = vxEnumCameraEditMode.Fly;

        /// <summary>
        /// What Time is it Mr. Wolf.
        /// </summary>
        public TimeOfDay TimeOfDay
        {
            get { return SandBoxFile.Enviroment.TimeOfDay; }
            set { SandBoxFile.Enviroment.TimeOfDay = value; }
        }

        /// <summary>
        /// Gets or sets the sandbox edit mode.
        /// </summary>
        /// <value>The sandbox edit mode.</value>
        public vxEnumSanboxEditMode SandboxEditMode
        {
            get { return _sandboxEditMode; }
            set { 
                _sandboxEditMode = value; 

                if(_sandboxEditMode != vxEnumSanboxEditMode.AddItem)
                    DisposeOfTempPart();
            }
        }
        vxEnumSanboxEditMode _sandboxEditMode = vxEnumSanboxEditMode.SelectItem;
                

        /// <summary>
        /// Top Toolbar which holds all of the New File, Open File, Start Simulation etc... buttons.
        /// </summary>
        //public vxToolbar MainToolbar;


        //public vxToolbar TerrainEditorToolbar;


        /// <summary>
        /// Player
        /// </summary>
        public CharacterControllerInput character;

        /// <summary>
        /// File Format
        /// </summary>
        public new vxSerializableScene3DData SandBoxFile
        {
            get { return (vxSerializableScene3DData)base.SandBoxFile; }
        }

        /// <summary>
        /// List of Current Selected Items
        /// </summary>
        public List<vxEntity3D> SelectedItems
        {
            get { return _selectedItems; }
        }


        private List<vxEntity3D> _selectedItems;

        /// <summary>
        /// The Currently Selected Type of Entity to be added in the Sandbox
        /// </summary>
        public vxEntity3D TempPart;


        /// <summary>
        /// The Selected Index, x > 0 means the index of the selected item, -1 means nothing is select, -2 means a snapped object
        /// </summary>
        public int SelectedIndex;

        /// <summary>
        /// A variable which only incrememnts to always give unique ID's
        /// </summary>
        public int IncrementalItemCount;


        /// <summary>
        /// The Matrix of the Snapped Object
        /// </summary>
        public Matrix ConnectedMatrix;

        /// <summary>
        /// Previous Working Plane Intersection Point
        /// </summary>
        public Vector3 PreviousIntersection = new Vector3();

        /// <summary>
        /// "Out Of Sight" Position
        /// </summary>
        public Vector3 OutofSight = new Vector3();

        /// <summary>
        /// Should Ray Selection Handling be enabled.
        /// </summary>
        public bool IsRaySelectionEnabled = false;


        /// <summary>
        /// The boolean for if the mouse is over snap box or not for entity placement..
        /// </summary>
        public bool IsMouseOverSnapBox = false;

        /// <summary>
        /// The is grid snap.
        /// </summary>
        public bool IsGridSnap = true;


        public Vector3 Intersection;

        protected bool IsSandbox = true;

        public vxSandboxNewItemDialog NewSandboxItemDialog;

        #region -- Sandbox Editor Entities --

        /// <summary>
        /// The entity editing gimbal which controls selected entity translation and rotation.
        /// </summary>
        public vxGizmo Gizmo
        {
            get { return _gizmo; }
        }
        private vxGizmo _gizmo;

        /// <summary>
        /// Working Plane
        /// </summary>
        public vxWorkingPlane WorkingPlane
        {
            get { return _workingPlane; }
        }
        private vxWorkingPlane _workingPlane;

        #endregion


        #region -- UI Elemnts --


        #region - Ribbon Control -

        vxRibbonToolbarButtonControl undoBtn;
        vxRibbonToolbarButtonControl redoBtn;
        vxRibbonToolbarButtonControl selcMode, tlbrNewEntity;

        #endregion


        #region - Context Menu -

        protected vxContextMenuControl ContextMenu;
        protected vxContextMenuItem CntxtMenuCameraToggle;
        protected vxContextMenuItem CntxtMenuViewProperties;

        #endregion


        #region - Property Controls -

        /// <summary>
        /// Main Tab Control Which Holds All Properties
        /// </summary>
        public vxSlideTabControl PropertiesTabControl;


        /// <summary>
        /// The vxScrollPanel control which is used too store Entity Properties. See the GetProperties Method for examples.
        /// </summary>
        public vxPropertiesControl EntityPropertiesControl;

        private vxPropertiesControl WorlPropertiesControl;

        private vxPropertiesControl EffectPropertiesControl;
        #endregion


        #region - Tree Controls -

        private vxTreeControl TreeControl;
        private vxTreeNode RootTreeNode;

        #endregion


        #endregion

        /****************************************************************************/
        /*                               EVENTS
        /****************************************************************************/
        /// <summary>
        /// The Event Fired when a New Item is Selected
        /// </summary>
        public event EventHandler<EventArgs> ItemSelected;

        /// <summary>
        /// The Height Map Terrain Manager.
        /// </summary>
        public vxTerrainManager TerrainManager;
        
        public vxSnapBox HoveredSnapBox;

        public Matrix HoveredSnapBoxWorld;
        
        private Ray MouseRay;


        public vxEnumTerrainEditMode TerrainEditState = vxEnumTerrainEditMode.Sculpt;


        public void ResetTree()
        {
            if (RootTreeNode != null)
            {
                RootTreeNode.Clear();

                vxTreeNode CameraTreeNode = new vxTreeNode(Engine, TreeControl, "Cameras", vxInternalAssets.Textures.TreeRoot);
                RootTreeNode.Add(CameraTreeNode);

                foreach (vxCamera3D camera in Cameras)
                {
                    CameraTreeNode.Add(new vxTreeNode(Engine, TreeControl, "Camera", vxInternalAssets.Textures.TreeRoot));
                }

                vxTreeNode EffectsTreeNode = new vxTreeNode(Engine, TreeControl, "Effects", vxInternalAssets.Textures.TreeRoot);
                RootTreeNode.Add(EffectsTreeNode);


                //foreach (vxPostProcessor pp in Renderer.PostProcessors)
                //{
                //    EffectsTreeNode.Add(pp.TreeNode);
                //}

                vxTreeNode EntitiesTreeNode = new vxTreeNode(Engine, TreeControl, "Entities", vxInternalAssets.Textures.TreeModel);
                RootTreeNode.Add(EntitiesTreeNode);
                EntitiesTreeNode.IsExpanded = true;
                foreach (vxGameObject item in Entities)
                {
                    item.AddToNode(TreeControl, EntitiesTreeNode);
                }
            }
        }

        public virtual bool IsSurface(vxEntity3D HoveredEntity)
        {
            if (HoveredEntity == TempPart)
            {
                return false;
            }
            else
                return true;
        }


        /// <summary>
        /// Starts the Sandbox.
        /// </summary>
        public override void SimulationStart()
        {
			//Renderer.SSLRPostProcess.DoSSR = true;

            foreach (var camera in Cameras)
                camera.ProjectionType = camera.DefaultProjectionType;

            //Clear out the Edtor Items
            EditorItems.Clear();

            foreach (vxEntity3D entity in Entities)
            {
                if (entity != null)
                {
                    EditorItems.Add(entity);
                    entity.OnSandboxStatusChanged(true);
                }
            }
        }


        /// <summary>
        /// Stops the Sandbox.
        /// </summary>
        public override void SimulationStop()
		{
			SandboxEditMode = vxEnumSanboxEditMode.SelectItem;

            foreach (var camera in Cameras)
                camera.ProjectionType = camera.EditorProjectionType;

            CurrentlySelectedKey = "";
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] != null)
                {
                    Entities[i].OnSandboxStatusChanged(false);

                    if (EditorItems.Contains(Entities[i]) == false)
                    {
                        Entities[i].Dispose();
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// This method is called near the end of the LoadContent method to load al GUI Items pertaining to the 
        /// Sandbox including the toolbars and item registration.
        /// </summary>
        public virtual void SetupSandboxGUIItems()
        {
            // Setup the Toolbars
            InitialiseRibbonControl();

            InitialiseTerrainToolbar();


            // Setup Properties Sliding Tab Control
            PropertiesTabControl = new vxSlideTabControl(Engine,
                350,
                Engine.GraphicsDevice.Viewport.Height - 140,
                new Vector2(-50, 140),
                vxGUIItemOrientation.Right);
            UIManager.Add(PropertiesTabControl);

            EntityPropertiesControl = CreatePropertiesControl("Entities");
            WorlPropertiesControl = CreatePropertiesControl("World");

            if (vxEngine.BuildType == vxBuildType.Debug)
                EffectPropertiesControl = CreatePropertiesControl("Renderer");

            // Now that all of the GUI Items have been initalised, now register all items.
            if(IsSandbox)
                RegisterSandboxEntities();  


            ContextMenu = new vxContextMenuControl(Engine);

            UIManager.Add(ContextMenu);

            ContextMenu.AddItem("Cut");
            ContextMenu.AddItem("Copy");
            ContextMenu.AddItem("Paste");
            ContextMenu.AddSplitter();
            ContextMenu.AddItem("Undo");
            ContextMenu.AddItem("Redo");
            ContextMenu.AddSplitter();
            CntxtMenuCameraToggle = new vxContextMenuItem(ContextMenu, "Orbit Selection",
                                                            vxInternalAssets.LoadInternalTexture2D("Textures/gui/icons/centroid"));
            CntxtMenuCameraToggle.Clicked += delegate
            {
                
                if (SelectedItems.Count > 0 && SelectedItems[0] != null)
                {
                    foreach (var Camera in Cameras)
                    {
                        Camera.CameraType = CameraType.Orbit;
                        Camera.CastAs<vxCamera3D>().OrbitTarget = SelectedItems[0].Position;
                        var rad = SelectedItems[0].BoundingShape.Radius;
                        Camera.CastAs<vxCamera3D>().OrbitZoom = 250 * rad;
                    }
                }

            };
            ContextMenu.AddSplitter();

            CntxtMenuViewProperties = new vxContextMenuItem(ContextMenu, "Properties",
                                                            vxInternalAssets.LoadInternalTexture2D("Textures/gui/icons/properties"));
            CntxtMenuViewProperties.Clicked += delegate
            {
                PropertiesTabControl.Pages[0].Open();
            };

            ContextMenu.Position = new Vector2(0);

        }


        /// <summary>
        /// Creates a new properties control window in the right hand properties slider control.
        /// </summary>
        /// <returns>The properties control.</returns>
        /// <param name="name">Name.</param>
        public vxPropertiesControl CreatePropertiesControl(string name)
        {
            vxSlideTabPage propertiesTabPage = new vxSlideTabPage(PropertiesTabControl, name);
            PropertiesTabControl.AddItem(propertiesTabPage);
            propertiesTabPage.Tab.Font = vxInternalAssets.Fonts.ViewerFont;

            vxPropertiesControl newPropertiesControl = new vxPropertiesControl(Engine, new Vector2(0, 24),
                                                              propertiesTabPage.Bounds.Width, 450);
            propertiesTabPage.AddItem(newPropertiesControl);

            return newPropertiesControl;

        }

        /// <summary>
        /// Creates the sandbox item tab in the main item tab control dialog.
        /// </summary>
        /// <returns>The sandbox item tab.</returns>
        /// <param name="tabname">Tabname.</param>
        public vxScrollPanel CreateSandboxItemTab(string tabname)
        {
            vxTabPageControl tabPage = new vxTabPageControl(Engine, tabname);
            NewSandboxItemDialog.TabControl.Add(tabPage);

            vxScrollPanel itemsScrollPanel = new vxScrollPanel(Engine, new Vector2(0, 0),
                                                                     tabPage.Width, tabPage.Height - tabPage.Tab.Height);
            tabPage.Add(itemsScrollPanel);

            return itemsScrollPanel;
        }

        /// <summary>
        /// This Scroll Panel holds all of the Generic Engine Items. If you want to add to this list, then you can
        /// call something similar too 'EngineItemsScrollPanel.AddItem(RegisterNewSandboxItem(vxTerrainEntity.Info));'
        /// </summary>
        public vxScrollPanel EngineItemsScrollPanel;

        /// <summary>
        /// Override this Method and add in your Register Sandbox Entities code.
        /// </summary>
        protected void RegisterSandboxEntities()
        {
            // Now use the Category/Sub Category structure to create the proper GUI laytout
            foreach (var category in vxEntityRegister.Categories.Values)
            {
                // for each category, create a new tab page
                vxTabPageControl ItemsTabPage = new vxTabPageControl(Engine, category.name);
                NewSandboxItemDialog.TabControl.Add(ItemsTabPage);

                var scrollPanel = new vxScrollPanel(Engine, Vector2.Zero, ItemsTabPage.Width, ItemsTabPage.Height - ItemsTabPage.Tab.Height);

                foreach (var subCategory in category.SubCategories.Values)
                {
                    // for each sub category, create a new section within the page
                    scrollPanel.AddItem(new vxScrollPanelSpliter(Engine, subCategory.Name));


                    foreach (var type in subCategory.types)
                    {
                        // now add each item to the proper category and sub category
                        scrollPanel.AddItem(RegisterNewSandboxItem(type));
                    }
                }
                ItemsTabPage.Add(scrollPanel);
            }
        }


        /// <summary>
        /// Shows the settings dialog for this level editor. This needs to be overriden.
        /// </summary>
        public virtual void OnShowSettingsDialog()
        {
            vxSceneManager.AddScene(new vxMessageBox("You should override the 'ShowSettingsDialog()'\n"+
                                              "method to add your own settings dialog.", "Settings"));
        }

        public virtual void OnShowHelp()
        {
            //Engine.OpenWebPage("https://github.com/VirtexEdgeDesign/VerticesEngine/wiki");
        }

        public virtual void OnShowAbout()
        {
            //Engine.OpenWebPage("https://virtexedgedesign.com/");
        }

        void UpdateViewTabItemState()
        {
            cameraTypeDropDown.Text = Cameras[0].CameraType.ToString();
            cameraProjTypeDropDown.Text = Cameras[0].ProjectionType.ToString();
        }
        #region Main Ribbon Bar
        vxRibbonControl RibbonControl;
        vxRibbonButtonControl gizmoGlobalTransl;
        vxRibbonButtonControl gizmoLocalTransl;
        vxRibbonDropdownControl cameraTypeDropDown;
        vxRibbonDropdownControl cameraProjTypeDropDown;

        void InitialiseRibbonControl()
        {

            RibbonControl = new vxRibbonControl(UIManager, new Vector2(0, 0));
            RibbonControl.TabStartOffset = 38;
            //            UIManager.Add(RibbonControl);

            vxRibbonTabPage HomeTabPage = new vxRibbonTabPage(RibbonControl, "Home");


            var FileOpenGroup = new vxRibbonControlGroup(HomeTabPage, "File");

            var openFile = new vxRibbonButtonControl(FileOpenGroup, "Open",
                                                     vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/open_32"), vxEnumButtonSize.Big);
            openFile.Clicked += Event_OpenFileToolbarItem_Clicked;
            var saveFile = new vxRibbonButtonControl(FileOpenGroup, "Save",
                                                     vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/save_16"));
            saveFile.Clicked += Event_SaveFileToolbarItem_Clicked;

            var saveAsFile = new vxRibbonButtonControl(FileOpenGroup, "Save As",
                                                       vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/save_as_16"));
            saveAsFile.Clicked += Event_SaveAsFileToolbarItem_Clicked;

            var importFile = new vxRibbonButtonControl(FileOpenGroup, "Import",
                                                       vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/import_16"));
            importFile.Clicked += ImportFileToolbarItem_Clicked;

            var exportFile = new vxRibbonButtonControl(FileOpenGroup, "Export",
                                                       vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/export_16"));
            exportFile.Clicked += ExportFileToolbarItem_Clicked;


            var gizmoGroup = new vxRibbonControlGroup(HomeTabPage, "Transforms");
            gizmoGlobalTransl = new vxRibbonButtonControl(gizmoGroup, "Global Translation",
                                                              vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/gimbal/transform/gimbal_global"));
            gizmoGlobalTransl.IsTogglable = true;
            gizmoGlobalTransl.ToggleState = true;
            gizmoLocalTransl = new vxRibbonButtonControl(gizmoGroup, "Local Translation",
                                                             vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/gimbal/transform/gimbal_local"));
            gizmoLocalTransl.IsTogglable = true;

            gizmoGlobalTransl.Clicked += delegate
            {
                gizmoLocalTransl.ToggleState = false;
                Gizmo.TransformationType = TransformationType.Global;
            };

            gizmoLocalTransl.Clicked += delegate
            {
                gizmoGlobalTransl.ToggleState = false;
                Gizmo.TransformationType = TransformationType.Local;
            };

            var cursorSnap = new vxRibbonButtonControl(gizmoGroup, "Grid Snap",
                                                             vxInternalAssets.LoadInternalTexture2D("Textures/gui/icons/cursor_grid"));
            
            cursorSnap.IsTogglable = true;
            cursorSnap.ToggleState = IsGridSnap;
            cursorSnap.Clicked+=delegate {
                IsGridSnap = cursorSnap.ToggleState;
            };


            var Help = new vxRibbonControlGroup(HomeTabPage, "About");
            var showSettings = new vxRibbonButtonControl(Help, "Settings",
                                                         vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/setting_32"), vxEnumButtonSize.Big);
            showSettings.Clicked += delegate
            {
                OnShowSettingsDialog();
            }; ;

            var showHelp = new vxRibbonButtonControl(Help, "Help",
                                                       vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/help_16"));
            showHelp.Clicked += delegate
            {
                OnShowHelp();
            };


            var showAbout = new vxRibbonButtonControl(Help, "About",
                                                       vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/logo_16"));

            showAbout.Clicked += delegate
            {
                OnShowAbout();
            };


            vxRibbonTabPage ViewTabPage = new vxRibbonTabPage(RibbonControl, "View");

            var cameraGroup = new vxRibbonControlGroup(ViewTabPage, "Camera");

            cameraTypeDropDown = new vxRibbonDropdownControl(cameraGroup, CameraEditMode.ToString(), "Orbit");

            foreach (var cameraType in vxUtil.GetValues<vxEnumCameraEditMode>())
            {
                cameraTypeDropDown.AddItem(cameraType.ToString());
            }

            cameraTypeDropDown.SelectionChanged += delegate {
                CameraEditMode = (vxEnumCameraEditMode)cameraTypeDropDown.SelectedIndex;
                UpdateViewTabItemState();
            };
              
            

            cameraProjTypeDropDown = new vxRibbonDropdownControl(cameraGroup, Cameras[0].ProjectionType.ToString(),"");

            foreach (var cameraProjType in vxUtil.GetValues<vxCameraProjectionType>())
            {
                cameraProjTypeDropDown.AddItem(cameraProjType.ToString());
            }

            cameraProjTypeDropDown.SelectionChanged += delegate
            {
                Cameras[0].ProjectionType = (vxCameraProjectionType)cameraProjTypeDropDown.SelectedIndex;
                UpdateViewTabItemState();
            };


            vxRibbonTabPage EntitiesTabPage = new vxRibbonTabPage(RibbonControl, "Entities");

            var EntitiesGroup = new vxRibbonControlGroup(EntitiesTabPage, "Entities");

            var addItemFile = new vxRibbonButtonControl(EntitiesGroup, "New Entity",
                                                        vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/entities/entity_add_32"), vxEnumButtonSize.Big);
            addItemFile.Clicked += AddToolbarItem_Clicked;
            var addTerrainEntity = new vxRibbonButtonControl(EntitiesGroup, "New Terrain",
                                                             vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/entities/terrain_add_16"));
            addTerrainEntity.Clicked += delegate
            {
                //OnNewItemAdded(vxTerrainEntity.Info.Key);
            };

            var addWaterEntity = new vxRibbonButtonControl(EntitiesGroup, "New Water",
                                                           vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/entities/water_add_16"));
            addWaterEntity.Clicked += delegate
            {
                
                //OnNewItemAdded(vxWaterEntity.Info.Key);
                OnNewItemAdded(typeof(vxWaterEntity).ToString());
            };


            //RegisterNewSandboxItem(new vxSandboxEntityRegistrationInfo(typeof(vxWaterEntity), typeof(vxWaterEntity).ToString(), "Water Entity", ""));




            var entityEditGroup = new vxRibbonControlGroup(EntitiesTabPage, "Edit Entities");
            var terrainEditStart = new vxRibbonButtonControl(entityEditGroup, "Terrain Editor",
                                                           vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/terrain_edit"), vxEnumButtonSize.Big);

            vxRibbonTabPage ToolsTabPage = new vxRibbonTabPage(RibbonControl, "Tools");
            var debugTools = new vxRibbonControlGroup(ToolsTabPage, "Debug Tools");
            //var fpsToggleBtn = new vxRibbonButtonControl(debugTools, "FPS Counter",
            //                                               vxInternalAssets.LoadInternalTexture2D("Textures/gui/icons/debug_fps"));
            //fpsToggleBtn.IsTogglable = true;
            //fpsToggleBtn.Clicked += delegate {
            //    Engine.DebugSystem.FpsCounter.IsVisible = !Engine.DebugSystem.FpsCounter.IsVisible;
            //    fpsToggleBtn.ToggleState = Engine.DebugSystem.FpsCounter.IsVisible;
            //};

            //var timeRulerToggleBtn = new vxRibbonButtonControl(debugTools, "Time Ruler",
            //                                                   vxInternalAssets.LoadInternalTexture2D("Textures/gui/icons/debug_graph"));
            //timeRulerToggleBtn.IsTogglable = true;
            //timeRulerToggleBtn.Clicked += delegate {
            //    Engine.DebugSystem.DebugTimer.IsVisible = !Engine.DebugSystem.DebugTimer.IsVisible;
            //    timeRulerToggleBtn.ToggleState = Engine.DebugSystem.DebugTimer.IsVisible;
            //};

            var consoleToggleBtn = new vxRibbonButtonControl(debugTools, "Pause on Console",
                                                               vxInternalAssets.LoadInternalTexture2D("Textures/gui/icons/debug_console"));
            consoleToggleBtn.IsTogglable = true;
            consoleToggleBtn.Clicked += delegate {
                //Engine.DebugSystem.DebugTimer.IsVisible = !Engine.DebugSystem.DebugTimer.IsVisible;
                //timeRulerToggleBtn.ToggleState = Engine.DebugSystem.DebugTimer.IsVisible;
            };


            RibbonControl.Add(HomeTabPage);
            RibbonControl.Add(ViewTabPage);
            RibbonControl.Add(EntitiesTabPage);
            RibbonControl.Add(ToolsTabPage);


            //var waterTabPage = new vxRibbonContextualTabPage(RibbonControl, "Water", "Edit", Color.DeepSkyBlue);

            //var watcameraGroup = new vxRibbonControlGroup(waterTabPage, "Water");
            //RibbonControl.Add(waterTabPage);
            //RibbonControl.Remove(waterTabPage);

            InitialiseTerrainToolbar();

            //RibbonControl.Remove(terrainTabPage);
            terrainEditStart.Clicked += delegate
            {
                SandboxEditMode = vxEnumSanboxEditMode.TerrainEdit;
                RibbonControl.AddContextTab(terrainTabPage);
                terrainTabPage.SelectTab();
            };



            HomeTabPage.SelectTab();



            // TOP TOOL BAR

            var fileNew = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/new_16"));
            fileNew.Clicked += Event_NewFileToolbarItem_Clicked;
            var fileOpen = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/open_16"));
            fileOpen.Clicked += Event_OpenFileToolbarItem_Clicked;

            var fileSave = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/save_16"));
            fileSave.Clicked += Event_SaveFileToolbarItem_Clicked;
            var fileSaveAs = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/save_as_16"));
            fileSaveAs.Clicked += Event_SaveAsFileToolbarItem_Clicked;

            new vxRibbonToolbarSplitterControl(RibbonControl);
            undoBtn = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/undo_16"));
            undoBtn.Clicked += UndoToolbarItem_Clicked;
            undoBtn.SetToolTip("Undo the previous command.");

            redoBtn = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/redo_16"));
            redoBtn.Clicked += RedoToolbarItem_Clicked;
            redoBtn.SetToolTip("Redo the previous command.");

            //new vxRibbonToolbarSplitterControl(RibbonControl);
            //var tlbrTestLevel = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/test_16"));
            //tlbrTestLevel.Clicked += RunGameToolbarItem_Clicked;
            RibbonControl.TitleButton.ButtonImage = vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/test_32");
            RibbonControl.TitleButton.Clicked += RunGameToolbarItem_Clicked;

            new vxRibbonToolbarSplitterControl(RibbonControl);
            tlbrNewEntity = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/entities/entity_add_16"));
            tlbrNewEntity.IsTogglable = true;

            selcMode = new vxRibbonToolbarButtonControl(RibbonControl, vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/main/select"));
            selcMode.IsTogglable = true;

            selcMode.ToggleState = true;
            SandboxEditMode = vxEnumSanboxEditMode.SelectItem;

            tlbrNewEntity.Clicked += delegate
            {
                tlbrNewEntity.ToggleState = true;
                selcMode.ToggleState = false;
                SandboxEditMode = vxEnumSanboxEditMode.AddItem;
                vxSceneManager.AddScene(NewSandboxItemDialog);
            };

            selcMode.Clicked += delegate
            {
                tlbrNewEntity.ToggleState = false;
                selcMode.ToggleState = true;
                SandboxEditMode = vxEnumSanboxEditMode.SelectItem;
                DisposeOfTempPart();
            }; ;



            //SetUndoRedoButtonStatus();
            CommandManager.OnChange += CommandManager_OnChange;

            undoBtn.IsEnabled = CommandManager.CanUndo;
            redoBtn.IsEnabled = CommandManager.CanRedo;
        }

        void CommandManager_OnChange(object sender, EventArgs e)
        {
            undoBtn.IsEnabled = CommandManager.CanUndo;
            redoBtn.IsEnabled = CommandManager.CanRedo;
        }

        void UndoToolbarItem_Clicked(object sender, UI.Events.vxGuiItemClickEventArgs e)
        {
            CommandManager.Undo();
        }

        void RedoToolbarItem_Clicked(object sender, UI.Events.vxGuiItemClickEventArgs e)
        {
            CommandManager.ReDo();
        }


        /// <summary>
        /// Event Fired too test the Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RunGameToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SimulationStart();
        }

        /// <summary>
        /// Event Fired too stop the test of the Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StopGameToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SimulationStop();
        }


        private void AddToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            vxSceneManager.AddScene(NewSandboxItemDialog);
        }

        #endregion

    }
}