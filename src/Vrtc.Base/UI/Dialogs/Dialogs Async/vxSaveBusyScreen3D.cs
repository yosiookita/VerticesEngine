using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VerticesEngine.Util;
//using VerticesEngine;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.Serilization;
using VerticesEngine.Utilities;

namespace VerticesEngine
{
    
    /// <summary>
    /// The save busy screen for 3D Scene.
    /// </summary>
    public class vxSaveBusyScreen3D : vxSaveBusyScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.vxSaveBusyScreen"/> class.
        /// </summary>
        public vxSaveBusyScreen3D(vxGameplayScene3D Scene)
            : base(Scene)
        {

        }


        //float percent = 0;

        public override void OnAsyncWriteSaveFile(object sender, DoWorkEventArgs e)
        {
            vxGameplayScene3D CurrentScene = (vxGameplayScene3D)e.Argument;

            // Now Proceed with Saving
            string path = vxIO.PathToSandbox;

            //First Check, if the Items Directory Doesn't Exist, Create It
            string ExtractionPath = vxIO.PathToTempFolder;

            if (CurrentScene.IsDumping)
                ExtractionPath = path + "/dump";


            //First Check, if the Items Directory Doesn't Exist, Create It
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);

            if (Directory.Exists(ExtractionPath) == false)
                Directory.CreateDirectory(ExtractionPath);

            var SandBoxFile3D = CurrentScene.SandBoxFile;
            SandBoxFile3D.Clear();

            float per = 0;
            float tot = CurrentScene.Entities.Count;
            foreach (var entity in CurrentScene.Entities)
            {
                vxEntity3D part = (vxEntity3D)entity;
                // Prepare the entity for saving
                part.PreSave();

                //Don't Save Construction Geometry or items specifeid not to be saved
                Type partType = part.GetType();
                if (part.IsSaveable == true)
                {
                    if (partType == typeof(vxTerrainEntity))
                    {
                        vxTerrainEntity terrain = (vxTerrainEntity)part;

                        SandBoxFile3D.Terrains.Add(terrain.TerrainData);
                    }
                    else
                    {
                        SandBoxFile3D.Entities.Add(new vxSerializableEntityState3D(part.HandleID,
                            part.ToString(),
                            part.WorldTransform,
                            part.UserDefinedData01,
                            part.UserDefinedData02,
                            part.UserDefinedData03,
                            part.UserDefinedData04,
                            part.UserDefinedData05));
                    }
                }

                System.Threading.Thread.Sleep(3);
                per++;
                SaveFileAsyncWriter.ReportProgress((int)(per * 70 / tot));
            }



            // Increment File Type
            SandBoxFile3D.FileReversion++;

            // Set Sun Position
            SandBoxFile3D.Enviroment.SunRotations = new Vector2(CurrentScene.SunEmitter.RotationX, CurrentScene.SunEmitter.RotationZ);


            // Set Fog
            //SandBoxFile3D.Enviroment.Fog.DoFog = CurrentScene.Renderer.IsFogEnabled;
            //SandBoxFile3D.Enviroment.Fog.FogNear = CurrentScene.Renderer.FogNear;
            //SandBoxFile3D.Enviroment.Fog.FogFar = CurrentScene.Renderer.FogFar;
            //SandBoxFile3D.Enviroment.Fog.FogColour.Color = CurrentScene.Renderer.FogColour;
            //SaveFileAsyncWriter.ReportProgress(80);
            //Write The Sandbox File
            XmlSerializer serializer = new XmlSerializer(SandBoxFile3D.GetType());
            using (TextWriter writer = new StreamWriter(ExtractionPath + "/level.xml"))
            {
                serializer.Serialize(writer, SandBoxFile3D);
            }
            SaveFileAsyncWriter.ReportProgress(90);


            //Lastly, save the file info xml file.
            var fileInfo = CurrentScene.GetFileInfo();
            fileInfo.Save(ExtractionPath);


            // Get Compressed File Name
            if (CurrentScene.IsDumping == false)
            {
                string compFile = path + "/" + CurrentScene.FileName + ".sbx";

                if (File.Exists(compFile))
                    File.Delete(compFile);
                
                vxIO.CompressDirectory(ExtractionPath, compFile, null);
            }
            CurrentScene.IsDumping = false;
            SaveFileAsyncWriter.ReportProgress(100);
            e.Result = true;
        }
    }
}
