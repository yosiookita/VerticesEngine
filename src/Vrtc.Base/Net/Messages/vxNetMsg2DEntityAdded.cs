
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
    /// Holds basic info for an Entity2D for use in Net games
    /// </summary>
    public class vxNetEntity2D
    {
        public string id;

        /// <summary>
        /// The type. Enum.Parse must be called.
        /// </summary>
        public string type;

        /// <summary>
        /// The position.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The rotation.
        /// </summary>
        public float Rotation;


        /// <summary>
        /// The direction. 
        /// </summary>
        public Vector2 Direction;

        /// <summary>
        /// The velocity. 
        /// </summary>
        public Vector2 Velocity;

        public string UserData = "";


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Net.Messages.vxNetEntity2D"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="type">Type.</param>
        /// <param name="pos">Position.</param>
        /// <param name="rot">Rot.</param>
        /// <param name="dir">Dir.</param>
        /// <param name="vel">Vel.</param>
        public vxNetEntity2D(string id, string type, Vector2 pos, float rot, Vector2 dir, Vector2 vel)
        {
            this.id = id;
            this.type = type;
            this.Position = pos;
            this.Rotation = rot;
            this.Direction = dir;
            this.Velocity = vel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Net.Messages.vxNetEntity2D"/> class.
        /// NOTE: Velocity and Direction will not be set here. You must set them outside of the constructor.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="type">Type.</param>
        /// <param name="entity2D">Entity2 d.</param>
        public vxNetEntity2D(string id, string type, vxEntity2D entity2D)
        {
            this.id = id;
            this.type = type;
            this.Position = entity2D.Position;
            this.Rotation = entity2D.Rotation;
            //this.Direction = ;
            //this.Velocity = vel;
        }
    }


    /// <summary>
    /// Net Message for Adding a New 2D Entity
    /// </summary>
    public class vxNetMsg2DEntityAdded : INetworkMessage
    {
        public vxNetEntity2D Entity2D;

        /// <summary>
        /// Gets or sets MessageTime to help with interpolating the actual position after lag.
        /// </summary>
        public double MessageTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerinfo"></param>
        public vxNetMsg2DEntityAdded(vxNetEntity2D entity2D)
        {
            this.Entity2D = entity2D;
            this.MessageTime = NetTime.Now;
        }

        /// <summary>
        /// Decoding Constructor to be used by client.
        /// </summary>
        /// <param name="im"></param>
        public vxNetMsg2DEntityAdded(NetIncomingMessage im)
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
                return vxNetworkMessageTypes.AddItem2D;
            }
        }

        public void DecodeMsg(NetIncomingMessage im)
        {
            this.MessageTime = im.ReadDouble();
            Entity2D = new vxNetEntity2D(
                im.ReadString(), // id
                im.ReadString(), // type
                im.ReadVector2(), // Pos
                im.ReadFloat(), // Rotation
                im.ReadVector2(), // dir
                im.ReadVector2() // vel
            );

            Entity2D.UserData = im.ReadString();
        }

        public void EncodeMsg(NetOutgoingMessage om)
        {
            //player ID
            //Console.WriteLine("Encoding: " + id);
            om.Write(this.MessageTime);
            om.Write(Entity2D.id);
            om.Write(Entity2D.type);
            om.Write(Entity2D.Position);
            om.Write(Entity2D.Rotation);
            om.Write(Entity2D.Direction);
            om.Write(Entity2D.Velocity);
            om.Write(Entity2D.UserData);
        }
    }
}
