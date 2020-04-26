using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;
using VerticesEngine.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using VerticesEngine.Community;
using VerticesEngine.Community.Dialogs;
using VerticesEngine.Plugins;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// Handles which installed mods are enabled and disabled.
    /// </summary>
    public class vxModManagerDialog : vxDialogBase
    {
        #region Fields

        public vxScrollPanel ScrollPanel;

        string FileExtentionFilter;


        /// <summary>
        /// Main text for the Message box
        /// </summary>
        public string Path;

        /// <summary>
        /// The mod status list
        /// </summary>
        List<vxModItemStatus> ModStatusList = new List<vxModItemStatus>();

        #endregion

        #region Events

        public new event EventHandler<vxSandboxFileSelectedEventArgs> Accepted;

        public new event EventHandler<vxGuiItemClickEventArgs> Cancelled;

        #endregion

        #region Initialization



        public static Texture2D InitialTexture;

        #region -- Mod List JSON File

        string modListFile = "mods.json";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxOpenSandboxFileDialog"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="FileExtentionFilter">File extention filter.</param>
        public vxModManagerDialog(string FileExtentionFilter) : this(FileExtentionFilter, null)
        {

        }

        public vxModManagerDialog(string FileExtentionFilter, NewFileDialogItemDelegate OnNewFileDialogItem)
            : base("Mods", vxEnumButtonTypes.OkCancel)
        {
            this.Path = vxIO.PathToMods;
            this.FileExtentionFilter = FileExtentionFilter;

        }

        public vxLabel GetLabel(string text, Vector2 pos)
        {
            return GetLabel(text, pos, vxGUITheme.Fonts.FontSmall);
        }

        public vxLabel GetLabel(string text, Vector2 pos, SpriteFont font)
        {
            var label = new vxLabel(Engine, text, pos);
            label.Theme.Text = new Themes.vxColourTheme(Color.Black);
            label.IsShadowVisible = true;
            label.ShadowColour = Color.Black * 0.5f;
            label.ShadowOffset *= 0.5f;
            InternalGUIManager.Add(label);
            label.Font = font;
            return label;
        }


        public override void LoadContent()
        {
            base.LoadContent();


            // Main Bounds
            Rectangle MainBounds = new Rectangle((int)(ArtProvider.GUIBounds.X),
                                       (int)(ArtProvider.GUIBounds.Y + (int)ArtProvider.Padding.Y),
                                       ArtProvider.GUIBounds.Width,
                                       ArtProvider.GUIBounds.Height - OKButton.Bounds.Height - (int)ArtProvider.Padding.Y * 2);

            float lineHeight = vxLayout.GetScaledHeight(vxGUITheme.Fonts.FontSmall.LineSpacing );

            float runningLabelPosition = MainBounds.Y;

            // Search Box
            // ----------------------------------------------------------------
            int searchStart = (int)MainBounds.X;

            /* Psuedo Code
             - read json
             - check if file exists
             - if exists, load it
             - 

*/           
            /*
            AddWhiteLabel("Search", "Search and Download Community Made Blueprints & Mods",
                          new Vector2(searchStart, runningLabelPosition),
                          MainBounds.Width / 2, true);
                          */
                          /*
            runningLabelPosition += lineHeight;

            // top label
            string dscTxt = "See what mod's you have installed as well as search and download new mods directly from here!";
            dscTxt = vxGUITheme.Fonts.FontSmall.WrapString(dscTxt, MainBounds.Width / 2);

            var descriptionLabel = GetLabel(dscTxt, new Vector2(searchStart, runningLabelPosition), vxGUITheme.Fonts.FontTiny);
            
            runningLabelPosition += descriptionLabel.Height;
            //descriptionLabel.Width = MainBounds.Width;



            // Search Community Mods
            // ----------------------------------------------------------------------------------------
            var searchTitleLabel = GetLabel("Search Community Mods", new Vector2(MainBounds.X - vxLayout.ScaleAvg * 40, runningLabelPosition),
                vxGUITheme.Fonts.FontSmall);

            runningLabelPosition += searchTitleLabel.Height;

            var searchTextBox = new vxTextbox(Engine, "", new Vector2(searchStart + MainBounds.Width / 4, runningLabelPosition));
            InternalGUIManager.Add(searchTextBox);

            searchTextBox.Height = Font.LineSpacing * 3 / 2;
            var searchCriteriaLabel = GetLabel("Search:", new Vector2(searchStart, runningLabelPosition));

            //runningLabelPosition += Font.LineSpacing/2;


            runningLabelPosition += lineHeight;
            var searchDropDown = new vxComboBox(Engine, "Published", new Vector2(searchStart + MainBounds.Width / 4, runningLabelPosition));
            searchDropDown.Height = vxLayout.GetScaledHeight(Font.LineSpacing);
            searchDropDown.Width = searchTextBox.Width;
            searchDropDown.SelectionChanged += delegate {
                searchTextBox.IsEnabled = (searchDropDown.SelectedIndex == 0);
            };
            foreach (var item in Enum.GetValues(typeof(vxWorkshopItemSearchCriteria)))
                searchDropDown.AddItem(item.ToString());
            InternalGUIManager.Add(searchDropDown);
            var searchCriteriaTxtbx = GetLabel("Criteria: ", new Vector2(searchStart, runningLabelPosition));





            runningLabelPosition += lineHeight * 1.25f;
            // Search Button
            var searchButton = new vxButtonControl(Engine, "Search", new Vector2(searchStart + MainBounds.Width / 4, runningLabelPosition));
            searchButton.Position = new Vector2(searchStart + MainBounds.Width / 4, runningLabelPosition);
            searchButton.ArtProvider.SpriteSheetRegion = new Rectangle(24, 72, 216, 72);
            InternalGUIManager.Add(searchButton);
            searchButton.Clicked += delegate
            {
                vxSceneManager.AddScene(new vxWorkshopSearchBusyScreen(Engine,
                                                                new vxWorkshopSearchCriteria(
                                                                    searchTextBox.Text,
                                                                    (vxWorkshopItemSearchCriteria)searchDropDown.SelectedIndex,
                                                                    new string[] { "Mod" }, new string[] { "" }
                                                                    )));
            };

            runningLabelPosition += searchButton.Height;
            runningLabelPosition += lineHeight;



            // Mod SDK
            // ----------------------------------------------------------------------------------------

            var modSDKTitleLabel = GetLabel("Search Community Mods", new Vector2(MainBounds.X - vxLayout.ScaleAvg * 40, runningLabelPosition),
                vxGUITheme.Fonts.FontSmall);

            runningLabelPosition += searchTitleLabel.Height;

            // top label
            string modDescTxt = "It's incredibly easy to create and share own mods and share them with the community! Download the SDK below!";
            modDescTxt = vxGUITheme.Fonts.FontSmall.WrapString(modDescTxt, MainBounds.Width / 2);

            var modDescriptionLabel = GetLabel(modDescTxt, new Vector2(searchStart, runningLabelPosition), vxGUITheme.Fonts.FontTiny);

            runningLabelPosition += modDescriptionLabel.Height;


            // View Workshop Button
            var getSDKBtn = new vxButtonControl(Engine, "Get the Mod SDK!", new Vector2(MainBounds.Center.X, runningLabelPosition));
            getSDKBtn.Position = new Vector2(searchStart + MainBounds.Width / 4 - getSDKBtn.Width / 2, runningLabelPosition);



            getSDKBtn.ArtProvider.SpriteSheetRegion = new Rectangle(24, 72, 216, 72);
            getSDKBtn.ArtProvider.Padding = new Vector2(10, 15) * vxLayout.Scale;
            InternalGUIManager.Add(getSDKBtn);
            getSDKBtn.Clicked += delegate {
                Engine.Game.OnGetModSDK();
            };

            runningLabelPosition += getSDKBtn.Height;
            runningLabelPosition += lineHeight/2;




            // Upload Mod Button
            var UploadMod = new vxButtonControl(Engine, "Upload a Mod!", new Vector2(MainBounds.Center.X, runningLabelPosition));
            UploadMod.Position = new Vector2(searchStart + MainBounds.Width / 4 - UploadMod.Width/2, runningLabelPosition);

            UploadMod.ArtProvider.SpriteSheetRegion = new Rectangle(24, 72, 216, 72);
            UploadMod.ArtProvider.Padding = new Vector2(10, 15) * vxLayout.Scale;
            InternalGUIManager.Add(UploadMod);
            UploadMod.Clicked += delegate {
                Engine.Game.OnUploadMod();
            };
            */
            
            // Installed Mods List
            // ----------------------------------------------------------------------------------------

            runningLabelPosition = MainBounds.Y;

            var titleLabel = GetLabel("Installed Mods", new Vector2(MainBounds.X, runningLabelPosition), 
                vxGUITheme.Fonts.FontSmall);

            runningLabelPosition += titleLabel.Height*1.25f;

            ScrollPanel = new vxScrollPanel(Engine,new Vector2(MainBounds.X + ArtProvider.Padding.X * 3, runningLabelPosition),
                                            (ArtProvider.GUIBounds.Width - (int)ArtProvider.Padding.X * 2),
                                            MainBounds.Height -(int)( titleLabel.Height * 1.25f));


            InternalGUIManager.Add(ScrollPanel);

            RefreshModList();


            var mods = vxPluginManager.GetAvailableModsInPath(vxIO.PathToMods);
        }


        /// <summary>
        /// Refreshes the mod list
        /// </summary>
        public void RefreshModList()
        {

            // reset the Scroll Panel
            ScrollPanel.Clear();
            ScrollPanel.ScrollBar.TravelPosition = 0;

            List<vxModItemStatus> tempModList = new List<vxModItemStatus>();
            // first check if the mod list is available
            modListFile = System.IO.Path.Combine(vxIO.PathToMods, "mods.json");
            if (File.Exists(modListFile))
            {
                var reader = new StreamReader(modListFile);
                string fileText = reader.ReadToEnd();
                reader.Close();
                tempModList = JsonConvert.DeserializeObject<List<vxModItemStatus>>(fileText);
            }

            // loop through each mod in the list
            foreach (var modPath in Engine.PlayerProfile.GetInstalledMods())
            //foreach (var modPath in vxPluginManager.GetAvailableModsInPath(vxIO.PathToMods))
            {
                // Create a new mod item
                vxModItemStatus modItem = new vxModItemStatus();
                modItem.Name = new FileInfo(modPath).Name;
                modItem.Path = modPath;
                modItem.IsEnabled = false;

                // now loop through all mods in teh settings 
                foreach(var modInfo in tempModList)
                {
                    if(modInfo.Path.ToLower() == modPath.ToLower())
                    {
                        modItem.IsEnabled = modInfo.IsEnabled;
                    }
                }

                ModStatusList.Add(modItem);
            }


            // get all files with the specified file extention filter
            // Now add a new mod
            for (int i = 0; i < ModStatusList.Count; i++)
            {
                try
                {
                    var mod = ModStatusList[i];
                    AddScrollItem(ref mod);
                }
                catch(Exception ex) { vxConsole.WriteException(this, ex); }
            }

            // Then finalise the scroll panel
            ScrollPanel.UpdateItemPositions();

            ProcessItemsAsync(0);
        }

        void AddScrollItem(ref vxModItemStatus file)
        {
            // Get a new File Dialog Item
            var dialogItem =  new vxModDialoglItem(Engine, ref file);


            //Set Button Width
            dialogItem.Width = Engine.GraphicsDevice.Viewport.Width - (4 * (int)this.ArtProvider.Padding.X);
            if (dialogItem.FileInfo.Name.GetFileNameFromPath() != "")
                ScrollPanel.AddItem(dialogItem);
        }





        public void ProcessItemsAsync(int index)
        {
            Console.WriteLine("PARSING: " + index);
            if (ScrollPanel.Items.Count > 0 && index < ScrollPanel.Items.Count)
            {
                var item = ScrollPanel.Items[index];
                if (item is vxModDialoglItem)
                {
                    currentProcessingItem = ((vxModDialoglItem)item);
                    //currentProcessingItem.Process(this, index);
                }
            }
        }

        vxModDialoglItem currentProcessingItem;


        public override void UnloadContent()
        {
            foreach(var item in ScrollPanel.Items)
            { 
                if (item is vxModDialoglItem)
                {
                    //((vxModDialoglItem)item).CancelProcessing();
                }
            }

            base.UnloadContent();
        }

        #endregion

        #region Handle Input


        public override void OnOKButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            if (currentProcessingItem != null)
            {
                //currentProcessingItem.CancelProcessing();
            }

            try
            {
                // re-save the list
                var modStatusListText = JsonConvert.SerializeObject(ModStatusList, Formatting.Indented);
                StreamWriter writer = new StreamWriter(modListFile);
                writer.WriteLine(modStatusListText);
                writer.Close();
            }
            catch (Exception ex)
            {
                vxConsole.WriteException(this, ex);
            }

            ExitScreen();
        }

        

        public override void OnCancelButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            // Raise the cancelled event, then exit the message box.
            if (Cancelled != null)
                Cancelled(this, e);

            ExitScreen();
        }


        #endregion
    }
}
