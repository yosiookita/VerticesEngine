using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VerticesEngine.Graphics;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Dialogs
{


    /// <summary>
    /// The graphic settings dialog.
    /// </summary>
    public class vxGraphicSettingsDialog : vxSettingsBaseDialog
    {

        /// <summary>
        /// The Graphics Settings Dialog
        /// </summary>
        public vxGraphicSettingsDialog()
            : base("Graphics Settings", typeof(vxGraphicalSettingsAttribute))
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();
        }


        protected override bool FilterAttribute(vxSettingsAttribute attribute)
        {
            if (attribute.GetType() == typeof(vxGraphicalSettingsAttribute))
            {
                var gSet = ((vxGraphicalSettingsAttribute)attribute);

                if (gSet.Usage.IsFlagSet(vxGameEnviromentType.TwoDimensional) && Engine.Game.Config.GameType.IsFlagSet(vxGameEnviromentType.TwoDimensional))
                {
                    return true;
                }

                if (gSet.Usage.IsFlagSet(vxGameEnviromentType.ThreeDimensional) && Engine.Game.Config.GameType.IsFlagSet(vxGameEnviromentType.ThreeDimensional))
                {
                    return true;
                }
            }

            return false;
        }

        Point Resolution;
        protected override void OnSettingsRetreival()
        {


            //Resolutions
            // *****************************************************************************************************
            List<DisplayMode> DisplayModes = new List<DisplayMode>();
            PresentationParameters pp = Engine.GraphicsDevice.PresentationParameters;
            string currentRes = string.Format("{0}x{1}", pp.BackBufferWidth, pp.BackBufferHeight);

            var resolution = new vxSettingsGUIItem(Engine, InternalGUIManager, LanguagePack[vxLocalisationKey.Resolution], currentRes);


            Resolution = vxScreen.Resolution;

            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                //Don't Show All Resolutions
                if (mode.Width > 599 || mode.Height > 479)
                {
                    // Add the Display mode to the list
                    DisplayModes.Add(mode);

                    // now add the text representation
                    resolution.AddOption(string.Format("{0}x{1}", mode.Width, mode.Height));
                }
            }
            resolution.ValueChangedEvent += delegate
            {
                var mode = DisplayModes[resolution.SelectedIndex];
                vxScreen.SetResolution(mode.Width, mode.Height);
                //vxScreen.SetResolution
                //Resolution = new Point(mode.Width, mode.Height);
            };
            ScrollPanel.AddItem(resolution);

            //Full Screen
            // *****************************************************************************************************
            var fullscreenSetting = new vxSettingsGUIItem(Engine, InternalGUIManager, LanguagePack[vxLocalisationKey.FullScreen],
                vxScreen.IsFullScreen ? Engine.Language[vxLocalisationKey.FullScreen] : Engine.Language[vxLocalisationKey.Windowed]);

            fullscreenSetting.AddOption(LanguagePack[vxLocalisationKey.FullScreen]);
            fullscreenSetting.AddOption(LanguagePack[vxLocalisationKey.Windowed]);

            fullscreenSetting.ValueChangedEvent += delegate {
                //TempGraphicalSettings.Screen.IsFullScreen = fullscreenSetting.ToBool();// LanguagePack[vxLocalisationKey.FullScreen] ;
                vxScreen.IsFullScreen = (fullscreenSetting.SelectedIndex == 0);
            };
            ScrollPanel.AddItem(fullscreenSetting);



            //VSync
            // *****************************************************************************************************
            var VSyncSettingsItem = new vxSettingsGUIItem(Engine, InternalGUIManager, "Vertical Sync",
                vxScreen.IsVSyncOn ? "On" : "Off");
            
            VSyncSettingsItem.AddOption("On");
            VSyncSettingsItem.AddOption("Off");
            /*
            VSyncSettingsItem.ValueChangedEvent += delegate
            {
                TempGraphicalSettings.Screen.IsVSyncOn = VSyncSettingsItem.ToBool();
            };
            */
            ScrollPanel.AddItem(VSyncSettingsItem);

            base.OnSettingsRetreival();

            this.Title = LanguagePack[vxLocalisationKey.Graphics];

                        /*




                        // Handle 3D Game Settings
                        // *****************************************************************************************************
                        if (Engine.Game.Config.GameType != vxEnumGameType.TwoDimensional)
                        {

                            // Field Of View Slider
                            // *****************************************************************************************************
                            FOVSettingsItem = new vxScrollPanelSliderItem(Engine,
                                InternalGUIManager, "Field of View", 45, 120, (float)MathHelper.Clamp(Engine.Settings.Graphics.Screen.FieldOfView, 45, 120));
                            FOVSettingsItem.Slider.Tick = 1;
                            FOVSettingsItem.Slider.ValueChanged += (sender, e) => TempGraphicalSettings.Screen.FieldOfView = (int)FOVSettingsItem.Slider.Value;
                            ScrollPanel.AddItem(FOVSettingsItem);

                            // Texture Qaulity
                            // *****************************************************************************************************
                            vxScrollPanelComboxBoxItem TextureQualityItem = new vxScrollPanelComboxBoxItem(
                               Engine, InternalGUIManager, "Texture Quality", GraphicSettings.Textures.Quality.ToString());

                            foreach (vxEnumTextureQuality type in Enum.GetValues(typeof(vxEnumTextureQuality)))
                                TextureQualityItem.AddOption(type.ToString());

                            ScrollPanel.AddItem(TextureQualityItem);






                            // Anti Aliassing
                            // *****************************************************************************************************
                            //AASettingsItem = new vxScrollPanelComboxBoxItem(
                            //    Engine, InternalGUIManager, "Anti Aliasing", GraphicSettings.AntiAliasing.Type.ToString());
                            //ScrollPanel.AddItem(AASettingsItem);

                            //foreach (vxEnumAntiAliasType type in Enum.GetValues(typeof(vxEnumAntiAliasType)))
                            //    AASettingsItem.AddOption(type.ToString());




                            //Shadows
                            // *****************************************************************************************************
                            ShadowsSettingsItem = new vxScrollPanelComboxBoxItem(
                               Engine, InternalGUIManager, "Shadow Quality", GraphicSettings.Shadows.Quality.ToString());
                            ScrollPanel.AddItem(ShadowsSettingsItem);

                            foreach (vxEnumQuality type in Enum.GetValues(typeof(vxEnumQuality)))
                                ShadowsSettingsItem.AddOption(type.ToString());




                            //Blur
                            // *****************************************************************************************************
                            BlurSettingsItem = new vxScrollPanelComboxBoxItem(
                                Engine, InternalGUIManager, "Scene Blur Quality", GraphicSettings.Blur.Quality.ToString());
                            ScrollPanel.AddItem(BlurSettingsItem);

                            foreach (vxEnumQuality type in Enum.GetValues(typeof(vxEnumQuality)))
                                BlurSettingsItem.AddOption(type.ToString());

                            // Motion Blur
                            // *****************************************************************************************************
                            MotionBlurSettingsItem = new vxScrollPanelComboxBoxItem(
                                Engine, InternalGUIManager, "Motion Blur Quality",
                                Engine.Settings.Graphics.MotionBlur.IsEnabled ? "On" : "Off");
                            MotionBlurSettingsItem.AddOption("Off");
                            MotionBlurSettingsItem.AddOption("On");
                            MotionBlurSettingsItem.ValueComboBox.SelectionChanged += delegate (object sender, vxComboBoxSelectionChangedEventArgs e)
                            {

                                if (e.SelectedItem.Text == "On")
                                    TempGraphicalSettings.MotionBlur.IsEnabled = true;
                                else
                                    TempGraphicalSettings.MotionBlur.IsEnabled = false;

                                vxConsole.WriteSettings("Setting Motion Blur to: " + TempGraphicalSettings.MotionBlur.IsEnabled);

                            };
                            ScrollPanel.AddItem(MotionBlurSettingsItem);


                            //Bloom
                            // *****************************************************************************************************
                            BloomSettingsItem = new vxScrollPanelComboxBoxItem(
                                Engine, InternalGUIManager, "Bloom  Quality", GraphicSettings.Bloom.Quality.ToString());
                            ScrollPanel.AddItem(BloomSettingsItem);

                            foreach (vxEnumQuality type in Enum.GetValues(typeof(vxEnumQuality)))
                                BloomSettingsItem.AddOption(type.ToString());

                            //Depth of Field
                            // *****************************************************************************************************

                            DOFSettingsItem = new vxScrollPanelComboxBoxItem(
                                Engine, InternalGUIManager, "Depth of Field", GraphicSettings.DepthOfField.IsEnabled);
                            ScrollPanel.AddItem(DOFSettingsItem);

                            DOFSettingsItem = new vxScrollPanelComboxBoxItem(
                Engine, InternalGUIManager, "Edge Detection", GraphicSettings.EdgeDetection.IsEnabled ? "On" : "Off");
                            ScrollPanel.AddItem(DOFSettingsItem);

                            DOFSettingsItem.AddOption("Off");
                            DOFSettingsItem.AddOption("On");



                            // Crepuscular Rays
                            // *****************************************************************************************************
                            GodRaySettingsItem = new vxScrollPanelComboxBoxItem(
                               Engine, InternalGUIManager, "Crepuscular Rays", GraphicSettings.GodRays.Quality.ToString());
                            ScrollPanel.AddItem(GodRaySettingsItem);

                            foreach (vxEnumQuality type in Enum.GetValues(typeof(vxEnumQuality)))
                                GodRaySettingsItem.AddOption(type.ToString());


                            //Edge Detect
                            // *****************************************************************************************************
                            EdgeDetectSettingsItem = new vxScrollPanelComboxBoxItem(
                                Engine, InternalGUIManager, "Edge Detection", GraphicSettings.EdgeDetection.IsEnabled ? "On" : "Off");
                            ScrollPanel.AddItem(EdgeDetectSettingsItem);

                            EdgeDetectSettingsItem.AddOption("Off");
                            EdgeDetectSettingsItem.AddOption("On");



                            //SSAO
                            // *****************************************************************************************************
                            SSAOSettingsItem = new vxScrollPanelComboxBoxItem(
                                  Engine, InternalGUIManager, "SSA0", GraphicSettings.SSAO.Quality.ToString());
                            ScrollPanel.AddItem(SSAOSettingsItem);

                            foreach (vxEnumQuality type in Enum.GetValues(typeof(vxEnumQuality)))
                                SSAOSettingsItem.AddOption(type.ToString());

                            // SSR
                            // *****************************************************************************************************
                            SSRSettingsItem = new vxScrollPanelComboxBoxItem(
                                Engine, InternalGUIManager, "Screen Space Reflections", GraphicSettings.Reflections.SSRSettings.Quality.ToString());
                            ScrollPanel.AddItem(SSRSettingsItem);

                            foreach (vxEnumQuality type in Enum.GetValues(typeof(vxEnumQuality)))
                                SSRSettingsItem.AddOption(type.ToString());
                        }
                        ApplyButton.Clicked += Btn_Apply_Clicked;
                        //Btn_Ok.Clicked += Btn_Ok_Clicked;
                        */
                    }

        void Btn_Apply_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();

            // Now Exit Screen, and Readd teh screen
            ExitScreen();
            vxSceneManager.AddScene(new vxGraphicSettingsDialog());
        }

        public override void OnOKButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            SetSettings();
            ExitScreen();
        }



        void SetSettings()
        {
            //Save Settings
            vxScreen.RefreshGraphics();
            Engine.Settings.Save(Engine);
            vxSettings.SaveINI();
            vxConsole.WriteLine("Settings Graphics to the Following Settings:");

            /*
            // First Set the Resolutions
            if (GraphicSettings.Screen.IsDirty)
            {
                vxScreen.IsFullScreen = GraphicSettings.Screen.IsFullScreen;
                vxScreen.IsVSyncOn = GraphicSettings.Screen.IsVSyncOn;
                vxScreen.SetResolution(GraphicSettings.Screen.Resolution);
                GraphicSettings.Screen.IsDirty = false;
            }


            // Handle Texture Changes
            // ******************************************************************************************
            if (GraphicSettings.Textures.IsDirty)
            {
                DebugSettingChange("Texture Quality", GraphicSettings.Textures.Quality);

                // TODO: Reenable
                //foreach (vxModel model in Engine.ContentManager.LoadedModels)
                //    model.SetTexturePackLevel(Engine.Settings.Graphics.Textures.Quality);

                Engine.Settings.Graphics.Textures.IsDirty = false;
            }


            if (Engine.Game.Config.GameType != vxEnumGameType.TwoDimensional)
            {

                // Anti Aliasing
                // ******************************************************************************************
                //GraphicSettings.AntiAliasing.Type = (vxEnumAntiAliasType)(AASettingsItem.SelectedIndex);
               // DebugSettingChange(nameof(GraphicSettings.AntiAliasing), GraphicSettings.AntiAliasing.Type);

                // Shadows
                // ******************************************************************************************
                GraphicSettings.Shadows.Quality = (vxEnumQuality)(ShadowsSettingsItem.SelectedIndex);
                DebugSettingChange(nameof(GraphicSettings.Shadows), GraphicSettings.Shadows.Quality);

                // Blur
                // ******************************************************************************************
                GraphicSettings.Blur.Quality = (vxEnumQuality)(BlurSettingsItem.SelectedIndex);
                DebugSettingChange(nameof(GraphicSettings.Blur), GraphicSettings.Blur.Quality);

                // Bloom
                // ******************************************************************************************
                GraphicSettings.Bloom.Quality = (vxEnumQuality)(BloomSettingsItem.SelectedIndex);
                DebugSettingChange(nameof(GraphicSettings.Bloom), GraphicSettings.Bloom.Quality);

                // Depth Of Field
                // ******************************************************************************************
                GraphicSettings.DepthOfField.IsEnabled = (DOFSettingsItem.SelectedItem.Text == "On");
                DebugSettingChange(nameof(GraphicSettings.DepthOfField), GraphicSettings.DepthOfField.IsEnabled);

                // God Rays
                // ******************************************************************************************
                GraphicSettings.GodRays.Quality = (vxEnumQuality)(GodRaySettingsItem.SelectedIndex);
                DebugSettingChange(nameof(GraphicSettings.GodRays), GraphicSettings.GodRays.Quality);

                // Cartoon Edge Detection
                // ******************************************************************************************
                GraphicSettings.EdgeDetection.IsEnabled = (EdgeDetectSettingsItem.SelectedItem.Text == "On");
                DebugSettingChange(nameof(GraphicSettings.EdgeDetection), GraphicSettings.EdgeDetection.IsEnabled);

                GraphicSettings.SSAO.Quality = (vxEnumQuality)(SSAOSettingsItem.SelectedIndex);
                DebugSettingChange(nameof(GraphicSettings.SSAO), GraphicSettings.SSAO.Quality);

                //SSRSettingsItem
                GraphicSettings.Reflections.SSRSettings.Quality = (vxEnumQuality)(SSRSettingsItem.SelectedIndex);
                DebugSettingChange(nameof(GraphicSettings.Reflections.SSRSettings), GraphicSettings.Reflections.SSRSettings.Quality);




            }
    */

        }

        void DebugSettingChange(string name, object setting)
        {
            vxConsole.WriteLine(string.Format("     {0} : {1}", name, setting));
        }
    }
}
