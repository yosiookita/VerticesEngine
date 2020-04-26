using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Graphics;

namespace VerticesEngine
{
    public class vxCamera : vxEntity
    {
        /// <summary>
        /// What is the back colour for this camera
        /// </summary>
        public Color BackBufferColour = Color.CornflowerBlue;


        /// <summary>
        /// The type of the camera.
        /// </summary>
        public CameraType CameraType
        {
            get
            {
                return _cameraType;
            }
            set
            {
                _cameraType = value;
                OnCameraTypeChanged();
            }
        }

        CameraType _cameraType = CameraType.Freeroam;

        /// <summary>
        /// Called when the Camera Type is Changed
        /// </summary>
        protected virtual void OnCameraTypeChanged() { }


        public vxCameraProjectionType DefaultProjectionType = vxCameraProjectionType.Perspective;

        public vxCameraProjectionType EditorProjectionType = vxCameraProjectionType.Perspective;


        /// <summary>
        /// Gets or sets the type of the projection.
        /// </summary>
        /// <value>The type of the projection.</value>
        public vxCameraProjectionType ProjectionType
        {
            get { return _projectionType; }
            set
            {
                _projectionType = value;
                CalculateProjectionMatrix();

                EditorProjectionType = value;

                if (_projectionType == vxCameraProjectionType.Orthographic)
                    CameraType = CameraType.Orbit;
            }
        }
        vxCameraProjectionType _projectionType = vxCameraProjectionType.Perspective;

        /// <summary>
        /// The viewport for this Camera.
        /// </summary>
        public new Viewport Viewport
        {
            get { return _viewport; }
            set
            {
                _viewport = value;
                AspectRatio = _viewport.AspectRatio;
            }
        }
        Viewport _viewport;

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public Matrix View
        {
            get { return _viewMatrix; }
            set { _viewMatrix = value; }
        }
        protected Matrix _viewMatrix;


        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        /// <value>The projection.</value>
        public Matrix Projection
        {
            get { return _projectionMatrix; }
            set { _projectionMatrix = value; }
        }
        protected Matrix _projectionMatrix;

        /// <summary>
        /// Gets the view projection.
        /// </summary>
        /// <value>The view projection.</value>
        public Matrix ViewProjection
        {
            get { return _viewProjection; }
        }
        protected Matrix _viewProjection;


        public Matrix InverseView;

        public Matrix InverseProjection;

        /// <summary>
        /// Gets the invert view projection matrix.
        /// </summary>
        /// <value>The invert view projection.</value>
        public Matrix InverseViewProjection
        {
            get { return _invertViewProjection; }
        }
        protected Matrix _invertViewProjection = Matrix.Identity;


        /// <summary>
        /// Gets the previous view projection matrix for use in Temporal Effects 
        /// such as Camera Motion Blur.
        /// </summary>
        /// <value>The previous view projection.</value>
        public Matrix PreviousViewProjection
        {
            get { return _previousviewProjection; }
        }
        protected Matrix _previousviewProjection = Matrix.Identity;



