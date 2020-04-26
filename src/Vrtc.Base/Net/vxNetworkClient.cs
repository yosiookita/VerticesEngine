using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using VerticesEngine;

using VerticesEngine.Net.Events;
using VerticesEngine.Net.Messages;
using Lidgren.Network;

namespace VerticesEngine.Net
{
    public enum vxNetworkBackend
    {
        CrossPlatform,
        Android,
        iOS,
        Steam
    }

    public enum vxNetConnectionStatus
    {
        None,
        InitiatedConnect,
        ReceivedInitiation,
        RespondedAwaitingApproval,
        RespondedConnect,
        Connected,
        Disconnecting,
        Disconnected
    }


    /// <summary>
    /// This acts as a funneling proxy for handling different Networking Backends so that it is opaque to the rest of the engine how
    /// net code is handled between different systems or chosen backends
    /// </summary>
    public class vxNetworkClient
    {
        #region -- Different Client Backends --
        
        vxLidgrenNetworkClientBackend LidgrenClientManager;

        #endregion

        #region -- Fields and Properties --

        /// <summary>
        /// Player Info
        /// </summary>
        public vxNetPlayerInfo PlayerInfo;

        public string UniqueID
        {
            get { return PlayerInfo.ID; }
        }



        /// <summary>
        /// The player manager
        /// </summary>
        public vxNetPlayerManager PlayerManager { get; internal set; }



        /// <summary>
        /// The current session status
        /// </summary>
        public vxEnumSessionStatus SessionStatus = vxEnumSessionStatus.WaitingForPlayers;

        /// <summary>
        /// The Default Server Port
        /// </summary>
        public int DefaultServerPort = 14242;

        /// <summary>
        /// Default Port Range
        /// </summary>
        public int PortRange = 20;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:VerticesEngine.Net.vxNetworkSessionManager"/>
        /// 's match is running.
        /// </summary>
        /// <value><c>true</c> if is match running; otherwise, <c>false</c>.</value>
        public bool IsMatchRunning { get; internal set; }

        /// <summary>
        /// Gets the network backend.
        /// </summary>
        /// <value>The network backend.</value>
        public vxNetworkBackend NetworkBackend
        {
            get { return _networkBackend; }
        }
        vxNetworkBackend _networkBackend = vxNetworkBackend.CrossPlatform;



        /// <summary>
        /// Gets the player network role.
        /// </summary>
        /// <returns>The player network role.</returns>
        public vxEnumNetworkPlayerRole PlayerNetworkRole
        {
            get { return playerNetworkRole; }
        }


        /// <summary>
        /// Sets the player network role.
        /// </summary>
        /// <param name="role">Role.</param>
        public void SetPlayerNetworkRole(vxEnumNetworkPlayerRole role)
        {
            playerNetworkRole = role;
        }
        vxEnumNetworkPlayerRole playerNetworkRole = vxEnumNetworkPlayerRole.Client;
        
        #endregion

        #region -- Net Events --

        /// <summary>
        /// This event is fired on the client side whenever a new player connects to the server.
        /// </summary>
        public event EventHandler<vxNetClientEventDiscoverySignalResponse> DiscoverySignalResponseRecieved;

        internal void OnDiscoverySignalResponseRecieved(vxNetMsgServerInfo info)
        {
            if (DiscoverySignalResponseRecieved != null)
                DiscoverySignalResponseRecieved(this, new vxNetClientEventDiscoverySignalResponse(info));

        }

        /// <summary>
        /// This event is fired whenever this player connects to the server.
        /// </summary>
        public event EventHandler<vxNetClientEventConnected> Connected;

        internal void OnServerConnected()
        {
            if (Connected != null)
                Connected(this, new vxNetClientEventConnected());
        }



        /// <summary>
        /// This event is fired whenever this player disconnects from the server.
        /// </summary>
        public event EventHandler<vxNetClientEventDisconnected> Disconnected;

        internal void OnServerDisconnected(string Reason)
        {
            if (Disconnected != null)
                Disconnected(this, new vxNetClientEventDisconnected(Reason));
        }


        /// <summary>
        /// This event is fired whenever this player disconnects from the server.
        /// </summary>
        public event EventHandler<EventArgs> MatchStart;

        internal void OnMatchStart()
        {
            IsMatchRunning = true;
            if (MatchStart != null)
                MatchStart(this, new EventArgs());
        }


