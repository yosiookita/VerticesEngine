#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Input.Events;
using VerticesEngine;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Dialogs.Utilities;
using VerticesEngine.Diagnostics;


#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
#endif

#endregion

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    public class vxDebugMenuScreen : vxSettingsBaseDialog
    {
        #region Fields
        /*
        vxMenuEntry displayDebugHUDMenuEntry;
		vxMenuEntry displayDebugRenderTargets;
		vxMenuEntry displayDebugInformation;
		vxMenuEntry displayDebugFrameCounter;
		vxMenuEntry displayDebugInGameConsole;
		vxMenuEntry textureGenMenuEntry;
        vxMenuEntry farseerExpMenuEntry;
        static bool dispDebugHUD = true;
        public static bool UseLibEXP = false;
        */
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxDebugMenuScreen() : base("Debug", typeof(vxDebugSettingAttribute))
        {
           
        }
        protected override void OnSettingsRetreival()
        {
            // add all tools
            foreach(var tool in vxDebug.DebugTools)
            {
                var settingsItem = new vxSettingsGUIItem(Engine, InternalGUIManager, tool.DebugToolName,  (tool.IsVisible ? "On" : "Off"));

                settingsItem.AddOption("On");
                settingsItem.AddOption("Off");
                
                settingsItem.ValueChangedEvent += delegate
                {
                    tool.IsVisible = !tool.IsVisible;
                    settingsItem.Text = tool.DebugToolName + " - " + (tool.IsVisible ? "Yes" : "No");
                };

                ScrollPanel.AddItem(settingsItem);
            }


            base.OnSettingsRetreival();

        }

        #endregion

        void displayDebugHUDMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {

#if __ANDROID__
            
            /*
            InputMethodManager imm = (InputMethodManager)Engine.Activity.GetSystemService(Activity.INPUT_METHOD_SERVICE);
            //Find the currently focused view, so we can grab the correct window token from it.
            View view = activity.getCurrentFocus();
            //If no view currently has focus, create a new one, just so we can grab a window token from it
            if (view == null)
            {
                view = new View(activity);
            }
            imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
            */
            var pView = Engine.Game.Services.GetService<View>();
            var inputMethodManager = Engine.Activity.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(pView, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
#endif

        }
        
    }
}
