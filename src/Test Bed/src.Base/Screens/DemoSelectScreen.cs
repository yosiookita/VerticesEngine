#region File Description
//-----------------------------------------------------------------------------
// MainvxMenuBaseScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Menus;
using VerticesEngine.Input.Events;
using VerticesEngine.Screens.Async;
//using VerticesEngine.UI.Menus;
#endregion

namespace Virtex.App.VerticesTechDemo
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class DemoSelectScreen : vxMenuBaseScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public DemoSelectScreen()
            : base("Select Demo")
        {
            
        }

        public override void LoadContent()
        {
            base.LoadContent();
			// Create our menu entries.
			vxMenuEntry introLevelMenuEntry = new vxMenuEntry(this, "Intro Level");
			vxMenuEntry sandboxMenuEntry = new vxMenuEntry(this, "Basic Sandbox");
			vxMenuEntry spoznzaMenuEntry = new vxMenuEntry(this, "Sponza Lighting");
            vxMenuEntry fpsMenuEntry = new vxMenuEntry(this, "First Person Village");
			vxMenuEntry terrainMenuEntry = new vxMenuEntry(this, "Height Map Terrain");
			vxMenuEntry lightingMenuEntry = new vxMenuEntry(this, "Shader Demo");
            vxMenuEntry exitMenuEntry = new vxMenuEntry(this, "Back");

			// Hook up menu event handlers.
			introLevelMenuEntry.Selected +=	introLevelMenuEntrySelected;
			sandboxMenuEntry.Selected += SandboxMenuEntry_Selected;
            spoznzaMenuEntry.Selected += spoznzaMenuEntrySelected;
            fpsMenuEntry.Selected += fpsMenuEntrySelected;
			terrainMenuEntry.Selected += TerrainMenuEntry_Selected;
            exitMenuEntry.Selected += OnCancel;

			// Add entries to the menu.
			MenuEntries.Add(introLevelMenuEntry);
			MenuEntries.Add(sandboxMenuEntry);
            MenuEntries.Add(spoznzaMenuEntry);
            MenuEntries.Add(fpsMenuEntry);
			MenuEntries.Add(terrainMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion

		#region Handle Input

		/// <summary>
		/// Event handler for when the Play Game menu entry is selected.
		/// </summary>
		void introLevelMenuEntrySelected(object sender, PlayerIndexEventArgs e)
		{
			vxLoadingScreen.Load(Engine, true, e.PlayerIndex,
				new IntroBackground());
		}

		void SandboxMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
		{
            vxLoadingScreen.Load(Engine, true, e.PlayerIndex,
                 new BasicSandboxTestLevel());
		}

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void spoznzaMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            vxLoadingScreen.Load(Engine, true, e.PlayerIndex,
                 new SponzaLightingTestLevel());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void fpsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            vxLoadingScreen.Load(Engine, true, e.PlayerIndex,
                               new FPSVillageTestBed());
        }

		void TerrainMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
		{
            vxLoadingScreen.Load(Engine, true, e.PlayerIndex,
                               new TechDemoTerrainSampleLevel());
		}

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        public override void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        #endregion
    }
}