        internal void OnMatchEnd()
        {
            IsMatchRunning = false;
            if (MatchStart != null)
                MatchStart(this, new EventArgs());
        }


        /// <summary>
        /// This event is fired on the client side whenever a new player connects to the server.
        /// </summary>
        public event EventHandler<vxNetClientEventPlayerConnected> OtherPlayerConnected;

        internal void OnOtherPlayerConnected(vxNetPlayerInfo newPlayerInfo)
        {
            if (OtherPlayerConnected != null)
                OtherPlayerConnected(this, new vxNetClientEventPlayerConnected(newPlayerInfo));
        }




        /// <summary>
        /// This event is fired on the client side whenever a player disconnects from the server.
        /// </summary>
        public event EventHandler<vxNetClientEventPlayerDisconnected> OtherPlayerDisconnected;

        internal void OnOtherPlayerDisconnected(vxNetPlayerInfo playerInfo)
        {
            LogClient("Player Disconnected - id:" + playerInfo.ID);

            if (OtherPlayerDisconnected != null)
                OtherPlayerDisconnected(this, new vxNetClientEventPlayerDisconnected(playerInfo));


            // Remove the player from the server's list
            if (PlayerManager.Players.ContainsKey(playerInfo.ID))
                PlayerManager.Players.Remove(playerInfo.ID);
        }

        /// <summary>
        /// When ever new information of a player is recieved.
        /// </summary>
        public event EventHandler<vxNetClientEventPlayerStatusUpdate> UpdatedPlayerInfoRecieved;

        internal void OnUpdatedPlayerInfoRecieved(vxNetPlayerInfo playerInfo)
        {
            // Update The Player info
            if (PlayerManager.Players.ContainsKey(playerInfo.ID))
                PlayerManager.Players[playerInfo.ID] = playerInfo;

            if (UpdatedPlayerInfoRecieved != null)
                UpdatedPlayerInfoRecieved(this, new vxNetClientEventPlayerStatusUpdate(playerInfo));


        }
        //Players[updatePlayerState.PlayerInfo.ID] = updatePlayerState.PlayerInfo;

        /// <summary>
        /// When the server updates the session status.
        /// </summary>
        public event EventHandler<vxNetClientEventSessionStatusUpdated> UpdateSessionStatus;

        internal void OnUpdateSessionStatus(vxEnumSessionStatus newStatus)
        {
            if (UpdateSessionStatus != null)
                UpdateSessionStatus(this, new vxNetClientEventSessionStatusUpdated(SessionStatus, newStatus));

            SessionStatus = newStatus;

        }

        /// <summary>
        /// This event fires when an updated Entity State is recieved from the Server.
        /// </summary>
        public event EventHandler<vxNetClientEventPlayerStateUpdate> UpdatePlayerEntityState;

        internal void OnUpdatePlayerEntityState(vxNetmsgUpdatePlayerEntityState updatePlayerEntityState, float timeDelay)
        {
            if (this.PlayerInfo.ID != updatePlayerEntityState.PlayerInfo.ID)
            {
                //Then Update the Player in the client List
                PlayerManager.Players[updatePlayerEntityState.PlayerInfo.ID] = updatePlayerEntityState.PlayerInfo;

                //Then fire any connected events
                if (UpdatePlayerEntityState != null)
                    UpdatePlayerEntityState(this, new vxNetClientEventPlayerStateUpdate(updatePlayerEntityState, timeDelay));
            }
        }


        public event EventHandler<vxNetEvent2DEntityAdded> Entity2DAdded;

        internal void OnEntity2DAdded(vxNetMsg2DEntityAdded msg, float timeDelay)
        {
            if (Entity2DAdded != null)
                Entity2DAdded(this, new vxNetEvent2DEntityAdded(msg, timeDelay));
        }


        public event EventHandler<vxNetEvent2DEntityStateUpdate> Entity2DStateUpdated;

        internal void OnUpdate2DEntityState(vxNetMsg2DEntityStateUpdate msg, float timeDelay)
        {
            if (this.PlayerInfo.ID != msg.id)
            {
                //Then fire any connected events
                if (Entity2DStateUpdated != null)
                    Entity2DStateUpdated(this, new vxNetEvent2DEntityStateUpdate(msg, timeDelay));
            }
        }

