
using Microsoft.Xna.Framework;
using System;
using VerticesEngine.Input;
using VerticesEngine.Input.Events;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// Open File Dialog
    /// </summary>
    public class vxSandboxNewItemDialog : vxDialogBase
    {
        #region Fields

        public vxScrollPanel ScrollPanel;


        int CurrentlySelected = -1;

		//List<vxFileDialogItem> List_Items = new List<vxFileDialogItem>();

        float TimeSinceLastClick = 1000;

        int HighlightedItem_Previous = -1;

        public vxTabControl TabControl;

        vxGameplayScene3D Sandbox;

        #endregion

        #region Events

        //public new event EventHandler<PlayerIndexEventArgs> Accepted;
        public new event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxSandboxNewItemDialog"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="path">Path.</param>
        /// <param name="FileExtentionFilter">File extention filter.</param>
        public vxSandboxNewItemDialog(vxGameplayScene3D Sandbox)
			: base("Add New Item", vxEnumButtonTypes.OkCancel)
        {

            this.Sandbox = Sandbox;

            base.LoadContent();

            Rectangle tabBounds = ArtProvider.GUIBounds.GetBorder(-10);
            tabBounds.Height = tabBounds.Bottom - tabBounds.Top - OKButton.Height;
            TabControl = new vxTabControl(Engine, tabBounds);
            InternalGUIManager.Add(TabControl);
        }

        public override Vector2 GetBoundarySize()
        {
            return base.GetBoundarySize();
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            //base.LoadContent();

//            OKButton.Clicked += new EventHandler<vxGuiItemClickEventArgs>(OnOKButtonClicked);
     //CancelButton_Cancel.Clicked += new EventHandler<vxGuiItemClickEventArgs>(OnCancelButtonClicked);

		}



		public override void UnloadContent()
        {
			//foreach (vxFileDialogItem fd in List_Items)
   //             fd.ButtonImage.Dispose();

            base.UnloadContent();
        }

        #endregion

        #region Handle Input


        public override void OnOKButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            ExitScreen();
        }

		public override void OnCancelButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            // Raise the cancelled event, then exit the message box.
            if (Cancelled != null)
                Cancelled(this, new PlayerIndexEventArgs(ControllingPlayer.Value));

            ExitScreen();
        }
        


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput()
        {
			// Handle Double Click
            if (vxInput.IsNewMouseButtonPress(MouseButtons.LeftButton))
            {
                if (TimeSinceLastClick < 20)
                {
                    if(CurrentlySelected == HighlightedItem_Previous)
            			OKButton.Select();
                }
                else
                {
                    TimeSinceLastClick = 0;
                }

                HighlightedItem_Previous = CurrentlySelected;
            }
        }
        
        #endregion
        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            TimeSinceLastClick++;
        }

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        

		public void GetHighlitedItem(object sender, vxGuiItemClickEventArgs e)
        {
			//foreach (vxFileDialogItem fd in List_Items)
   //         {
   //             fd.UnSelect();
   //         }
			//int i = e.GUIitem.Index;

   //         List_Items[i].ThisSelect();
            //CurrentlySelected = i;           
        }

        #endregion
    }
}