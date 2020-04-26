using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VerticesEngine;
using VerticesEngine.Graphics;

namespace VerticesEngine.Particles
{

    /// <summary>
    /// A particle set which holds all of the particle pool.
    /// </summary>
    public class vxParticlePool : IDisposable
    {

        public List<vxIParticle> Pool = new List<vxIParticle>();

        public string Key;

        /// <summary>
        /// The particle layer, i.e. are they drawn before or after the main scene draw call.
        /// </summary>
        //public vxEnumParticleLayer ParticleLayer = vxEnumParticleLayer.Behind;

        public vxParticlePool(object Key)
        {
            this.Key = Key.ToString();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Pool.Count; i++)
            {
                if (Pool[i].IsAlive)
                    Pool[i].Update(gameTime);
            }
        }

        int initIndex = 0;

        /// <summary>
        /// Inits a new particle in the particle pool.
        /// </summary>
        /// <param name="key">Key.</param>
        internal virtual vxIParticle SpawnParticle(vxGameObject emitter)
        {
            initIndex = (initIndex + 1) % Pool.Count;
            return Pool[initIndex].OnParticleSpawned(emitter);
        }



        /// <summary>
        /// Draws the particles which should be behind the scene.
        /// </summary>
        public void DrawUnderlayItems(vxCamera camera)
        {
            for (int p = 0; p < Pool.Count; p++)
            {
                if (Pool[p].IsAlive == true && Pool[p].ParticleLayer == vxEnumParticleLayer.Behind)
                {
                    Pool[p].Draw(camera, vxRenderer.Passes.ParticlePrePass);
                }
            }
        }

        /// <summary>
        /// Draws the particles which should be in front of the scene.
        /// </summary>
        public void DrawOverlayItems(vxCamera camera)
        {
            for (int p = 0; p < Pool.Count; p++)
            {
                if (Pool[p].IsAlive == true && Pool[p].ParticleLayer == vxEnumParticleLayer.Front)
                {
                    Pool[p].Draw(camera, vxRenderer.Passes.ParticlePostPass);
                }
            }

        }

        public void Dispose()
        {
            foreach (IDisposable particle in Pool)
            {
                particle.Dispose();
            }
            Pool.Clear();
        }
    }
}
