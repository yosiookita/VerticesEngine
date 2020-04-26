using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Commands;
using VerticesEngine.Diagnostics;
using VerticesEngine.Graphics;
using VerticesEngine.Input;

namespace VerticesEngine.Util
{
    /// <summary>
    /// Axis Object for editing Sandbox Entity Position in the Sandbox Enviroment.
    /// </summary>
    public class vxGizmoPlanePanEntity : vxGizmoTransformationBaseEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Entities.Util.vxGizmoPlanePanEntity"/> class.
        /// </summary>
        /// <param name="Gimbal">Gimbal.</param>
        /// <param name="AxisDirections">Axis directions.</param>
        public vxGizmoPlanePanEntity(vxGimbal Gimbal, AxisDirections AxisDirections)
            : base(Gimbal.Scene, Gimbal, AxisDirections)
        {
            BaseAlpha = 0.25f;
            Scale = Vector3.One * 3;
        }

        public override vxModel OnLoadModel() { return vxInternalAssets.Models.UnitPlanePan; }

        // Screen Space Positions
        Vector2 ssPoint1 = new Vector2(0);
        Vector2 ssPoint2 = new Vector2(0);

        Vector3 DeltaPosition = new Vector3();

        bool FirstCheck = true;
        Matrix OffsetPos;

        public override void RenderOverlayMesh(vxCamera3D Camera)
        {
            //if(SelectionState == vxSelectionState.Selected)
            base.RenderOverlayMesh(Camera);

            Vector3 pnt1 = new Vector3(), pnt2 = new Vector3(), pnt3 = new Vector3();

            MainAxis.Normalize();
            MovementPlane = Vector3.One - MainAxis;
            switch (AxisDirections)
            {
                case AxisDirections.X:
                    pnt1 = new Vector3(MovementPlane.Y, 0, 0);
                    pnt2 = new Vector3(MovementPlane.Y, MovementPlane.Y, 0);
                    pnt3 = new Vector3(0, MovementPlane.Z, 0);
                    break;
                case AxisDirections.Y:
                    pnt1 = new Vector3(MovementPlane.X, 0, 0);
                    pnt2 = new Vector3(MovementPlane.X, 0, MovementPlane.X);
                    pnt3 = new Vector3(0, 0, MovementPlane.X);
                    //OffsetPos = Matrix.CreateRotationY(MathHelper.PiOver2);// new Vector3(3.0f, 0, 0) * Gimbal.ZoomFactor / (Gimbal.scale * 2);
                    break;
                case AxisDirections.Z:
                    pnt1 = new Vector3(0, 0, MovementPlane.Y);
                    pnt2 = new Vector3(0, MovementPlane.Y, MovementPlane.Y);
                    pnt3 = new Vector3(0, 0, MovementPlane.Y);
                    break;
            }

            vxDebug.DrawLine(Vector3.Transform(pnt1, ScaleMatrix * OffsetPos * WorldTransform),
                                         Vector3.Transform(pnt2, ScaleMatrix * OffsetPos * WorldTransform), PlainColor);

            vxDebug.DrawLine(Vector3.Transform(pnt2, ScaleMatrix * OffsetPos * WorldTransform),
                                         Vector3.Transform(pnt3, ScaleMatrix * OffsetPos * WorldTransform), PlainColor);
        }

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

        Vector3 MovementPlane = Vector3.One;
        public override void Update(GameTime gameTime)
        {

            if (Scene.Cameras[0].CameraType == CameraType.Freeroam)
            {
                OffsetPos = Matrix.Identity;
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
                        OffsetPos = Matrix.CreateRotationY(MathHelper.Pi);// new Vector3(3.0f, 0, 0) * Gimbal.ZoomFactor / (Gimbal.scale * 2);
                        break;
                }
                // Calculate the Movement Plane
                // (1,1,1) - (1,0,0) = (0,1,1) => Movement Plane
                MainAxis.Normalize();
                MovementPlane = Vector3.One - MainAxis;
                //MovementPlane.Normalize();

                //Set the World of the Arrows
                //Scale = Vector3.One * ParentCursor.ZoomFactor / (ParentCursor.scale * 2);
                WorldTransform = OffsetPos * Matrix.CreateScale(vxGimbal.ScreenSpaceZoomFactor / (Gimbal.scale * 2)) *
                              Matrix.CreateWorld(Gimbal.Position, MainAxis, PerpendicularAxis);

                base.Update(gameTime);


                Color = PlainColor;

                //Handle if Selected
                if (SelectionState == vxSelectionState.Selected)
                {
                    

                    //if (Input.IsNewMouseButtonPress(MouseButtons.LeftButton))
                    if (FirstCheck)
                    {
                        FirstCheck = false;
                        DeltaPosition = Vector3.Zero;
                        StartPosition = Gimbal.Position;

                        // set the inital position
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


                    // TODO Add UnProject/Ray Casting/Plane Intersection code
                    //GraphicsDevice.Viewport.Unproject();
                    //Plane pl = new Plane()

                    switch (AxisDirections)
                    {
                        case AxisDirections.X:
                            MovementPlane = new Vector3(0, 1-MouseDelta.Y, MouseDelta.X);
                            break;
                        case AxisDirections.Y:
                            MovementPlane = new Vector3(MouseDelta.X, 0, MouseDelta.Y);
                            break;
                        case AxisDirections.Z:
                            MovementPlane = new Vector3(MouseDelta.X, 1-MouseDelta.Y, 0);
                            break;
                    }


                    Vector2 ssVec = ssPoint2 - ssPoint1;


                    //MouseDelta.Normalize();
                    float dot = 1;//Vector2.Dot(MouseDelta, ssVec) / 15f;
                    if (MouseDelta.Length() > 0.001f)
                    {
                        Vector3 delta = (MovementPlane) * (dot) * vxGimbal.ScreenSpaceZoomFactor / (Gimbal.scale * 40);
                        DeltaPosition += delta;
                        // now apply to all entities.
                        for (int i = 0; i < Scene.SelectedItems.Count; i++)
                        {
                            var entity = Scene.SelectedItems[i];
                            entity.WorldTransform = entity.WorldTransform * Matrix.CreateTranslation(delta);
                        }
                    }


                    // If it's released, then apply the Command 
                    if (vxInput.IsNewMouseButtonRelease(MouseButtons.LeftButton))
                    {
                        // get the Delta
                        for (int i = 0; i < Scene.SelectedItems.Count; i++)
                        {
                            var entity = Scene.SelectedItems[i];
                            DeltaPosition = entity.WorldTransform.Translation - entity.PreSelectionWorld.Translation;
                            entity.WorldTransform = entity.PreSelectionWorld;
                        }

                        if (DeltaPosition != Vector3.Zero)
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
                    if (Gimbal.SelectionState == vxSelectionState.Selected)
                    {

                    }
                }
            }
        }
    }
}