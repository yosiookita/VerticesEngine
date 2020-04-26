
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using VerticesEngine.Graphics;

namespace VerticesEngine.Util
{
    public class vxScaleCube : vxEditorEntity
    {
        Matrix preMat = Matrix.Identity;
        Vector3 prePos;
        BasicEffect BasicEffect;

        float ScreenSpaceZoomFactor = 1;


        /// <summary>
        /// The Cube is moved
        /// </summary>
        public event EventHandler<EventArgs> Moved;

        public vxScaleCube(vxGameplayScene3D scene, Vector3 StartPosition, vxEntity3D Parent) :
        base(scene, vxEntityCategory.Entity)
        {
            this.Parent = Parent;

            BasicEffect = new BasicEffect(Scene.GraphicsDevice);
            BasicEffect.DiffuseColor = Color.Magenta.ToVector3();
        }

        public override vxModel OnLoadModel()
        {
            return vxInternalAssets.Models.UnitBox;
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Math.Abs(Vector3.Subtract(WorldTransform.Translation, prePos).Length()) > 0.0005f)
            {
                if (SelectionState == vxSelectionState.Selected)
                {
                    // Raise the 'Moved' event.
                    if (Moved != null)
                        Moved(this, new EventArgs());
                }
            }

            prePos = WorldTransform.Translation;

        }

        public override void RenderOverlayMesh(vxCamera3D Camera)
        {
            if (Scene.SandboxCurrentState == vxEnumSandboxStatus.EditMode && Model != null)
            {
                ScreenSpaceZoomFactor = Math.Abs(Vector3.Subtract(Position, Camera.Position).Length());

                foreach (vxModelMesh mesh in Model.Meshes)
                {
                    BasicEffect.DiffuseColor = (SelectionState == vxSelectionState.Selected ? Color.DarkOrange :  Color.White).ToVector3();

                    BasicEffect.World = Matrix.CreateScale(ScreenSpaceZoomFactor / 100) * WorldTransform;
                    BasicEffect.View = Camera.View;
                    BasicEffect.Projection = Camera.Projection;
                    mesh.Draw(BasicEffect);
                }
            }
        }

    }
}