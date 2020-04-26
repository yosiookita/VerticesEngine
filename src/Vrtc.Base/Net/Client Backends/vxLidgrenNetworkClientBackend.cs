
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using VerticesEngine;
using VerticesEngine.Mathematics;
using VerticesEngine.Net.Events;
using VerticesEngine.Net.Messages;
using VerticesEngine.Utilities;

namespace VerticesEngine.Net
{
    /// <summary>
    /// The general network client manager which uses Lidgren as the back end.
    /// </summary>
    public class vxLidgrenNetworkClientBackend : INetworkManager
    {
        #region Constants and Fields

        vxEngine Engine
        {
            get
            {
                return SessionManager.Engine;
            }
        }


        /// <summary>
        /// The NetPeer Server object
        /// </summary>
        private NetClient netClient;

        /// <summary>
        /// The is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return isDisposed; }
        }
        private bool isDisposed;



        SendOrPostCallback ClientCallBackLoop;

        public string UserName;

        #endregion


        vxNetworkClient SessionManager;

        public vxLidgrenNetworkClientBackend(vxNetworkClient sessionManager)
        {
            //this.Engine = engine;

            SessionManager = sessionManager;

            

			//Why? Bc Linux hates me.
			if (SynchronizationContext.Current == null)
				SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());


            //PlayerManager = new vxNetPlayerManager(this.Engine);
            var config = new NetPeerConfiguration(this.Engine.GameName)
            {
                //SimulatedMinimumLatency = 0.2f,
                // SimulatedLoss = 0.1f
            };

            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.Error);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            //config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);

            netClient = new NetClient(config);


            ClientCallBackLoop = new SendOrPostCallback(ClientMsgCallback);
            netClient.RegisterReceivedCallback(ClientCallBackLoop);
        }

        public void Initialise()
        {
            netClient.Start();
        }

        public string UniqueIdentifier
        {
            get
            {
                return "id"+netClient.UniqueIdentifier.ToString();
            }
        }
        
        #region Callback Loop

        /// <summary>
		/// Method for Receiving Messages kept in a Global Scope for the Engine.
		/// </summary>
		/// <param name="peer">Peer.</param>
		void ClientMsgCallback(object peer)
        {
            NetIncomingMessage im;

            while ((im = netClient.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        //LogClient("DEBUG: " + im.ReadString());
                        break;
                    /**************************************************************/
                    //DiscoveryResponse
                    /**************************************************************/
                    case NetIncomingMessageType.DiscoveryResponse:

                        SessionManager.LogClient("     ------- Server found at: " + im.SenderEndPoint);
                        //Console.w im.SenderEndPoint.Address
                        //Fire the RecieveDiscoverySignalResponse Event by passing down the decoded Network Message
                        SessionManager.OnDiscoverySignalResponseRecieved(new vxNetMsgServerInfo(im));
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                {
                                    SessionManager.LogClient(string.Format("{0} Connected", im.SenderEndPoint));

                                    //Fire the Connected Event
                                    SessionManager.OnServerConnected();

                                    SessionManager.PlayerInfo.SetID(UniqueIdentifier);
                                }
                                break;
                            case NetConnectionStatus.Disconnected:
                                {
                                    SessionManager.LogClient(string.Format("{0} Disconnected", im.SenderEndPoint));
                                    //Console.WriteLine();

                                    string reason = im.ReadString();
                                    //Console.WriteLine(im.read);
                                    //Fire the Connected Event
                                    SessionManager.OnServerDisconnected(reason);
                                }
                                break;
                            default:
                                SessionManager.LogClient(im.ReadString());
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        var gameMessageType = (vxNetworkMessageTypes)im.ReadByte();
                        switch (gameMessageType)
                        {
                            case vxNetworkMessageTypes.UpdatePlayersList:

                                //Get the Message Containing the Updated Player List
                                var updatedPlayerList = new vxNetmsgUpdatePlayerList(im);

                                //Now Loop through each player in the list
                                foreach(vxNetPlayerInfo serverPlayer in updatedPlayerList.Players)
                                {
                                    //First Check if the Server Player is in the clients list. If not, then add the server player to the clients list.
                                    if (SessionManager.PlayerManager.Contains(serverPlayer))
                                    {
                                        SessionManager.PlayerManager.Players[serverPlayer.ID] = serverPlayer;
                                    }
                                    else
                                    {   
                                        //Add Player to Player manager
                                        SessionManager.PlayerManager.Add(serverPlayer);

                                        //Now Fire the Event Handler
                                        SessionManager.OnOtherPlayerConnected(serverPlayer);
                                    }
                                }
                                break;

                            case vxNetworkMessageTypes.PlayerDisconnected:

                                //For what ever reason, a player has disconnected, so we need to remove it from the player list
                                var rmvMsg = new vxNetmsgRemovePlayer(im);

                                //Fire the Disconnected Event
                                SessionManager.OnOtherPlayerDisconnected(rmvMsg.PlayerInfo);

                                break;

                            case vxNetworkMessageTypes.UpdatePlayerLobbyStatus:

                                //Decode the list
                                var updatePlayerState = new vxNetmsgUpdatePlayerLobbyStatus(im);

                                //Update the internal server list
                                //PlayerManager.Players[updatePlayerState.PlayerInfo.ID] = updatePlayerState.PlayerInfo;
                                SessionManager.OnUpdatedPlayerInfoRecieved(updatePlayerState.PlayerInfo);
    
                                break;

                            case vxNetworkMessageTypes.SessionStatus:
                                var newSessionStatus = new vxNetmsgUpdateSessionStatus(im);

                                //Set the new Session Status
                                SessionManager.OnUpdateSessionStatus(newSessionStatus.SessionStatus);
                                break;


                            case vxNetworkMessageTypes.UpdatePlayerEntityState:
                                
                                //First decode the message
                                var updatePlayerEntityState = new vxNetmsgUpdatePlayerEntityState(im);

                                var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(updatePlayerEntityState.MessageTime));

                                SessionManager.OnUpdatePlayerEntityState(updatePlayerEntityState, timeDelay);

                                break;

                            case vxNetworkMessageTypes.BeginGame:
                                var beginMatch = new vxNetmsgBeginGame(im);

                                //Console.WriteLine(beginMatch.MessageData);
                                SessionManager.OnMatchStart();
                                break;

                            case vxNetworkMessageTypes.AddItem2D:
                                var newEntity2DMsg = new vxNetMsg2DEntityAdded(im);

                                var newEntity2DTimeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(newEntity2DMsg.MessageTime));

                                SessionManager.OnEntity2DAdded(newEntity2DMsg, newEntity2DTimeDelay);

                                break;

                            case vxNetworkMessageTypes.RemoveItem:
                                
                                break;

                            case vxNetworkMessageTypes.Other:
                                
                                break;




                                // Update 2D Entity
                            case vxNetworkMessageTypes.vxNetmsgUpdate2DEntityState:

                                //Decode the entity state
                                var entityState = new vxNetMsg2DEntityStateUpdate(im);
                                var entity2DTimeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(entityState.MessageTime));
                                SessionManager.OnUpdate2DEntityState(entityState, entity2DTimeDelay);

                                break;
                        }
                        break;
                }
                Recycle(im);
            }
        }

        #endregion


        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        public void Connect(string Address, int Port)
        {
            this.netClient.Connect(new IPEndPoint(NetUtility.Resolve(Address), Port));
        }

        /// <summary>
        /// Connect with a hail message
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Port"></param>
        public void Connect(string Address, int Port, NetOutgoingMessage hail)
        {
            this.netClient.Connect(new IPEndPoint(NetUtility.Resolve(Address), Port), hail);
        }

        /// <summary>
        /// The create message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetOutgoingMessage CreateMessage()
        {
            return netClient.CreateMessage();
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
            
            netClient.Disconnect("Client Disconnected");
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// The read message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetIncomingMessage ReadMessage()
        {
            return netClient.ReadMessage();
        }

        /// <summary>
        /// The recycle.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public void Recycle(NetIncomingMessage im)
        {
            this.netClient.Recycle(im);
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="gameMessage">
        /// The game message.
        /// </param>
        public void SendMessage(INetworkMessage gameMessage)
        {
            NetOutgoingMessage om = this.netClient.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.EncodeMsg(om);

            this.netClient.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
        }


        /// <summary>
        /// Emit a discovery signal
        /// </summary>
        public void SendDiscoverySignal(int port)
        {
            netClient.DiscoverLocalPeers(port);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Disconnect();
                }

                this.isDisposed = true;
            }

            netClient.Disconnect("client dispose");
            netClient.Shutdown("client shutdown ");
        }



        int local = -50;
        int Req = -50;
        public void Draw()
        {
            if (netClient != null)
            {
                Req = 5;
                local = vxMathHelper.Smooth(local, Req, 8);
                string output = string.Format(
                    "NETWORK DEBUG INFO: | User Roll: {3} | Client Name: {0} | Port: {1} | Broadcast Address: {2} | Status: {4}",
                    netClient.Configuration.AppIdentifier,
                    netClient.Configuration.Port.ToString(),
                    netClient.Configuration.BroadcastAddress,
                    SessionManager.PlayerNetworkRole.ToString(),
                    netClient.Status.ToString());

                int pad = 3;
            
            Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank, new Rectangle(0, local + 0, 1000, (int)vxInternalAssets.Fonts.DebugFont.MeasureString(output).Y + 2 * pad), Color.Black * 0.75f);
            Engine.SpriteBatch.DrawString(vxInternalAssets.Fonts.DebugFont, output, new Vector2(pad, local + pad), Color.White);
            }
        }

        #endregion
    }
}
