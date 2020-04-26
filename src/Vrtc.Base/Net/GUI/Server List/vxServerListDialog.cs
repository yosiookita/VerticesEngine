using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using VerticesEngine;
using VerticesEngine.Utilities;
using VerticesEngine.Input;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.Net.Events;
using VerticesEngine.UI;



namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// This Dislog Displays all active server's on the connected master server.
    /// </summary>
    public class vxServerListDialog : vxDialogBase
    {
        #region Fields

        vxListView ScrollPanel;

        private System.ComponentModel.BackgroundWorker BckgrndWrkr_FileOpen;

       // string FileExtentionFilter;

        int CurrentlySelected = -1;

		List<vxFileDialogItem> List_Items = new List<vxFileDialogItem>();

        float TimeSinceLastClick = 1000;

        int HighlightedItem_Previous = -1;
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public vxServerListDialog()
            : base("Server List", vxEnumButtonTypes.OkApplyCancel)
        {
            
            BckgrndWrkr_FileOpen = new System.ComponentModel.BackgroundWorker();
            BckgrndWrkr_FileOpen.WorkerReportsProgress = true;
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




            OKButton.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Ok_Clicked);
          CancelButton.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Cancel_Clicked);
          ApplyButton.Clicked += new EventHandler<vxGuiItemClickEventArgs>(Btn_Apply_Clicked);
          ApplyButton.Text = "Refresh";

            ScrollPanel = new vxListView(Engine,
                new Vector2(
					this.ArtProvider.GUIBounds.X + this.ArtProvider.Padding.X, 
					this.ArtProvider.GUIBounds.Y + this.ArtProvider.Padding.Y),
				(int)(this.ArtProvider.GUIBounds.Width - this.ArtProvider.Padding.X * 2),
				(int)(this.ArtProvider.GUIBounds.Height - OKButton.Bounds.Height - this.ArtProvider.Padding.Y * 3));            

            InternalGUIManager.Add(ScrollPanel);


            // TODO: Add into Session Manager
            //Engine.GameServerListRecieved += new EventHandler<vxGameServerListRecievedEventArgs>(Engine_GameServerListRecieved);
            //Send the request string
            //Engine.SendMessage("vrtx_request_serverList");
        }

        void Btn_Ok_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            
        }

        void Btn_Cancel_Clicked(object sender, vxGuiItemClickEventArgs e)
        {

        }

        void Engine_GameServerListRecieved(object sender, vxGameServerListRecievedEventArgs e)
        {
            int index = 0;
            foreach (string parsestring in e.ServerList)
            {
                if (index != 0)
                {					
					vxConsole.WriteNetworkLine("IP: " + parsestring.ReadXML("ip") + ", Port: " + parsestring.ReadXML("port"));

                    vxListViewItem item = new vxListViewItem(Engine,
                        parsestring.ReadXML("ip"));
					item.ButtonWidth = ScrollPanel.Width - (int)(4 * this.ArtProvider.Padding.X);
                    
                    ScrollPanel.AddItem(item);
                }
                index++;
            }
        }

        void Btn_Apply_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            //Send the request string
            //Engine.SendMessage("vrtx_request_serverList");
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput()
        {
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


        bool FirstLoop = true;
        //float LoadingAlpha = 0;
        //float LoadingAlpha_Req = 1;
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            TimeSinceLastClick++;

            if (FirstLoop)
            {
                FirstLoop = false;

                BckgrndWrkr_FileOpen.RunWorkerAsync();
            }
        }

        #region Draw

        string FileName = "";
        int GetHighlitedItem(int i)
        {
			foreach (vxFileDialogItem fd in List_Items)
            {
                fd.UnSelect();
            }

            List_Items[i].ThisSelect();
            CurrentlySelected = i;

            FileName = List_Items[i].FileName;
            return 0;
        }

        #endregion
    }
}

