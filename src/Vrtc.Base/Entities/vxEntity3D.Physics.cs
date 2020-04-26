

//Virtex vxEngine Declaration
using BEPUphysics.Entities;

namespace VerticesEngine
{
    public partial class vxEntity3D : vxEntity
    {

        /// <summary>
        /// The Physics Collider for this Entity
        /// </summary>
        public Entity PhysicsCollider;

        public bool IsDynamic = true;

        /// <summary>
        /// The Mass of this Entity
        /// </summary>
        public float Mass
        {
            get { return _mass; }
            set
            {
                _mass = value;
                if (PhysicsCollider != null)
                    PhysicsCollider.Mass = value;
            }
        }
        float _mass = 10;

        public virtual void OnPhysicsColliderSetup()
        {

        }

        public virtual void OnPhysicsColliderAddToSim()
        {

            if (PhysicsCollider != null)
            {
                //Add Entities too Physics Sim
                Scene.PhyicsSimulation.Add(PhysicsCollider);
                Scene.PhysicsDebugViewer.Add(PhysicsCollider);
                PhysicsCollider.BecomeKinematic();

            }
            editorTransform = WorldTransform;
        }

    }
}