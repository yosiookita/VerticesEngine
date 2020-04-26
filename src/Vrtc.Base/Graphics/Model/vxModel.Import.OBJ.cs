using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;


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
        static vxImportResult ImportOBJ(vxEngine Engine, string path)
        {
            vxImportResult ImportResult = new vxImportResult();

            // Read in the file.
            StreamReader reader = new StreamReader(path);

            // reaad in all text
            string fileText = reader.ReadToEnd();

            // split the line into lines
            string[] lines = System.Text.RegularExpressions.Regex.Split(fileText, @"\r?\n|\r");

            // Variable holding the Normal for this current 'facet' group
            Vector3 currentNormal = Vector3.Zero;

            // Model to return
            vxModel model = new vxModel( path);
            model.ModelMain = vxInternalAssets.Models.UnitBox.ModelMain;

            // initial mesh
            vxModelMesh mesh = new vxModelMesh();

            List<Vector3> VerticesPoints = new List<Vector3>();
            List<Vector3> Normals = new List<Vector3>();
            List<Vector2> UVs = new List<Vector2>();

            List<vxMeshVertex> Vertices = new List<vxMeshVertex>();
            List<ushort> Indices = new List<ushort>();
            vxModelMeshPart part;

            // Now loop through the 'lines'
            foreach (string line in lines)
            {
                string[] tokens = line.Split(new char[0]);

                if (tokens.Length > 0)
                {
                    switch(tokens[0])
                    {
                        // create new mesh object
                        case "o":

                            if(Vertices.Count > 0)
                            {
                                part = new vxModelMeshPart( Vertices.ToArray(), Indices.ToArray(), Indices.Count / 3);
                                mesh.MeshParts.Add(part);
                            }

                            mesh = new vxModelMesh();
                            mesh.Name = tokens[1];
                            model.AddModelMesh(mesh);


                            Vertices = new List<vxMeshVertex>();
                            Indices = new List<ushort>();

                            // Clear All
                            VerticesPoints.Clear();
                            Normals.Clear();
                            UVs.Clear();
                            break;
                        case "v":
                            VerticesPoints.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                            break;
                        case "vn":
                            Normals.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                            break;
                        case "vt":
                            UVs.Add(new Vector2(float.Parse(tokens[1]), 1-float.Parse(tokens[2])));
                            break;
                        case "f":

                            // split token by bracket
                            for (int i = 1; i < 4; i++)
                            {
                                string[] indices = tokens[4-i].Split('/');

                                var tempVert = new vxMeshVertex();

                                var v = int.Parse(indices[0])-1;
                                var vt = int.Parse(indices[1])-1;
                                var vn = int.Parse(indices[2])-1;

                                tempVert.Position = VerticesPoints[v];
                                tempVert.Normal = Normals[vn];
                                tempVert.BiNormal = Normals[vn];
                                tempVert.Tangent = Normals[vn];
                                tempVert.TextureCoordinate = UVs[vt];

                                Vertices.Add(tempVert);
                                Indices.Add((ushort)Indices.Count());
                            }

                            break;

                    }
                }
            }

            part = new vxModelMeshPart(Vertices.ToArray(), Indices.ToArray(),Indices.Count/3);
            mesh.MeshParts.Add(part);

            model.LoadTextures(Engine.Game.Content, "",true);

            ImportResult = new vxImportResult(model);

            return ImportResult;
        }
    }
}

