
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;
//using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;

namespace VerticesEngine.Net.Messages
{
    /// <summary>
    /// This message is used during the discovery phase to glean basic server information.
    /// </summary>
    public class vxNetmsgUpdatePlayerRegInfo : INetworkMessage
    {

        /// <summary>
        /// The Server Name
        /// </summary>
        public vxNetPlayerInfo PlayerInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerinfo"></param>
        public vxNetmsgUpdatePlayerRegInfo(vxNetPlayerInfo playerinfo)
        {
            this.PlayerInfo = playerinfo;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetmsgUpdatePlayerRegInfo(NetIncomingMessage im)
        {
            PlayerInfo = new vxNetPlayerInfo(1.ToString(), "any", vxEnumNetPlayerStatus.ReadyToPlay);
            this.DecodeMsg(im);
        }

        /// <summary>
        /// The Message Type
        /// </summary>
        public vxNetworkMessageTypes MessageType
        {
            get
            {
                return vxNetworkMessageTypes.UpdatePlayerEntityState;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            //player ID
            PlayerInfo.ID = im.ReadInt64().ToString();
            PlayerInfo.PlayerIndex = im.ReadInt32();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            //player ID
            om.Write(PlayerInfo.ID);
            om.Write(PlayerInfo.PlayerIndex);
        }
    }
}
