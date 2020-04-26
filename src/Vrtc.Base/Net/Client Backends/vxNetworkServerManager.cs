
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VerticesEngine;
using VerticesEngine.Net.Events;
using VerticesEngine.Net.Messages;

namespace VerticesEngine.Net
{
    public class vxNetworkServer : INetworkManager
    {
        public NetPeerStatus ServerStatus
        {
            get { return netServer.Status; }
        }


        /// <summary>
        /// The Server Name
        /// </summary>
        public string ServerName { get; private set; }

        /// <summary>
        /// Gets the number of players.
        /// </summary>
        /// <value>The number of players.</value>
        public int NumberOfPlayers
        {
            get { return PlayerManager.Players.Count; }
        }

        /// <summary>
        /// Gets the max number of players.
        /// </summary>
        /// <value>The max number of players.</value>
        public int MaxNumberOfPlayers { get; private set; }

        #region Constants and Fields

        public vxEngine Engine;

        /// <summary>
        /// The NetPeer Server object
        /// </summary>
        public NetServer Server
        {
            get { return netServer; }
        }
        private NetServer netServer;

        /// <summary>
        /// The is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return isDisposed; }
        }
        private bool isDisposed;

        int defaultPort;


        /// <summary>
        /// Gets the session status.
        /// </summary>
        /// <value>The session status.</value>
        public vxEnumSessionStatus SessionStatus
        {
            get
            {
                return _sessionStatus;
            }
        }
        vxEnumSessionStatus _sessionStatus = vxEnumSessionStatus.WaitingForPlayers;

		public vxNetPlayerManager PlayerManager;


        SendOrPostCallback ServerCallBackLoop;

        #endregion

        #region Events
        
        /// <summary>
        /// This event fires when ever a Discovery Signal is Recieved
        /// </summary>
        public event EventHandler<vxNetServerEventDiscoverySignalRequest> DiscoverySignalRequestRecieved;

        /// <summary>
        /// This event fires whenever a new client connects.
        /// </summary>
        public event EventHandler<vxNetServerEventClientConnected> ClientConnected;

        /// <summary>
        /// This event fires whenever a new client disconnects.
        /// </summary>
        public event EventHandler<vxNetServerEventClientDisconnected> ClientDisconnected;


        /// <summary>
        /// This event updates the player status within the server.
        /// </summary>
        public event EventHandler<vxNetServerEventPlayerStatusUpdate> UpdatePlayerStatus;

        /// <summary>
        /// This Event fires when ever the server recieves updated information for an Entity State 
        /// from a client.
        /// </summary>
        public event EventHandler<vxNetServerEventPlayerStateUpdate> UpdatePlayerEntityState;

        #endregion

