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
    //[vxSandbox3DItem("AOBlocks", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/tess/tess")]
    [vxSandbox3DItem("AOBlocks", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/stuff/intro")]
    public class AOBlocks : TechDemoItem
    {

        public AOBlocks(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, null, StartPosition)
        {
            SpecularIntensity = 1;
            SpecularPower = 1f;
            //HasReflectionMap = true;
            FresnelBias = 0.051f;
            //spitfire.FresnelBias = 1;
            FresnelPower = 3;
            ReflectionIntensity = 1;
            //ReflectionMap = DefaultTexture;
            //World *= Matrix.CreateRotationX(MathHelper.PiOver2);
        }
        public override void OnPhysicsColliderSetup()
        {
            // As a check, create the entity as a unit cube
            PhysicsCollider = new Sphere(StartPosition, 0.5f, 1);
            //entity.Material.Bounciness = 100;
            base.OnPhysicsColliderSetup();
        }

        //public override VerticesEngine.Graphics.vxModel OnLoadModel()
        //{
        //    return Engine.ContentManager.LoadModel(EntityDescription.FilePath);
        //}

    }
}
