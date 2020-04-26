#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Input.Events;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.Localization;
using VerticesEngine;


#endregion

namespace VerticesEngine.UI.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    public class vxSettingsMenuScreen : vxMenuBaseScreen
    {
        #region Initialization

        vxMenuEntry ControlsMenuEntry;
        vxMenuEntry LocalizationMenuEntry;
        vxMenuEntry ProfileMenuEntry;
        vxMenuEntry GraphicsMenuEntry;
        vxMenuEntry AudioMenuEntry;
        vxMenuEntry CreditsMenuEntry;
        vxMenuEntry displayDebugHUDMenuEntry;

        vxMenuEntry cancelMenuEntry;
        public vxSettingsMenuScreen()
            : base(vxLocalisationKey.Settings)
        {

        }



        public override void LoadContent()
        {
            base.LoadContent();

            // Create our menu entries.
            ProfileMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Profile);
            ControlsMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Settings);
            LocalizationMenuEntry = new vxMenuEntry(this,vxLocalisationKey.Localisation);
            GraphicsMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Graphics);
            AudioMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Audio);
            CreditsMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Credits);
            displayDebugHUDMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Debug);

            cancelMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Back);

            // Hook up menu event handlers.
            ProfileMenuEntry.Selected += ProfileMenuEntry_Selected;
            ControlsMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(ControlsMenuEntry_Selected);
            GraphicsMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(GraphicsMenuEntry_Selected);
            LocalizationMenuEntry.Selected += LocalizationMenuEntry_Selected;
            AudioMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(AudioMenuEntry_Selected);
            displayDebugHUDMenuEntry.Selected += DisplayDebugHUDMenuEntry_Selected;
            CreditsMenuEntry.Selected += CreditsMenuEntry_Selected;
            //Back
            cancelMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(cancelMenuEntry_Selected);


            // Add entries to the menu.
            //if (Engine.Game.Config.HasProfileSupport)
              //  MenuEntries.Add(ProfileMenuEntry);

            if (Engine.Game.Config.HasControlsSettings)
                MenuEntries.Add(ControlsMenuEntry);

            if (Engine.Game.Config.HasLanguageSettings)
                MenuEntries.Add(LocalizationMenuEntry);

            if (Engine.Game.Config.HasGraphicsSettings)
                MenuEntries.Add(GraphicsMenuEntry);

            if (Engine.Game.Config.HasAudioSettings)
                MenuEntries.Add(AudioMenuEntry);

            if (vxEngine.BuildType == vxBuildType.Debug)
                MenuEntries.Add(displayDebugHUDMenuEntry);

            MenuEntries.Add(CreditsMenuEntry);

            // Add any extra settings the player wants
            Engine.Game.AddSettingsScreens(this);

            MenuEntries.Add(cancelMenuEntry);
        }

        void ProfileMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxSceneManager.AddScene(new vxProfileMenuScreen(), e.PlayerIndex);
        }

        void ControlsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxSceneManager.AddScene(new vxControlsMenuScreen(), e.PlayerIndex);
        }

        void GraphicsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxSceneManager.AddScene(new vxGraphicSettingsDialog(), e.PlayerIndex);
        }

        private void LocalizationMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            //vxSceneManager.AddScreen(new vxLocalizationDialog(), e.PlayerIndex);
            vxSceneManager.AddScene(new vxLocalizationMenuScreen(false), e.PlayerIndex);
        }

        void AudioMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxSceneManager.AddScene(new vxAudioMenuScreen(), e.PlayerIndex);
        }

        private void DisplayDebugHUDMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            vxSceneManager.AddScene(new vxDebugMenuScreen(), e.PlayerIndex);
        }

        void CreditsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            Engine.Game.OnShowCreditsPage();
        }

        void cancelMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

        #endregion
    }
}
