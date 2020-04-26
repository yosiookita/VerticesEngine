using Microsoft.Xna.Framework;
using System;
using VerticesEngine.Localization;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class vxLocalizationDialog : vxDialogBase
    {

        /// <summary>
        /// The Graphics Settings Dialog
        /// </summary>
        public vxLocalizationDialog()
			: base("Localization", vxEnumButtonTypes.OkApplyCancel)
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();


            //Full Screen
            /*****************************************************************************************************/
            vxComboBox combo = new vxComboBox(Engine, Engine.Language.LanguageName,
                                              new Vector2(this.ArtProvider.GUIBounds.X, this.ArtProvider.GUIBounds.Y));
            
            InternalGUIManager.Add(combo);
			
            //Add in languages
            foreach (vxLanguagePack language in Engine.Languages)
            {
                combo.AddItem(language.LanguageName);
            }

            combo.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {

                Engine.Language = Engine.Languages[e.SelectedIndex];

                vxConsole.WriteLine("Setting Language to: " + Engine.Language.LanguageName);
            };
                        
          //ApplyButtonly.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
          //OKButtonOk.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);
        }

		public override void OnApplyButtonClicked(object sender, vxGuiItemClickEventArgs e)
		{
            SetSettings();

        }

        public override void OnOKButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
            ExitScreen();
        }

        void SetSettings()
        {
            Console.WriteLine("Setting Items");


            foreach(vxBaseScene screen in vxSceneManager.GetScreens())
            {
                screen.OnLocalisationChanged();
            }

            //Save Settings
            Engine.Settings.Save(Engine);
        }
    }
}
