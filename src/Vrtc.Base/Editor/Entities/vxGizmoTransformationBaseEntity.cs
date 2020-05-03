using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VerticesEngine.Graphics;

namespace VerticesEngine.Util
{

    /// <summary>
    /// The base class for gimbal transformation entities such as Translation Arrows, Rotators and Panning Sqaures.
    /// </summary>
    public class vxGizmoTransformationBaseEntity : vxEditorEntity
    {
        public AxisDirections AxisDirections;

        public vxGizmo Gimbal;

        public Vector3 MainAxis = Vector3.Zero;

        public Vector3 PerpendicularAxis = Vector3.Zero;

        public Color Color;

        public float BaseAlpha = 1;

        BasicEffect BasicEffect;

        public vxGizmoTransformationBaseEntity(vxGameplayScene3D scene, vxGizmo Gimbal, AxisDirections AxisDirections) : base(scene, vxEntityCategory.Axis)
        {
            this.Gimbal = Gimbal;

            Scale = Vector3.One * 7;

            //HitBox = new Box(Vector3.Zero, 2, 2, 25);
            WorldTransform = Matrix.CreateScale(1, 1, 12);

            int colStrength = 225;
            int s = 10;
            this.AxisDirections = AxisDirections;
            switch (AxisDirections)
            {
                case AxisDirections.X:
                    PlainColor = new Color(colStrength, s, s);
                    WorldTransform = Matrix.CreateScale(25, 2, 2);
                    break;
                case AxisDirections.Y:
                    PlainColor = new Color(s, colStrength, s);
                    WorldTransform = Matrix.CreateScale(2, 25, 2);
                    break;
                case AxisDirections.Z:
                    PlainColor = new Color(s, s, colStrength);
                    WorldTransform = Matrix.CreateScale(2, 2, 25);
                    break;
            }

            BasicEffect = new BasicEffect(Scene.GraphicsDevice);
            BasicEffect.DiffuseColor = PlainColor.ToVector3();
        }



        public override void RenderOverlayMesh(vxCamera3D Camera)
        {
            if (Scene.SandboxCurrentState == vxEnumSandboxStatus.EditMode && Model != null)
            {
                for (int m = 0; m < Model.Meshes.Count; m++)
                {
                    /*
                    float f = 1;
                    if (SelectionState != vxSelectionState.Selected &&
                      Gimbal.SelectionState == vxSelectionState.Selected)
                        f = 0.25f;

                    //AlphaValue = Mathematics.vxMathHelper.Smooth(AlphaValue, f * BaseAlpha, 4);
                    //BasicEffect.Alpha = AlphaValue;
                    */
                    BasicEffect.World = WorldTransform;
                    BasicEffect.View = Camera.View;
                    BasicEffect.Projection = Camera.Projection;
                    Model.Meshes[m].Draw(BasicEffect);
                }
            }
        }
    }
}
