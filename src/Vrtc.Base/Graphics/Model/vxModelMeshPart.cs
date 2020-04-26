using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using VerticesEngine.Utilities;
using VerticesEngine.Graphics;

namespace VerticesEngine.Graphics
{
	/// <summary>
	/// The model mesh part which holds the Geometry data such as
	/// Vertices, Normal, UV Texture Coordinates, Binormal and Tangent data.
	/// </summary>
	public class vxModelMeshPart
	{
		/// <summary>
		/// The vertices for this Mesh. By default for the Vertices Engine, it includes Position, Normal, 
		/// UV Texture Coordinate, Tangent and BiNormal.
		/// </summary>
		public vxMeshVertex[] MeshVertices;

		/// <summary>
		/// The indices of this Mesh.
		/// </summary>
		public ushort[] Indices;

		/// <summary>
		/// The vertex buffer.
		/// </summary>
		public VertexBuffer VertexBuffer;

		/// <summary>
		/// The index buffer.
		/// </summary>
		public IndexBuffer IndexBuffer;


		/// <summary>
		/// Gets the number vertices.
		/// </summary>
		/// <value>The number vertices.</value>
		public int NumVertices
		{
			get
			{
				return MeshVertices.Count();
			}
		}

		/// <summary>
		/// Mesh Name
		/// </summary>
		public object Tag;


		/// <summary>
		/// Gets or sets the collection of Vertices.
		/// </summary>
		/// <value>The vertices.</value>
		public Vector3[] VerticesPoints;


		public int PrimitiveCount = 0;

		public int StartIndex = 0;

		public int VertexOffset = 0;


		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.Base.vxModelMeshPart"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		/// <param name="part">The part to extract the Position, Normal, UV, Tangent and BiNormal data from.</param>
		public vxModelMeshPart(string modelPath, ModelMeshPart part)
        {
            if (part != null)
			{
				// Now Parse out the Vertex Info from the model mesh part
				vxConsole.WriteVerboseLine("Stride is: " + part.VertexBuffer.VertexDeclaration.VertexStride);

				try
				{
					if (part.VertexBuffer.VertexDeclaration.VertexStride != 56)
					{
						Console.WriteLine("Error With Vertex Stride. Stride must be 56");
						Console.WriteLine("=======================================");
                        Console.WriteLine("File Path: " + modelPath);
						Console.WriteLine("Vertex Element Layout is:");
						foreach (VertexElement elmnt in part.VertexBuffer.VertexDeclaration.GetVertexElements())
							Console.WriteLine(elmnt.VertexElementUsage);

						Console.WriteLine("Please re-compile model with proper vertex elements.");

						throw new Exception("Vertex Stride Exception");
					}

					// Create the Mesh Vertice Array
					MeshVertices = new vxMeshVertex[part.VertexBuffer.VertexCount];

					// Now extract the data from the part's Vertex Buffer
					part.VertexBuffer.GetData<vxMeshVertex>(MeshVertices);

					// Account for the World Orientation in XNA/MG compared to Blender
					for (int vi = 0; vi < MeshVertices.Length; vi++)
					{
						var v = MeshVertices[vi];
						var matrix = Matrix.CreateRotationX(-MathHelper.PiOver2);
						MeshVertices[vi].Position = Vector3.Transform(v.Position, matrix);
						MeshVertices[vi].Normal = Vector3.TransformNormal(v.Normal, matrix);
						MeshVertices[vi].Tangent = Vector3.TransformNormal(v.Tangent, matrix);
						MeshVertices[vi].BiNormal = Vector3.TransformNormal(v.BiNormal, matrix);

					}

					// Create the Mesh Indices Array
					Indices = new ushort[part.IndexBuffer.IndexCount];

					// Now extract the data from the part's Indices Buffer
					part.IndexBuffer.GetData<ushort>(Indices);

					PrimitiveCount = part.PrimitiveCount;
                    part.Tag = modelPath;

					StartIndex = part.StartIndex;
					VertexOffset = part.VertexOffset;

					//Console.WriteLine("");
					//Console.WriteLine("Part: ");
					//Console.WriteLine("======================");
					//Console.WriteLine("StartIndex : "+StartIndex);
					//Console.WriteLine("VertexOffset : " + VertexOffset);


					Init();
				}
				catch (Exception ex)
				{
					// Sometimes there's errors, often time due to vertex stride is different than what's expected.
					vxConsole.WriteLine("ERROR: " + ex.Message + " >> Stride is: " + part.VertexBuffer.VertexDeclaration.VertexStride);
				}
			}
		}



        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxModelMeshPart"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="vertices">Vertices.</param>
        /// <param name="indices">Indices.</param>
        /// <param name="primitiveCount">Primitive count.</param>
		public vxModelMeshPart(vxMeshVertex[] vertices, ushort[] indices, int primitiveCount)
		{
			// Create the Mesh Vertice Array
			MeshVertices = new vxMeshVertex[vertices.Length];
			MeshVertices = vertices;
            
			// Create the Mesh Indices Array
			Indices = new ushort[indices.Length];
			Indices = indices;

			PrimitiveCount = primitiveCount;

			StartIndex = 0;
			VertexOffset = 0;


			Init();

		}

		public virtual void Init()
		{
			VertexBuffer = new VertexBuffer(vxGraphics.GraphicsDevice, typeof(vxMeshVertex), MeshVertices.Length, BufferUsage.None);
			IndexBuffer = new IndexBuffer(vxGraphics.GraphicsDevice, typeof(ushort), Indices.Length, BufferUsage.WriteOnly);
			SetData();
		}

		public virtual void SetData()
		{
			VertexBuffer.SetData<vxMeshVertex>(MeshVertices);
			IndexBuffer.SetData(Indices);
		}


		/// <summary>
		/// Draws this mesh with the given effect.
		/// </summary>
		/// <param name="drawEffect">Draw effect.</param>
		public virtual void Draw(Effect drawEffect)
		{
			if (VertexBuffer != null)
			{
                vxGraphics.GraphicsDevice.Indices = IndexBuffer;
                vxGraphics.GraphicsDevice.SetVertexBuffer(VertexBuffer);

				foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
				{
					pass.Apply();
                    vxGraphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, VertexOffset, StartIndex, PrimitiveCount);
				}
			}
		}
	}
}
