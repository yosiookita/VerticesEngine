
//Virtex vxEngine Declaration
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using Microsoft.Xna.Framework;
using VerticesEngine.Entities;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    [vxSandbox3DItem("Grid Ramp1", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/grid/ramp1/model")]
    public class GridRamp1 : TechDemoItem
    {

        public GridRamp1(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, null, StartPosition)
        {
            SpecularIntensity = 0;
            Mass = 1000;

            SpecularIntensity = 1;
            SpecularPower = 0.05f;
            //IsMotionBlurEnabled = false;
        }


        /*
        StaticMesh PhysicsSkin;
        public override void OnPhysicsColliderSetup()
        {
            ModelDataExtractor.GetVerticesAndIndicesFromModel(Model.ModelMain, out MeshVertices, out MeshIndices);

            PhysicsSkin = new StaticMesh(MeshVertices, MeshIndices, 
                new AffineTransform(new Vector3(0.01f), Quaternion.CreateFromRotationMatrix(WorldTransform), Position));

            Scene.PhyicsSimulation.Add(PhysicsSkin);
            Scene.PhysicsDebugViewer.Add(PhysicsSkin);

            base.OnPhysicsColliderSetup();
        }*/

        public override void OnPhysicsColliderSetup()
        {
            //foreach (ModelMesh mesh in Model.ModelMain.Meshes)
            //    mesh.ParentBone.Transform = Matrix.CreateScale(100);
            
            ModelDataExtractor.GetVerticesAndIndicesFromModel(Model.ModelMain, out MeshVertices, out MeshIndices);

            var transform = new AffineTransform(new Vector3(0.01f), Quaternion.Identity, StartPosition);

            PhysicsCollider = new MobileMesh(MeshVertices, MeshIndices,transform, 
                                    BEPUphysics.CollisionShapes.MobileMeshSolidity.Counterclockwise);

            base.OnPhysicsColliderSetup();
        }

        //public override void Dispose()
        //{
        //    base.Dispose();
        //    Scene.PhyicsSimulation.Remove(PhysicsSkin);
        //    Scene.PhysicsDebugViewer.Remove(PhysicsSkin);
        //}
    }
}
