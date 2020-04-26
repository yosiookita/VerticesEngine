﻿
//XNA/MONOGAME
using Microsoft.Xna.Framework;

//Virtex vxEngine Declaration
using VerticesEngine;
using VerticesEngine.Entities;
using VerticesEngine.Graphics;
using System.Linq;
using System;
using VerticesEngine.Utilities;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    public class Envrio : vxEntity3D
    {
        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public Envrio(vxGameplayScene3D scene, vxModel entityModel, Vector3 StartPosition)
            : base(scene, entityModel, StartPosition)
        {
            
        }
    }
}
