using Microsoft.Xna.Framework;
using VerticesEngine.Mathematics;

namespace VerticesEngine.Particles
{
    public enum vxEnumParticleLayer
	{
		Front,
		Behind
	}

	/// <summary>
	/// 2D Particle Object for use in the vxParticleSystem2D Manager Class.
	/// </summary>
    public class vxParticle2D : vxEntity2D, vxIParticle
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

        public Rectangle Source;

        /// <summary>
        /// The index of this particle in the pool.
        /// </summary>
        public int index = 0;
		
        /// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.Particles.vxParticle2D"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="StartPosition">Start position.</param>
        public vxParticle2D(vxGameplayScene2D Scene, Rectangle Source, int index) : base(Scene, Source, Vector2.Zero)
        {
			// Remove the Entity from the main Entity List
            Scene.Entities.Remove(this);

            this.index = index;

            // Now add it to the main list
            //Scene.ParticleSystem.Add(this);
            IsAlive = false;

            this.Source = Source;

            Origin = new Vector2(Source.Width / 2f, Source.Height / 2f);
        }

        /// <summary>
        /// Activates the particle in the pool as if it was new.
        /// </summary>
        /// <param name="emitter">The Emiting Entity.</param>
        public virtual vxIParticle OnParticleSpawned(vxGameObject emitter)
        {
            IsAlive = true;
            return this;
        }

		public override void Dispose()
		{
			if (PhysicCollider != null)
			{
				PhysicCollider.Dispose();
			}
		}


        /// <summary>
        /// Update Particle
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        // block draw
        public override void Draw(vxCamera camera, string renderpass)
        {
            // Smooth out the Alpha Value
            Alpha = vxMathHelper.Smooth(Alpha, Alpha_Req, AlphaChnageSteps);

            Engine.SpriteBatch.Draw(MainSpriteSheet, Position, Source, DisplayColor * Alpha,
                Rotation, Origin, Scale, SpriteEffect, LayerDepth);

            DrawAnimation();
        }
    }
}