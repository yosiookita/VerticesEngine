using Microsoft.Xna.Framework;
using System.Collections.Generic;
using VerticesEngine.Input;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// The graphic settings dialog.
    /// </summary>
    public class vxKeyboardSettingsDialog : vxDialogBase
    {
		vxKeyBindings QWERTYPresetKeyBindings;
		vxKeyBindings AZERTYPresetKeyBindings;

		List<vxKeyBindingSettingsGUIItem> KeyBindingGUIItems = new List<vxKeyBindingSettingsGUIItem>();

        /// <summary>
        /// The Graphics Settings Dialog
        /// </summary>
        public vxKeyboardSettingsDialog()
			: base("Control Settings", vxEnumButtonTypes.OkApplyCancel)
        {

        }

		/// <summary>
		/// Load graphics content for the screen.
		/// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            this.Title = Engine.Language[vxLocalisationKey.KeyboardSettings];

			QWERTYPresetKeyBindings = new vxKeyBindings(Engine, KeyboardTypes.QWERTY);
			AZERTYPresetKeyBindings = new vxKeyBindings(Engine, KeyboardTypes.AZERTY);


			//All Items below are stored in this column as it's the longest word
			
            float Margin = Engine.GraphicsDevice.Viewport.Width/2 - this.viewportSize.X/2 + 25;
            float MarginTwo = Margin + 450;

            int horiz = 0;
			int horizTwo = 0;




			// Keyboard Preset Settings Item
			/*****************************************************************************************************/
			var KeyboardPresetSettingsItem = new vxScrollPanelComboxBoxItem(
                Engine, InternalGUIManager, "Preset",
                "QWERTY", 
				new Vector2(this.ArtProvider.GUIBounds.X, this.ArtProvider.GUIBounds.Y + horiz));
            horiz += 45;
			KeyboardPresetSettingsItem.ValueComboBox.AddItem(KeyboardTypes.QWERTY.ToString());
            KeyboardPresetSettingsItem.ValueComboBox.AddItem(KeyboardTypes.AZERTY.ToString());
			KeyboardPresetSettingsItem.ValueComboBox.AddItem(KeyboardTypes.CUSTOM.ToString());
            KeyboardPresetSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e) {

				vxKeyBindings presetKeyBindings = new vxKeyBindings(Engine);

				bool continueOn = true;

				switch ((KeyboardTypes)e.SelectedIndex)
				{
					case KeyboardTypes.QWERTY:
						presetKeyBindings = QWERTYPresetKeyBindings;
						break;
						case KeyboardTypes.AZERTY:
						presetKeyBindings = AZERTYPresetKeyBindings;
						break;
					default:
						continueOn = false;
					break;
				}

				if(continueOn)
				foreach (vxKeyBindingSettingsGUIItem guiitem in KeyBindingGUIItems)
				{
						guiitem.KeyBinding = presetKeyBindings.Bindings[guiitem.BindingID];
					guiitem.Button.Text = presetKeyBindings.Bindings[guiitem.BindingID].Key.ToString();
				}
                
            };






			// Key Bindings
			/*****************************************************************************************************/

			foreach (KeyValuePair<object, vxKeyBinding> binding in vxInput.KeyBindings.Bindings)
			{
				var keyBinding = new vxKeyBindingSettingsGUIItem(
					Engine, InternalGUIManager,
					binding.Value.Name, 
					binding.Value,
					binding.Key,
					new Vector2(this.ArtProvider.GUIBounds.X + MarginTwo, this.ArtProvider.GUIBounds.Y + horizTwo));

				KeyBindingGUIItems.Add(keyBinding);

				horizTwo += 45;
			}



           //ApplyButtony.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
           //OKButtonk.Clicked += new EventHandler<vxGuiItemClickEventArgs>OnOKButton_Clicked);
        }

        void Btn_Apply_Clicked(object sender, vxGuiItemClickEventArgs e)
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
            //Save Settings
            //Engine.SaveSettings(Engine);

            //Set Graphics
            //Engine.GraphicsSettingsManager.Apply();
			foreach (vxKeyBindingSettingsGUIItem guiitem in KeyBindingGUIItems)
			{
				vxInput.KeyBindings.Bindings[guiitem.BindingID] = guiitem.KeyBinding;
			}
        }
    }
}
