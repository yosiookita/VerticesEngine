
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
    public class vxNetmsgServerShutdown : INetworkMessage
    {

        public string reason;
            /// <summary>
            /// Initialization Constructor to be used on Server Side.
            /// </summary>
            /// <param name="ServerName"></param>
            /// <param name="ServerIP"></param>
            /// <param name="ServerPort"></param>
        public vxNetmsgServerShutdown(string reason)
        {
            this.reason = reason;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetmsgServerShutdown(NetIncomingMessage im)
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
                return vxNetworkMessageTypes.ServerShutdown;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            reason = im.ReadString();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            om.Write(reason);
        }
    }
}
