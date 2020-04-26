using System;
using Microsoft.Xna.Framework;
using VerticesEngine.Physics.Entities;

namespace VerticesEngine.Physics
{
    public enum Physics2DSolver
    {
        Farseer,
        Aether,
        Box2D
    }

	public interface vxPhysics2DSystem
	{
        Physics2DSolver PhysicsBackend { get; }

        void Step(float elapsedTime);

        vxPhysics2DCircleCollider CreateCircleCollider(Vector2 position, float radius, float density);

        vxPhysics2DRectCollider CreateRectCollider(Vector2 position, float width, float height, float density);
	}

    public static class vxPhysics
    {
        public static vxPhysics2DSystem CreateSystem(Physics2DSolver solver, Vector2 gravity)
        {
            vxPhysics2DSystem system = null;
            switch (solver)
            {
                case Physics2DSolver.Farseer:
                    system = new vxPhysicsSimulation2DFarseer(gravity);
                    break;

            }
            return system;
        }
    }
}
