using Microsoft.Xna.Framework;
using VerticesEngine.Input;

namespace VerticesEngine.Util
{
    /// <summary>
    /// Editor Entity
    /// </summary>
    public class vxEditorEntity : vxEntity3D
    {
        public vxEditorEntity(vxGameplayScene3D scene, vxEntityCategory category)
            : base(scene, null, Vector3.Zero, category)
        {

            foreach (var mesh in Model.Meshes)
            {
                mesh.Material.IsShadowCaster = false;
                mesh.Material.IsDefferedRenderingEnabled = false;
            }

            IsExportable = false;
            IsSaveable = false;

            //Remove from the main list so that it can be drawn over the entire scene
            //Scene.Entities.Remove(this);
            Scene.EditorEntities.Add(this);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            // If it's released, then apply the Command 
            if (vxInput.IsNewMouseButtonRelease(MouseButtons.LeftButton))
            {
                // Finally Deselect this
                SelectionState = vxSelectionState.None;
            }
        }

        public override void Draw(vxCamera Camera, string renderpass)
        {
            //base.Draw(Camera, renderpass);
        }
    }
}
