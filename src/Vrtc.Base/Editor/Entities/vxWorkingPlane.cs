using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
//Virtex vxEngine Declaration

using VerticesEngine.Graphics;
using VerticesEngine.Input;

namespace VerticesEngine.Util
{
    public class vxWorkingPlane : vxEntity3D
    {
        /// <summary>
        /// Working Plane Object
        /// </summary>
        public Plane WrknPlane;

        /// <summary>
        /// Vertices Collection of Grid
        /// </summary>
        List<VertexPositionColor> vertices;

        /// <summary>
        /// Basic Effect to Render Working Plane
        /// </summary>
        BasicEffect WireFrameBasicEffect;
        BasicEffect PlaneBasicEffect;

        public float HeightOffset = 0;

        //public bool IsVisible = true;


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Entities.Util.vxWorkingPlane"/> class.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="entityModel">Entity model.</param>
        /// <param name="StartPosition">Start position.</param>
        public vxWorkingPlane(vxGameplayScene3D scene, vxModel entityModel, Vector3 StartPosition)
            : base(scene, entityModel, StartPosition)
        {
            IsSaveable = false;
            IsExportable = false;
            //Render even in debug mode
            RenderEvenInDebug = true;

            WrknPlane = new Plane(Vector3.Up, -Position.Y);


            WireFrameBasicEffect = new BasicEffect(this.Engine.GraphicsDevice);
            PlaneBasicEffect = new BasicEffect(this.Engine.GraphicsDevice);

            int size = 10000;


            vertices = new List<VertexPositionColor>();
            for (int i = -size; i < size + 1; i += 10)
            {
                Color color = i % 100 == 0 ? Color.White : Color.Gray * 1.5f;

                vertices.Add(new VertexPositionColor(
                     new Vector3(i, 0, -size),
                     color
                     ));

                vertices.Add(new VertexPositionColor(
                     new Vector3(i, 0, size),
                     color
                     ));


                vertices.Add(new VertexPositionColor(
                    new Vector3(-size, 0, i),
                    color
                    ));

                vertices.Add(new VertexPositionColor(
                     new Vector3(size, 0, i),
                     color
                     ));

            }
        }


        //public override void RenderPrepPass(vxCamera3D Camera) { }
        //public override void RenderToShadowMap(int ShadowMapIndex) { }

        /// <summary>
        /// Updates the Working Plane
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //Update If In Edit Mode
            if (Scene.SandboxCurrentState == vxEnumSandboxStatus.EditMode)
            {
                //Set the Working Plane Height
                if (Scene.UIManager.HasFocus == false
                    && vxInput.MouseState.MiddleButton == ButtonState.Released &&
                    Scene.SandboxEditMode == vxEnumSanboxEditMode.AddItem)
                {
                    if (vxInput.ScrollWheelDelta > 0)
                        HeightOffset += 0.5f;
                    else if (vxInput.ScrollWheelDelta < 0)
                        HeightOffset -= 0.5f;
                }
            }

            if (Scene.SandboxCurrentState == vxEnumSandboxStatus.EditMode &&
                (Scene.SandboxEditMode == vxEnumSanboxEditMode.AddItem||
                 Scene.SandboxEditMode == vxEnumSanboxEditMode.TerrainEdit))
            {
                WorldTransform = Matrix.CreateScale(1);

                WorldTransform *= Matrix.CreateTranslation(Vector3.Up * (0.5f + HeightOffset));

                WrknPlane.D = -HeightOffset - 0.5f;

                //AlphaValue = 0.5f;
            }
            else
            {
                WorldTransform = Matrix.CreateScale(1);
                WorldTransform *= Matrix.CreateTranslation(Vector3.Up * (1000000));
                WrknPlane.D = -1000000 - 0.5f;
            }
            base.Update(gameTime);
        }
        /*
        public override void RenderEncodedIndex(vxCamera3D Camera)
        {
            //base.RenderEncodedIndex(Camera);
        }
        */
        //public override void RenderMesh(vxCamera3D Camera)
        //{
        //    if (IsVisible)
        //    {
        //        //Set Basic Effect Info
        //        WireFrameBasicEffect.VertexColorEnabled = true;
        //        WireFrameBasicEffect.View = Camera.View;
        //        WireFrameBasicEffect.Projection = Camera.Projection;
        //        WireFrameBasicEffect.World = WorldTransform;

        //        //Render Vertices List
        //        foreach (EffectPass pass in WireFrameBasicEffect.CurrentTechnique.Passes)
        //        {
        //            pass.Apply();
        //            this.Engine.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
        //        }
        //    }
        //}

        public void DrawPlane(vxCamera3D Camera)
        {
            vxModel m = vxInternalAssets.Models.ViewerPlane;

            foreach (Graphics.vxModelMesh mesh in m.Meshes)
            {
                PlaneBasicEffect.DiffuseColor = Color.DeepSkyBlue.ToVector3();
                PlaneBasicEffect.View = Camera.View;
                PlaneBasicEffect.Projection = Camera.Projection;
                PlaneBasicEffect.World = Matrix.CreateScale(100) * Matrix.CreateTranslation(Vector3.One * -0.05f) * WorldTransform;

                PlaneBasicEffect.Alpha = 0.5f;
                //}
                mesh.Draw(PlaneBasicEffect);
            }
        }
    }
}
