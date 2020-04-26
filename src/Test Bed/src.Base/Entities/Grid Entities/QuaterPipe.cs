
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
    //[vxSandbox3DItem("Quater Pipe", SandboxCategories.GridItems, SandboxSubCategories.GridItems, "Models/grid/quaterpipe/model")]
    public class QuaterPipe : TechDemoItem
    {

        public QuaterPipe(vxGameplayScene3D scene, Vector3 StartPosition) :
            base(scene, null, StartPosition)
        {
            SpecularIntensity = 0;
            Mass = 1000;

            SpecularIntensity = 1;
            SpecularPower = 0.05f;
            //IsMotionBlurEnabled = false;

            ModelDataExtractor.GetVerticesAndIndicesFromModel(Model.ModelMain, out MeshVertices, out MeshIndices);

            PhysicsSkin = new StaticMesh(MeshVertices, MeshIndices,
                new AffineTransform(new Vector3(0.01f), Quaternion.CreateFromRotationMatrix(WorldTransform), new Vector3(0,4.5f, 0)));

            Scene.PhyicsSimulation.Add(PhysicsSkin);
            Scene.PhysicsDebugViewer.Add(PhysicsSkin);

            base.OnPhysicsColliderSetup();
        }

        StaticMesh PhysicsSkin;
        public override void OnPhysicsColliderSetup() { }
        public override void OnPhysicsColliderAddToSim()
        {
            
        }

        /*
        public override void OnPhysicsColliderSetup()
        {
            ModelDataExtractor.GetVerticesAndIndicesFromModel(Model.ModelMain, out MeshVertices, out MeshIndices);

            PhysicsSkin = new StaticMesh(MeshVertices, MeshIndices, 
                new AffineTransform(new Vector3(0.01f), Quaternion.CreateFromRotationMatrix(WorldTransform), Position));

            Scene.PhyicsSimulation.Add(PhysicsSkin);
            Scene.PhysicsDebugViewer.Add(PhysicsSkin);

            base.OnPhysicsColliderSetup();
        }*/
        /*
        public override void OnPhysicsColliderSetup()
        {
            //foreach (ModelMesh mesh in Model.ModelMain.Meshes)
            //    mesh.ParentBone.Transform = Matrix.CreateScale(100);
            
            ModelDataExtractor.GetVerticesAndIndicesFromModel(Model.ModelMain, out MeshVertices, out MeshIndices);

            var transform = new AffineTransform(new Vector3(0.01f), Quaternion.Identity, StartPosition);

            PhysicsCollider = new MobileMesh(MeshVertices, MeshIndices,transform, 
                                    BEPUphysics.CollisionShapes.MobileMeshSolidity.Counterclockwise);

            IsDynamic = false;

            PhysicsCollider.BecomeKinematic();

            base.OnPhysicsColliderSetup();
        }*/

        //public override void Dispose()
        //{
        //    base.Dispose();
        //    Scene.PhyicsSimulation.Remove(PhysicsSkin);
        //    Scene.PhysicsDebugViewer.Remove(PhysicsSkin);
        //}
    }
}
