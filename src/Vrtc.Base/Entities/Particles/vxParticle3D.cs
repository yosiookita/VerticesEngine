using Microsoft.Xna.Framework;
using VerticesEngine.Graphics;

namespace VerticesEngine.Particles
{
    /// <summary>
    /// 3D Particle Object for use in the vxParticleSystem3D Manager Class.
    /// </summary>
    public class vxParticle3D : vxEntity3D, vxIParticle
    {

        /// <summary>
        /// Boolean of whether to keep the Particle Alive or not.
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        /// Is the particle infront or behind the scene
        /// </summary>
        public vxEnumParticleLayer ParticleLayer
        {
            get { return _particleLayer; }
            set { _particleLayer = value; }
        }
        public vxEnumParticleLayer _particleLayer = vxEnumParticleLayer.Front;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticesEngine.Particles.vxParticle3D"/> class.
        /// </summary>
        /// <param name="Engine">The Vertices Engine Reference.</param>
        /// <param name="model">Model.</param>
        /// <param name="StartPosition">Start position.</param>
        public vxParticle3D(vxGameplayScene3D scene, vxModel model, Vector3 StartPosition) 
            : base(scene, model, StartPosition)
        {

        }

        /// <summary>
        /// Disposes the Particle
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
        public virtual vxIParticle OnParticleSpawned(vxGameObject emitter)
        {
            this.IsAlive = true;
            return this;
        }

        public void UpdateParticle(GameTime gameTime)
        {

        }

        public void DrawParticle(vxCamera Camera, string renderpass)
        {

        }
    }
}