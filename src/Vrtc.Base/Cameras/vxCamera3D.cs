using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using VerticesEngine.Graphics;
using VerticesEngine.Input;
using VerticesEngine.Mathematics;
using VerticesEngine.Utilities;

namespace VerticesEngine
{
    /// <summary>
    /// Whether or not to use the default free-flight camera controls.
    /// Set to false when using vehicles or character controllers.
    /// </summary>
    public enum CameraType
	{
		Freeroam,
		CharacterFPS,
		ChaseCamera,
		Orbit,
		OnRails,
	}

    public enum vxCameraProjectionType
    {
        Perspective,
        Orthographic
    }

	/// <summary>
	/// Simple Camera class.
	/// </summary>
    public class vxCamera3D : vxCamera
	{
		/// <summary>
		/// Focal Distance Used During Depth of Field Calculations.
		/// </summary>
		public float FocalDistance
		{
			get { return _focalDistance; }
			set { _focalDistance = value; }
		}
		float _focalDistance = 40;


		/// <summary>
		/// Focal Width Used in the Depth of Field Calculations.
		/// </summary>
		public float FocalWidth
		{
			get { return _focalWidth; }
			set { _focalWidth = value; }
		}
		float _focalWidth = 75;


		/// <summary>
		/// Velocity of camera.
		/// </summary>
		public Vector3 Velocity
		{
			get { return _velocity; }
			set { _velocity = value; }
		}
		private Vector3 _velocity;


		/// <summary>
		/// Gets or sets the yaw rotation of the camera.
		/// </summary>
		public float Yaw
		{
			get { return _yaw; }
			set { _yaw = MathHelper.WrapAngle(value); }
		}
		private float _yaw;

		/// <summary>
		/// Gets or sets the pitch rotation of the camera.
		/// </summary>
		public float Pitch
		{
			get { return _pitch; }
			set
			{
				_pitch = value;
				if (_pitch > MathHelper.PiOver2 * .99f)
					_pitch = MathHelper.PiOver2 * .99f;
				else if (_pitch < -MathHelper.PiOver2 * .99f)
					_pitch = -MathHelper.PiOver2 * .99f;
			}
		}
		private float _pitch;


		//float _zoom = -15;

		#region Chase Camera Code

		#region Chased object properties (set externally each frame)

		/// <summary>
		/// Position of object being chased.
		/// </summary>
		public Vector3 ChasePosition
		{
			get { return _chasePosition; }
			set { _chasePosition = value; }
		}
		private Vector3 _chasePosition;

		/// <summary>
		/// Direction the chased object is facing.
		/// </summary>
		public Vector3 ChaseDirection
		{
			get { return _chaseDirection; }
			set { _chaseDirection = value; }
		}
		private Vector3 _chaseDirection;

		/// <summary>
		/// Chased object's Up vector.
		/// </summary>
		public Vector3 Up
		{
			get { return up; }
			set { up = value; }
		}
		private Vector3 up = Vector3.Up;

		#endregion

		#region Desired camera positioning (set when creating camera or changing view)

		/// <summary>
		/// Desired camera position in the chased object's coordinate system.
		/// </summary>
		public Vector3 DesiredPositionOffset
		{
			get { return _desiredPositionOffset; }
			set { _desiredPositionOffset = value; }
		}
		private Vector3 _desiredPositionOffset = new Vector3(0, 2.0f, 2.0f);

		/// <summary>
		/// Desired camera position in world space.
		/// </summary>
		public Vector3 DesiredPosition
		{
			get
			{
				// Ensure correct value even if update has not been called this frame
				UpdateWorldPositions();

				return _desiredPosition;
			}
		}
		private Vector3 _desiredPosition;

		/// <summary>
		/// Look at point in the chased object's coordinate system.
		/// </summary>
		public Vector3 LookAtOffset
		{
			get { return _lookAtOffset; }
			set { _lookAtOffset = value; }
		}
		private Vector3 _lookAtOffset = new Vector3(0, 2.8f, 0);

		/// <summary>
		/// Look at point in world space.
		/// </summary>
		public Vector3 LookAt
		{
			get
			{
				// Ensure correct value even if update has not been called this frame
				UpdateWorldPositions();

				return _lookAt;
			}
		}
		private Vector3 _lookAt;

		#endregion

		#region Camera physics (typically set when creating camera)

		/// <summary>
		/// Physics coefficient which controls the influence of the camera's position
		/// over the spring force. The stiffer the spring, the closer it will stay to
		/// the chased object.
		/// </summary>
		public float Stiffness
		{
			get { return stiffness; }
			set { stiffness = value; }
		}
		private float stiffness = 450000.0f;

		/// <summary>
		/// Physics coefficient which approximates internal friction of the spring.
		/// Sufficient damping will prevent the spring from oscillating infinitely.
		/// </summary>
		public float Damping
		{
			get { return damping; }
			set { damping = value; }
		}
		private float damping = 35000.0f;

		/// <summary>
		/// Mass of the camera body. Heaver objects require stiffer springs with less
		/// damping to move at the same rate as lighter objects.
		/// </summary>
		public float Mass
		{
			get { return mass; }
			set { mass = value; }
		}
		private float mass = 1000.0f;