        public vxNetworkServer(vxEngine engine, int defaultPort)
        {
            this.Engine = engine;
            this.defaultPort = defaultPort;
            PlayerManager = new vxNetPlayerManager(Engine);

            ServerName = "Rob's Game";

            MaxNumberOfPlayers = 4;

			//Why? Bc Linux hates me.
			if (SynchronizationContext.Current == null)
				SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        #region Callback Loop

        private void ServerMsgCallBack(object peer)
        {
            NetIncomingMessage im;

            while ((im = this.Server.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        //LogServer(im.ReadString());
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        Console.WriteLine(im.ReadFloat());
                        //this.PlayerManager.Entites [im.SenderConnection.RemoteUniqueIdentifier].Ping = im.ReadFloat();

                        break;

                    /**************************************************************/
                    //Handle Discovery
                    /**************************************************************/
                    case NetIncomingMessageType.DiscoveryRequest:


                        LogServer(string.Format("     ------- Discovery Request Recieved from: {0}", im.SenderEndPoint));
                        vxNetServerEventDiscoverySignalRequest discoveryRequestEvent = new vxNetServerEventDiscoverySignalRequest(im.SenderEndPoint);

                        if (DiscoverySignalRequestRecieved != null)
                            DiscoverySignalRequestRecieved(this, discoveryRequestEvent);


                        if (discoveryRequestEvent.SendResponse == true)
                        {
                            // Create a response and write some example data to it
                            NetOutgoingMessage response = this.Server.CreateMessage();

                            //Send Back Connection Information, the client will still need to Authenticate with a Secret though                            
                            //response.Write(discoveryRequestEvent.Response);
                            var resp = new vxNetMsgServerInfo(ServerName, 
                                Server.Configuration.BroadcastAddress.ToString(), 
                                Server.Configuration.Port.ToString(), 
                                                              NumberOfPlayers, MaxNumberOfPlayers);

                            resp.EncodeMsg(response);

                            // Send the response to the sender of the request
                            this.Server.SendDiscoveryResponse(response, im.SenderEndPoint);
                        }
                        else
                        {
                            //The discovery response was blocked in the event. Notify on the server why but do not respond to the client.
                            LogServer(string.Format("\n--------------------------\nDISCOVERY REQUEST BLOCKED\n IPEndpoint: '{0}'\nREASON: '{1}' \n--------------------------\n", im.SenderEndPoint, discoveryRequestEvent.Response));
                        }


                        break;



                    /**************************************************************/
                    //Handle Connection Approval
                    /**************************************************************/
                    case NetIncomingMessageType.ConnectionApproval:
                        string s = im.ReadString();
                        if (s == "secret")
                        {
                            NetOutgoingMessage approve = Server.CreateMessage();
                            im.SenderConnection.Approve(approve);
                            LogServer(string.Format("{0} Connection Approved", im.SenderEndPoint));
                        }
                        else
                        {
                            im.SenderConnection.Deny();

                            LogServer(string.Format("{0} Connection DENIED!", im.SenderEndPoint));
                        }
                        break;


                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                LogServer(string.Format(" \t\t {0} Connected", im.SenderEndPoint));
                                
                                if (ClientConnected != null)
                                    ClientConnected(this, new vxNetServerEventClientConnected(im.SenderConnection.RemoteUniqueIdentifier));


                                //Now Update all clients with the Player list using the server Player List
                                var newPlayerList = new vxNetmsgUpdatePlayerList(this.PlayerManager);
                                SendMessage(newPlayerList);

                                break;
                            case NetConnectionStatus.Disconnected:
                                LogServer(string.Format("{0} Disconnected", im.SenderEndPoint));

                                //For what ever reason, a player has disconnected, so we need to remove it from the player list

                                //Send a message to all clients to remove this client from their list.
                                var id = "id" + im.SenderConnection.RemoteUniqueIdentifier.ToString();

                                if (PlayerManager.Players.ContainsKey(id))
                                {
                                    var rmvMsg = new vxNetmsgRemovePlayer(PlayerManager.Players[id]);

                                    //Send the message to all clients.
                                    SendMessage(rmvMsg);

                                    //Fire the Server Event for any outside events 
                                    if (ClientDisconnected != null)
                                        ClientDisconnected(this, new vxNetServerEventClientDisconnected(id));

                                    //Finally remove the player from the server's list
                                    if (PlayerManager.Players.ContainsKey(id))
                                        PlayerManager.Players.Remove(id);
                                }
                                break;
                            default:
                                LogServer(im.ReadString());
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        var gameMessageType = (vxNetworkMessageTypes)im.ReadByte();
                        switch (gameMessageType)
                        {
                            
                            /**************************************************************/
                            //Handle New Player Connection
                            /**************************************************************/
                            case vxNetworkMessageTypes.PlayerConnected:

                                //Decode the Message
                                var newPlayer = new vxNetmsgAddPlayer(im);

                                //Add the new player info to the Server List
                                newPlayer.PlayerInfo.SetID("id"+im.SenderConnection.RemoteUniqueIdentifier.ToString());
                                PlayerManager.Add(newPlayer.PlayerInfo);

                                newPlayer.PlayerInfo.Status = vxEnumNetPlayerStatus.InServerLobbyNotReady;

                                //Now Update all clients with the Player list using the server Player List
                                var newPlayerList = new vxNetmsgUpdatePlayerList(this.PlayerManager);
                                SendMessage(newPlayerList);

                                break;



                            /**************************************************************/
                            //Handle Player Lobby Status Update
                            /**************************************************************/
                            case vxNetworkMessageTypes.UpdatePlayerLobbyStatus:

                                //Decode the list
                                var updatePlayerState = new vxNetmsgUpdatePlayerLobbyStatus(im);
                                
                                //Update the internal server list
                                PlayerManager.Players[updatePlayerState.PlayerInfo.ID] = updatePlayerState.PlayerInfo;

                                //Now resend it back to all clients
                                SendMessage(updatePlayerState);

                                if(SessionStatus == vxEnumSessionStatus.WaitingForPlayers)
                                {
                                    bool startServerGame = true;

                                    // Now Loop through all players
                                    foreach(var player in PlayerManager.Players)
                                    {
                                        if(player.Value.Status != vxEnumNetPlayerStatus.ReadyToPlay)
                                        {
                                            startServerGame = false;
                                        }
                                    }

                                    if(startServerGame)
                                    {
                                        SendMessage(new vxNetmsgBeginGame("match begin"));
                                    }
                                }


                                if (UpdatePlayerStatus != null)
                                    UpdatePlayerStatus(this, new vxNetServerEventPlayerStatusUpdate(updatePlayerState.PlayerInfo));
                                break;



                            /**************************************************************/
                            //Handles an Update to a Player's Entity State
                            /**************************************************************/
                            case vxNetworkMessageTypes.UpdatePlayerEntityState:
                                
                                //First decode the message
                                var updatePlayerEntityState = new vxNetmsgUpdatePlayerEntityState(im);
                                
                                //get the time delay
                                var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(updatePlayerEntityState.MessageTime));
                                
                                //Then Update the Player in the Server List
                                PlayerManager.Players[updatePlayerEntityState.PlayerInfo.ID] = updatePlayerEntityState.PlayerInfo;
                                
                                //Then fire any tied events in the server
                                if (UpdatePlayerEntityState != null)
                                    UpdatePlayerEntityState(this, new vxNetServerEventPlayerStateUpdate(updatePlayerEntityState.PlayerInfo));

                                
                                //Finally, relay the updated state too all clients.
                                SendMessage(updatePlayerEntityState);

                                break;


                            case vxNetworkMessageTypes.AddItem:

                                break;

                            case vxNetworkMessageTypes.AddItem2D:
                                // rebroadcast
                                SendMessage(new vxNetMsg2DEntityAdded(im));

                                break;

                                // Update 2D Entity
                            case vxNetworkMessageTypes.vxNetmsgUpdate2DEntityState:
                                // rebroadcast
                                SendMessage(new vxNetMsg2DEntityStateUpdate(im));

                                break;

                            case vxNetworkMessageTypes.RemoveItem:

                                break;

                            case vxNetworkMessageTypes.Other:

                                break;

                        }
                        break;


                    default:
                        break;
                }

                this.Server.Recycle(im);
            }
        }

