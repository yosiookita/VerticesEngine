using System;
using System.IO;
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
	public class vxTerrainMeshPart : vxModelMeshPart
	{

        public vxMeshVertex[] Vertices = new vxMeshVertex[4];

        public new short[] Indices = new short[6];

        public int Dimension
        {
            get { return dimension; }
            set
            {
                dimension = value;
				//Effect.Parameters["textureSize"].SetValue((float)value);
				//Effect.Parameters["texelSize"].SetValue(1 / (float)value);
            }
        }
        int dimension = 128;

       
        public float CellSize
        {
            get { return _cellSize; }
            set
            {
                _cellSize = value;
                //Effect.Parameters["CellSize"].SetValue(value);
            }
        }
        private float _cellSize = 10;

        public Texture2D DisplacementMap
        {
            get { return _displacementMap; }
            set
            {
                _displacementMap = value;
               // Effect.Parameters["displacementMap"].SetValue(value.ToVector4(Engine.GraphicsDevice));
            }
        }
        Texture2D _displacementMap;

		/// <summary>
		/// Gets or sets the max height of the terrain.
		/// </summary>
		/// <value>The height of the max.</value>
		public float MaxHeight
		{
			get { return _maxHeight; }
			set
			{
				_maxHeight = value;
				//Effect.Parameters["maxHeight"].SetValue(_maxHeight);
			}
		}
		float _maxHeight = 92;

        public float[,] HeightData;

        public Vector3 Position;

        /// <summary>
        /// Indexer to Access Height Data Array
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public float this[int i, int j]
        {
            get { return HeightData[i,j]; }
            set {

                // The setter has to update a few different Values.

                // First Update the Main Height Data Array.
                HeightData[i, j] = value;

                // Get the previous position for the X and Y Coordinates
                Vector3 PreviousVector = Vertices[i * (Dimension + 1) + j].Position;

                // Now Set the Vertices Position
                Vertices[i * (Dimension + 1) + j].Position = new Vector3(PreviousVector.X, value, PreviousVector.Z);
                
                VerticesPoints[i * (Dimension + 1) + j] = new Vector3(PreviousVector.X, value, PreviousVector.Z);
            }
        }

        /// <summary>
        /// Returns the Position of a Vertices given a Grid Coordinate of X and Y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 GetPositionAt(int x, int y)
        {
            return Vertices[x * (Dimension + 1) + y].Position;
        }



        /// <summary>
        /// Creates a Terrain mesh Part
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="HeightMap"></param>
        /// <param name="Position"></param>
        /// <param name="CellSize"></param>
        public vxTerrainMeshPart(Texture2D HeightMap, int CellSize) :this(HeightMap.ToHeightMapDataArray(), CellSize)
        {

        }

        public vxTerrainMeshPart(float[,] HeightData, int CellSize) : base("", null)
        {
            Dimension = HeightData.GetLength(0)-1;

            this.HeightData = HeightData;

            //this.Position = Position;

            this.CellSize = CellSize;

            // Generate the Vertices Grid
            GenerateVertices();

            // Load the Data from the Height Data Array
            LoadFromHeightArray();

            CalculateNormals();

            // Initialise the Dynamic Buffers
            Init();
        }

		public override void Init()
		{
            VertexBuffer = new DynamicVertexBuffer(vxGraphics.GraphicsDevice, typeof(vxMeshVertex), Vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData<vxMeshVertex>(Vertices);

            IndexBuffer = new DynamicIndexBuffer(vxGraphics.GraphicsDevice, typeof(short), Indices.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData(Indices);
		}


        /// <summary>
        /// Generates a Grid with the Dimension
        /// </summary>
        public void GenerateVertices()
        {

            Vertices = new vxMeshVertex[(dimension + 1) * (dimension + 1)];

            VerticesPoints = new Vector3[Vertices.Length];

            Indices = new short[dimension * dimension * 6];


            for (int i = 0; i < dimension + 1; i++)
            {
                for (int j = 0; j < dimension + 1; j++)
                {
                    vxMeshVertex vert = new vxMeshVertex();

                    // Set the Position based on the cell size
                    vert.Position = Position + new Vector3(i * _cellSize, 0, j * _cellSize);

                    // always have the normal up
                    vert.Normal = Vector3.Up;

                    // set the UV coordinates
                    vert.TextureCoordinate = new Vector2((float)i / dimension, (float)j / dimension);

                    vert.Tangent = Vector3.Right;
                    vert.BiNormal = Vector3.Cross(vert.Normal, vert.Tangent);

                    Vertices[i * (dimension + 1) + j] = vert;
                    VerticesPoints[i * (dimension + 1) + j] = vert.Position;
                }
            }

            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
					Indices[6 * (i * dimension + j) + 2] = (short)(i * (dimension + 1) + j);
                    Indices[6 * (i * dimension + j) + 1] = (short)(i * (dimension + 1) + j + 1);
                    Indices[6 * (i * dimension + j)] = (short)((i + 1) * (dimension + 1) + j + 1);

                    Indices[6 * (i * dimension + j) + 5] = (short)(i * (dimension + 1) + j);
                    Indices[6 * (i * dimension + j) + 4] = (short)((i + 1) * (dimension + 1) + j + 1);
                    Indices[6 * (i * dimension + j) + 3] = (short)((i + 1) * (dimension + 1) + j);
                }

            }
        }

        void LoadFromHeightArray()
        {
            for (int i = 0; i < dimension+1; i++)
            {
                for (int j = 0; j < dimension+1; j++)
                {
                    vxMeshVertex vert = Vertices[i * (dimension + 1) + j];
                    
                    Vertices[i * (dimension + 1) + j].Position = new Vector3(vert.Position.X, vert.Position.Y + HeightData[i, j], vert.Position.Z);
                    VerticesPoints[i * (dimension + 1) + j] = Vertices[i * (dimension + 1) + j].Position;
                }
            }
        }

        public void CalculateNormals()
        {
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    int i1 = (i * (dimension + 1) + j);
                    int i2 = (i * (dimension + 1) + j + 1);
                    int i3 = ((i + 1) * (dimension + 1) + j + 1);


                    Vector3 v1 = Vertices[i1].Position;
                    Vector3 v2 = Vertices[i2].Position;
                    Vector3 v3 = Vertices[i3].Position;


                    // The Normal is the Cross Product of v2-v1 and v3-v1
                    Vector3 d1 = v1 - v3;
                    Vector3 d2 = v1 - v2;

                    Vector3 normal = Vector3.Cross(d2, d1);

                    normal.Normalize();


                    Vertices[i1].Normal = normal;
                    Vertices[i2].Normal = normal;
                    Vertices[i3].Normal = normal;

                }
            }
        }


        public virtual void UpdateDynamicVertexBuffer()
        {
            VertexBuffer.SetData<vxMeshVertex>(Vertices);
        }


        public virtual void UpdateDynamicIndexBuffer()
        {
            IndexBuffer.SetData(Indices);
        }

        


		/// <summary>
		/// Draws this mesh with the given effect.
		/// </summary>
		/// <param name="drawEffect">Draw effect.</param>
		public override void Draw(Effect drawEffect)
		{
			if (VertexBuffer != null)
			{
                vxGraphics.GraphicsDevice.Indices = IndexBuffer;
                vxGraphics.GraphicsDevice.SetVertexBuffer(VertexBuffer);

                foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
				{
					pass.Apply();
                    vxGraphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, (dimension + 1) * (dimension + 1), 0, 2 * dimension * dimension);
                }
			}
		}
	}
}
