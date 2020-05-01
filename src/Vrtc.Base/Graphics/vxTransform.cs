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
                isTransformDirty = true;
                _scale = value;
            }
        }
        private Vector3 _scale = Vector3.One;


        /// <summary>
        /// The position of this transform in 3D space
        /// </summary>
        public Vector3 Position
        {
            get { return _worldTransform.Translation; }
            set
            {
                isTransformDirty = true;
                _position = value;
            }
        }
        private Vector3 _position = Vector3.Zero;



        /// <summary>
        /// The Rotation Quaternion
        /// </summary>
        public Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
                isTransformDirty = true;
                _rotation = value;
            }
        }
        private Quaternion _rotation = Quaternion.Identity;


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

        bool isTransformDirty = false;

        void CalcWorldTransform()
        {
            _worldTransform = Matrix.CreateScale(_scale) * Matrix.CreateFromQuaternion(_rotation) * Matrix.CreateTranslation(_position);
            
            isTransformDirty = false;
        }
    }
}
