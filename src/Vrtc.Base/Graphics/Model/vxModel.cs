using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;


namespace VerticesEngine.Graphics
{
    /// <summary>
    /// A Model Class which loads and processes all data at runtime. Although this add's to load times,
    /// it allows for more control as well as modding for any and all models which are used in the game.
    /// Using three different models to handle different types of rendering does add too over all installation
    /// size, it is necessary to allow the shaders to be compiled for cross platform use.
    /// </summary>
    public partial class vxModel : vxGameObject
    {
        /// <summary>
        /// The Models Name (Most often just the File Name).
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The model path.
        /// </summary>
        public readonly string ModelPath;

        /// <summary>
        /// The Primitive Count for this Entity Model. It is the summation of all Meshes and Parts.
        /// This can be used when debuging how many primitives are being drawn per draw call.
        /// </summary>
        public int TotalPrimitiveCount = 0;

        /// <summary>
        /// This is the Main Model which is drawn to the screen
        /// using which ever main Shader is in the model.
        /// </summary>
        public Model ModelMain;

        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
        }
        BoundingBox _boundingBox;

        /// <summary>
        /// Sets the Distortion map for the the Utility Effect
        /// </summary>
        public Texture2D DistortionMap
        {
            get { return _distortionMap; }
            set
            {
                _distortionMap = value;

                foreach (var mesh in Meshes)
                    mesh.Material.UtilityEffect.DistortionMap = _distortionMap;
            }
        }
        Texture2D _distortionMap;

        //#endregion

        #region Null Texture References

        public Texture2D NullDiffuseTexture
        {
            get
            {
                return vxInternalAssets.Textures.DefaultDiffuse;
            }
        }

        public Texture2D NullNormalMap
        {
            get
            {
                return vxInternalAssets.Textures.DefaultNormalMap;
            }
        }

        public Texture2D NullSpecularMap
        {
            get
            {
                return vxInternalAssets.Textures.DefaultSurfaceMap;
            }
        }

        //public Texture2D NullReflectionMap
        //{
        //    get
        //    {
        //        if (Engine.InternalAssets != null)
        //            return vxInternalAssets.Textures.DefaultReflectionMap;
        //        else
        //            return Engine.InternalContentManager.Load<Texture2D>("Textures/nullTextures/null_reflections");
        //    }
        //}

        #endregion


        /// <summary>
        /// Basic Constructor. Note: All Items must be instantiated outside of this function.
        /// </summary>
        public vxModel(string ModelPath="") : base()
        {
            this.ModelPath = ModelPath;

            if (this.ModelPath != "")
                this.Name = Path.GetFileName(ModelPath);
            else
                this.Name = "null";


            this.Engine.ContentManager.LoadedModels.Add(this);
        }

        /// <summary>
        /// The model meshes.
        /// </summary>
        public List<vxModelMesh> Meshes = new List<vxModelMesh>();

        public void AddModelMesh(vxModelMesh mesh)
        {
            Meshes.Add(mesh);
            //this.TreeNode.Add(mesh.TreeNode);
        }


        /// <summary>
        /// The relative texture path.
        /// </summary>
        public string TexturePath = "";


        public void UpdateBoundingBox()
        {
            //_boundingBox = vxGeometryHelper.GetModelBoundingBox(ModelMain, Matrix.CreateRotationX(-MathHelper.PiOver2));
            _boundingBox = GetModelBoundingBox(this, Matrix.Identity);
        }

        Texture2D LoadTexture(string path, ContentManager Content, bool IsBeingImported)
        {
            if (IsBeingImported)
            {
#if __ANDROID__
                Stream fileStream = vxGame.Activity.Assets.Open("Content/" + path + ".png");
                return Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
#else
                using (var fileStream = new FileStream(path + ".png", FileMode.Open))
                {
                    return Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
                }
#endif
            }
            else
            {
                return Content.Load<Texture2D>(path);
            }
        }

        /// <summary>
        /// Loads the textures.
        /// </summary>
        /// <param name="TexturePath">Texture path.</param>
        public void LoadTextures(string TexturePath = "")
        {
            LoadTextures(Engine.Game.Content, TexturePath);
        }

