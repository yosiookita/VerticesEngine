using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;

namespace VerticesEngine.Net
{
    /// <summary>
    /// This holds all data needed to update the state of a multiplayer entity.
    /// </summary>
    public class vxNetEntityState
    {
        public vxNetEntityState()
        {

        }

        public Vector3 Velocity { get; set; }
        public bool IsRightDown { get; set; }
        public bool IsLeftDown { get; set; }
        public bool IsThrustDown { get; set; }
        public float TurnAmount { get; set; }
        public float ThrustAmount { get; set; }
        public float Rotation { get; set; }
        public Vector3 Position { get; set; }
    }
}
