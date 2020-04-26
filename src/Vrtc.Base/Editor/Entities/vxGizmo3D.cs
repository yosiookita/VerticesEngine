
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
//Virtex vxEngine Declaration
using VerticesEngine.Graphics;

namespace VerticesEngine.Util
{
    public enum AxisDirections
    {
        X,
        Y,
        Z,
    }

        enum SelectedGizmoItem
    {
        Axis,
        Rotator,
        Pan,
        None
    }


    /// <summary>
    /// Transformation type.
    /// </summary>
    public enum TransformationType
    {
        Local,
        Global
    }



    /// <summary>
    /// A 3D gimbal entity for use in editing Sandbox Entity Position and Rotation.
    /// </summary>
	public class vxGimbal : vxEditorEntity
    {
        float CubeSize = 2.5f;

        public static float ScreenSpaceZoomFactor = 25;


        public List<vxGizmoAxisTranslationEntity> Axes = new List<vxGizmoAxisTranslationEntity>();
        public List<vxGizmoAxisRotationEntity> Rotators = new List<vxGizmoAxisRotationEntity>();
        public List<vxGizmoPlanePanEntity> Pans = new List<vxGizmoPlanePanEntity>();

        Vector3 CursorAverage = Vector3.Zero;

        //public List<vxEntity3D> SelectedItems = new List<vxEntity3D>();

        //Reset
        public int scale = 8;

        /// <summary>
        /// The type of the rotation.
        /// </summary>
        public TransformationType TransformationType = TransformationType.Global;


        Vector3 PreviousPosition = Vector3.Zero;



        public override vxModel OnLoadModel() { return vxInternalAssets.Models.UnitBox; }

        /// <summary>
        /// Get's whether the Mouse is currently Hovering under ANY Axis in the Cursor.
        /// </summary>
        public bool IsMouseHovering
        {
            get { return GetIsMouseHovering(); }
        }

        private bool GetIsMouseHovering()
        {
            //Search through each axis too see if either one is Hovered or Selected (i.e. Not Unselected)
            for (int a = 0; a < Axes.Count; a++)
            {
                if (Axes[a].SelectionState != vxSelectionState.None)
                    return true;
            }

            //If they're all Unselected, then return false
            return false;
        }

