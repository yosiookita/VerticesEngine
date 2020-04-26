using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using VerticesEngine;
using VerticesEngine.UI;

namespace VerticesEngine
{


    public enum EntityType
    {
        BaseEntity,
        Joint,
        Particle
    }

    /// <summary>
    /// Base Entity in the Virtex vxEngine which controls all Rendering and Provides
    /// position and world matrix updates to the other required entities.
    /// </summary>
    public class vxEntity : vxGameObject, ICloneable
    {
        /// <summary>
        /// Gets the current scene of the game
        /// </summary>
		public vxGameplaySceneBase CurrentScene
        {
			get { return _currentScene; }
        }
        private vxGameplaySceneBase _currentScene;

        /// <summary>
        /// List of Components attached to this Entity
        /// </summary>
        protected List<vxComponent> Components = new List<vxComponent>();

        /// <summary>
        /// Whether or not too keep Updating the current Entity
        /// </summary>
        public bool KeepUpdating = true;

		/// <summary>
		/// Should it be Rendered in Debug
		/// </summary>
		public bool RenderEvenInDebug = false;

		/// <summary>
		/// Is there something to render, or is this just
		/// an empty placeholder.
		/// </summary>
        public bool ShouldDraw = true;


        public Matrix WorldTransform;

        /// <summary>
        /// The Bounding Sphere which is used to do frustrum culling.
        /// </summary>
        public BoundingSphere BoundingShape
        {
            get { return _boundingSphere; }
        }
        protected BoundingSphere _boundingSphere = new BoundingSphere();

        /// <summary>
        /// Should this entity be checked  for culling. Items like the Sky box shouldn't ever be.
        /// </summary>
        public bool IsEntityCullable = true;


        private bool _firstUpdate = true;

        /// <summary>
		/// Initializes a new instance of the <see cref="VerticesEngine.Entities.vxEntity"/> class. The Base Entity Object for the Engine.
        /// </summary>
        /// <param name="scene">The current Scene for this entity to be added to.</param>
        public vxEntity(vxGameplaySceneBase scene)
        {
            _currentScene = scene;
        }


        /// <summary>
        /// Clones this Entity.
        /// </summary>
        /// <returns>A Clone copy of this object</returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual void OnFirstUpdate(GameTime gameTime)
        {

        }

        /// <summary>
        /// Update the Entity.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public virtual void Update(GameTime gameTime)
        {
			if(_firstUpdate==true)
            {
                _firstUpdate = false;
                OnFirstUpdate(gameTime);
            }

            // update each of the internal components
            for (int c = 0; c < Components.Count; c++)
            {
                if (Components[c].IsEnabled)
                {
                    Components[c].Update();
                }
            }
        }

        public virtual void PostUpdate() { }

        protected virtual void MarkForDisposal()
        {
            CurrentScene.AddForDisposal(this);
        }

        /// <summary>
        /// The Pre Draw Code.
        /// </summary>
        public virtual void PreDraw(string renderpass)
        {

        }


        /// <summary>
        /// Draws this Entity with it's default material
        /// </summary>
        /// <param name="Camera"></param>
        public virtual void Draw(vxCamera Camera, string renderpass)
        {

        }

        /// <summary>
        /// The Post Draw Code.
        /// </summary>
        public virtual void PostDraw(string renderpass)
        {

        }

        public override void Dispose()
        {
            for (int c = 0; c < Components.Count; c++)
            {
                Components[c].Dispose();
            }
            Components.Clear();

            base.Dispose();
        }

        #region -- Component Management --

        /// <summary>
        /// Add's a component to this entitiy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T: vxComponent
        {
            // create a new component
            var component = (T)Activator.CreateInstance(typeof(T));

            // add it to the list
            Components.Add(component);

            // now initialise it
            component.InternalInitialise(this);

            return component;
        }


        /// <summary>
        /// Get's a component from the collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : vxComponent
        {
            for(int c = 0; c < Components.Count; c++)
            {
                if(Components[c].GetType() is T)
                {
                    return Components[c] as T;
                }
            }

            // if nothing is return, then return null if nothing
            return null;
        }


        #endregion


        #region -- Sandbox Code --


        public virtual void OnSandboxStatusChanged(bool IsRunning) { }



        /// <summary>
        /// Initializes a new instance of the <see cref="T:Virtex.App.CartoonPhysics.ItemBase"/> class.
        /// </summary>
        public virtual void OnNewItemAdded() { }


        /// <summary>
        /// This is called after all entities have been loaded, this is helpful
        /// for if different entities are interrelated and need to be hooked up
        /// together or referenced.
        /// </summary>
        public virtual void PostFileLoad() { }

        #endregion


        #region -- Internal Engine Methods --

        /// <summary>
        /// Called internally by the engine when enabled changes
        /// </summary>
        /// <param name="value"></param>
        internal override void InternalOnEnabledChanged(bool value)
        {
            for (int c = 0; c < Components.Count; c++)
            {
                Components[c].IsEnabled = value;
            }
        }

        #endregion

        #region -- Utilities --

        public T CastAs<T>() where T : vxEntity
        {
            return (T)this;
        }

        #endregion
    }
}

