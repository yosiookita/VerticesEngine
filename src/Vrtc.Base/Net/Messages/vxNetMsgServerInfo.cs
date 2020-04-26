
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
    public class vxNetMsgServerInfo : INetworkMessage
    {

        /// <summary>
        /// The Server Name
        /// </summary>
        public string ServerName { get; private set; }

        /// <summary>
        /// The Server's IP
        /// </summary>
        public string ServerIP { get; private set; }


        /// <summary>
        /// The Server's Port
        /// </summary>
        public string ServerPort { get; private set; }

        /// <summary>
        /// Gets the number of players.
        /// </summary>
        /// <value>The number of players.</value>
        public int NumberOfPlayers { get; private set; }

        /// <summary>
        /// Gets the max number of players.
        /// </summary>
        /// <value>The max number of players.</value>
        public int MaxNumberOfPlayers { get; private set; }

        /// <summary>
        /// Gets the ping.
        /// </summary>
        /// <value>The ping.</value>
        //public float Ping { get; private set; }

        //public object UserData { get; internal set; }

         /// <summary>
         /// Initializes a new instance of the <see cref="T:VerticesEngine.Net.Messages.vxNetMsgServerInfo"/> class.
         /// </summary>
         /// <param name="ServerName">Server name.</param>
         /// <param name="ServerIP">Server ip.</param>
         /// <param name="ServerPort">Server port.</param>
         /// <param name="NumberOfPlayers">Number of players.</param>
         /// <param name="MaxNumberOfPlayers">Max number of players.</param>
        public vxNetMsgServerInfo(string ServerName, string ServerIP, string ServerPort, int NumberOfPlayers, int MaxNumberOfPlayers)
        {
            this.ServerName = ServerName;
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.NumberOfPlayers = NumberOfPlayers;
            this.MaxNumberOfPlayers = MaxNumberOfPlayers;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetMsgServerInfo(NetIncomingMessage im)
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
                return vxNetworkMessageTypes.ServerInfo;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            //Console.WriteLine(im.SenderEndPoint.Address);
            //Console.WriteLine(im.SenderEndPoint.AddressFamily);
            //Console.WriteLine(im.SenderEndPoint.Port);
            this.ServerName = im.ReadString();
            this.ServerIP = im.SenderEndPoint.Address.ToString();//im.ReadString();
            Console.WriteLine(im.ReadString());
            this.ServerPort = im.SenderEndPoint.Port.ToString();//im.ReadString();
            Console.WriteLine(im.ReadString());

            this.NumberOfPlayers = im.ReadInt32();
            this.MaxNumberOfPlayers = im.ReadInt32();
                
            //Ping = im.SenderConnection.AverageRoundtripTime;
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            om.Write(this.ServerName);
            om.Write(this.ServerIP);
            om.Write(this.ServerPort);
            om.Write(this.NumberOfPlayers);
            om.Write(this.MaxNumberOfPlayers);
        }
    }
}
