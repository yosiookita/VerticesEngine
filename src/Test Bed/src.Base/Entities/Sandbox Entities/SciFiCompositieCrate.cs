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
using VerticesEngine.Entities.Util;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    [vxSandbox3DItem("Concrete Cube", SandboxCategories.RealWorldItems, SandboxSubCategories.ConstructionItems, "Models/items/crate scifi/model")]
    public class SciFiCompositieCrate : TechDemoItem
    {

        public SciFiCompositieCrate(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, null, StartPosition)
        {
            SpecularIntensity = 1;
            SpecularPower = 1;
            FresnelBias = 0.05f;
            //spitfire.FresnelBias = 1;
            FresnelPower = 3;
            ReflectionIntensity = 1;


        }

    }
}
