
using BEPUphysics.BroadPhaseEntries;
using BEPUutilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using VerticesEngine.Graphics;
using VerticesEngine.Input;
using VerticesEngine.Serilization;

namespace VerticesEngine
{
    /// <summary>
    /// A 3D Terrain entity which is formed from a Height Map.
    /// </summary>
    public partial class vxTerrainEntity : vxEntity3D
    {
        //public static vxEntityRegistrationInfo Info
        //{
        //    get
        //    {
        //        return new vxEntityRegistrationInfo(true,
        //            typeof(vxTerrainEntity).ToString(),
        //        "Height Map",
        //        "Textures/terrain/terrain",
        //                                            delegate (vxGameplayScene3D scene)
        //            {
        //                return new vxTerrainEntity(scene, Vector3.Zero, 4);
        //            });
        //    }
        //}

        /// <summary>
        /// The owning Terrain Manager
        /// </summary>
        public vxTerrainManager TerrainManager
        {
            get { return Scene.TerrainManager; }
        }

        /// <summary>
        /// The overall dimensions of the Terrain.
        /// </summary>
        public int Dimension
        {
            get { return TerrainMesh.Dimension; }
            set { TerrainMesh.Dimension = value; }
        }


        /// <summary>
        /// The Individual Grid Size.
        /// </summary>
        public int CellSize
        {
            get { return _cellSize; }
        }
        int _cellSize = 2;


        /// <summary>
        /// The Height Displacement Map
        /// </summary>
        Texture2D DisplacementMap
        {
            get { return TerrainMesh.DisplacementMap; }
            set { TerrainMesh.DisplacementMap = value; }
        }

        // The Textures
        public Texture2D Texture01;// { set { TerrainMesh.Effect.Parameters["Texture01"].SetValue(value); } }
        public Texture2D Texture02;// { set { TerrainMesh.Effect.Parameters["Texture02"].SetValue(value); } }
        public Texture2D Texture03;// { set { TerrainMesh.Effect.Parameters["Texture03"].SetValue(value); } }
        public Texture2D Texture04;// { set { TerrainMesh.Effect.Parameters["Texture04"].SetValue(value); } }

        // Texture Weight maps
        Texture2D TextureWeightMap;// { set { TerrainMesh.Effect.Parameters["textureWeightMap"].SetValue(value); } }


        public vxTerrainMeshPart TerrainMesh
        {
            get { return (vxTerrainMeshPart)Model.Meshes[0].MeshParts[0]; }
        }
        //public vxTerrainMeshPart TerrainMesh_MidPoly;
        //public vxTerrainMeshPart TerrainMesh_LowPoly;



        float TextureUVScale
        {
            get { return _textureUVScale; }
            set
            {
                _textureUVScale = value;
                //TerrainMesh.MaxHeight.Effect.Parameters["TxtrUVScale"].SetValue(value);
            }
        }
        float _textureUVScale;


