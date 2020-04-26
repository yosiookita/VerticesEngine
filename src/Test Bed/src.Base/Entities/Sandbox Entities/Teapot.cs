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
using VerticesEngine.Mathematics;
using VerticesEngine.Graphics;
using VerticesEngine.Entities.Util;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    public class Teapot : TechDemoItem
    {
        vxLightEntity light;
        //public static vxEntityRegistrationInfo EntityDescription
        //{
        //    get
        //    {
        //        return new vxEntityRegistrationInfo(
        //            typeof(Teapot).ToString(),
        //        "Teapot",
        //        "Models/items/teapot/teapot",
        //            delegate (vxGameplayScene3D scene)
        //            {
        //                return new Teapot(scene, Vector3.Zero);
        //            });
        //    }
        //}


        public Teapot(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, VerticesTechDemoGame.Model_Items_Teapot, StartPosition)
        {
            light = new vxLightEntity(scene, StartPosition, LightType.Point, Color.Orange, 2, 1);
        }

        public override void OnPhysicsColliderSetup()
        {
            PhysicsCollider = new Box(Position, 2, 2, 2);

            base.OnPhysicsColliderSetup();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            light.Position = WorldTransform.Translation;
        }
    }
}