        void AddListToChildren(List<vxGameObject> items)
        {
            foreach (vxGameObject item in items)
            {
                Children.Add(item);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Entities.Util.vxGizmo3D"/> class.
        /// </summary>
        /// <param name="scene">Scene.</param>
        public vxGimbal(vxGameplayScene3D scene) : base(scene, vxEntityCategory.Entity)
        {
            //Always Start The Cursor at the Origin
            WorldTransform = Matrix.CreateScale(CubeSize);

            //model = vxInternalAssets.Models.UnitBox.ModelMain;




            //Build the Axises
            AddAxis(new vxGizmoAxisTranslationEntity(this, AxisDirections.X));
            AddAxis(new vxGizmoAxisTranslationEntity(this, AxisDirections.Y));
            AddAxis(new vxGizmoAxisTranslationEntity(this, AxisDirections.Z));

            /*
            AddPan(new vxGizmoPlanePanEntity(this, AxisDirections.X));
            AddPan(new vxGizmoPlanePanEntity(this, AxisDirections.Y));
            AddPan(new vxGizmoPlanePanEntity(this, AxisDirections.Z));
            */
            AddRotator(new vxGizmoAxisRotationEntity(this, AxisDirections.X));
            AddRotator(new vxGizmoAxisRotationEntity(this, AxisDirections.Y));
            AddRotator(new vxGizmoAxisRotationEntity(this, AxisDirections.Z));



        }

        void AddAxis(vxGizmoAxisTranslationEntity axis)
        {
            Axes.Add(axis);
            //Children.Add(axis);
        }

        void AddRotator(vxGizmoAxisRotationEntity rotator)
        {
            Rotators.Add(rotator);
            //Children.Add(rotator);
        }

        void AddPan(vxGizmoPlanePanEntity pan)
        {
            Pans.Add(pan);
            //Children.Add(pan);
        }



        /// <summary>
        /// This is called every time a new entity is selected. This allows the gimbal to
        /// reset it's delta values.
        /// </summary>
        public void OnNewEntitySelection()
        {
            for(int i = 0; i < Scene.SelectedItems.Count; i++)
            {
                Scene.SelectedItems[i].PreSelectionWorld = Scene.SelectedItems[i].WorldTransform;
            }
        }



        public void Update(GameTime gameTime, Ray MouseRay)
        {
            PreviousPosition = this.Position;

            //Now Base Update to set Highlighting Colours
            base.Update(gameTime);

            //Always re-set the Selection State as it dependas on the child elements
            SelectionState = vxSelectionState.None;
            for (int a = 0; a < Axes.Count; a++)
            {
                if (Axes[a].SelectionState == vxSelectionState.Selected)
                {
                    SelectionState = vxSelectionState.Selected;

                }
            }
            for (int p = 0; p < Pans.Count; p++)
            {
                if (Pans[p].SelectionState == vxSelectionState.Selected)
                {
                    SelectionState = vxSelectionState.Selected;

                }
            }
            for (int r = 0; r < Rotators.Count; r++)
            {
                if (Rotators[r].SelectionState == vxSelectionState.Selected)
                {
                    SelectionState = vxSelectionState.Selected;

                }
            }

            if(TransformationType == TransformationType.Global)
            {
                for (int i = 0; i < Scene.SelectedItems.Count; i++)
                {
                    WorldTransform = Matrix.CreateTranslation(Scene.SelectedItems[i].Position);
                }
            }
            else if (TransformationType == TransformationType.Local)
            {
                for (int i = 0; i < Scene.SelectedItems.Count; i++)
                {
                    WorldTransform = Scene.SelectedItems[i].WorldTransform;
                }
            }


            // Update Cursor                        
            //**********************************************************

            CursorAverage = Vector3.Zero;
            for (int ind = 0; ind < Scene.SelectedItems.Count; ind++)
            {
                Scene.SelectedItems[ind].SelectionState = vxSelectionState.Selected;
                CursorAverage += Scene.SelectedItems[ind].WorldTransform.Translation;
            }

            {
                CursorAverage /= Scene.SelectedItems.Count;
                _worldTransform.Translation = CursorAverage;
            }
        }

        public override void Draw(vxCamera Camera, string renderpass)
        {
            //base.Draw(Camera, renderpass);
        }

        //public override void RenderPrepPass(vxCamera3D Camera) { }

        //public override void RenderMesh(vxCamera3D Camera) {}

        public override void RenderOverlayMesh(vxCamera3D Camera)
        {
            if (Scene.SandboxCurrentState == vxEnumSandboxStatus.EditMode)
            {
                SelectionColour = Color.White;
                //Set the Zoom Factor based off of distance from camera
                ScreenSpaceZoomFactor = Math.Abs(Vector3.Subtract(Position, Camera.Position).Length());

                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.BlendState = BlendState.AlphaBlend;
                Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                /*
                foreach (var entity in Rotators)
                    entity.RenderOverlayMesh(Camera);

                //foreach (var entity in Pans)
                    //entity.RenderOverlayMesh(Camera);

                Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

                foreach (var entity in Axes)
                    entity.RenderOverlayMesh(Camera);
                */



                /*
                Model model = Model.ModelMain;

                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["World"].SetValue(Matrix.CreateScale(ZoomFactor / (scale * 5)) * WorldTransform);

                        if (effect.Parameters["WorldInverseTranspose"] != null)
                            effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(WorldTransform)));

                        if (effect.Parameters["Texture"] != null)
                            effect.Parameters["Texture"].SetValue(vxInternalAssets.Textures.DefaultSurfaceMap);

                        // These Only need to be set once per model per loop. Not per entity.
                        if (effect.Parameters["View"] != null)
                            effect.Parameters["View"].SetValue(Camera.View);
                        if (effect.Parameters["Projection"] != null)
                            effect.Parameters["Projection"].SetValue(Camera.Projection);

                        if (effect.Parameters["CameraPos"] != null)
                            effect.Parameters["CameraPos"].SetValue(Camera.WorldMatrix.Translation);
                    }
                    // Draw the mesh, using the effects set above.
                    //mesh.Draw();
                }
                    */
            }
        }


        //public override void RenderDataMaskPass(vxCamera3D Camera)
        //{
        //    if (Scene.SandboxCurrentState == vxEnumSandboxStatus.EditMode && IsInCameraViewFrustum)
        //    {
        //        SelectionColour = Color.White;
        //        //Set the Zoom Factor based off of distance from camera
        //        ZoomFactor = Math.Abs(Vector3.Subtract(Position, Camera.Position).Length());

        //        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        //        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        //        Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;
        //        // Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


        //        foreach (var entity in Rotators)
        //            entity.RenderDataMaskPass(Camera);

        //        //foreach (var entity in Pans)
        //        //entity.RenderOverlayMesh(Camera);

        //        //Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

        //        foreach (var entity in Axes)
        //            entity.RenderDataMaskPass(Camera);


        //    }
        //}
        /*
                public override void RenderEncodedIndex(vxCamera3D Camera)
        {
            if (Scene.SandboxState == vxEnumSandboxStatus.EditMode && IsInCameraViewFrustum)
            {
                //Set the Zoom Factor based off of distance from camera
                ZoomFactor = Math.Abs(Vector3.Subtract(Position, Camera.Position).Length());

                GraphicsDevice.RasterizerState = RasterizerState.CullNone;


                //base.RenderEncodedIndex(Camera);
                //Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                foreach (var entity in Rotators)
                    entity.RenderEncodedIndex(Camera);
                
                //foreach (var pan in Pans)
                    //pan.RenderEncodedIndex(Camera);
                
                foreach (var entity in Axes)
                    entity.RenderEncodedIndex(Camera);
                
                //Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            }
        }
        */
    }
}
