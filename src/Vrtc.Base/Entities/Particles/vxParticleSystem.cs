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
    /// A scene's particle system which holds all particle pools
    /// </summary>
    public class vxParticleSystem : vxGameObject
    {
        /// <summary>
        /// Particle Pools
        /// </summary>
        Dictionary<string, vxParticlePool> ParticlePools = new Dictionary<string, vxParticlePool>();

        public vxParticleSystem()
        {
			
        }

        /// <summary>
        /// Add's a particle pool to this system
        /// </summary>
        /// <param name="particlePool"></param>
        public void AddPool(vxParticlePool particlePool)
        {
            ParticlePools.Add(particlePool.Key.ToString(), particlePool);
        }

        /// <summary>
        /// Initialises' a new particle with the givien particle pool key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="emitter"></param>
        public virtual vxIParticle SpawnParticle(object key, vxGameObject emitter)
        {
            if (ParticlePools.ContainsKey(key.ToString()))
                return ParticlePools[key.ToString()].SpawnParticle(emitter);
            else
                return null;
        }

        public override void Dispose()
        {
            foreach (var pool in ParticlePools)
                pool.Value.Dispose();

            ParticlePools.Clear();

            base.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var poolKeyPair in ParticlePools)
                poolKeyPair.Value.Update(gameTime);
        }

		/// <summary>
		/// Draws the particles which should be behind the scene.
		/// </summary>
		public void DrawUnderlayItems(vxCamera camera)
        {
            foreach (var poolKeyPair in ParticlePools)
                poolKeyPair.Value.DrawUnderlayItems(camera);
		}

		/// <summary>
		/// Draws the particles which should be in front of the scene.
		/// </summary>
        public void DrawOverlayItems(vxCamera camera)
        {
            foreach (var poolKeyPair in ParticlePools)
                poolKeyPair.Value.DrawOverlayItems(camera);
        }
    }
}