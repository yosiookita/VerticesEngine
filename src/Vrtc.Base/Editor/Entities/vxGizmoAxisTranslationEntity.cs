using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Commands;
using VerticesEngine.Graphics;
using VerticesEngine.Input;

namespace VerticesEngine.Util
{

    /// <summary>
    /// Axis Object for editing Sandbox Entity Position in the Sandbox Enviroment.
    /// </summary>
    public class vxGizmoAxisTranslationEntity : vxGizmoTransformationBaseEntity
    {
       
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:VerticesEngine.Entities.Util.vxGizmoAxisTranslationEntity"/> class.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="Gimbal">Gimbal.</param>
        /// <param name="AxisDirections">Axis directions.</param>
        public vxGizmoAxisTranslationEntity(vxGizmo Gimbal, AxisDirections AxisDirections)
            :base(Gimbal.Scene, Gimbal, AxisDirections)
        {
            
        }

        public override vxModel OnLoadModel() { return vxInternalAssets.Models.UnitArrow; }


        // Screen Space Positions
        Vector2 ssPoint1 = new Vector2(0);
        Vector2 ssPoint2 = new Vector2(0);

        Vector3 DeltaPosition = new Vector3();

        public override void Draw(vxCamera Camera, string renderpass)
        {

            ssPoint1 = GraphicsDevice.ProjectToScreenPosition(Position,
                                           Camera.Projection,
                                           Camera.View);
            ssPoint2 = GraphicsDevice.ProjectToScreenPosition(Position + MainAxis,
                                           Camera.Projection,
                                           Camera.View);

            base.Draw(Camera, renderpass);
        }

        bool FirstCheck = true;

        public override void Update(GameTime gameTime)
        {
            if (Scene.SandboxCurrentState == vxEnumSandboxStatus.EditMode)
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

                //Set the World of the Arrows
                //Scale = Vector3.One * ParentCursor.ZoomFactor / (ParentCursor.scale * 2);
                WorldTransform = Matrix.CreateScale(vxGizmo.ScreenSpaceZoomFactor / (Gimbal.scale)) *
                    Matrix.CreateWorld(Gimbal.Position, MainAxis, PerpendicularAxis);

                base.Update(gameTime);

                //hightLiteFactor = 1;
                Color = PlainColor;

                //Handle if Selected
                if (SelectionState == vxSelectionState.Selected)
                {
                    //hightLiteFactor = 2.0f;
                    Vector3 MovementAxis = MainAxis;
                    MovementAxis.Normalize();

                    //if (Input.IsNewMouseButtonPress(MouseButtons.LeftButton))
                    if(FirstCheck)
                    {
                        FirstCheck = false;
                        DeltaPosition = Vector3.Zero;
                        StartPosition = Gimbal.Position;

                        // set the inital position
                        for (int i = 0; i < Scene.SelectedItems.Count; i++)
                        {
                            var entity = Scene.SelectedItems[i];
                            entity.PreSelectionWorld = entity.WorldTransform;
                        }
                    }
                        

                    Color = Color.DeepSkyBlue;

                    // Get the Mouse Delta 
                    Vector2 MouseDelta = vxInput.Cursor - vxInput.PreviousCursor;
                    float MouseDeltaFactor = MouseDelta.Length();

                    // Now get the Dot Product of the 

                    Vector2 ssVec = ssPoint2 - ssPoint1;


                    //MouseDelta.Normalize();
                    float dot = Vector2.Dot(MouseDelta, ssVec) / 15f;

                    Vector3 delta = MovementAxis * (dot) * vxGizmo.ScreenSpaceZoomFactor / (Gimbal.scale * 40);
                    DeltaPosition += delta;
                    // now apply to all entities.
                    for (int i = 0; i < Scene.SelectedItems.Count; i++)
                    {
                        var entity = Scene.SelectedItems[i];
                        entity.WorldTransform = entity.WorldTransform * Matrix.CreateTranslation(delta);
                    }

                    // If it's released, then apply the Command 
                    if(vxInput.IsNewMouseButtonRelease(MouseButtons.LeftButton))
                    {
                        // get the Delta
                        for (int i = 0; i < Scene.SelectedItems.Count; i++)
                        {
                            var entity = Scene.SelectedItems[i];
                            DeltaPosition = entity.WorldTransform.Translation - entity.PreSelectionWorld.Translation;
                            entity.WorldTransform = entity.PreSelectionWorld;
                        }

                        if(DeltaPosition != Vector3.Zero)
                        {
                            // then create an entry in the command manager of the delta which applies it
                            Scene.CommandManager.Add(
                                new vxCMDTranslateSandbox3DItem(Scene,
                                                                Scene.SelectedItems,
                                                                DeltaPosition));
                            FirstCheck = true;
                        }

                        // Finally Deselect this
                        SelectionState = vxSelectionState.None;
                    }

                }
                else
                {
                    if(Gimbal.SelectionState == vxSelectionState.Selected)
                    {
                         
                    }
                }
            }
        }
    }
}