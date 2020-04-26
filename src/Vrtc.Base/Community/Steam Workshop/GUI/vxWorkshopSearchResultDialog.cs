using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using VerticesEngine;
using VerticesEngine.Utilities;
using VerticesEngine.Input.Events;
using VerticesEngine.Input;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;
using VerticesEngine.Mathematics;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.Serilization;
using VerticesEngine.UI;
using VerticesEngine.UI.Themes;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.UI.Dialogs.Utilities;
using VerticesEngine.Community;
using VerticesEngine.Community.Events;

namespace VerticesEngine.Community.Dialogs
{
    /// <summary>
    /// Steam workshop search result dialog.
    /// </summary>
    public class vxWorkshopSearchResultDialog : vxDialogBase
    {
        #region Fields

        public vxScrollPanel ScrollPanel;

        vxLabel searchResultText;

        int pageNumber;
        const int itemsPerPage = 10;

        vxButtonImageControl NextGroup;
        vxButtonImageControl PrevGroup;

        #endregion

        #region Events

        //public new event EventHandler<vxWorkshopItemOpenEventArgs> Accepted;
        //public new event EventHandler<vxGuiItemClickEventArgs> Cancelled;

        #endregion

        #region Initialization



        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:VerticesEngine.Community.Dialogs.vxWorkshopSearchResultDialog"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="pageNumber">Page number.</param>
        public vxWorkshopSearchResultDialog(int pageNumber)
            : base("Steam Workshop", vxEnumButtonTypes.OkCancel)
        {
            this.pageNumber = pageNumber;
        }


        public vxLabel GetLabel()
        {
            var label = new vxLabel(Engine, "", Vector2.Zero);
            label.Theme.Text = new vxColourTheme(Color.Black);
            label.IsShadowVisible = true;
            label.ShadowColour = Color.Black * 0.5f;
            InternalGUIManager.Add(label);
            label.Font = vxGUITheme.Fonts.FontSmall;
            return label;
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            // Top Image
            searchResultText = new vxLabel(Engine, "Searching...", new Vector2(
                ArtProvider.GUIBounds.X + ArtProvider.Padding.X * 2,
                ArtProvider.GUIBounds.Y + ArtProvider.Padding.Y * 2));
            InternalGUIManager.Add(searchResultText);
            searchResultText.Theme.Text = new vxColourTheme(Color.Black);
            //searchResultText.DoShadow = true;


            // Scroll Panel
            ScrollPanel = new vxScrollPanel(Engine, new Vector2(
                ArtProvider.GUIBounds.X,
                ArtProvider.GUIBounds.Y + ArtProvider.Padding.Y * 3 + searchResultText.Height),
                                            (ArtProvider.GUIBounds.Width),
                                            ArtProvider.GUIBounds.Height - OKButton.Bounds.Height - (int)ArtProvider.Padding.Y * 6);


            InternalGUIManager.Add(ScrollPanel);


            // Arrows
            int ArrowButtonSize = vxLayout.GetScaledSize(108);

            //var backgroundBounds = ArtProvider.GUIBounds;
            var bounds = ScrollPanel.Bounds;

            NextGroup = new vxButtonImageControl(Engine,
                                                         new Vector2(bounds.Right + ArtProvider.Padding.X,
                                                                     bounds.Center.Y - ArrowButtonSize / 2),
                                               vxGUITheme.SpriteSheetLoc.ArrowBtnFwd, vxGUITheme.SpriteSheetLoc.ArrowBtnFwd)
            {
                DrawHoverBackground = false,
                Alpha = 0,
                Width = ArrowButtonSize,
                Height = ArrowButtonSize,
                IsShadowVisible = true
            };

            NextGroup.Clicked += delegate
            {
                vxSceneManager.RemoveScene(this);
                vxSceneManager.AddScene(new vxWorkshopSearchResultDialog( pageNumber + 1));

            };

            InternalGUIManager.Add(NextGroup);



            PrevGroup = new vxButtonImageControl(Engine,
                                                         new Vector2(bounds.Left - ArrowButtonSize - ArtProvider.Padding.X,
                                                                     bounds.Center.Y - ArrowButtonSize / 2),
                                               vxGUITheme.SpriteSheetLoc.ArrowBtnBack, vxGUITheme.SpriteSheetLoc.ArrowBtnBack)
            {
                DrawHoverBackground = false,
                Alpha = 0,
                Width = ArrowButtonSize,
                Height = ArrowButtonSize,
                IsShadowVisible = true
            };
            PrevGroup.Clicked += delegate
            {
                vxSceneManager.RemoveScene(this);
                vxSceneManager.AddScene(new vxWorkshopSearchResultDialog( pageNumber - 1));

            };

            InternalGUIManager.Add(PrevGroup);

            NextGroup.IsEnabled = false;
            PrevGroup.IsEnabled = false;
            OKButton.IsEnabled = false;




            int start = pageNumber * itemsPerPage;

            int max = Math.Min(pageNumber * itemsPerPage + itemsPerPage, vxWorkshop.PreviousSearch.Results.Count);

            for (int ind = start; ind < max; ind++)
            {
                AddScrollItem(ind);
            }
            ScrollPanel.ResetLayout();
            searchResultText.Text = "Displaying " + (start + 1) + " - " + max + " of " + vxWorkshop.PreviousSearch.Results.Count + " Items Found.";
            ProcessItemsAsync(0);


            if (pageNumber * itemsPerPage + itemsPerPage < vxWorkshop.PreviousSearch.Results.Count)
                NextGroup.IsEnabled = true;

            if (pageNumber != 0)
                PrevGroup.IsEnabled = true;
        }








