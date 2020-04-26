#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Input.Events;
using VerticesEngine;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.Profile;
using System;


#endregion

namespace VerticesEngine.UI.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class vxProfileMenuScreen : vxMenuBaseScreen
    {
        #region Initialization

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <value>The profile.</value>
        vxIPlayerProfile Profile
        {
            get { return Engine.PlayerProfile; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxProfileMenuScreen()
            : base("Profile")
        {

        }
        vxMenuEntry signOutMenuEntry;
        vxMenuEntry ViewLeaderboardsMenuEntry;
        vxMenuEntry ViewAchievementsMenuEntry;
        public override void LoadContent()
        {
            base.LoadContent();

            ViewLeaderboardsMenuEntry = new vxMenuEntry(this, vxLocalisationKey.ViewLeaderboards);
            ViewAchievementsMenuEntry = new vxMenuEntry(this, vxLocalisationKey.ViewAchievements);
            signOutMenuEntry = new vxMenuEntry(this, vxLocalisationKey.SignIn);

            if (Engine.PlayerProfile.IsSignedIn)
                signOutMenuEntry.SetLocalisedText(vxLocalisationKey.SignOut);

            var backMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Back);

            // View Leaderboards
            ViewLeaderboardsMenuEntry.Selected += delegate
            {
                Console.WriteLine("ViewLeaderboardsMenuEntry");
                Engine.PlayerProfile.ViewAllLeaderboards();
            };

            // View Achievements
            ViewAchievementsMenuEntry.Selected += delegate
            {
                Engine.PlayerProfile.ViewAchievments();
            };

            signOutMenuEntry.Selected += delegate
            {

                if (Engine.PlayerProfile.IsSignedIn)
                    Engine.PlayerProfile.SignOut();
                else
                    Engine.PlayerProfile.SignIn();
            };

            backMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(backMenuEntry_Selected);


            // Add entries to the menu.
            MenuEntries.Add(ViewAchievementsMenuEntry);
            MenuEntries.Add(ViewLeaderboardsMenuEntry);
#if !__IOS__
            MenuEntries.Add(signOutMenuEntry);
#endif
			MenuEntries.Add(backMenuEntry);
        }


        void backMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

#endregion

        public override void Draw(GameTime gameTime)
        {

            if (Engine.PlayerProfile.IsSignedIn)
            {
                signOutMenuEntry.Text = "Sign Out";
                ViewAchievementsMenuEntry.IsEnabled = true;
                ViewLeaderboardsMenuEntry.IsEnabled = true;
            }
            else
            {
                signOutMenuEntry.Text = "Sign In";
                ViewAchievementsMenuEntry.IsEnabled = false;
                ViewLeaderboardsMenuEntry.IsEnabled = false;
            }

            base.Draw(gameTime);
        }

    }
}
