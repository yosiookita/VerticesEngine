using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

//Virtex vxEngine Declaration

using VerticesEngine.Entities.Util;
using VerticesEngine.Scenes;
using VerticesEngine.Entities;

namespace Virtex.App.VerticesTechDemo
{
    [vxSandbox3DItem("Wooden Crate", SandboxCategories.RealWorldItems, SandboxSubCategories.ConstructionItems, "Models/items/crate wooden/model")]
    public class WoodenCrate : TechDemoItem
    {
       
        //public static vxEntityRegistrationInfo EntityDescription
        //{
        //    get
        //    {
        //        return new vxEntityRegistrationInfo(
        //            typeof(WoodenCrate).ToString(),
        //        "Wooden Crate",
        //            "Models/items/crate wooden/model",
        //            delegate (vxGameplayScene3D scene)
        //            {
        //                return new WoodenCrate(scene, Vector3.Zero);
        //            });
        //    }
        //}


        public WoodenCrate(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, null, StartPosition)
        {
            SpecularIntensity = 0.15f;
            SpecularPower = 0.01f;
        }

        //public override VerticesEngine.Graphics.vxModel OnLoadModel()
        //{
        //    return Engine.ContentManager.LoadModel(EntityDescription.FilePath);
        //}

        //public override void RenderMesh(VerticesEngine.Cameras.vxCamera3D Camera)
        //{
        //    base.RenderMesh(Camera);
        //}
    }
}
