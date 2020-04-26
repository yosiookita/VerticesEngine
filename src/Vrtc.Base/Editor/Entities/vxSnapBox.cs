
using BEPUphysics.Entities;
//Virtex vxEngine Declaration
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using VerticesEngine.Graphics;

namespace VerticesEngine.Util
{

    public class vxSnapBox : vxEntity3D
    {
		Entity PhysicsBody;

        Vector3 EndLocalRotation;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.Entities.Util.vxSnapBox"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		/// <param name="SnapBoxModel">Snap box model.</param>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		/// <param name="Length">Length.</param>
        public vxSnapBox(vxGameplayScene3D scene, vxModel SnapBoxModel, int Width, int Height, int Length)
            : base(scene, SnapBoxModel, Vector3.Zero)
        {
            EndLocalRotation = new Vector3(MathHelper.PiOver2, -MathHelper.PiOver4, MathHelper.PiOver4);
            //DoShadowMapping = false;
            PhysicsBody = new Box(Vector3.Zero, Width, Height, Length);

			Scene.PhyicsSimulation.Add(PhysicsBody);
			Scene.PhysicsDebugViewer.Add(PhysicsBody);
			PhysicsBody.CollisionInformation.CollisionRules.Personal = BEPUphysics.CollisionRuleManagement.CollisionRule.NoSolver;
			PhysicsBody.CollisionInformation.Tag = this;
            IsSaveable = false;
            IsExportable = false;
            //DoPrepPassRenders = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Parent == null)
                this.MarkForDisposal();
			 
            base.Update(gameTime);
        }

        //public override void RenderToShadowMap(int ShadowMapIndex)
        //{
        //    if (SandboxState == vxEnumSandboxStatus.EditMode)
        //        base.RenderToShadowMap(ShadowMapIndex);
        //}

        //public override void RenderPrepPass(vxCamera3D Camera)
        //{
        //    if (SandboxState == vxEnumSandboxStatus.EditMode)
        //        base.RenderPrepPass(Camera);
        //}

        //public override void RenderMesh(vxCamera3D Camera)
        //{
        //    if (SandboxState == vxEnumSandboxStatus.EditMode)
        //        base.RenderMesh(Camera);
        //}

        public override void SetOffsetOrientation()
        {
            base.SetOffsetOrientation();
            PhysicsBody.WorldTransform = WorldTransform;
        }
    }
}