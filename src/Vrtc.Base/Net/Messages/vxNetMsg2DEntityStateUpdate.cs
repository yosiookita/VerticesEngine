
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
    public class vxNetMsg2DEntityStateUpdate : INetworkMessage
    {
        /// <summary>
        /// The Server Name
        /// </summary>
        public string id;

        public Vector2 Position;

        public float Rotation;

        public Vector2 Velocity;

        public Vector2 Direction;

        /// <summary>
        /// Gets or sets MessageTime to help with interpolating the actual position after lag.
        /// </summary>
        public double MessageTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerinfo"></param>
        public vxNetMsg2DEntityStateUpdate(string id, Vector2 pos, float rot, Vector2 vel, Vector2 dir)
        {
            this.id = id;
            Position = pos;
            Rotation = rot;
            Velocity = vel;
            Direction = dir;
            this.MessageTime = NetTime.Now;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetMsg2DEntityStateUpdate(NetIncomingMessage im)
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
                return vxNetworkMessageTypes.vxNetmsgUpdate2DEntityState;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            //player ID
            id = im.ReadString();

            //Console.WriteLine("Decodeing: " + id);

            this.MessageTime = im.ReadDouble();

            Position = im.ReadVector2();
            Rotation = im.ReadFloat();
            Velocity = im.ReadVector2();
            Direction = im.ReadVector2();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            //player ID
            //Console.WriteLine("Encoding: " + id);
            om.Write(id);
            om.Write(this.MessageTime);

            om.Write(Position);
            om.Write(Rotation);
            om.Write(Velocity);
            om.Write(Direction);
        }
    }
}
