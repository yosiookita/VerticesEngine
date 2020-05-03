using Microsoft.Xna.Framework;
using VerticesEngine.Graphics;
using VerticesEngine.Input;

namespace VerticesEngine.Util
{
    /// <summary>
    /// Axis Object for editing Sandbox Entity Position in the Sandbox Enviroment.
    /// </summary>
    public class vxGizmoAxisRotationEntity : vxGizmoTransformationBaseEntity
    {
       
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:VerticesEngine.Entities.Util.vxGizmoAxisRotationEntity"/> class.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="Gimbal">Gimbal.</param>
        /// <param name="AxisDirections">Axis directions.</param>
        public vxGizmoAxisRotationEntity(vxGizmo Gimbal, AxisDirections AxisDirections)
            : base(Gimbal.Scene, Gimbal, AxisDirections)
        {

            Scale = Vector3.One * 12;
        }

        public override vxModel OnLoadModel() { return vxInternalAssets.Models.UnitTorus; }


        public override void Update(GameTime gameTime)
        {
            switch (AxisDirections)
            {
                case AxisDirections.X:
                    MainAxis = Gimbal.WorldTransform.Right;
                    PerpendicularAxis = Gimbal.WorldTransform.Up;
                    break;
                case AxisDirections.Y:
                    MainAxis = Gimbal.WorldTransform.Up;
                    PerpendicularAxis = Gimbal.WorldTransform.Backward;
                    break;
                case AxisDirections.Z:
                    MainAxis = Gimbal.WorldTransform.Backward;
                    PerpendicularAxis = Gimbal.WorldTransform.Up;
                    break;
            }

            Color = PlainColor;
            //Set the World of the Arrows
            WorldTransform = Matrix.CreateScale(1.25f * vxGizmo.ScreenSpaceZoomFactor / (Gimbal.scale)) *
                Matrix.CreateWorld(Gimbal.Position, MainAxis, PerpendicularAxis);


            base.Update(gameTime);

            //Handle if Selected
            if (SelectionState == vxSelectionState.Selected)
            {
                Color = Color.DeepSkyBlue;

                Rotation = (vxInput.Cursor.Y - vxInput.PreviousCursor.Y) / 100;
                //Console.WriteLine(Rotation/1000);
                Matrix rot = Matrix.Identity;

                MainAxis.Normalize();


                Vector3 rotAxis = Vector3.Zero;
                if (Gimbal.TransformationType == TransformationType.Global)
                {

                    switch (AxisDirections)
                    {
                        case AxisDirections.X:
                            rotAxis = Vector3.UnitX;
                            rot = Matrix.CreateRotationX(Rotation);
                            break;
                        case AxisDirections.Y:
                            rotAxis = Vector3.UnitY;
                            rot = Matrix.CreateRotationY(Rotation);
                            break;
                        case AxisDirections.Z:
                            rotAxis = Vector3.UnitZ;
                            rot = Matrix.CreateRotationZ(Rotation);
                            break;
                    }
                    for (int i = 0; i < Scene.SelectedItems.Count; i++)
                    {
                        var entity = Scene.SelectedItems[i];
                        // Get the World Matrix without the Translation
                        Matrix orientation = entity.WorldTransform * Matrix.CreateTranslation(-Position);

                        // Rotate it with the new rotation about the origin
                        orientation = orientation * rot;

                        // reapply the oreintation and Translation.
                        entity.WorldTransform = orientation * Matrix.CreateTranslation(Position);
                        entity.OnGimbalRotate(rotAxis, Rotation);
                    }
                }
                else if (Gimbal.TransformationType == TransformationType.Local)
                {
                    for (int i = 0; i < Scene.SelectedItems.Count; i++)
                    {
                        var entity = Scene.SelectedItems[i];
                        switch (AxisDirections)
                        {
                            case AxisDirections.X:
                                rotAxis = entity.WorldTransform.Right;
                                break;
                            case AxisDirections.Y:
                                rotAxis = entity.WorldTransform.Up;
                                break;
                            case AxisDirections.Z:
                                rotAxis = entity.WorldTransform.Backward;
                                break;
                        }
                        rotAxis.Normalize();
                        rot = Matrix.CreateFromAxisAngle(rotAxis, Rotation);

                        // Get the World Matrix without the Translation
                        Matrix orientation = entity.WorldTransform * Matrix.CreateTranslation(-Position);

                        // Rotate it with the new rotation about the origin
                        orientation = orientation * rot;

                        // reapply the oreintation and Translation.
                        entity.WorldTransform = orientation * Matrix.CreateTranslation(Position);
                        entity.OnGimbalRotate(rotAxis, Rotation);
                    }
                }
            }

            if (SelectionState == vxSelectionState.Selected && vxInput.IsNewMouseButtonRelease(MouseButtons.LeftButton))
                SelectionState = vxSelectionState.None;
        }
        float Rotation = 0;
    }
}