        /// <summary>
        /// Loads the textures.
        /// </summary>
        /// <param name="Content">Content.</param>
        /// <param name="TexturePath">Texture path.</param>
        public void LoadTextures(ContentManager Content, string TexturePath = "", bool IsBeingImported=false)
        {
            // Set the Bounding Box
            UpdateBoundingBox();

            string rootDirectory = IsBeingImported ? "" : Content.RootDirectory + "/";
            string ext = IsBeingImported ? ".png" : ".xnb";
            if (TexturePath != "")
                TexturePath += "/";

            this.TexturePath = TexturePath;

            //foreach (vxModelMesh m in Meshes)
            //{
            //    if (m.MeshParts[0].Effect == null)
            //        m.MeshParts[0].Effect = m.MainEffect;
            //}
            //Load the Textures for hte Model Main.
            //foreach (ModelMesh mesh in ModelMain.Meshes)
            foreach (var mesh in Meshes)
            {
                Texture2D _diffusetexture;
                Texture2D _normalMap;
                Texture2D _surfaceMap;

                //Console.WriteLine(mesh.Name);
                //First Create The Path to the Textures and Maps
                string pathToDiffTexture = ModelPath.GetParentPathFromFilePath() + "/" + TexturePath + mesh.Name + "_dds";
                string pathToNrmMpTexture = ModelPath.GetParentPathFromFilePath() + "/" + TexturePath + mesh.Name + "_nm";
                string pathToSpecMpTexture = ModelPath.GetParentPathFromFilePath() + "/" + TexturePath + mesh.Name + "_sm";
                string pathToDistMpTexture = ModelPath.GetParentPathFromFilePath() + "/" + TexturePath + mesh.Name + "_dm";


                // Load/Create Diffuse Texture 
                //**************************************************************************************************************


                //First try to find the corresponding diffuse texture for this mesh, 
                //if it isn't found, then set the null texture as a fall back
                if (File.Exists(rootDirectory + pathToDiffTexture + ext))
                    _diffusetexture = LoadTexture(pathToDiffTexture, Content, IsBeingImported);
                else
                    _diffusetexture = NullDiffuseTexture;


                foreach (vxModelMesh m in Meshes)
                    if (m.Name == mesh.Name)
                    {
                        m.Material.Texture = _diffusetexture;
                        //if (m.MainEffect.Parameters["Texture"] != null)
                        //{
                        //    m.MainEffect.Parameters["Texture"].SetValue(_diffusetexture);
                        //    m.MeshParts[0].Effect.Parameters["Texture"].SetValue(_diffusetexture);
                        //}
                    }

                //foreach (var m in Meshes)
                //    if (m.Name == mesh.Name)
                //    {
                //        m.UtilityEffect.DiffuseTexture = _diffusetexture;
                //        m.DebugEffect.DiffuseTexture = _diffusetexture;

                //    }


                // Load/Create Normal Map 
                //**************************************************************************************************************
                bool HasNormalMap = false;
                //First try to find the corresponding normal map texture for this mesh, 
                //if it isn't found, then set the null texture as a fall back
                if (File.Exists(rootDirectory + pathToNrmMpTexture + ext))
                {
                    _normalMap = LoadTexture(pathToNrmMpTexture, Content, IsBeingImported);
                    HasNormalMap = true;
                }
                else
                {
                    _normalMap = NullNormalMap;
                }

                foreach (vxModelMesh m in Meshes)
                    if (m.Name == mesh.Name)
                    {
                        m.Material.NormalMap = _normalMap;
                        m.Material.IsNormalMapEnabled = HasNormalMap;
                        //if (m.MainEffect.Parameters["NormalMap"] != null)
                        //{
                        //    m.MainEffect.Parameters["NormalMap"].SetValue(_normalMap);
                        //    m.MeshParts[0].Effect.Parameters["NormalMap"].SetValue(_normalMap);
                        //}
                        //if (m.MainEffect.Parameters["HasNormalMap"] != null)
                        //{
                        //    m.MainEffect.Parameters["HasNormalMap"].SetValue(HasNormalMap);
                        //    m.MeshParts[0].Effect.Parameters["HasNormalMap"].SetValue(HasNormalMap);
                        //}
                    }

                //foreach (var m in Meshes)
                //    if (m.Name == mesh.Name)
                //        m.UtilityEffect.NormalMap = _normalMap;

                // Load/Create Surface Map 
                //**************************************************************************************************************

                //First try to find the corresponding specular map texture for this mesh, 
                //if it isn't found, then set the null texture as a fall back
                if (File.Exists(rootDirectory + pathToSpecMpTexture + ext))
                    _surfaceMap = LoadTexture(pathToSpecMpTexture, Content, IsBeingImported);
                else
                    _surfaceMap = NullSpecularMap;


                foreach (var m in Meshes)
                    if (m.Name == mesh.Name)
                    {
                        m.Material.UtilityEffect.SurfaceMap = _surfaceMap;
                        //if (m.MainEffect.Parameters["SurfaceMap"] != null)
                        //{
                        //    m.MainEffect.Parameters["SurfaceMap"].SetValue(_surfaceMap);
                        //    m.MeshParts[0].Effect.Parameters["SurfaceMap"].SetValue(_surfaceMap);
                        //}
                    }
                //foreach (var m in Meshes)
                //    if (m.Name == mesh.Name)
                //        m.UtilityEffect.SurfaceMap = _surfaceMap;


                // Load/Create Distortion Map 
                //**************************************************************************************************************

                //First try to find the corresponding normal map texture for this mesh, 
                //if it isn't found, then set it to just null as a fall back
                if (File.Exists(rootDirectory + pathToDistMpTexture + ext))
                {
                    foreach (var m in Meshes)
                        if (m.Name == mesh.Name)
                        {
                        m.Material.UtilityEffect.DistortionMap = LoadTexture(pathToDistMpTexture, Content, IsBeingImported);
                            m.Material.IsDistortionEnabled = true;
                        }
                }


                // Now get the Primitive Count for Debuging Purposes.
                //TotalPrimitiveCount = vxGeometryHelper.GetModelPrimitiveCount(ModelMain);

                int primCount = 0;
                foreach (vxModelMesh m in Meshes)
                {
                    foreach (vxModelMeshPart part in mesh.MeshParts)
                    {
                        primCount += part.PrimitiveCount;
                    }
                }
                TotalPrimitiveCount = primCount;
            }
        }
        /*
        public void SetTexturePackLevel(vxEnumTextureQuality quality)
        {

            ContentManager Content = Engine.Game.Content;

            //Console.WriteLine("==============================================================================");
            //Console.WriteLine("Path: " + PathToModel);
            //Console.WriteLine("------------------------------------------------------------------------------");
            string basePath = ModelPath.GetParentPathFromFilePath() + "/" + TexturePath + quality + "/";
            //Load the Textures for hte Model Main.
            foreach (ModelMesh mesh in ModelMain.Meshes)

            {

                Texture2D _diffusetexture;
                Texture2D _normalMap;
                Texture2D _specularMap;
                //Texture2D _reflectionMap;

                //Console.WriteLine(mesh.Name);
                //First Create The Path to the Textures and Maps
                string pathToDiffTexture = basePath + mesh.Name + "_dds";
                string pathToNrmMpTexture = basePath + mesh.Name + "_nm";
                string pathToSpecMpTexture = basePath + mesh.Name + "_sm";
                string pathToReflMpTexture = basePath + mesh.Name + "_rm";
                string pathToDistMpTexture = basePath + mesh.Name + "_dm";



                // Load/Create Diffuse Texture 
                //**************************************************************************************************************

                //First try to find the corresponding diffuse texture for this mesh, 
                //if it isn't found, then set the null texture as a fall back
                if (File.Exists(Content.RootDirectory + "/" + pathToDiffTexture + ".xnb"))
                    _diffusetexture = Content.Load<Texture2D>(pathToDiffTexture);
                else
                    _diffusetexture = NullDiffuseTexture;

                foreach (var part in mesh.MeshParts)
                {
                    if (part.Effect.Parameters["Texture"] != null)
                        part.Effect.Parameters["Texture"].SetValue(_diffusetexture);
                }

                foreach (var m in Meshes)
                    if (m.Name == mesh.Name)
                    {
                        m.UtilityEffect.DiffuseTexture = _diffusetexture;
                        m.DebugEffect.DiffuseTexture = _diffusetexture;
                    }

                // Load/Create Normal Map 
                //**************************************************************************************************************

                //First try to find the corresponding normal map texture for this mesh, 
                //if it isn't found, then set the null texture as a fall back
                if (File.Exists(Content.RootDirectory + "/" + pathToNrmMpTexture + ".xnb"))
                    _normalMap = Content.Load<Texture2D>(pathToNrmMpTexture);
                else
                    _normalMap = NullNormalMap;

                foreach (var m in Meshes)
                    if (m.Name == mesh.Name)
                        m.UtilityEffect.NormalMap = _normalMap;

                // Load/Create Specular Map 
                //**************************************************************************************************************

                //First try to find the corresponding specular map texture for this mesh, 
                //if it isn't found, then set the null texture as a fall back
                if (File.Exists(Content.RootDirectory + "/" + pathToSpecMpTexture + ".xnb"))
                    _specularMap = Content.Load<Texture2D>(pathToSpecMpTexture);
                else
                    _specularMap = NullSpecularMap;

                foreach (var m in Meshes)
                    if (m.Name == mesh.Name)
                        m.UtilityEffect.SurfaceMap = _specularMap;

                // Load/Create Reflection Map 
                //**************************************************************************************************************

                //First try to find the corresponding normal map texture for this mesh, 
                //if it isn't found, then set it to just null as a fall back
                //if (File.Exists(Content.RootDirectory + "/" + pathToReflMpTexture + ".xnb"))
                //    _reflectionMap = Content.Load<Texture2D>(pathToReflMpTexture);
                //else
                //_reflectionMap = NullReflectionMap;

                //foreach (var m in ModelMeshes)
                //if (m.Name == mesh.Name)
                //m.UtilityEffect.ReflectionMap = _reflectionMap;

                // Load/Create Distortion Map 
                //**************************************************************************************************************

                //First try to find the corresponding normal map texture for this mesh, 
                //if it isn't found, then set it to just null as a fall back
                if (File.Exists(Content.RootDirectory + "/" + pathToDistMpTexture + ".xnb"))
                {
                    foreach (var m in Meshes)
                        if (m.Name == mesh.Name)
                            m.UtilityEffect.DistortionMap = Content.Load<Texture2D>(pathToDistMpTexture);
                }


                // Load/Create Distortion Map 
                //**************************************************************************************************************

                //First try to find the corresponding normal map texture for this mesh, 
                //if it isn't found, then set it to just null as a fall back
                if (File.Exists(Content.RootDirectory + "/" + pathToDistMpTexture + ".xnb"))
                {
                    foreach (var m in Meshes)
                        if (m.Name == mesh.Name)
                            m.UtilityEffect.DistortionMap = Content.Load<Texture2D>(pathToDistMpTexture);
                }

                // Now get the Primitive Count for Debuging Purposes.
                TotalPrimitiveCount = vxGeometryHelper.GetModelPrimitiveCount(ModelMain);
            }
        }
        */

        public static BoundingBox GetModelBoundingBox(vxModel model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (vxModelMesh mesh in model.Meshes)
            {
                foreach (vxModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    if (meshPart.VertexBuffer != null)
                    {
                        int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                        int vertexBufferSize = meshPart.NumVertices * vertexStride;

                        // Get vertex data as float
                        float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                        meshPart.VertexBuffer.GetData<float>(vertexData);

                        // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                        for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                        {
                            Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                            min = Vector3.Min(min, transformedPosition);
                            max = Vector3.Max(max, transformedPosition);
                        }
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }

        public static void GetVerticesAndIndicesFromModel(vxModel model, out Vector3[] vertices, out int[] indices)
        {
            var verticesList = new List<Vector3>();
            var indicesList = new List<int>();
            //var transforms = new Matrix[collisionModel.Bones.Count];
            //collisionModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    foreach (var v in part.MeshVertices)
                        verticesList.Add(v.Position * 100);

                    foreach (var ind in part.Indices)
                        indicesList.Add(ind);
                }
            }
            vertices = verticesList.ToArray();
            indices = indicesList.ToArray();
        }

    }
}

