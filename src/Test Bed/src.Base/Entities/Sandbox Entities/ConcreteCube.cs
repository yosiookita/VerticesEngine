using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Virtex vxEngine Declaration
using VerticesEngine;
using VerticesEngine.Entities;
using BEPUphysics.Entities.Prefabs;
using VerticesEngine.Utilities;
using VerticesEngine.Graphics;
using VerticesEngine.Entities.Util;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    [vxSandbox3DItem("Concrete Cube", SandboxCategories.RealWorldItems, SandboxSubCategories.ConstructionItems, "Models/items/concrete_cube/concrete_cube")]
    public class ConcreteCube : TechDemoItem
	{

        public ConcreteCube(vxGameplayScene3D scene, Vector3 StartPosition) :
			base(scene, VerticesTechDemoGame.Model_Items_Concrete, StartPosition)
		{
			SpecularIntensity = 1;
            Mass = 1000;
		}

		public override void OnPhysicsColliderSetup()
		{
			PhysicsCollider = new Box(Position, 2, 2, 2);

			base.OnPhysicsColliderSetup();
		}
	}
}