        float MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                _maxHeight = value;
                //TerrainMesh.Effect.Parameters["maxHeight"].SetValue(value);
            }
        }
        float _maxHeight;

        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                _isInEditMode = value;
                //TerrainMesh.Effect.Parameters["IsEditMode"].SetValue(_isInEditMode);
            }
        }
        bool _isInEditMode;




        int TextureMapSize = 256;

        Terrain PhysicsMesh;
        //Box entity;

        RenderTarget2D TextureMap;
        RenderTarget2D CanvasTextureMap;


        // a check to see if the entire buffer needs an update
        bool BufferNeedsFullUpdate = false;


		/// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Entities.vxTerrainEntity"/> class.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="StartPosition">Start position.</param>
        /// <param name="CellSize">Cell size.</param>
        public vxTerrainEntity(vxGameplayScene3D scene, Vector3 StartPosition, int CellSize = 2)
			: this(scene, null, StartPosition, CellSize)
        {

        }

        Texture2D InitialHeightMap;

		public virtual Texture2D GetInitialTexture()
		{
            return vxInternalAssets.LoadInternalTexture2D("Textures/terrain/Heightmap");
        }



        /// <summary>
        /// Creates a New Instance of a Terrain Entity
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="StartPosition"></param>
        /// <param name="WaterScale"></param>
        public vxTerrainEntity(vxGameplayScene3D scene, Texture2D HeightMap, Vector3 StartPosition, int CellSize = 2)
            : base(scene, null, StartPosition)
        {
			if (HeightMap == null)
				HeightMap = GetInitialTexture();

            InitialHeightMap = HeightMap;
            _cellSize = CellSize;

            Model = OnLoadModel();

            //InitShaders();

            EditMode = vxEnumTerrainEditMode.Disabled;

            // Set Mesh
            WorldTransform = Matrix.CreateRotationY(-MathHelper.PiOver2) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateTranslation(StartPosition);


            // Setup Cursor and Brushes
            CursorColour = Color.DeepSkyBlue;
            CursorTexture = vxInternalAssets.LoadInternalTexture2D("Textures/terrain/cursor/cursor");
            TextureBrush = vxInternalAssets.Textures.Blank;

            #region Initialise Texture Weight Map

            TextureMapSize = Dimension;
            TextureMap = new RenderTarget2D(GraphicsDevice, TextureMapSize, TextureMapSize, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            CanvasTextureMap = new RenderTarget2D(GraphicsDevice, TextureMapSize, TextureMapSize);//, false, SurfaceFormat., DepthFormat.Depth24);
            TextureUVScale = 10;
            MaxHeight = 128;

            Tag = this;


            #endregion

            UpdatePhysicsMesh();
        }

        public override vxModel OnLoadModel() {
            //vxModel model = new vxModel(Engine, null);
            //model.LoadTextures(Engine.Game.Content, "");
            if (InitialHeightMap == null)
            {
                return vxInternalAssets.Models.UnitBox;
            }

            var terrainModel = new vxModel();

            var terrainMeshPart = new vxTerrainMeshPart(InitialHeightMap, _cellSize);

            //            Model = new vxModel(Engine, null);
            terrainModel.Meshes.Add(new vxModelMesh());

            terrainModel.Meshes[0].MeshParts.Add(terrainMeshPart);

            return terrainModel;
        }

        public override void PreSave()
        {
            base.PreSave();

            TerrainData = new vxSerializableTerrainData(
                this.HandleID,
                this.ToString(),
                this.WorldTransform,
                Dimension,
                CellSize);

            TerrainData.Dimension = Dimension;
            TerrainData.CellSize = CellSize;

            TerrainData.HeightData.Clear();

            foreach (vxMeshVertex vertex in TerrainMesh.Vertices)
                TerrainData.HeightData.Add(
                    new vxSerializableTerrainVertex(vertex.Position, 0.5f));

        }


        /// <summary>
        /// This holds the Terrain Data to be saved.
        /// </summary>
        public vxSerializableTerrainData TerrainData = new vxSerializableTerrainData();


        public override void PostEntityLoad()
        {
            base.PostEntityLoad();


            // Check Dimensions and Cell Size;
            //if (Dimension == TerrainData.Dimension)
            //    vxConsole.WriteLine("Terrain Dimension Matches");

            foreach (vxSerializableTerrainVertex vertex in TerrainData.HeightData)
            {
                int x = (int)(vertex.X / TerrainData.CellSize);
                int z = (int)(vertex.Z / TerrainData.CellSize);

                // Set the Height
                TerrainMesh[x, z] = vertex.Y;
            }

            TerrainMesh.CalculateNormals();
            TerrainMesh.UpdateDynamicVertexBuffer();

            //OnAdded();
        }

        public override void Dispose()
        {
            base.Dispose();

            //Scene.PhyicsSimulation.Remove(entity);
            //Scene.PhysicsDebugViewer.Remove(entity);
        }

        public override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();

            int size = 40;

            int x = (int)Position.X / size;
            int z = (int)Position.Z / size;

            Vector3 newPosition = new Vector3(x * size, Position.Y, z * size);

            _worldTransform = Matrix.CreateRotationY(-MathHelper.PiOver2) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateTranslation(newPosition);
        }

        public override void OnSandboxStatusChanged(bool IsRunning)
        {
            base.OnSandboxStatusChanged(IsRunning);

            if (IsRunning)
            {
                PhysicsMesh = new Terrain(TerrainMesh.HeightData,
new AffineTransform(new Vector3(CellSize, 1, CellSize), Quaternion.Identity, Position));

                Scene.PhyicsSimulation.Add(PhysicsMesh);
                Scene.PhysicsDebugViewer.Add(PhysicsMesh);

                //entity.CollisionInformation.Tag = null;

                //Scene.PhysicsDebugViewer.Remove(entity);
            }
            else
            {
                //Scene.PhysicsDebugViewer.Add(entity);

                PhysicsMesh.Tag = null;

                Scene.PhyicsSimulation.Remove(PhysicsMesh);
                Scene.PhysicsDebugViewer.Remove(PhysicsMesh);
                PhysicsMesh = null;
            }
        }

        /// <summary>
        /// Updates the Physics Mesh
        /// </summary>
        public virtual void UpdatePhysicsMesh()
        {
            //entity = new Box(Position + new Vector3(Dimension * CellSize / 2, 0, Dimension * CellSize / 2), Dimension * CellSize, 10, Dimension * CellSize);

            //Scene.PhyicsSimulation.Add(entity);
            //Scene.PhysicsDebugViewer.Add(entity);
        }

        public override void OnAdded()
        {
            base.OnAdded();

            //Scene.PhyicsSimulation.Remove(entity);
            //Scene.PhysicsDebugViewer.Remove(entity);

            UpdatePhysicsMesh();

            _worldTransform = Matrix.CreateRotationY(-MathHelper.PiOver2) *
                Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateTranslation(Position);


        }

        //public override void SetIndex(int NewIndex)
        //{
        //    entity.CollisionInformation.Tag = NewIndex;
        //    base.SetIndex(NewIndex);
        //}



        public override void OnFirstUpdate(GameTime gameTime)
        {
            base.OnFirstUpdate(gameTime);

            // Add into the Terrain manager on the first loop
            TerrainManager.Add(this);

            // Set the Texture Map
            Engine.GraphicsDevice.SetRenderTarget(TextureMap);
            Engine.GraphicsDevice.Clear(Color.Black);
            Engine.GraphicsDevice.BlendState = BlendState.Opaque;

            // Sometime another spritebatch is running, this is a check
            try
            {
                Engine.SpriteBatch.Begin("Terrain First Update",0, BlendState.Opaque, SamplerState.PointClamp, null, null);
            }
            catch
            {
                Engine.SpriteBatch.End();
                Engine.SpriteBatch.Begin("Terrain First Update",0, BlendState.Opaque, SamplerState.PointClamp, null, null);
            }
            Engine.SpriteBatch.Draw(InitialHeightMap, TextureMap.Bounds, Color.White);
            Engine.SpriteBatch.End();
            Engine.GraphicsDevice.SetRenderTarget(null);
            TextureWeightMap = TextureMap;
        }

        float Minheight = -200;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            IsVisible = true;


            // Set whether it's in Edit mode or not
            IsInEditMode = (Scene.SandboxEditMode == vxEnumSanboxEditMode.TerrainEdit);

            if (IsInEditMode)
            {
                CursorPosition = Scene.Intersection.ToVector2();// - new Vector2(CursorScale * CellSize / 4);// - Position.ToVector2();
                //Console.WriteLine(Scene.Intersection);
                // Set Scroll Size
                if (vxInput.ScrollWheelDelta > 0)
                    CursorScale += 0.25f + CursorScale / 10;
                else if (vxInput.ScrollWheelDelta < 0)
                    CursorScale -= 0.25f + CursorScale / 10;

                // Set and Clamp the Cursor Scale
                CursorScale = MathHelper.Clamp(CursorScale, 2, float.MaxValue);

                // Set the Base version of the Cursor Colour
                CursorColour = Color.DeepSkyBlue;

                // The Modify Vector Factor, simple factor to handle Additive or Subtractive Modeling
                int ModifyVectorFactor = 0;

                if (vxInput.MouseState.LeftButton == ButtonState.Pressed)
                {
                    ModifyVectorFactor = 1;
                }

                if (vxInput.MouseState.RightButton == ButtonState.Pressed)
                {
                    ModifyVectorFactor = -1;
                }


                // Handle Input
                if (ModifyVectorFactor != 0)
                {
                    BufferNeedsFullUpdate = false;

                    //vxConsole.WriteToInGameDebug("___________");
                    //vxConsole.WriteToInGameDebug(nameof(CursorPosition), CursorPosition);
                    //vxConsole.WriteToInGameDebug(nameof(FootPrint), FootPrint);
                    //vxConsole.WriteToInGameDebug(nameof(CellSize), CellSize);

                    switch (Scene.TerrainEditState)
                    {
                        // Handle Sculpt Mode
                        case vxEnumTerrainEditMode.Sculpt:

                            // Set Colour Based on Modify Vector
                            CursorColour = ModifyVectorFactor > 0 ? Color.DarkOrange : Color.Magenta;

                            // Gets the location of the Center Point in Relation to the Height Map
                            Point HeightMapRelativeLocation = GetHeightLocation(CursorPosition);

                            int Radius = (int)(CursorScale) / 2;

                            for (int x = HeightMapRelativeLocation.X - Radius + 2; x < HeightMapRelativeLocation.X + Radius; x++)
                            {
                                for (int y = HeightMapRelativeLocation.Y - Radius + 2; y < HeightMapRelativeLocation.Y + Radius; y++)
                                {
                                    Point point = new Point(x, y);

                                    if (IsInBounds(point))
                                    {
                                        // This is the Distance from the center
                                        float disFac = (CursorPosition - GetWorldLocation(point)).Length() / (Radius * CellSize);

                                        // This is a linearised value of the distance this point is from the "center" point.
                                        // Essentially controling the amount of change. As a basic instantiation, it does a linear factor first.
                                        float DistanceFactor = MathHelper.Clamp(1 - disFac, 0, 1);


                                        switch (Scene.FalloffRate)
                                        {
                                            case vxEnumFalloffRate.Flat:
                                                DistanceFactor = 1;
                                                break;
                                            case vxEnumFalloffRate.Linear:
                                                // Do nothing, this is the start case

                                                break;
                                            case vxEnumFalloffRate.Smooth:
                                                //Console.WriteLine(string.Format("{0} : {1} ", i, Math.Atan(j)));

                                                // the funtion y = (sin(x - 3.14159 / 2) + 1) / 2 gives a shape which 
                                                // is smooth from y = 0 to y = 1 for
                                                // x = 0 too x = 2 * pi.

                                                float xi = (float)Math.PI * DistanceFactor;
                                                float yi = (float)(Math.Sin(xi - Math.PI / 2) + 1) / 2;

                                                //DistanceFactor = (float)Math.Sin(DistanceFactor * (float)Math.PI / 2);
                                                //float x = 
                                                DistanceFactor = yi;
                                                break;
                                        }

                                        switch (Scene.AreaOfEffectMode)
                                        {
                                            case vxEnumAreaOfEffectMode.Delta:
                                                // Do nothing, this is the basic value.
                                                break;

                                            case vxEnumAreaOfEffectMode.Averaged:

                                                // First get the height distance between current location
                                                // and the center point.
                                                //float CenterHeight = TerrainMesh.Vertices[HeightMapRelativeLocation.X * (Dimension + 1) + HeightMapRelativeLocation.Y].Position.Y;
                                                //float ThisHeight = TerrainMesh.Vertices[x * (Dimension + 1) + y].Position.Y;
                                                float CenterHeight = TerrainMesh[HeightMapRelativeLocation.X, HeightMapRelativeLocation.Y];
                                                float ThisHeight = TerrainMesh[x, y];

                                                float HeightDelta = (CenterHeight - ThisHeight) * DistanceFactor / 10;

                                                DistanceFactor *= HeightDelta;

                                                break;
                                        }

                                        // Get the Factor for the New Height
                                        float NewHeight = DistanceFactor * ModifyVectorFactor;

                                        // Make sure it's within the bounds. Too high can cause GPU glitch outs.
                                        if (NewHeight <= MaxHeight && NewHeight >= Minheight)
                                            TerrainMesh[x, y] += NewHeight;


                                        BufferNeedsFullUpdate = true;
                                    }
                                }
                            }

                            if (BufferNeedsFullUpdate)
                                TerrainMesh.UpdateDynamicVertexBuffer();

                            break;


                        case vxEnumTerrainEditMode.TexturePaint:

                            // Blend the Previous Texture Map with the current brush
                            Engine.GraphicsDevice.SetRenderTarget(CanvasTextureMap);

                            //Engine.GraphicsDevice.Clear(Color.Transparent);

                            int Scale = (int)(CursorScale * (TextureMapSize / 128.0f)) / 2;

                            Point BrushLocation = new Point((int)(CursorPosition.X * (TextureMapSize / 128.0f) / CellSize) - Scale / 2, (int)(CursorPosition.Y * (TextureMapSize / 128.0f) / CellSize) - Scale / 2);

                            float factor = ((float)Scene.TexturePaintType) / 4 - 0.25f;

                            if (factor > 0.7f)
                                factor = 0.825f;
                            Console.WriteLine(factor);
                            Engine.SpriteBatch.Begin("Terrain - Texture Paint",0, BlendState.Opaque, SamplerState.PointClamp, null, null);
                            //Engine.SpriteBatch.Begin();
                            Engine.SpriteBatch.Draw(TextureMap, Vector2.Zero, Color.White);
                            //Engine.SpriteBatch.Draw(TextureBrush, new Rectangle(BrushLocation.X, BrushLocation.Y, Scale, Scale), Color.Black);
                            Engine.SpriteBatch.Draw(TextureBrush, new Rectangle(BrushLocation.X, BrushLocation.Y, Scale, Scale), Color.White * factor);
                            Engine.SpriteBatch.End();

                            // Now draw the Canvas Map to the Texture Map
                            Engine.GraphicsDevice.SetRenderTarget(TextureMap);
                            Engine.SpriteBatch.Begin("Terrain - Draw Canvas", 0, BlendState.Opaque, SamplerState.PointClamp, null, null);
                            Engine.SpriteBatch.Draw(CanvasTextureMap, Vector2.Zero, Color.White);
                            Engine.SpriteBatch.End();
                            Engine.GraphicsDevice.SetRenderTarget(null);

                            TextureWeightMap = TextureMap;
                            break;
                    }
                }
                else if (vxInput.IsNewMouseButtonRelease(MouseButtons.LeftButton) ||
                    vxInput.IsNewMouseButtonRelease(MouseButtons.RightButton))
                {
                    if (BufferNeedsFullUpdate)
                    {
                        //Current3DScene.BEPUPhyicsSpace.Remove(PhysicsMesh);
                        //Current3DScene.BEPUDebugDrawer.Remove(PhysicsMesh);

                        //UpdatePhysicsMesh();

                        // Recalcuate the normals
                        TerrainMesh.CalculateNormals();
                        TerrainMesh.UpdateDynamicVertexBuffer();
                    }
                }
            }

            // Code only during Non-Edit Mode.
            else
            {

            }
        }

        #region Utility Methods

        bool IsInBounds(Point point)
        {
            return (point.X >= 0 &&
                point.Y >= 0 &&
                point.X <= Dimension &&
                point.Y <= Dimension);
        }

        Point GetHeightLocation(Vector2 Pos)
        {
            Pos = Pos - Position.ToVector2();

            return new Point((int)(Pos.X / CellSize), (int)(Pos.Y / CellSize));
        }

        Vector2 GetWorldLocation(Point point)
        {
            return new Vector2(point.X * CellSize, point.Y * CellSize) + Position.ToVector2();
        }

        #endregion

        //bool DoWireFrame = true;


        //public override void RenderMesh(vxCamera3D Camera)
        //{

        //    if (SelectionState == vxSelectionState.None && IsInEditMode == false)
        //        DoWireFrame = false;
        //    else
        //        DoWireFrame = true;


        //    //if (SelectionState != vxSelectionState.None)
        //    //{
        //    //if (SelectionState == vxSelectionState.Hover)
        //    //    SelectionColour = Color.DeepSkyBlue;
        //    //else if (SelectionState == vxSelectionState.Selected)
        //    //SelectionColour = Color.DarkOrange;
        //    //Set the Selection Colour based off of Selection State
        //    switch (SelectionState)
        //    {
        //        case vxSelectionState.Selected:
        //            SelectionColour = Color.DarkOrange;
        //            EmissiveColour = Color.DarkOrange;
        //            break;
        //        case vxSelectionState.Hover:
        //            SelectionColour = Color.DeepSkyBlue;
        //            EmissiveColour = Color.DeepSkyBlue;
        //            break;
        //        case vxSelectionState.None:
        //            SelectionColour = Color.Black;
        //            EmissiveColour = Color.Black;
        //            break;
        //    }


        //    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

        //    if (IsEntityDrawable)
        //    {
        //        // Draw the model.
        //        foreach (vxModelMesh mesh in Model.Meshes)
        //        {
        //            mesh.OutlineEffect.CurrentTechnique = mesh.OutlineEffect.Techniques["Technique_Outline"];

        //            mesh.OutlineEffect.SelectionColour = Color.HotPink;// SelectionColour;
        //            mesh.OutlineEffect.LineThickness = Math.Abs((WorldTransform.Translation - Camera.Position).Length() * 2);
        //            mesh.Draw(mesh.OutlineEffect);

        //            //LineThickness = 0;
        //        }
        //    }

        //    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        //    //}
        //    //else
        //    //SelectionColour = Color.Transparent;






        //    Matrix world = Matrix.CreateTranslation(Position);

        //    TerrainMesh.Effect.Parameters["world"].SetValue(world);
        //    TerrainMesh.Effect.Parameters["wvp"].SetValue(world * Camera.ViewProjection);

        //    if (TerrainMesh.Effect.Parameters["CameraPos"] != null)
        //        TerrainMesh.Effect.Parameters["CameraPos"].SetValue(Camera.WorldMatrix.Translation);

        //    if (TerrainMesh.Effect.Parameters["SelectionColour"] != null)
        //        TerrainMesh.Effect.Parameters["SelectionColour"].SetValue(SelectionColour.ToVector4());

        //    //if (part.Effect.Parameters["SelectionColour"] != null)
        //    //    part.Effect.Parameters["SelectionColour"].SetValue(_selectionColour.ToVector4());

        //    TerrainMesh.Draw(TerrainMesh.Effect);






        //    // Draw a wireframe mesh over top if it's in 
        //    if (DoWireFrame)
        //        if (IsInEditMode || SelectionState != vxSelectionState.None)
        //        {
        //            //GraphicsDevice.DepthStencilState = DepthStencilState.None;
        //            GraphicsDevice.RasterizerState = Scene.rs_wire;
        //            Color WireColour = Color.Transparent;

        //            if (IsInEditMode)
        //                WireColour = (Scene.TerrainEditState == vxEnumTerrainEditMode.Sculpt) ? Color.DimGray : Color.HotPink;
        //            else
        //                WireColour = (SelectionState == vxSelectionState.None) ? Color.Transparent : EmissiveColour;

        //            RenderDebugWire(Camera, true, WireColour);

        //            //foreach (vxModelMesh mesh in Model.ModelMeshes)
        //            //{
        //            //    mesh.DebugEffect.CurrentTechnique = mesh.UtilityEffect.Techniques["Technique_Debug"];

        //            //    mesh.DebugEffect.World = World;
        //            //    mesh.DebugEffect.DoDebugWireFrame = DoWireFrame;
        //            //    mesh.DebugEffect.DoTexture = false;

        //            //    if (IsInEditMode)
        //            //        mesh.DebugEffect.WireColour = (CurrentSandboxLevel.TerrainEditState == vxEnumTerrainEditMode.Sculpt) ? Color.DimGray : Color.HotPink;
        //            //    else
        //            //        mesh.DebugEffect.WireColour = (SelectionState == vxSelectionState.None) ? Color.Transparent : EmissiveColour;

        //            //    mesh.Draw(mesh.DebugEffect);
        //            //}
        //            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        //            GraphicsDevice.RasterizerState = Scene.rs_solid;
        //        }

        //    //Engine.SpriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
        //    //Engine.SpriteBatch.Draw(TextureMap, new Rectangle(0, 48, 128, 128), Color.White);
        //    //Engine.SpriteBatch.End();
        //    SelectionState = vxSelectionState.None;
        //}
    }
}
