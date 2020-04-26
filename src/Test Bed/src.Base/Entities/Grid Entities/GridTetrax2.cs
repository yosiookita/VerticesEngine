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
using BEPUphysics.Paths.PathFollowing;
using BEPUphysics.Entities;

using VerticesEngine.Mathematics;
using VerticesEngine.Entities.Util;
using BEPUphysics;
using VerticesEngine.Entities;
using VerticesEngine.Scenes;
using BEPUutilities;

namespace Virtex.App.VerticesTechDemo
{
    [vxSandbox3DItem("Grid Tetra", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/items/gridtetrax2/model")]
    public class GridTetrax2 : TechDemoItem
    {

        public GridTetrax2(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, null, StartPosition)
        {

            SpecularIntensity = 0.5f;
            SpecularPower = 0.1f;
            Mass = 50;

        }


        public override void OnPhysicsColliderSetup()
        {
            foreach (ModelMesh mesh in Model.ModelMain.Meshes)
                mesh.ParentBone.Transform = Matrix.CreateScale(100);
            
            ModelDataExtractor.GetVerticesAndIndicesFromModel(Model.ModelMain, out MeshVertices, out MeshIndices);

            var transform = new AffineTransform(new Vector3(0.01f), Quaternion.Identity, StartPosition);

            PhysicsCollider = new MobileMesh(MeshVertices, MeshIndices,transform, 
                                    BEPUphysics.CollisionShapes.MobileMeshSolidity.Counterclockwise);

            base.OnPhysicsColliderSetup();
        }
    }
}
