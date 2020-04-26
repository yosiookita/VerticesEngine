using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using VerticesEngine.ContentManagement;
//Virtex vxEngine Declaration
using VerticesEngine.Graphics;

namespace VerticesEngine
{

    public class vxSkyBoxEntity : vxEntity3D
    {

        /// <summary>
        /// The effect file that the skybox will use to render
        /// </summary>
        public Effect SkyBoxEffect;

        /// <summary>
        /// Is the sky rotating
        /// </summary>
        public bool IsRotating = false;


        RasterizerState NewRS;
        RasterizerState OldRS;

        /// <summary>
        /// The size of the cube, used so that we can resize the box
        /// for different sized environments.
        /// </summary>
        private float size = 150f;

        float rotation = 0;

        Matrix rotationMatrix;

        /// <summary>
        /// Gets or sets the skybox texture cube.
        /// </summary>
        /// <value>The skybox texture cube.</value>
        public TextureCube SkyboxTextureCube
		{
			set
			{
				_skyboxTextureCube = value;
               // SetEffectParameter("SkyBoxTexture", value);
                if (Model != null)
                {
                    foreach (vxModelMesh mesh in Model.Meshes)
                        if (SkyBoxEffect.Parameters["SkyBoxTexture"] != null)
                            SkyBoxEffect.Parameters["SkyBoxTexture"].SetValue(_skyboxTextureCube);

                    if (Model.ModelMain != null)
                        foreach (var part in Model.ModelMain.Meshes.SelectMany(m => m.MeshParts))
                            if (part.Effect.Parameters["SkyBoxTexture"] != null)
                                part.Effect.Parameters["SkyBoxTexture"].SetValue(_skyboxTextureCube);
                }
            }
			get { return _skyboxTextureCube; }
		}
		TextureCube _skyboxTextureCube;

		/// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Entities.vxSkyBoxEntity"/> class.
        /// </summary>
        /// <param name="scene">Scene.</param>
        public vxSkyBoxEntity(vxGameplayScene3D scene):base(scene, Vector3.Zero)
        {
            IsEntityCullable = false;
            IsSaveable = false;
            IsExportable = false;

            foreach (var mesh in Model.Meshes)
                mesh.Material.IsDefferedRenderingEnabled = false;

            NewRS = new RasterizerState();
            NewRS.CullMode = CullMode.None;
        }


        public override vxModel OnLoadModel()
        {
            SkyBoxEffect = vxInternalAssets.LoadInternalEffect("Shaders/Skybox/SkyBoxShader");

            return vxContentManager.Instance.LoadModel("Shaders/Skybox/cube", vxContentManager.Instance, SkyBoxEffect);
        }


        public override void Draw(vxCamera Camera, string renderpass)
        {
            if (IsVisible && renderpass == vxRenderer.Passes.OpaquePass)
            {
                size = Camera.FarPlane * 0.55f;

                OldRS = Engine.GraphicsDevice.RasterizerState;
                Engine.GraphicsDevice.RasterizerState = NewRS;

                rotation += 0.1f;
                rotationMatrix = IsRotating ? Matrix.CreateRotationY(rotation) : Matrix.Identity;

                Vector3 Position = ((vxCamera3D)Camera).Position;

                // Go through each pass in the effect, but we know there is only one...
                foreach (EffectPass pass in SkyBoxEffect.CurrentTechnique.Passes)
                {
                    // Draw all of the components of the mesh, but we know the cube really
                    // only has one mesh
                    foreach (ModelMesh mesh in Model.ModelMain.Meshes)
                    {
                        // Assign the appropriate values to each of the parameters
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            part.Effect = SkyBoxEffect;

                            part.Effect.CurrentTechnique = SkyBoxEffect.Techniques["Skybox"];

                            part.Effect.Parameters["World"].SetValue(rotationMatrix *
                                Matrix.CreateScale(size) * Matrix.CreateTranslation(Position));

                            part.Effect.Parameters["View"].SetValue(Camera.View);
                            part.Effect.Parameters["Projection"].SetValue(Camera.Projection);
                            part.Effect.Parameters["CameraPosition"].SetValue(Position);
                        }

                        // Draw the mesh with the skybox effect
                        mesh.Draw();
                    }
                }
                Engine.GraphicsDevice.RasterizerState = OldRS;
            }
        }
    }
}
