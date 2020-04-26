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
using VerticesEngine.Entities.Util;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    public class Rock : TechDemoItem
    {
       
        //public static vxEntityRegistrationInfo EntityDescription
        //{
        //    get
        //    {
        //        return new vxEntityRegistrationInfo(
        //            typeof(Rock).ToString(),
        //        "Rock",
        //        "Models/items/rock/model",
        //            delegate (vxGameplayScene3D scene)
        //            {
        //                return new Rock(scene, Vector3.Zero);
        //            });
        //    }
        //}


        public Rock(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, VerticesTechDemoGame.Model_Items_Rock, StartPosition)
        {
            SpecularIntensity = 1;
        }
    }
}
