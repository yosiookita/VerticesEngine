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
    public class vxNetEntity2DState : vxNetEntityState
    {
        /// <summary>
        /// Returns the 
        /// </summary>
        public new Vector2 Position;

        /// <summary>
        /// Rotation
        /// </summary>
        public new float Rotation;

        /// <summary>
        /// The Current Velocity of the Entity. This isn't overly useful if the player is accelerating. 
        /// </summary>
        public new Vector2 Velocity;


        public vxNetEntity2DState()
        {

        }
    }
}
