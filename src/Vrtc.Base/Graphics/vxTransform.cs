using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// This is the main transform class which houses an entities scale, position and orientations
    /// </summary>
    public class vxTransform
    {
        /// <summary>
        /// The scale to apply to this transform
        /// </summary>
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                CalcWorldTransform();
            }
        }
        private Vector3 _scale = Vector3.One;


        /// <summary>
        /// The position of this transform in 3D space
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                CalcWorldTransform();
            }
        }
        private Vector3 _position = Vector3.Zero;
        
        #region -- Rotation --

        /// <summary>
        /// The Rotation Quaternion
        /// </summary>
        public Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;

                CalcWorldTransform();
            }
        }
        private Quaternion _rotation = Quaternion.Identity;


        /// <summary>
        /// Provides the needed Y rotation for the model
        /// </summary>
        public float Yaw
        {
            get { return _yaw; }
            set
            {
                _yaw = value;
                OnSetYawPitchRoll();
            }
        }
        float _yaw = 0;

        /// <summary>
        /// Provides the needed Z rotation for the model
        /// </summary>
        public float Pitch
        {
            get { return _pitch; }
            set
            {
                _pitch = value;
                OnSetYawPitchRoll();
            }
        }
        float _pitch = 0;

        /// <summary>
        /// Provides the needed X rotation for the model
        /// </summary>
        public float Roll
        {
            get { return _roll; }
            set
            {
                _roll = value;
                OnSetYawPitchRoll();
            }
        }
        float _roll = 0;

        void OnSetYawPitchRoll()
        {
            Rotation = Quaternion.CreateFromYawPitchRoll(_yaw,_pitch, _roll);
        }

        #endregion

        /// <summary>
        /// The world transform which makes up the scale, rotation and position as a single matrix. This is 
        /// usually passed through to the shaders as the model or world matrix
        /// </summary>
        public Matrix WorldTransform
        {
            get {
                if (isTransformDirty)
                    CalcWorldTransform();
                
                return _worldTransform;
            }
            set
            {
                _worldTransform = value;
                
                _position = _worldTransform.Translation;
                _rotation = Quaternion.CreateFromRotationMatrix(_worldTransform);
            }
        }
        private Matrix _worldTransform = Matrix.Identity;

        /// <summary>
        /// Has a dependant variable such as position or scale been changed that requires the world transform be recalculated?
        /// </summary>
        private bool isTransformDirty = false;

        /// <summary>
        /// calucaltes the world transform matrix
        /// </summary>
        private void CalcWorldTransform()
        {
            _worldTransform = Matrix.CreateScale(_scale) * Matrix.CreateFromQuaternion(_rotation) * Matrix.CreateTranslation(_position);
            OnTransformUpdated();
            isTransformDirty = false;
        }

        public Action OnTransformUpdated;
    }
}
