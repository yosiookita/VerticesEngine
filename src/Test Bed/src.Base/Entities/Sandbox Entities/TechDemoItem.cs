
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using VerticesEngine.Entities;
using VerticesEngine.Graphics;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    public class TechDemoItem : vxEntity3D
    {

        public TechDemoItem(vxGameplayScene3D scene, vxModel model, Vector3 StartPosition) :
            base(scene, model, StartPosition)
        {

        }



        /// <summary>
        /// Sets up the Physics Entities
        /// </summary>
        public override void OnPhysicsColliderSetup()
        {
            // As a check, create the entity as a unit cube
            if (PhysicsCollider == null)
                PhysicsCollider = new Box(StartPosition, 1, 1, 1);

            base.OnPhysicsColliderSetup();
        }

    }
}
