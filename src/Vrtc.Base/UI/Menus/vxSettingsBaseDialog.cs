using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection;
using VerticesEngine.Graphics;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// This is the base dialog for creating settings from a specified attribute type
    /// </summary>
    public class vxSettingsBaseDialog : vxDialogBase
    {

        protected vxScrollPanel ScrollPanel;

        Type settingsType;

        /// <summary>
        /// The Graphics Settings Dialog
        /// </summary>
        public vxSettingsBaseDialog(string title, Type settingsType)
            : base(title, vxEnumButtonTypes.OkApplyCancel)
        {
            this.settingsType = settingsType;
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            //this.Title = LanguagePack[vxLocalisationKey.Graphics];

            ScrollPanel = new vxScrollPanel(Engine,
    new Vector2(
        ArtProvider.GUIBounds.X + ArtProvider.Padding.X,
        ArtProvider.GUIBounds.Y + ArtProvider.Padding.Y) + ArtProvider.PosOffset,
    ArtProvider.GUIBounds.Width - (int)ArtProvider.Padding.X * 2,
    ArtProvider.GUIBounds.Height - OKButton.Bounds.Height - (int)ArtProvider.Padding.Y * 4);
            InternalGUIManager.Add(ScrollPanel);

            vxSettingsGUIItem.ArrowSpacing = 0;

            OnSettingsRetreival();
        }

        protected virtual bool FilterAttribute(vxSettingsAttribute attribute)
        {
            return true;
        }

        protected virtual void OnSettingsRetreival()
        {
            // get all Graphical Settings
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // this is handeled manually above
                if (type == typeof(vxScreen))
                    continue;

                // get member
                var properties = type.GetProperties().Where(field => Attribute.IsDefined(field, this.settingsType));
                foreach (PropertyInfo property in properties)
                {
                    var attribute = property.GetCustomAttribute<vxSettingsAttribute>();

                    // check if this attribute is needed for this game type
                    //if(attribute.us)
                    if (FilterAttribute(attribute) == false)
                        continue;

                    if (attribute.IsMenuSetting)
                    {
                        string settingValue = property.GetValue(property).ToString();
                        var settingsGUIItem = new vxSettingsGUIItem(Engine, InternalGUIManager, attribute.DisplayName.SplitIntoSentance(), settingValue);

                        // Now set values based on type
                        if (property.PropertyType == typeof(bool))
                        {
                            //property.SetValue(setting, bool.Parse(settings[set.DisplayName]));
                            settingsGUIItem.AddOption("On");
                            settingsGUIItem.AddOption("Off");
                            settingValue = (bool)property.GetValue(property) ? "On" : "Off";

                            
                        }

                        else if (property.PropertyType.IsEnum)
                        {
                            //property.SetValue(setting, Enum.Parse(setting.FieldType, settings[set.DisplayName]));
                            foreach (var propType in Enum.GetValues(property.PropertyType))
                                settingsGUIItem.AddOption(propType.ToString());
                        }

                        else
                        {
                            Console.WriteLine("----- No Setting Found for '" + property.PropertyType.ToString() + "' -----");
                        }

                        settingsGUIItem.Value = settingValue;
                        ScrollPanel.AddItem(settingsGUIItem);
                    }
                }
            }

        }

        void Btn_Apply_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();

            // Now Exit Screen, and Readd teh screen
            ExitScreen();
            //vxSceneManager.AddScene(new vxGraphicSettingsDialog());
        }

        public override void OnOKButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
            ExitScreen();
        }



        void SetSettings()
        {
            //TODO: Save current settings files

            // then re-load them


            //Save Settings
            Engine.Settings.Save(Engine);
        }
    }
}