        void AddScrollItem(int index)
        {
            // Get a new File Dialog Item
            var fileDialogButton = new vxWorkshopDialogItem(Engine, index);

            // Hookup Click Events
            fileDialogButton.Clicked += OnItemClicked;
            fileDialogButton.DoubleClicked += delegate
            {
                OKButton.Select();
            };

            //Set Button Width
            fileDialogButton.Width = Engine.GraphicsDevice.Viewport.Width - (4 * (int)this.ArtProvider.Padding.X);
            if (fileDialogButton.Text != "")
                ScrollPanel.AddItem(fileDialogButton);

            //ScrollPanel.ResetLayout();
            ScrollPanel.ScrollBar.TravelPosition = 0;
        }

        //int imageWidth = 0;




        vxWorkshopDialogItem SelectedDialogItem;

        public void OnItemClicked(object sender, vxGuiItemClickEventArgs e)
        {
            OKButton.IsEnabled = true;

            foreach (var item in ScrollPanel.Items)
                item.ToggleState = false;

            e.GUIitem.ToggleState = true;

            if (e.GUIitem is vxWorkshopDialogItem)
            {
                // Get the GUI Item
                SelectedDialogItem = ((vxWorkshopDialogItem)e.GUIitem);
            }
        }



        public void ProcessItemsAsync(int index)
        {
            if (ScrollPanel.Items.Count > 0 && index < ScrollPanel.Items.Count)
            {
                var item = ScrollPanel.Items[index];
                if (item is vxWorkshopDialogItem)
                {
                    ((vxWorkshopDialogItem)item).Process(this, index);
                }
            }
        }


        #endregion

        #region Handle Input


        public override void OnOKButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            if (SelectedDialogItem != null)
            {
                if (SelectedDialogItem.Item.IsSubscribed && SelectedDialogItem.Item.ItemType == vxWorkshopItemType.SandboxFile)
                {
                    Engine.Game.OnWorkshopItemOpen(SelectedDialogItem.Item);
                    ExitScreen();
                }
                else if (SelectedDialogItem.Item.ItemType == vxWorkshopItemType.Mod)
                {
                    
                }
            }
        }

        public override void OnCancelButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            ExitScreen();
        }


        #endregion


    }
}
