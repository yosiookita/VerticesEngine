using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using VerticesEngine.Net.Messages;
using Microsoft.Xna.Framework;

namespace VerticesEngine.Net.Events
{
    /// <summary>
    /// This event is fired whenever a discovery response is recieved from a server.
    /// </summary>
    public class vxNetClientEventDiscoverySignalResponse : EventArgs
    {
        public vxNetMsgServerInfo NetMsgServerInfo
        {
            get { return m_vxNetMsgServerInfo; }
        }
        vxNetMsgServerInfo m_vxNetMsgServerInfo;

        /// <summary>
        /// The address of where the Discovery Signal originates from.
        /// </summary>
        public string Address
        {
            get { return m_vxNetMsgServerInfo.ServerIP.ToString(); }
        }

        /// <summary>
        /// The port of where the Discovery Signal originates from.
        /// </summary>
        public int Port
        {
            get { return Convert.ToInt32(m_vxNetMsgServerInfo.ServerPort); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetClientEventDiscoverySignalResponse(vxNetMsgServerInfo NetMsgServerInfo)
        {
            this.m_vxNetMsgServerInfo = NetMsgServerInfo;
        }
    }


    /// <summary>
    /// This event is fired whenever this player connects to the server.
    /// </summary>
    public class vxNetClientEventConnected : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetClientEventConnected()
        {
            
        }
    }


    /// <summary>
    /// This event is fired whenever this player disconnects from the server.
    /// </summary>
    public class vxNetClientEventDisconnected : EventArgs
    {
        public string Reason;

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetClientEventDisconnected(string Reason)
        {
            this.Reason = Reason;
        }
    }


    /// <summary>
    /// This event is fired on the client side whenever a new player connects to the server.
    /// </summary>
    public class vxNetClientEventPlayerConnected : EventArgs
    {
        /// <summary>
        /// Information pertaining to the New Connected Player.
        /// </summary>
        public vxNetPlayerInfo ConnectedPlayer
        {
            get { return m_connectedPlayer; }
        }
        vxNetPlayerInfo m_connectedPlayer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetClientEventPlayerConnected(vxNetPlayerInfo player)
        {
            m_connectedPlayer = player;
        }
    }
    

    /// <summary>
    /// This event is fired on the client side whenever a player disconnects from the server.
    /// </summary>
    public class vxNetClientEventPlayerDisconnected : EventArgs
    {
        /// <summary>
        /// A copy of information pertaining to the disconnected player. The player is still in the PlayerManager until after
        /// this Event is fired.
        /// </summary>
        public vxNetPlayerInfo DisconnectedPlayer
        {
            get { return m_disconnectedPlayer; }
        }
        vxNetPlayerInfo m_disconnectedPlayer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetClientEventPlayerDisconnected(vxNetPlayerInfo player)
        {
            m_disconnectedPlayer = player;
        }
    }


    /// <summary>
    /// This event is fired on the client side whenever a player needs to be updated with information from the server.
    /// </summary>
    public class vxNetClientEventPlayerStatusUpdate : EventArgs
    {
        /// <summary>
        /// The player that needs updating.
        /// </summary>
        public vxNetPlayerInfo PlayerToUpdate
        {
            get { return m_playerToUpdate; }
        }
        vxNetPlayerInfo m_playerToUpdate;

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetClientEventPlayerStatusUpdate(vxNetPlayerInfo player)
        {
            m_playerToUpdate = player;
        }
    }


    /// <summary>
    /// This event is fired whenever a client disconnects to this server.
    /// </summary>
    public class vxNetClientEventSessionStatusUpdated : EventArgs
    {
        /// <summary>
        /// ID of the Client that has been added.
        /// </summary>
        public vxEnumSessionStatus NewSessionStatus;

        public vxEnumSessionStatus PreviousSessionStatus;

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetClientEventSessionStatusUpdated(vxEnumSessionStatus oldStatus, vxEnumSessionStatus newStatus)
        {
            NewSessionStatus = newStatus;

            PreviousSessionStatus = oldStatus;
        }
    }

    /// <summary>
    /// This event is fired on the client side whenever a player needs to be updated with information from the server.
    /// </summary>
    public class vxNetClientEventPlayerStateUpdate : EventArgs
    {
        /// <summary>
        /// The player that needs updating.
        /// </summary>
        public vxNetPlayerInfo PlayerToUpdate
        {
            get { return m_playerToUpdate; }
        }
        vxNetPlayerInfo m_playerToUpdate;

        /// <summary>
        /// The time difference between when the message was sent and when it was recieved by the recieving client.
        /// </summary>
        public float TimeDelay
        {
            get { return m_timeDelay; }
        }
        float m_timeDelay;

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetClientEventPlayerStateUpdate(vxNetmsgUpdatePlayerEntityState updateMsg, float delay)
        {
            m_playerToUpdate = updateMsg.PlayerInfo;


            m_timeDelay = delay;
        }
    }




    /// <summary>
    /// This event is fired on the client side whenever an object needs to be added.
    /// </summary>
    public class vxNetEvent2DEntityAdded : EventArgs
    {
        public vxNetEntity2D Entity2D;

        public float TimeDelay
        {
            get { return m_timeDelay; }
        }
        float m_timeDelay;

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetEvent2DEntityAdded(vxNetMsg2DEntityAdded msg, float delay)
        {
            Entity2D = msg.Entity2D;

            m_timeDelay = delay;
        }
    }



    /// <summary>
    /// This event is fired on the client side whenever a player needs to be updated with information from the server.
    /// </summary>
    public class vxNetEvent2DEntityStateUpdate : EventArgs
    {
        /// <summary>
        /// The player that needs updating.
        /// </summary>
        public string id;

        public Vector2 Position;

        public float Rotation;

        public Vector2 Velocity;

        public Vector2 Direction;

        /// <summary>
        /// The time difference between when the message was sent and when it was recieved by the recieving client.
        /// </summary>
        public float TimeDelay
        {
            get { return m_timeDelay; }
        }
        float m_timeDelay;

        /// <summary>
        /// Constructor.
        /// </summary>
        public vxNetEvent2DEntityStateUpdate(vxNetMsg2DEntityStateUpdate msg, float delay)
        {
            id = msg.id;

            Position = msg.Position;

            Rotation = msg.Rotation;

            Velocity = msg.Velocity;

            Direction = msg.Direction;

            m_timeDelay = delay;
        }
    }
}
