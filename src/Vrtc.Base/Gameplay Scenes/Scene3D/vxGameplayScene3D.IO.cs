
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using VerticesEngine.Graphics;
using VerticesEngine.Serilization;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.Utilities;

namespace VerticesEngine
{
    public partial class vxGameplayScene3D
    {
        public bool IsLoadingFile = false;

        public Type FileType;


        /// <summary>
        /// Initialises the Save File. If the XML Save file uses a different type, then
        /// this can be overridden.
        /// </summary>
        /// <returns></returns>
        public override vxSerializableSceneBaseData InitSaveFile()
        {
            return new vxSerializableScene3DData();
        }


        /// <summary>
        /// Returns a Deserializes the File. If you want to use a different type to Serialise than the base 'vxSerializableScene3DData'
        /// then you must override this or it will throw an error.
        /// </summary>
        public override vxSerializableSceneBaseData DeserializeFile(string path)
        {
            vxSerializableScene3DData file;
            XmlSerializer deserializer = new XmlSerializer(typeof(vxSerializableScene3DData));
            TextReader reader = new StreamReader(path);
            file = (vxSerializableScene3DData)deserializer.Deserialize(reader);
            reader.Close();

            return file;
        }

        public override vxGameplaySceneBase OnNewSandbox()
        {
            return new vxGameplayScene3D(vxStartGameMode.Editor);
        }



        public override vxSandboxFileLoadResult LoadFile(string FilePath, int version)
        {
            switch (version)
            {
                case 1:
                    LoadFileVersion1(FilePath);
                    break;
                //case 2:
                //LoadFileVersion2(FilePath);
                //break;
                default:
                    base.LoadFile(FilePath, version);
                    break;
            }

            return new vxSandboxFileLoadResult();
        }

        /// <summary>
        /// Loads the Current File.
        /// </summary>
        public void LoadFileVersion1(string FilePath)
        {
            int entitiesCount = 0;
            IsLoadingFile = true;
            //If it's not a new file, then open the specifeid file
            if (FilePath != "")
            {
                //Deserialize the input xml file
                string xmlFilePath = vxIO.PathToTempFolder + "/level.xml";
                base.SandBoxFile = DeserializeFile(xmlFilePath);

                // Set Sun Position
                SunEmitter.RotationX = SandBoxFile.Enviroment.SunRotations.X;
                SunEmitter.RotationZ = SandBoxFile.Enviroment.SunRotations.Y;

                // Set Fog
                //Renderer.IsFogEnabled = SandBoxFile.Enviroment.Fog.DoFog;
                //Renderer.FogNear = SandBoxFile.Enviroment.Fog.FogNear;
                //Renderer.FogFar = SandBoxFile.Enviroment.Fog.FogFar;
                //Renderer.FogColour = SandBoxFile.Enviroment.Fog.FogColour.Color;

                //This locks in the system to force the orientation
                SelectedIndex = -2;
                foreach (vxSerializableEntityState3D part in SandBoxFile.Entities)
                {
                    //Console.WriteLine("id: {0}, Type: {1}", part.id, part.Type);
                    TempPart = AddSandboxItem(part.Type, part.Orientation);
                    
                    if (TempPart != null)
                    {
                        entitiesCount++;
                        TempPart.UserDefinedData01 = part.UserDefinedData01;
                        TempPart.UserDefinedData02 = part.UserDefinedData02;
                        TempPart.UserDefinedData03 = part.UserDefinedData03;
                        TempPart.UserDefinedData04 = part.UserDefinedData04;
                        TempPart.UserDefinedData05 = part.UserDefinedData05;

                        TempPart.PostEntityLoad();
                    }
                }

                foreach (vxSerializableTerrainData terrainData in SandBoxFile.Terrains)
                {
                    //Console.WriteLine("id: {0}, Type: {1}", part.id, part.Type);
                    TempPart = AddSandboxItem(terrainData.Type, terrainData.Orientation);

                    if (TempPart != null)
                    {
                        var temp_terrain = (vxTerrainEntity)TempPart;
                        
                        temp_terrain.TerrainData = terrainData;

                        temp_terrain.PostEntityLoad();
                    }
                }

				// Now call Post Load Initalise on all entities
				foreach (var entity in Entities)
					entity.PostFileLoad();
            }
            IsLoadingFile = false;
            vxConsole.WriteIODebug(string.Format("     {0} Items Loaded from File.", entitiesCount));
            vxConsole.WriteIODebug(string.Format("     {0} Terrains Loaded from File.", TerrainManager.Terrains.Count));
                
        }


        public override vxSaveBusyScreen GetAsyncSaveScreen()
        {
            return new vxSaveBusyScreen3D(this);
        }

