using System;
using System.Collections.Generic;
using System.Text;

namespace VerticesEngine
{
    /// <summary>
    /// Component to be attached to an entitiy
    /// </summary>
    public abstract class vxComponent : IDisposable
    {
        /// <summary>
        /// Is this enabled
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;

                if(_isEnabled)
                {
                    OnEnabled();
                }
                else
                {
                    OnDisabled();
                }
            }
        }
        private bool _isEnabled = true;

        /// <summary>
        /// The entitiy which owns this component
        /// </summary>
        public vxEntity Entity
        {
            get { return _entity; }
        }
        private vxEntity _entity;


        /// <summary>
        /// Internal initialise
        /// </summary>
        /// <param name="entity"></param>
        internal void InternalInitialise(vxEntity entity)
        {
            _entity = entity;
            Initialise();
        }

        /// <summary>
        /// Called on Initialise
        /// </summary>
        protected virtual void Initialise() { }


        /// <summary>
        /// Called when disposed
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// Called on Component Enabled
        /// </summary>
        protected internal virtual void OnEnabled(){ }


        /// <summary>
        /// Called on Component Disabled
        /// </summary>
        protected internal virtual void OnDisabled() { }


        /// <summary>
        /// Update loop for this component
        /// </summary>
        protected internal virtual void Update()
        {

        }
    }
}
