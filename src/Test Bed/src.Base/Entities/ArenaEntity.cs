

using System;
using System.Linq;
using BEPUutilities;
using Microsoft.Xna.Framework;
using VerticesEngine;
using VerticesEngine.Entities;
using VerticesEngine.Graphics;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using VerticesEngine.Utilities;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    public class ArenaEntity : vxEntity3D
    {
        public ArenaEntity(vxGameplayScene3D scene, vxModel entityModel, Vector3 StartPosition)
            : base(scene, entityModel, StartPosition)
        {
            ModelDataExtractor.GetVerticesAndIndicesFromModel(entityModel.ModelMain, out MeshVertices, out MeshIndices);

            StaticMesh PhysicsSkin = new StaticMesh(MeshVertices, MeshIndices,
                new AffineTransform(new Vector3(0.01f),
                                    Quaternion.Identity,
                    StartPosition));

            Scene.PhyicsSimulation.Add(PhysicsSkin);
            Scene.PhysicsDebugViewer.Add(PhysicsSkin);
        }
    }
}