        /// <summary>
        /// Imports the file toolbar item clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        public virtual void ImportFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {

            var FileExplorerDialog = new vxFileExplorerDialog(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            vxSceneManager.AddScene(FileExplorerDialog, ControllingPlayer);
            FileExplorerDialog.Accepted += delegate {
               // Console.WriteLine(FileExplorerDialog.SelectedItem);


                var result = vxModel.Import(Engine, FileExplorerDialog.SelectedItem);

                if (result.ImportResultStatus == vxImportResultStatus.Success)
                {
                    var entity = new vxEntity3D(this, result.ImportedModel, Vector3.Zero);
                }
                else
                {
                    var msgbx = new vxMessageBox("Error Loading File", "Error");
                    vxSceneManager.AddScene(msgbx);
                }
            };
        }






        /// <summary>
        /// Exports the Current File too an STL file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void ExportFileToolbarItem_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            try
            {
                bool UsePhysicsMesh = true;
                //string path = vxIO.Path_Sandbox + "\\" + sandBoxFile.Name;
                string path = Path.Combine(vxIO.PathToSandbox, "Export");

                //First Check, if the Items Directory Doesn't Exist, Create It
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                Console.Write("Exporting File...");
                StreamWriter writer = new StreamWriter(path + FileName + "_export.stl");
                writer.WriteLine("solid Exported from Vertices Engine");
                foreach (vxEntity3D entity in Entities)
                {
                    Matrix correctionMatrix = entity.WorldTransform * Matrix.CreateRotationX(MathHelper.PiOver2);

                    if(entity.IsExportable == true)
                    if (UsePhysicsMesh == false)
                    {
                        foreach (ModelMesh mesh in entity.Model.ModelMain.Meshes)
                        {
                            foreach (ModelMeshPart meshpart in mesh.MeshParts)
                            {
                                //ExtractModelMeshPartData(meshpart, ref correctionMatrix, writer);
                                
                                //First Get the Position/Normal Texture Data
                                VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[meshpart.VertexBuffer.VertexCount];
                                meshpart.VertexBuffer.GetData(vertices);

                                ushort[] drawOrder = new ushort[meshpart.IndexBuffer.IndexCount];
                                meshpart.IndexBuffer.GetData(drawOrder);


                                    for (ushort i = 0; i < drawOrder.Length-3; i++)
                                    {
                                        /*
                                        Vector3 Pt1 = vxGeometryHelper.RotatePoint(correctionMatrix, vertices[drawOrder[i]].Position);
                                        Vector3 Pt2 = vxGeometryHelper.RotatePoint(correctionMatrix, vertices[drawOrder[i+1]].Position);
                                        Vector3 Pt3 = vxGeometryHelper.RotatePoint(correctionMatrix, vertices[drawOrder[i+2]].Position);
*/
                                        
                                        Vector3 Pt1 = Vector3.Transform(vertices[drawOrder[i]].Position, correctionMatrix);
                                        Vector3 Pt2 = Vector3.Transform(vertices[drawOrder[i+1]].Position, correctionMatrix);
                                        Vector3 Pt3 = Vector3.Transform(vertices[drawOrder[i+2]].Position, correctionMatrix);


                                        Vector3 Normal = vertices[drawOrder[i]].Normal;
                                        //Normal.Normalize();
                                        writer.WriteLine(string.Format("facet normal {0} {1} {2}", Normal.X, Normal.Y, Normal.Z));
                                        writer.WriteLine("outer loop");
                                        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt1.X, Pt1.Y, Pt1.Z));
                                        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt2.X, Pt2.Y, Pt2.Z));
                                        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt3.X, Pt3.Y, Pt3.Z));
                                        writer.WriteLine("endloop");
                                        writer.WriteLine("endfacet");
                                    }
                            }

                        }
                    }
                    else
                    {

                        //if (entity.MeshIndices != null)
                        //{
                        //    for (int i = 0; i < entity.MeshIndices.Length; i += 3)
                        //    {
                        //        Vector3 Pt1 = vxGeometryHelper.RotatePoint(correctionMatrix, entity.MeshVertices[entity.MeshIndices[i]]);
                        //        Vector3 Pt2 = vxGeometryHelper.RotatePoint(correctionMatrix, entity.MeshVertices[entity.MeshIndices[i + 1]]);
                        //        Vector3 Pt3 = vxGeometryHelper.RotatePoint(correctionMatrix, entity.MeshVertices[entity.MeshIndices[i + 2]]);

                        //        Vector3 Normal = Vector3.Cross(Pt2, Pt1);
                        //        Normal.Normalize();
                        //        writer.WriteLine(string.Format("facet normal {0} {1} {2}", Normal.X, Normal.Y, Normal.Z));
                        //        writer.WriteLine("outer loop");
                        //        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt1.X, Pt1.Y, Pt1.Z));
                        //        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt2.X, Pt2.Y, Pt2.Z));
                        //        writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt3.X, Pt3.Y, Pt3.Z));
                        //        writer.WriteLine("endloop");
                        //        writer.WriteLine("endfacet");

                        //    }
                        //}
                    }
                }
                writer.WriteLine("endsolid");
                writer.Close();
                Console.WriteLine("Done!");
            }
            catch(Exception ex)
            {
                vxConsole.WriteException(this,ex);
            }
        }


        /// <summary>  
        /// Get all the triangles from each mesh part (Changed for XNA 4)  
        /// </summary>  
        public void ExtractModelMeshPartData(ModelMeshPart meshPart, ref Matrix transform, StreamWriter writer )
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            // Before we add any more where are we starting from  
            int offset = 0;

            // == Vertices (Changed for XNA 4.0)  

            // Read the format of the vertex buffer  
            VertexDeclaration declaration = meshPart.VertexBuffer.VertexDeclaration;
            VertexElement[] vertexElements = declaration.GetVertexElements();
            // Find the element that holds the position  
            VertexElement vertexPosition = new VertexElement();
            foreach (VertexElement vert in vertexElements)
            {
                if (vert.VertexElementUsage == VertexElementUsage.Position &&
                    vert.VertexElementFormat == VertexElementFormat.Vector3)
                {
                    vertexPosition = vert;
                    // There should only be one  
                    break;
                }
            }
            // Check the position element found is valid  
            if (vertexPosition == null ||
                vertexPosition.VertexElementUsage != VertexElementUsage.Position ||
                vertexPosition.VertexElementFormat != VertexElementFormat.Vector3)
            {
                throw new Exception("Model uses unsupported vertex format!");
            }
            // This where we store the vertices until transformed  
            Vector3[] allVertex = new Vector3[meshPart.NumVertices];
            // Read the vertices from the buffer in to the array  
            meshPart.VertexBuffer.GetData<Vector3>(
                meshPart.VertexOffset * declaration.VertexStride + vertexPosition.Offset,
                allVertex,
                0,
                meshPart.NumVertices,
                declaration.VertexStride);
            // Transform them based on the relative bone location and the world if provided  
            for (int i = 0; i != allVertex.Length; ++i)
            {
                Vector3.Transform(ref allVertex[i], ref transform, out allVertex[i]);
            }
            // Store the transformed vertices with those from all the other meshes in this model  
            vertices.AddRange(allVertex);

            // == Indices (Changed for XNA 4)  

            // Find out which vertices make up which triangles  
            if (meshPart.IndexBuffer.IndexElementSize != IndexElementSize.SixteenBits)
            {
                // This could probably be handled by using int in place of short but is unnecessary  
                throw new Exception("Model uses 32-bit indices, which are not supported.");
            }
            // Each primitive is a triangle  
            short[] indexElements = new short[meshPart.PrimitiveCount * 3];
            meshPart.IndexBuffer.GetData<short>(
                meshPart.StartIndex * 2,
                indexElements,
                0,
                meshPart.PrimitiveCount * 3);
            // Each TriangleVertexIndices holds the three indexes to each vertex that makes up a triangle  
            //TriangleVertexIndices[] tvi = new TriangleVertexIndices[meshPart.PrimitiveCount];
            for (int i = 0; i != meshPart.PrimitiveCount; ++i)
            {
                // The offset is because we are storing them all in the one array and the   
                // vertices were added to the end of the array.  
                indices.Add(indexElements[i * 3 + 0] + offset);
                //tvi[i].B = indexElements[i * 3 + 1] + offset;
                //tvi[i].C = indexElements[i * 3 + 2] + offset;
            }
            // Store our triangles  
            //indices.AddRange(tvi);

            for (int i = 0; i < indices.Count-3; i += 3)
            {
                Matrix correctionMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
                Vector3 Pt1 = vertices[i];
                Vector3 Pt2 = vertices[i + 1];
                Vector3 Pt3 = vertices[i + 2];

                Vector3 Normal = Vector3.Cross(Pt2, Pt1);
                Normal.Normalize();
                //Normal.Normalize();
                writer.WriteLine(string.Format("facet normal {0} {1} {2}", Normal.X, Normal.Y, Normal.Z));
                writer.WriteLine("outer loop");
                writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt1.X, Pt1.Y, Pt1.Z));
                writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt2.X, Pt2.Y, Pt2.Z));
                writer.WriteLine(string.Format("vertex {0} {1} {2}", Pt3.X, Pt3.Y, Pt3.Z));
                writer.WriteLine("endloop");
                writer.WriteLine("endfacet");
            }
        }
    }
}