        #endregion

        #endregion

        #region -- Components --

        private vxCameraOrbitController OrbitController;
        private vxCameraFreeRoamController FreeRoamController;
        private vxCameraFpsController FpsController;

        #endregion

        public vxCamera3D(vxGameplaySceneBase sceneBase) : base(sceneBase)
        {

        }

		//public vxCamera3D(vxEngine Engine, CameraType CameraType, Vector3 position, float pitch, float yaw, Matrix projectionMatrix)
		public vxCamera3D(vxGameplaySceneBase sceneBase, CameraType CameraType,
						  Vector3 Position,
						  float Pitch = 0, float Yaw = 0,
						  float NearPlane = 0.1f, float FarPlane = 1000,
						 float FieldOfView = MathHelper.PiOver4) : base(sceneBase)
        {
            // add controllers
            OrbitController = AddComponent<vxCameraOrbitController>();
            FreeRoamController = AddComponent<vxCameraFreeRoamController>();
            FpsController = AddComponent<vxCameraFpsController>();

			// Get a reference to the Engine
			this.Engine = vxEngine.Instance;

			// What type of Camera is it, Orbiting, FPS, Chase etc...
			this.CameraType = CameraType;



			// Set Position and Orientation
			this.Position = Position;
			this.Yaw = Yaw;
			this.Pitch = Pitch;

			// Now set Optional Items
			_fieldOfView = FieldOfView;
			_nearPlane = NearPlane;
			_farPlane = FarPlane;

            // Where is the Camera's Viewport on the Screen.
            Viewport = Engine.GraphicsDevice.Viewport;
            _aspectRatio = Viewport.AspectRatio;


			CalculateProjectionMatrix();

			if (CameraType == CameraType.ChaseCamera)
				Reset();
        }

		public void DrawDebugOutput()
		{
			string output = string.Format("Viewport: {0}\nAspect Ratio:{1}", Viewport.ToString(), Viewport.AspectRatio);

            //Engine.SpriteBatch.Begin();
            vxEngine.Instance.SpriteBatch.Begin("Camera Debug");
            vxEngine.Instance.SpriteBatch.DrawString(vxInternalAssets.Fonts.DebugFont, output, Vector2.Zero, Color.Black);
            vxEngine.Instance.SpriteBatch.DrawString(vxInternalAssets.Fonts.DebugFont, output, Vector2.Zero - Vector2.One, Color.White);
            vxEngine.Instance.SpriteBatch.End();
			//Engine.SpriteBatch.End();(
		}

        protected override void InitialiseRenderer()
        {
            base.InitialiseRenderer();

            Renderer.RenderingPasses.Add(new vxMainScene3DRenderPass(Renderer));
            Renderer.RenderingPasses.Add(new vxCascadeShadowRenderPass(Renderer));
            Renderer.RenderingPasses.Add(new vxScenePrepRenderingPass(Renderer));

            // lighting post processes
            Renderer.RenderingPasses.Add(new vxSceneLightRenderingPass(Renderer));
            Renderer.RenderingPasses.Add(new vxEdgeDetectPostProcess(Renderer));
            //Renderer.RenderingPasses.Add(new vxMainScene3DAlphaRenderPass(Renderer));
            Renderer.RenderingPasses.Add(new vxDistortionPostProcess(Renderer));
            //Renderer.RenderingPasses.Add(new vxAntiAliasPostProcess(Renderer));


            // First Light the scene
            //DefferredRenderPostProcess = new vxDefferredRenderPostProcess(this);
            //PostProcessors.Add(DefferredRenderPostProcess);


            //// Create a blur utility mask with the scene
            //BlurScenePostProcess = new vxBlurScenePostProcess(this);
            //PostProcessors.Add(BlurScenePostProcess);


            //// Now perform any screenspace magic
            //SSRPostProcess = new vxSSRPostProcess(this);
            //PostProcessors.Add(SSRPostProcess);

            //SSAOPostProcess = new vxSSAOPostProcess(this);
            //PostProcessors.Add(SSAOPostProcess);

            //ScreenSpacePostProcesses = new vxScreenSpacePostProcess(this);
            //PostProcessors.Add(ScreenSpacePostProcesses);


            //// Now distort any reflections and the main lit scene
            //DistortionPostProcess = new vxDistortionPostProcess(this);
            //PostProcessors.Add(DistortionPostProcess);


            //// Now apply any fancy post processing like bloom
            //FinalScenePostProcess = new vxFinalScenePostProcess(this);
            //PostProcessors.Add(FinalScenePostProcess);

            //// Then smooth it out
            //FXAAPostProcess = new vxAntiAliasPostProcess(this);
            //PostProcessors.Add(FXAAPostProcess);

            //// and finally blur the scene
            //CameraMotionBlurPostProcess = new vxCameraMotionBlurPostProcess(this);
            //PostProcessors.Add(CameraMotionBlurPostProcess);



            //RandomTexture2D = vxInternalAssets.Textures.RandomValues;

            //foreach (var pstprcs in PostProcessors)
            //    pstprcs.LoadContent(StartupConfig);
        }

		


