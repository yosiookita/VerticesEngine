
using Microsoft.Xna.Framework;
//Virtex vxEngine Declaration
using VerticesEngine.Utilities;

namespace VerticesEngine
{

    public class vxSunEntity : vxEntity3D
    {
        /// <summary>
        /// This World Position of the "Sun"
        /// </summary>
        public Vector3 SunWorldPosition;

        // The Projected Screen Space Position
        public Vector3 ScreenSpacePosition;

        // The Actual Screen Position of the sun
        public Vector2 ScreenPosition
        {
            get
            {
                return new Vector2(ScreenSpacePosition.X, ScreenSpacePosition.Y);
            }
        }

        public float RotationX
        {
            get
            {
                return _rotationX;
            }

            set
            {
                _rotationX = value;
                UpdateSunWorld();
            }
        }
        private float _rotationX = 0.75f;

        public float RotationZ
        {
            get
            {
                return rotationZ;
            }

            set
            {
                rotationZ = value;
                UpdateSunWorld();
            }
        }
        private float rotationZ = 0.6f;


        public bool IsOnScreen = false;
        //public bool IsSunOccluded = false;


        public Vector3 LightDirection
        {
            get { return Vector3.Normalize(SunWorldPosition); }
        }

		public override VerticesEngine.Graphics.vxModel OnLoadModel()
		{
			return null;
		}
        private void UpdateSunWorld()
        {
            Scale = 1000;// Scene.Camera.FarPlane;

            WorldTransform = Matrix.CreateScale(Scale * 0.1f) *
    Matrix.CreateRotationX(MathHelper.PiOver2) *
    Matrix.CreateRotationY(inc) *
    Matrix.CreateRotationX(-MathHelper.PiOver2) *
    Matrix.CreateRotationX(RotationX) *
    Matrix.CreateRotationZ(RotationZ);

            SunWorldPosition = vxGeometryHelper.RotatePoint(WorldTransform, new Vector3(0, 0, Scale));

            Scene.LightPositions = SunWorldPosition;
        }

        /// <summary>
        /// SnapBox for allowing tracks to snap together
        /// </summary>
        /// <param name="Scene"></param>
        public vxSunEntity(vxGameplayScene3D Scene)
            : base(Scene, null, Vector3.Zero)
        {
            Scene.Entities.Remove(this);

			//RotationX = 0.75f;
			//RotationZ = 0.6f;
            //DoEdgeDetect = false;
            //IsSkyBox = true;
        }
        new float Scale = 10000;


        float inc = 0;
        //public int TextureSize = 3;
		public void DrawGlow(vxCamera3D Camera)
        {
            // Debug
            //vxConsole.WriteToInGameDebug(nameof(RotationX), RotationX);
            //vxConsole.WriteToInGameDebug(nameof(RotationZ), RotationZ);

            //if (vxInput.KeyboardState.IsKeyDown(Keys.Up))
            //    RotationX += 0.005f;
            //if (vxInput.KeyboardState.IsKeyDown(Keys.Down))
            //    RotationX -= 0.005f;
            //if (vxInput.KeyboardState.IsKeyDown(Keys.Left))
            //    RotationZ += 0.005f;
            //if (vxInput.KeyboardState.IsKeyDown(Keys.Right))
            //    RotationZ -= 0.005f;

            IsOnScreen = false;

            if (Vector3.Dot(Camera.WorldMatrix.Forward, SunWorldPosition) < 0)
            {
                //RenderTargets.RT_MaskMap
                IsOnScreen = true;

                ScreenSpacePosition = Engine.GraphicsDevice.Viewport.Project(
                    SunWorldPosition,
                    Camera.Projection,
                    Camera.View, Matrix.Identity);

                //Texture2D sunTexture = vxInternalAssets.Textures.Texture_Sun_Glow;
                //int Width = vxInternalAssets.Textures.Texture_Sun_Glow.Width * TextureSize;
                //int Height = vxInternalAssets.Textures.Texture_Sun_Glow.Height * TextureSize;

                //Engine.SpriteBatch.Begin();
                //Engine.SpriteBatch.Draw(sunTexture,
                //    new Rectangle((int)(ScreenSpacePosition.X - Width / 2), (int)(ScreenSpacePosition.Y - Height / 2), Width, Height),
                //                        Color.White);
                //Engine.SpriteBatch.End();
            }
        }
		
        //public override void RenderPrepPass(vxCamera3D Camera) { }
        //public override void RenderMesh(vxCamera3D Camera) { }
    }
}