        public BoundingFrustum BoundingFrustum;


        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>The field of view.</value>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; CalculateProjectionMatrix(); }
        }
        protected float _fieldOfView;

        [vxGraphicalSettings("Camera.FieldOfView")]
        public static float DefaultFieldOfView
        {
            get { return _defaultFieldOfView; }
            set { _defaultFieldOfView = value; }
        }
        static float _defaultFieldOfView = 60;


        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public float AspectRatio
        {
            get { return _aspectRatio; }
            set { _aspectRatio = value; CalculateProjectionMatrix(); }
        }
        protected float _aspectRatio;


        /// <summary>
        /// Gets or sets the near plane.
        /// </summary>
        /// <value>The near plane.</value>
        public float NearPlane
        {
            get { return _nearPlane; }
            set { _nearPlane = value; CalculateProjectionMatrix(); }
        }
        protected float _nearPlane;


        /// <summary>
        /// Gets or sets the far plane.
        /// </summary>
        /// <value>The far plane.</value>
        public float FarPlane
        {
            get { return _farPlane; }
            set { _farPlane = value; CalculateProjectionMatrix(); }
        }
        protected float _farPlane;

        public float Zoom
        {
            get { return _zoom; }
            set {
                _zoom = value;
                _zoom = MathHelper.Clamp(_zoom, MinZoom, MaxZoom);
                // Console.WriteLine(_zoom);
            }
        }
        protected float _zoom = -15;

        public float MinZoom = 0.02f;
        public float MaxZoom = 80000f;

        /// <summary>
        /// The orbit target of the Camera in Orbit mode.
        /// </summary>
        public Vector3 OrbitTarget = Vector3.Zero;

        /// <summary>
        /// Gets or sets the Requested orbit zoom factor.
        /// </summary>
        /// <value>The orbit zoom.</value>
        public float OrbitZoom
        {
            get { return _reqZoom; }
            set { _reqZoom = value; }
        }
        float _reqZoom = -15;


        /// <summary>
        /// Gets or sets the requested yaw rotation of the camera.
        /// </summary>
        public float ReqYaw
        {
            get { return _reqYaw; }
            set { _reqYaw = MathHelper.WrapAngle(value); }
        }
        private float _reqYaw;

        /// <summary>
        /// Gets or sets the requested pitch rotation of the camera.
        /// </summary>
        public float ReqPitch
        {
            get { return _reqPitch; }
            set
            {
                _reqPitch = value;
                if (_reqPitch > MathHelper.PiOver2 * .99f)
                    _reqPitch = MathHelper.PiOver2 * .99f;
                else if (_reqPitch < -MathHelper.PiOver2 * .99f)
                    _reqPitch = -MathHelper.PiOver2 * .99f;
            }
        }
        private float _reqPitch;


        /// <summary>
        /// Gets the world transformation of the camera.
        /// </summary>
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }
        private Matrix worldMatrix;

        /// <summary>
        /// Position of camera in world space.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        private Vector3 _position;

        /// <summary>
        /// The main scene renderer for this camera
        /// </summary>
        public vxRenderer Renderer;

        vxDebugRenderPass debugRenderPass;

        /// <summary>
        /// Whether or not the Camera should or can take input currently.
        /// </summary>
        public bool CanTakeInput = true;


        public vxCamera(vxGameplaySceneBase sceneBase) : base(sceneBase)
        {
            Renderer = new vxRenderer(this);
            _viewport = GraphicsDevice.Viewport;

            BoundingFrustum = new BoundingFrustum(Matrix.Identity);

            // initialise the renderer
            InitialiseRenderer();

            debugRenderPass = new vxDebugRenderPass(Renderer);
            Renderer.RenderingPasses.Add(debugRenderPass);

            // now reset the graphics
            OnGraphicsRefresh();

            // now register renderer items
            foreach (var pass in Renderer.RenderingPasses)
                pass.RegisterRenderTargetsForDebug();
        }

        protected virtual void InitialiseRenderer()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            debugRenderPass.Update();
        }

        public virtual void ResetCamera()
        {

        }

        /// <summary>
        /// This is the normalised bounds of where this camera is rendered too.
        /// </summary>
        public Rectangle NormalisedBounds = new Rectangle(0, 0, 1, 1);

        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();

            _viewport = GraphicsDevice.Viewport;
            //_viewport = new Viewport(
            //    NormalisedBounds.X * GraphicsDevice.Viewport.Width,
            //    NormalisedBounds.Y * GraphicsDevice.Viewport.Height,
            //    NormalisedBounds.wi * GraphicsDevice.Viewport.Width,
            //    NormalisedBounds.Y * GraphicsDevice.Viewport.Height);

            Renderer.OnGraphicsRefresh();
        }



        protected internal void CalculateProjectionMatrix()
        {
            if (ProjectionType == vxCameraProjectionType.Perspective)
                _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _nearPlane, _farPlane);
            else
                _projectionMatrix = Matrix.CreateOrthographic(_viewport.Width * _zoom / 10000, _viewport.Height * _zoom / 10000, -_farPlane, _farPlane);

        }


        /// <summary>
        /// Performs Frustm Culling on the current list of items
        /// </summary>
        void PreCull()
        {
            Renderer.totalItemsToDraw = 0;

            // loop through all entities and create a list of 
            for(int i = 0; i < Engine.CurrentScene.Entities.Count; i++)
            {
                // is this item in the bounding frustum
                if(Engine.CurrentScene.Entities[i].IsEntityCullable == false ||
                    Engine.CurrentScene.Entities[i].IsEntityCullable && BoundingFrustum.Intersects(Engine.CurrentScene.Entities[i].BoundingShape))
                {
                    // add it's index to the draw list
                    Renderer.drawList[Renderer.totalItemsToDraw] = i;
                    Renderer.totalItemsToDraw++;
                }
            }
        }

        void ReCalculateAllMatrices()
        {
            // calculate ViewProjection
            _viewProjection = _viewMatrix * _projectionMatrix;

            // calculate inverse projection mat
            _invertViewProjection = Matrix.Invert(_viewProjection);


            InverseView = Matrix.Invert(_viewMatrix);

            InverseProjection = Matrix.Invert(_projectionMatrix);

            //Console.WriteLine(_zoom);
            //BoundingFrustum = new BoundingFrustum(_invertViewProjection);
            BoundingFrustum.Matrix = _viewProjection;
        }


        public void Render()
        {
            // set this camera as the active camera
            Engine.GraphicsDevice.Viewport = Viewport;

            // first recalculate all matrices
            ReCalculateAllMatrices();

            // now perform the culling step
            PreCull();

            // Loop through all entities in the scene and render them
            Renderer.RenderScene();
        }


        public override void Dispose()
        {
            base.Dispose();

            Renderer.Dispose();
        }


        public T CastAs<T>() where T : vxCamera
        {
            return (T)this;
        }
    }
}
