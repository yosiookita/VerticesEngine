using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration
using VerticesEngine;
using BEPUphysics.Entities.Prefabs;

using VerticesEngine.Entities.Util;
using VerticesEngine.Scenes;
using VerticesEngine.Entities;

namespace Virtex.App.VerticesTechDemo
{
    [vxSandbox3DItem("Grid Cube x 2", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/items/gridcubex2/model")]
    public class GridCubex2 : TechDemoItem
    {

        public GridCubex2(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, null, StartPosition)
        {
            SpecularIntensity = 0;
            Mass = 1000;

            SpecularIntensity = 1;
            SpecularPower = 0.05f;
            //IsMotionBlurEnabled = false;
        }


        public override void OnPhysicsColliderSetup()
        {
            PhysicsCollider = new Box(Position, 2, 2, 2);

            base.OnPhysicsColliderSetup();
        }

    }
}
