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
using Lidgren.Network;
using VerticesEngine.Net;
using VerticesEngine.Net.Messages;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Dialogs
{


    /// <summary>
    /// This is a Server Dialog which searches and retrieves any game servers on this LAN.
    /// It also allows the Player to set up a local server as well.
    /// </summary>
    public class vxSeverLANListDialog : vxDialogBase
    {
        #region Fields

        /// <summary>
        /// The vxListView GUI item which contains the list of all broadcasting servers on the subnet.
        /// </summary>
        public vxScrollPanel ScrollPanel;

        /// <summary>
        /// Let's the player create a new local server.
        /// </summary>
        public vxButtonControl CreateNewLocalServerButton;
        //NetPeerConfiguration newServerConfig;


        /// <summary>
        /// The Server Name too Give your Game. CHANGE THIS!!!
        /// </summary>
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }
        private string _serverName = "default game server name";

        //int CurrentlySelected = -1;

        List<vxServerListItem> List_Items = new List<vxServerListItem>();


        #endregion



        /// <summary>
        /// Constructor.
        /// </summary>
        public vxSeverLANListDialog(string Title)
            : base(Title, vxEnumButtonTypes.OkApplyCancel)
        {
            //_serverName = Title;
        }


        /// <summary>
        /// Sets up Local Server Dialog. It also sends out the subnet broadcast here searching for any available servers on this subnet.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            ScrollPanel = new vxScrollPanel(Engine,
    new Vector2(
                                    this.ArtProvider.GUIBounds.X,
                                    this.ArtProvider.GUIBounds.Y + OKButton.Bounds.Height),
                                this.ArtProvider.GUIBounds.Width,
                                this.ArtProvider.GUIBounds.Height - OKButton.Bounds.Height * 3 / 2);


            InternalGUIManager.Add(ScrollPanel);

            //Vector2 viewportSize = new Vector2(Engine.GraphicsDevice.Viewport.Width, Engine.GraphicsDevice.Viewport.Height);
            Rectangle GUIBounds = ArtProvider.GUIBounds;

            //Create The New Server Button
            CreateNewLocalServerButton = new vxButtonControl(Engine, "Create LAN Server", 
                                                           new Vector2(GUIBounds.X / 2 - 115, GUIBounds.Y / 2 + 20));

            //Set the Button's Position relative too the background rectangle.
            CreateNewLocalServerButton.Position = new Vector2(
				this.ArtProvider.GUIBounds.X, 
				this.ArtProvider.GUIBounds.Y) + new Vector2(
                Engine.GUITheme.Padding.X * 2,
					this.ArtProvider.GUIBounds.Height - vxGUITheme.ArtProviderForButtons.DefaultHeight - Engine.GUITheme.Padding.Y * 2);

            CreateNewLocalServerButton.Clicked += Btn_CreateNewLocalServer_Clicked;
            InternalGUIManager.Add(CreateNewLocalServerButton);


            //The Cancel Button is Naturally the 'Back' button
            CancelButton.Clicked += new EventHandler<vxGuiItemClickEventArgs>(OnCancelButtonClicked);
            ApplyButton.Text = "Refresh";


            
            //Initialise the network client
            vxNetworkManager.Client.Initialise();

            //Now setup the Event Handlers
            vxNetworkManager.Client.DiscoverySignalResponseRecieved += ClientManager_DiscoverySignalResponseRecieved;

            //By Default, The Game will start looking for other networked games as a client.
            vxNetworkManager.Client.SetPlayerNetworkRole(vxEnumNetworkPlayerRole.Client);
            
            //Finally at the end, send out a pulse of discovery signals 
            vxConsole.WriteLine("Sending Discovery Signal...");

            SendDiscoverySignal();

            OKButton.IsEnabled = false;


                //ScrollPanel.AddItem(new vxScrol);
        }


        #region Handle GUI Events


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);


            Rectangle GUIBounds = ArtProvider.GUIBounds;


            CreateNewLocalServerButton.Height = OKButton.Height;
            CreateNewLocalServerButton.Position = new Vector2(
                ArtProvider.GUIBounds.Right - CreateNewLocalServerButton.Width, 
                ArtProvider.GUIBounds.Top);
        }


        public void GetHighlitedItem(object sender, vxGuiItemClickEventArgs e)
        {
            //foreach (vxServerListItem fd in List_Items)
            //{
            //    fd.UnSelect();
            //    fd.ToggleState = false;
            //}
            //e.GUIitem.ToggleState = true;

            foreach (var it in ScrollPanel.Items)
                it.ToggleState = false;

            e.GUIitem.ToggleState = true;

            currentlySelected = (vxServerListItem)e.GUIitem;
            //SelectedServerIp = item.ServerAddress;

            //List_Items[i].ThisSelect();
            //CurrentlySelected = i;

            //SelectedServerIp = List_Items[i].ServerAddress;

            OKButton.IsEnabled = true;
        }
        vxServerListItem currentlySelected;

            //string address = "localhost";
        public override void OnOKButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            var ip = currentlySelected.ServerAddress;
            var port = Convert.ToInt32(currentlySelected.ServerPort);
            //Connect to the Selected Server
            Connect(ip, port);

            //Now Add go to the Server Lobby. The Lobby info will be added in by the global Client Connection Object.
            OpenServerLobby();
            ExitScreen();
        }


        /// <summary>
        /// Connect the specified ipAddress, port and HailMsg.
        /// </summary>
        /// <param name="ipAddress">Ip address.</param>
        /// <param name="port">Port.</param>
        /// <param name="HailMsg">Hail message.</param>
        public void Connect(string ipAddress, int port)
        {
            vxNetworkManager.Client.LogClient(string.Format("Connecting to Server: {0} : {1}", ipAddress, port));

            NetOutgoingMessage approval = vxNetworkManager.Client.CreateMessage();
            approval.Write("secret");
            vxNetworkManager.Client.Connect(ipAddress, port, approval);

            vxNetworkManager.Client.LogClient("Done!");
        }

        /// <summary>
        /// Closes the Local Server Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnCancelButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            vxNetworkManager.Client.Disconnect();

            base.OnCancelButtonClicked(sender, e);
        }

        public override void OnApplyButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
            SendDiscoverySignal();
        }

        public override void UnloadContent()
        {
            //Now Deactivate all Event Handlers
            vxNetworkManager.Client.DiscoverySignalResponseRecieved -= ClientManager_DiscoverySignalResponseRecieved;

            base.UnloadContent();
        }
        #endregion


        private void ClientManager_DiscoverySignalResponseRecieved(object sender, vxNetClientEventDiscoverySignalResponse e)
        {
            AddDiscoveredServer(e.NetMsgServerInfo);
        }

        void SendDiscoverySignal()
        {
            List_Items.Clear();

            ScrollPanel.Clear();

            //TODO: increase port range (send out a 100 signals?)
            // Emit a discovery signal
            int port = DefaultServerPort;
            while (port < DefaultServerPort + PortRange)
            {
                vxNetworkManager.Client.SendDiscoverySignal(port);
                port++;
            }
        }

        int PortRange
        {
            get { return vxNetworkManager.Client.PortRange; }
        }


        int DefaultServerPort
        {
            get { return vxNetworkManager.Client.DefaultServerPort; }
        }

        string NewServerName = "Rob's Game";

        int NewServerMaxPlayers = 4;

        /// <summary>
        /// The event Fired when the user wants to create their own Local Server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Btn_CreateNewLocalServer_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            bool KeepLooping = true;


            int port = vxNetworkManager.Client.DefaultServerPort;

            while (KeepLooping)
            {
                try
                {
                    vxNetworkManager.Server.Initialise(port, NewServerName, NewServerMaxPlayers);


                    //Set the User's Network Roll to be Server.
                    vxNetworkManager.Client.SetPlayerNetworkRole(vxEnumNetworkPlayerRole.Server);

                    OpenServerLobby();
                    ExitScreen();

                    KeepLooping = false;
                }
                catch (Exception ex)
                {
                    if (port > DefaultServerPort + PortRange)
                        KeepLooping = false;
                    
                    vxConsole.WriteLine("Could not start Server on port: " + port);
                    vxConsole.WriteException(this, ex);

                    port++;
                }
            }
        }

        /// <summary>
        /// This Method is Called to Open the Server Lobby. If your game uses an inherited version of vxSeverLobbyDialog then
        /// you should override this function.
        /// </summary>
        public virtual void OpenServerLobby()
        {
            var lobby = new vxSeverLobbyDialog("Lobby");
            lobby.IsCustomButtonPosition = IsCustomButtonPosition;
            vxSceneManager.AddScene(lobby, PlayerIndex.One);
        }


        #region Client Networking Code

        protected virtual void AddDiscoveredServer(vxNetMsgServerInfo response)
        {
            Texture2D thumbnail = vxInternalAssets.LoadInternalTexture2D("Textures/gui/net/network_hub");
            vxServerListItem item = new vxServerListItem(Engine, response,
                                                         new Vector2(
                                                             (int)(2 * ArtProvider.Padding.X),
                                                             ArtProvider.Padding.Y + (ArtProvider.Padding.Y / 10 + 68) * (List_Items.Count + 1)),
        thumbnail,
        List_Items.Count)
            {
                //Set Item Width
                Width = ScrollPanel.Width - (int)(2 * this.ArtProvider.Padding.X) - ScrollPanel.ScrollBarWidth
            };

            //Set Clicked Event
            item.Clicked += GetHighlitedItem;

            //Add item too the list
            List_Items.Add(item);


            foreach (vxServerListItem it in List_Items)
                ScrollPanel.AddItem(it);

            ScrollPanel.UpdateItemPositions();
        }

        #endregion
    }
}