        #endregion
        
        #region Public Methods and Operators

        /// <summary>
        /// The connect.
        /// </summary>
        public virtual void Initialise()
        {
            Initialise(defaultPort, "Rob's Game", 4);
        }

        public virtual void Initialise(int port, string name, int maxPlayers)
        {
            var config = new NetPeerConfiguration(this.Engine.GameName)
            {
                Port = port,// Convert.ToInt32("14242"),
                 //SimulatedMinimumLatency = 0.02f, 
                // SimulatedLoss = 0.1f 
            };
            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.Error);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            this.netServer = new NetServer(config);

            ServerCallBackLoop = new SendOrPostCallback(ServerMsgCallBack);
            this.netServer.RegisterReceivedCallback(ServerCallBackLoop);
        }

        public void Start()
        {
            this.netServer.Start();
        }

        /// <summary>
        /// The create message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetOutgoingMessage CreateMessage()
        {
            

            return this.netServer.CreateMessage();
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
            if(netServer != null)
            this.netServer.Shutdown("Server Shut Down");
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// The read message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetIncomingMessage ReadMessage()
        {
            return this.netServer.ReadMessage();
        }

        /// <summary>
        /// The recycle.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public void Recycle(NetIncomingMessage im)
        {
            this.netServer.Recycle(im);
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="gameMessage">
        /// The game message.
        /// </param>
        public void SendMessage(INetworkMessage gameMessage)
        {
            NetOutgoingMessage om = this.netServer.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.EncodeMsg(om);

            this.netServer.SendToAll(om, NetDeliveryMethod.ReliableUnordered);
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

            if(Server != null)
            Server.Shutdown("shutdown");
        }


        public void LogServer(string log)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("SERVER >> " + log);
            Console.ResetColor();
        }

        public void LogServerError(string log)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("SERVER >> " + log);
            Console.ResetColor();
        }

        #endregion
    }
}
