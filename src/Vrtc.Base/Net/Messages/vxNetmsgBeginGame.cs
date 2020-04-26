
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Net;

namespace VerticesEngine.Net.Messages
{
    /// <summary>
    /// This message is used during the discovery phase to glean basic server information.
    /// </summary>
    public class vxNetmsgBeginGame : INetworkMessage
    {
        public string MessageData = "";
        
       /// <summary>
       /// Initializes a new instance of the <see cref="T:VerticesEngine.Net.Messages.vxNetmsgBeginGame"/> class.
       /// </summary>
       /// <param name="MessageData">Message data.</param>
        public vxNetmsgBeginGame(string MessageData)
        {
            this.MessageData = MessageData;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetmsgBeginGame(NetIncomingMessage im)
        {
            this.DecodeMsg(im);
        }

        /// <summary>
        /// The Message Type
        /// </summary>
        public vxNetworkMessageTypes MessageType
        {
            get
            {
                return vxNetworkMessageTypes.BeginGame;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            MessageData = im.ReadString();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            om.Write(MessageData);
        }
    }
}
