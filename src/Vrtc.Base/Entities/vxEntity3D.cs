using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
//Virtex vxEngine Declaration
using VerticesEngine.Audio;
using VerticesEngine.Diagnostics;
using VerticesEngine.Graphics;
using VerticesEngine;
using VerticesEngine.Utilities;
using VerticesEngine.ContentManagement;

namespace VerticesEngine
{
    public enum vxEntityCategory
    {
        Axis,
        Rotator,
        Pan,
        Entity,
        Particle
    }


    /// <summary>
    /// Base Entity in the Virtex vxEngine which controls all Rendering and Provides
    /// position and world matrix updates to the other required entities.
    /// </summary>
    public partial class vxEntity3D : vxEntity
    {

        /// <summary>
        /// The handle identifier for this entity.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.BasicProperties, "The Handle ID for this Entity.")]
        public int HandleID
        {
            get
            {
                return _handleID;
            }
        }
        int _handleID = -1;

        public Color IndexEncodedColour
        {
            get { return _indexEncodedColour; }
            set
            {
                _indexEncodedColour = value;
                if (Model != null)
                    foreach(var mesh in Model.Meshes)
                    {
                        mesh.Material.UtilityEffect.Parameters["IndexEncodedColour"].SetValue(value.ToVector4());
                    }
            }
        }
        Color _indexEncodedColour = Color.Black;

        public new vxEntity3D Parent;

        /// <summary>
        /// The current scene of the game
        /// </summary>
        public vxGameplayScene3D Scene;

        /// <summary>
        /// Direction Entity is facing.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Entity's up vector.
        /// </summary>
        public Vector3 Up;

        /// <summary>
        /// Entity's right vector.
        /// </summary>
        public Vector3 Right;

        /// <summary>
        /// Current Entity velocity.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Entity world transform matrix.
        /// </summary>
        public new Matrix WorldTransform
        {
            get
            {
                if (PhysicsCollider != null)
                    return PhysicsCollider.WorldTransform;

                return _worldTransform;
            }
            set
            {
                if (PhysicsCollider != null)
                    PhysicsCollider.WorldTransform = value;

                _worldTransform = value;

                OnWorldTransformChanged();
            }
        }
        protected Matrix _worldTransform = Matrix.Identity;

        /// <summary>
        /// Location of Entity in world space.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.EntityTransformProperties, "The Entities 3D World Space Position")]
        public Vector3 Position
        {
            get
            {
                if (PhysicsCollider != null)
                    return PhysicsCollider.Position;

                return _worldTransform.Translation;
            }
            set
            {
                if (PhysicsCollider != null)
                    PhysicsCollider.Position = value;
                //_position = value;
                _worldTransform.Translation = value;

                OnWorldTransformChanged();
            }
        }
        //Vector3 _position;

        /// <summary>
        /// Fired when the World Matrix is Updated or changed. Helpful for entities which are dependant on this one.
        /// </summary>
        public virtual void OnWorldTransformChanged() { }

        /// <summary>
        /// The Start Position of the Entity
        /// </summary>
        public Vector3 StartPosition { get; set; }

