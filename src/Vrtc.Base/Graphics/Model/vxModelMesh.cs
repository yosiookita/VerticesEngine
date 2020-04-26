using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using VerticesEngine.Graphics;

namespace VerticesEngine.Graphics
{
	/// <summary>
	/// The model mesh.
	/// </summary>
    public class vxModelMesh : vxGameObject
	{
		/// <summary>
		/// Mesh Name
		/// </summary>
		public string Name;

		/// <summary>
		/// The model mesh parts.
		/// </summary>
		public List<vxModelMeshPart> MeshParts;

        /// <summary>
        /// The material for this mesh. Note: a mesh may only have one material.
        /// </summary>
        public vxMaterial Material;

        /// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.Base.vxModelMesh"/> class with the Main Shader.
        /// </summary>
        public vxModelMesh(): this(new vxMaterial(new vxShader(vxInternalAssets.Shaders.MainShader)))
		{
			
		}

        /// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.Base.vxModelMesh"/> class.
        /// </summary>
        /// <param name="Engine">Engine Reference</param>
        /// <param name="material">The material to use with this mesh, if you want a seperate instance of the material then clone it in the constructor</param>
		public vxModelMesh(vxMaterial material):base()
		{
			MeshParts = new List<vxModelMeshPart>();

            Material = material;
        }


		public virtual void Draw()
		{
            Material.SetPass();

			for (int mp =0; mp < MeshParts.Count; mp++)
                MeshParts[mp].Draw(Material.Shader);
		}


		public virtual void Draw(Effect drawEffect)
        {
            for (int mp = 0; mp < MeshParts.Count; mp++)
                MeshParts[mp].Draw(drawEffect);
		}
	}
}
