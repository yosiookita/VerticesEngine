using Microsoft.Xna.Framework;
using VerticesEngine.Input.Events;
using VerticesEngine.Localization;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.MessageBoxs;

namespace VerticesEngine.UI.Menus
{
    /// <summary>
    /// A basic menu for choosing a small set of languages
    /// </summary>
    public class vxLocalizationMenuScreen : vxMenuBaseScreen
    {
        bool IsShownOnLaunch = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxLocalizationMenuScreen"/> class.
        /// </summary>
		public vxLocalizationMenuScreen(bool IsShownOnLaunch)
            : base(vxLocalisationKey.SetLanguage)
        {
            this.IsShownOnLaunch = IsShownOnLaunch;
		}


		public override void LoadContent()
		{
			base.LoadContent();

			foreach (vxLanguagePack language in Engine.Languages)
			{
				vxMenuEntry tempLanguageMenuEntry = new vxMenuEntry(this, language.LanguageName);
                tempLanguageMenuEntry.Clicked += TempLanguageMenuEntry_Clicked;
                MenuEntries.Add(tempLanguageMenuEntry);
			}
		}

        string tempLang = "";

        void TempLanguageMenuEntry_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            vxMenuEntry menuentry = (vxMenuEntry)e.GUIitem;

            vxMessageBox confirmMsgBox = new vxMessageBox(string.Format(LanguagePack[vxLocalisationKey.SetLanguageConfirmation], menuentry.Text), "Set Language");
            confirmMsgBox.Accepted += ConfirmMsgBox_Accepted;
            vxSceneManager.AddScene(confirmMsgBox);
            tempLang = menuentry.Text;
        }

        void ConfirmMsgBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            Engine.Game.SetLanguage(tempLang);

            this.OnCancel(PlayerIndex.One);
        }

        void TempLanguageMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {

        }
    }
}