        /// <summary>
        /// Model Scale
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.EntityTransformProperties, "The Entities 3D Scale")]
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                ScaleMatrix = Matrix.CreateScale(value);
            }
        }
        Vector3 _scale = Vector3.One;

        public Matrix ScaleMatrix = Matrix.CreateScale(Vector3.One);



        //             MODEL ROTATIONS
        /***********************************************/

        /// <summary>
        /// Provides the needed X rotation for the model
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.EntityTransformProperties, "The Entities Roll Angle (In Radians)")]
        public float Roll
        {
            get { return _roll; }
            set
            {
                _roll = value;
                OnSetTransform();
            }
        }
        float _roll = 0;

        /// <summary>
        /// Provides the needed Y rotation for the model
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.EntityTransformProperties, "The Entities Yaw Angle (In Radians)")]
        public float Yaw
        {
            get { return _yaw; }
            set
            {
                _yaw = value;
                OnSetTransform();
            }
        }
        float _yaw = 0;

        /// <summary>
        /// Provides the needed Z rotation for the model
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.EntityTransformProperties, "The Entities Pitch Angle (In Radians)")]
        public float Pitch
        {
            get { return _pitch; }
            set
            {
                _pitch = value;
                OnSetTransform();
            }
        }
        float _pitch = 0;




        /// <summary>
        /// Item Bounding Box
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.ModelProperties, "The Entities Bounding Box")]
        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
        }
        BoundingBox _boundingBox = new BoundingBox();

        /// <summary>
        /// The Model Center. This is not to be confused with the Model Position. 
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.ModelProperties, "The Entities Geometric Center")]
        public Vector3 ModelCenter
        {
            get { return _ModelCenter; }
            set { _ModelCenter = value; }
        }
        Vector3 _ModelCenter = new Vector3();

        /// <summary>
        /// Gets the file path to the main model asset for this entity
        /// </summary>
        public virtual string AssetPath
        {
            get { return _filePath; }
        }
        string _filePath = "";

        /// <summary>
        /// This is the editor transform to reset the model to after testing
        /// </summary>
        protected Matrix editorTransform = Matrix.Identity;

        /// <summary>
        /// Entity world transform matrix.
        /// </summary>
        public Matrix EndOrientation = Matrix.Identity;


        /// <summary>
        /// The child entities.
        /// </summary>
        public List<vxEntity3D> ChildEntities = new List<vxEntity3D>();



        public bool HasEndSnapbox = true;

        public string UserDefinedData01;// = "no-data";
        public string UserDefinedData02;// = "no-data";
        public string UserDefinedData03;// = "no-data";
        public string UserDefinedData04;// = "no-data";
        public string UserDefinedData05;// = "no-data";


        public object Tag = "";

        //Load in mesh data and create the collision mesh.
        public Vector3[] MeshVertices;

        public int[] MeshIndices;

        public bool CanBePlacedOnSurface = false;

        public Matrix PreSelectionWorld = Matrix.Identity;


        /// <summary>
        /// Base Entity Object for the Engine.
        /// <para>If a model is not specefied, this entity will call the GetModel() method. Override this method
        /// with the and return the model for this Entity.
        /// Otherwise this Entity is used as an 'Empty' in the Engine.</para>
        /// </summary>
        /// <param name="Scene">The current Scene that will own this entity.</param>
        /// <param name="StartPosition">The Start Position of the Entity</param>
        public vxEntity3D(vxGameplayScene3D Scene, Vector3 StartPosition) : this(Scene, null, StartPosition) { }

        /// <summary>
        /// Base Entity Object for the Engine.
        /// </summary>
        /// <param name="Scene">The current Scene that will own this entity.</param>
        /// <param name="EntityModel">The Entities Model to be used.</param>
        /// <param name="StartPosition">The Start Position of the Entity.</param>
        public vxEntity3D(vxGameplayScene3D Scene, vxModel EntityModel, Vector3 StartPosition, vxEntityCategory Entity3DType = vxEntityCategory.Entity) :
        base(Scene)
        {
            this.Scene = Scene;

            _worldTransform = Matrix.CreateTranslation(StartPosition);

            _sandboxEntityType = Entity3DType;

            //Set Position Data
            //Position = StartPosition;

            this.StartPosition = StartPosition;

            // If the model parameter is passed, then add it in, if not, then fire the virtual method GetModel
            // to try and see if any inheriting classes have overriden this method.
            if (EntityModel != null)
                Model = EntityModel;
            else
                Model = OnLoadModel();


            // Add to the main list
            Scene.Entities.Add(this);

            // Set Handle
            _handleID = Scene.GetNewEntityHandle();

            // Initialise Shaders
            InitShaders();

            OnPhysicsColliderSetup();

            OnPhysicsColliderAddToSim();

            this.TreeNode.Icon = vxInternalAssets.Textures.TreeMesh;
        }


        /// <summary>
        /// Updates the Transform
        /// </summary>
        public virtual void OnSetTransform()
        {
            //New Matrix
            Matrix NewWorld = Matrix.Identity;

            NewWorld *= Matrix.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
            NewWorld *= Matrix.CreateTranslation(Position);

            WorldTransform = NewWorld;
        }

        /// <summary>
        /// This method is fired if a model is not passed through the constructor of if 'null' is passed.
        /// This is handy for specifing different models for different conditions for a certain Entity as well as althoughts the model 
        /// not to be loaded until needed.
        /// </summary>
        /// <returns></returns>
        public virtual vxModel OnLoadModel()
        {
            // loop through attributes and load the model for the first sandbox item attribute
            foreach (var attribute in GetType().GetCustomAttributes<vxRegisterAsSandboxEntityAttribute>())
            {
                _filePath = attribute.AssetPath;
                return vxContentManager.Instance.LoadModel(AssetPath);
            }
            return vxInternalAssets.Models.UnitBox;
        }


        /// <summary>
        /// Initialise the Main Shader.
        /// <para>If a different shader is applied to this model, this method should be overridden</para>
        /// </summary>
        public virtual void InitShaders()
        {
           
        }


        public override void Dispose()
        {
            try
            {
                if(PhysicsCollider != null)
                {
                    Scene.PhyicsSimulation.Remove(PhysicsCollider);
                    Scene.PhysicsDebugViewer.Remove(PhysicsCollider);
                }

                if (vxGameObject.NameRegister.Contains(this.Id))
                    vxGameObject.NameRegister.Remove(this.Id);
                
                //First Remove from the Items List in the Sandbox Screen                
                Scene.ResetTree();

                //Now Dispose all Child Entities
                foreach (vxEntity3D entity in ChildEntities)
                    entity.Dispose();
            }
            catch (Exception ex)
            {
                vxConsole.WriteException(this, ex);
            }

            //First Remove From Entities List
            if (Scene.Entities.Contains(this))
                Scene.Entities.Remove(this);


            base.Dispose();
        }


        /// <summary>
        /// This Method Is Called when the item is successfully added into the world.
        /// </summary>
        public virtual void OnAdded()
        {

        }

        public virtual void AddChild(vxEntity3D Entity, Matrix OffSetOrientation)
        {
            Entity.Parent = this;
            Entity.WorldTransform = OffSetOrientation * WorldTransform;
            ChildEntities.Add(Entity);
            SetChildOrientation();
        }

        public virtual void AddChildrenEntities()
        {

        }

        public virtual void SetChildOrientation()
        {
            foreach (vxEntity3D entity in ChildEntities)
                entity.SetOffsetOrientation();
        }

        public virtual void SetOffsetOrientation()
        {
            WorldTransform = this.Parent.EndOrientation * this.Parent.WorldTransform;
        }

        #region -- Update Code --

        public override void OnFirstUpdate(GameTime gameTime)
        {
            base.OnFirstUpdate(gameTime);

            // set the index colour 
            IndexEncodedColour = new Color(
                (int)(_handleID % 255),
                (int)Math.Floor((((float)_handleID) / 255.0f) % (255)),
                (int)Math.Floor((((float)_handleID) / (255.0f * 255.0f)) % (255)));
        }

        /// <summary>
        /// Updates the Entity
        /// </summary>
        public override void Update(GameTime gameTime)
        {

            if (IsDynamic && PhysicsCollider != null)
            {
                if (SandboxState == vxEnumSandboxStatus.Running)
                    _worldTransform = PhysicsCollider.WorldTransform;
                else
                    PhysicsCollider.WorldTransform = _worldTransform;
            }

            base.Update(gameTime);

            //Set the Selection Colour based off of Selection State
            switch (SelectionState)
            {
                case vxSelectionState.Selected:
                    SelectionColour = Color.DarkOrange;
                    break;
                case vxSelectionState.None:
                    SelectionColour = Color.Black;
                    break;
            }

            // Reset the Bounding Sphere's Center Position
            _boundingSphere.Center = Vector3.Transform(ModelCenter, WorldTransform);


            IsEntityDrawable = false;

            // Get whether or not the Entity is Visible 
            if (Model != null && Model.ModelMain != null   && ShouldDraw == true)
            {
                if (vxDebug.IsDebugMeshVisible == false || RenderEvenInDebug == true || IsAlphaNoShadow == false)
                {
                    IsEntityDrawable = true;
                }
            }

            TempWorld = ScaleMatrix * WorldTransform;


        }

        #endregion

        #region -- Utility Methods --


        public virtual BoundingSphere GetBoundingShape()
        {
            return BoundingSphere.CreateFromBoundingBox(Model.BoundingBox);
        }


        #endregion
    }
}