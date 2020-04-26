using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VerticesEngine;

namespace VerticesEngine.Particles
{
    public interface vxIParticle
    {
        /// <summary>
        /// Boolean of whether to keep the Particle Alive or not.
        /// </summary>
        bool IsAlive { get; set; }

        /// <summary>
        /// Is the particle infront or behind the scene
        /// </summary>
        vxEnumParticleLayer ParticleLayer { get; set; }

        vxIParticle OnParticleSpawned(vxGameObject emitter);

        void Update(GameTime gameTime);

        //void DrawParticle(vxCamera Camera, string renderpass);
        void Draw(vxCamera Camera, string renderpass);
    }
}
