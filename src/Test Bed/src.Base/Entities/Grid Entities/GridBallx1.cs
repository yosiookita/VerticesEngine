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
using VerticesEngine.Entities;
using BEPUphysics.Entities.Prefabs;
using VerticesEngine.Entities.Util;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    [vxSandbox3DItem("Grid Ball x 1", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/items/gridballx1/model")]
    public class GridBallx1 : TechDemoItem
    {

        public GridBallx1(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, null, StartPosition)
        {
            SpecularIntensity = 1;
            SpecularPower = 1f;
            FresnelBias = 0.051f;
            FresnelPower = 3;
            ReflectionIntensity = 1;
        }
        public override void OnPhysicsColliderSetup()
        {
            // As a check, create the entity as a unit cube
            PhysicsCollider = new Sphere(StartPosition, 0.5f, 1);
            //entity.Material.Bounciness = 100;
            base.OnPhysicsColliderSetup();
        }

    }
}
