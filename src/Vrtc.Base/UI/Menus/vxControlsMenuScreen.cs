#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Input.Events;
using VerticesEngine;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Dialogs;


#endregion

namespace VerticesEngine.UI.Menus
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class vxControlsMenuScreen : vxMenuBaseScreen
    {
        #region Fields

//        vxSliderMenuEntry MouseInvertedMenuEntry;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxControlsMenuScreen()
            : base(vxLocalisationKey.Controls)
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();

            var keyboardMenuEntry = new vxMenuEntry(this, vxLocalisationKey.KeyboardSettings);
			var gamepadMenuEntry = new vxMenuEntry(this, vxLocalisationKey.GamepadSettings);
			var mouseMenuEntry = new vxMenuEntry(this, vxLocalisationKey.MouseSettings);

			var backMenuEntry = new vxMenuEntry(this, vxLocalisationKey.Back);

			//Accept and Cancel
			keyboardMenuEntry.Selected += delegate {
				vxSceneManager.AddScene(new vxKeyboardSettingsDialog(), PlayerIndex.One);	
			};
			backMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(backMenuEntry_Selected);


			// Add entries to the menu.
			MenuEntries.Add(keyboardMenuEntry);
			MenuEntries.Add(gamepadMenuEntry);
			MenuEntries.Add(mouseMenuEntry);
			MenuEntries.Add(backMenuEntry);
        }


        void backMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //Rectangle rec = new Rectangle(Engine.GraphicsDevice.Viewport.Width/2 - Engine.Texture_Control.Width/2, 
            //    Engine.GraphicsDevice.Viewport.Height/2 - Engine.Texture_Control.Height/2,
            //    Engine.Texture_Control.Width, Engine.Texture_Control.Height);

            //Engine.SpriteBatch.Begin();
            //Engine.SpriteBatch.Draw(Engine.Texture_Control, rec, Color.White * TransitionAlpha);
            //Engine.SpriteBatch.End();
        }

    }
}
