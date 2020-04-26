#region File Description
//-----------------------------------------------------------------------------
// MainvxMenuBaseScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using VerticesEngine.Net;

#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using VerticesEngine.UI.Menus;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.UI.Controls;
using VerticesEngine.Input.Events;
using VerticesEngine;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.Screens.Async;
using VerticesEngine.Screens;
#endregion

namespace Virtex.App.VerticesTechDemo
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainvxMenuBaseScreen : vxMenuBaseScreen
    {
        #region Initialization
        
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainvxMenuBaseScreen()
            : base("Vertice Engine - Tech Demo")
        {
            
        }

        public override void LoadContent()
        {
            base.LoadContent();

            // Create our menu entries.
            vxMenuEntry playGameMenuEntry = new vxMenuEntry(this, "Play");
            vxMenuEntry sandboxGameMenuEntry = new vxMenuEntry(this, "Sandbox");
            vxMenuEntry modelViewGameMenuEntry = new vxMenuEntry(this, "Model Viewer");
            
            vxMenuEntry demoGameMenuEntry = new vxMenuEntry(this, "Demo Screen");
            vxMenuEntry optionsMenuEntry = new vxMenuEntry(this, "Options");
            vxMenuEntry exitMenuEntry = new vxMenuEntry(this, "Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            demoGameMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(demoGameMenuEntry_Selected);
            modelViewGameMenuEntry.Selected += ModelViewGameMenuEntry_Selected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            /*
			MenuEntries.Add(multiplayerMenuEntry);
            MenuEntries.Add(sandboxGameMenuEntry);

            */
            //MenuEntries.Add(demoGameMenuEntry);
            MenuEntries.Add(modelViewGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion

        #region Handle Input
        
        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
		{
            vxSceneManager.AddScene(new DemoSelectScreen(), PlayerIndex.One);
        }
        

        /// <summary>
        /// Eventhandler for the Demo Selection Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void demoGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxSceneManager.AddScene(new DemoSelectScreen(), e.PlayerIndex);
        }

        void ModelViewGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            //Engine.AddScreen(new ModelViewer(Engine));

            vxLoadingScreen.Load(Engine, true, e.PlayerIndex,
                                 new ModelViewer(Engine));
        }

        void TextureGenMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
		{
		}

		/// <summary>
		/// Event handler for when the Options menu entry is selected.
		/// </summary>
		void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            vxSceneManager.AddScene(new vxSettingsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        public override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to quit?";
            vxMessageBox confirmExitMessageBox = new vxMessageBox(message, "quit?");
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            vxSceneManager.AddScene(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            Engine.Game.Exit();
        }


        #endregion
    }
}