        /// <summary>
		/// Update the Camera by the specified GameTime.
		/// </summary>
		/// <returns>The update.</returns>
		/// <param name="time">Time.</param>
		public override void Update(GameTime time)
		{
            base.Update(time);

            // Get the Previous ViewProjection matrix
            _previousviewProjection = _viewProjection;// View * Projection;

			// Chase Camera
			// =============================================================================
			if (CameraType == CameraType.ChaseCamera)
			{
				UpdateWorldPositions();

				float elapsed = 0.0167f;// (float)gameTime.ElapsedGameTime.TotalSeconds;

				elapsed = ((float)time.ElapsedGameTime.Milliseconds) / 1000;
				//elapsed = (float)time.ElapsedGameTime.TotalSeconds;

				// Calculate spring force
				Vector3 stretch = Position - _desiredPosition;
				Vector3 force = -stiffness * stretch - damping * Velocity;

				// Apply acceleration
				Vector3 acceleration = force / mass;

				Velocity += acceleration * elapsed;

				// Apply velocity
				Position += Velocity * elapsed;

				WorldMatrix = Matrix.CreateWorld(Position, View.Forward, View.Up);

				UpdateMatrices();
			}


			vxConsole.WriteInGameDebug(this, "Camera Position: " + this.Position);
		}

        protected override void OnCameraTypeChanged()
        {
            // disable all camera controllers
            OrbitController.IsEnabled = CameraType == CameraType.Orbit;
            FreeRoamController.IsEnabled = CameraType == CameraType.Freeroam;
            FpsController.IsEnabled = CameraType == CameraType.CharacterFPS;
        }




		/// <summary>
		/// Rebuilds object space values in world space. Invoke before publicly
		/// returning or privately accessing world space values.
		/// </summary>
		private void UpdateWorldPositions()
		{
			if (CameraType == CameraType.ChaseCamera)
			{
				// Construct a matrix to transform from object space to worldspace
				Matrix transform = Matrix.Identity;
				transform.Forward = ChaseDirection;
				transform.Up = Up;
				transform.Right = Vector3.Cross(Up, ChaseDirection);

				// Calculate desired camera properties in world space
				_desiredPosition = ChasePosition +
					Vector3.TransformNormal(DesiredPositionOffset, transform);
				_lookAt = ChasePosition +
					Vector3.TransformNormal(LookAtOffset, transform);
			}
		}

		/// <summary>
		/// Rebuilds camera's view and projection matricies.
		/// </summary>
		private void UpdateMatrices()
		{
			if (CameraType == CameraType.ChaseCamera)
			{
				View = Matrix.CreateLookAt(Position, LookAt, Up);
				Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView,
					AspectRatio, NearPlane, FarPlane);
			}
		}

		/// <summary>
		/// The Reflection View Matrix.
		/// </summary>
		public Matrix ReflectionView = Matrix.Identity;

		/// <summary>
		/// Set's the Reflection View based off of the given Plane.
		/// </summary>
		/// <param name="SurfacePlane"></param>
		/// <returns></returns>
		public void SetReflectionView(Plane SurfacePlane)
		{
			if (CameraType == CameraType.ChaseCamera)
			{
				Vector3 ReflcPos = new Vector3(
					   this.Position.X,
					   -this.Position.Y + SurfacePlane.D * 1,
					   this.Position.Z);

				Vector3 ReflcLookAt = new Vector3(
					   this.LookAt.X,
					   -this.LookAt.Y + SurfacePlane.D * 1,
					   this.LookAt.Z);

				Vector3 ReflcUp = Vector3.Reflect(WorldMatrix.Up, SurfacePlane.Normal);
				ReflectionView = Matrix.CreateLookAt(ReflcPos, ReflcLookAt, -ReflcUp);
			}
			else
			{
				Vector3 ReflcPos = new Vector3(
					this.WorldMatrix.Translation.X,
					-this.WorldMatrix.Translation.Y + SurfacePlane.D * 1,
					this.WorldMatrix.Translation.Z);

				Vector3 ReflcLookAt = ReflcPos + new Vector3(
					this.WorldMatrix.Forward.X,
					-this.WorldMatrix.Forward.Y,
					this.WorldMatrix.Forward.Z);

				Vector3 ReflcUp = Vector3.Reflect(WorldMatrix.Up, SurfacePlane.Normal);

				ReflectionView = Matrix.CreateLookAt(ReflcPos, ReflcLookAt, -ReflcUp);
			}
		}

		/// <summary>
		/// Forces camera to be at desired position and to stop moving. The is useful
		/// when the chased object is first created or after it has been teleported.
		/// Failing to call this after a large change to the chased object's position
		/// will result in the camera quickly flying across the world.
		/// </summary>
		public void Reset()
		{
			if (CameraType == CameraType.ChaseCamera)
			{
				UpdateWorldPositions();

				// Stop motion
				Velocity = Vector3.Zero;

				// Force desired position
				Position = _desiredPosition;

				UpdateMatrices();
			}
		}
	}

}