        #endregion


        public void Reset()
        {
            
        }

        public void Start(vxNetworkBackend networkBackend)
        {
            // first reset the Network Manager
            Reset();

            // Now set the new one
            _networkBackend = networkBackend;
        }


        public vxEngine Engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Net.vxNetworkClient"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        public vxNetworkClient(vxEngine Engine) 
        {
            this.Engine = Engine;
            PlayerManager = new vxNetPlayerManager(Engine);
            IsMatchRunning = false;
        }

        public void Initialise()
        {
            LidgrenClientManager = new vxLidgrenNetworkClientBackend(this);
            LidgrenClientManager.Initialise();

            List<string> UserNames = new List<string>();
            UserNames.Add("Kilijette");
            UserNames.Add("Infamousth");
            UserNames.Add("IteMon");
            UserNames.Add("Lauthro");
            UserNames.Add("Fantilis");
            UserNames.Add("Gramtron");
            UserNames.Add("HappyCent");
            UserNames.Add("Alchend");
            UserNames.Add("Deantingki");
            UserNames.Add("Sereneson");
            UserNames.Add("RocketSoftMax");
            UserNames.Add("Gurligerzo");
            UserNames.Add("Gentagou");
            UserNames.Add("MagicTinV2");
            UserNames.Add("Delkerhe");
            UserNames.Add("FelineTimes");
            UserNames.Add("Goldersear");
            UserNames.Add("Instanteg");
            UserNames.Add("Microckst");

            Random rand = new Random(DateTime.Now.Second);
            string UserName = UserNames[rand.Next(0, UserNames.Count)];

            long id = DateTime.Now.Ticks;
            Console.WriteLine(id);
            PlayerInfo = new vxNetPlayerInfo(id.ToString(), UserName, vxEnumNetPlayerStatus.None);
        }


        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
          switch(_networkBackend)
            {
                case vxNetworkBackend.CrossPlatform:
                    LidgrenClientManager.Disconnect();
                    break;
            }


            // Shutdown the server
            if (PlayerNetworkRole == vxEnumNetworkPlayerRole.Server)
            {
                //SendMessage(new vxNetmsgServerShutdown("Audiaos"));

                // wait for any client side disconnect code to run.
                Thread.Sleep(200);

                vxNetworkManager.Server.Disconnect();
            }


            // Finally, Clear all Players
            PlayerManager.Players.Clear();
        }


        public void Dispose()
        {
            if(LidgrenClientManager != null)
                LidgrenClientManager.Dispose();
        }

        public void SendDiscoverySignal(int port)
        {
            LidgrenClientManager.SendDiscoverySignal(port);
        }

        public NetOutgoingMessage CreateMessage()
        {
            return LidgrenClientManager.CreateMessage();
        }

        public void Connect(string Address, int Port)
        {
            LidgrenClientManager.Connect(Address, Port);
        }

        /// <summary>
        /// Connect with a hail message
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Port"></param>
        public void Connect(string Address, int Port, NetOutgoingMessage hail)
        {
            LidgrenClientManager.Connect(Address, Port, hail);
        }

        public void SendMessage(INetworkMessage gameMessage)
        {
            switch (_networkBackend)
            {
                case vxNetworkBackend.CrossPlatform:
                    if(LidgrenClientManager != null)
                        LidgrenClientManager.SendMessage(gameMessage);
                    break;
            }
        }


        public void InviteFriend()
        {
            // TODO: Implement For Steam
        }

        public void OnAllPlayersReady()
        {
            
        }

        public void OnPlayerDisconnect()
        {

        }

        public void OnPlayerInvite()
        {
            // TODO: Implement For Steam
        }

        public void OnPlayerJoined()
        {

        }

        public void StartGame()
        {

        }

        public void StartQuickGame()
        {

        }


        public void OnMessageReceived(object msg)
        {

        }

        public void Draw()
        {
            switch (_networkBackend)
            {
                case vxNetworkBackend.CrossPlatform:
                    if(LidgrenClientManager != null)
                        LidgrenClientManager.Draw();
                    break;
            }
        }


        public void LogClient(string log)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CLIENT >> " + log);
            Console.ResetColor();
        }

        public void LogClientError(string log)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("CLIENT >> " + log);
            Console.ResetColor();
        }
    }
}