
using Microsoft.Xna.Framework;


//Virtex vxEngine Declaration
using VerticesEngine;

namespace VerticesEngine
{
    public partial class vxEntity3D : vxEntity
    {
        #region -- Sandbox Fields and Properties --

        /// <summary>
        /// The type of the sandbox entity.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.BasicProperties)]
        public vxEntityCategory SandboxEntityType
        {
            get { return _sandboxEntityType; }
        }
        public readonly vxEntityCategory _sandboxEntityType;

        /// <summary>
        /// Is this entity Exportable.
        /// </summary>
        public bool IsExportable = true;

        /// <summary>
        /// Is this Entity Saveable.
        /// </summary>
        public bool IsSaveable = true;

        /// <summary>
        /// Description of the Entity
        /// </summary>
        public string Description;


        public Color SelectionColour
        {
            set
            {
                _selectionColour = value;
                if (Model != null)
                {
                    foreach (var mesh in Model.Meshes)
                        mesh.Material.UtilityEffect.SelectionColour = value;
                }
            }
            get { return _selectionColour; }
        }
        Color _selectionColour;


        public bool IsShadowCaster
        {
            set
            {
                _isShadowCaster = value;
                if (Model != null)
                {
                    foreach (var mesh in Model.Meshes)
                        mesh.Material.IsShadowCaster = value;
                }
            }
            get { return _isShadowCaster; }
        }
        bool _isShadowCaster = true;



        public string ItemKey = "<none>";

        #endregion



        /// <summary>
        /// A method which allows for certain opperations to be preformed just before the entity is saved to a file.
        /// </summary>
        public virtual void PreSave() { }


        /// <summary>
        /// A method which allows for certain opperations to be preformed after the entity is loaded from a file.
        /// </summary>
        public virtual void PostEntityLoad() { }


        /// <summary>
        /// Called when the gimbal translates this entity.
        /// </summary>
        public virtual void OnGimbalTranslate(Vector3 delta) { }


        /// <summary>
        /// Called when the gimbal rotates this entity.
        /// </summary>
        public virtual void OnGimbalRotate(Vector3 axis, float delta) { }



        public override void OnSandboxStatusChanged(bool IsRunning)
        {
            SandboxState = IsRunning ? vxEnumSandboxStatus.Running : vxEnumSandboxStatus.EditMode;


            if (IsDynamic && PhysicsCollider != null)
            {
                if (SandboxState == vxEnumSandboxStatus.Running)
                {
                    editorTransform = PhysicsCollider.WorldTransform;
                    PhysicsCollider.BecomeDynamic(Mass);
                }
                else
                {
                    WorldTransform = editorTransform;

                    // Zero out any Physics
                    PhysicsCollider.AngularVelocity = Vector3.Zero;
                    PhysicsCollider.LinearVelocity = Vector3.Zero;
                    PhysicsCollider.BecomeKinematic();
                }
            }
        }


    }
}