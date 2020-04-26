using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;

//Virtex vxEngine Declaration
using VerticesEngine;
using Microsoft.Xna.Framework.Graphics;

namespace VerticesEngine.Serilization
{

    /// <summary>
    /// This holds the Serializable data for a vxScene3D including all Entities, Terrains, as well as
    /// Level and Enviroment Data.
    /// </summary>
    public class vxSerializableScene3DData : vxSerializableSceneBaseData
    {

        [XmlElement("enviroment")]
        public vxSerializableEnviroment Enviroment;


        [XmlElement("entity")]
        public List<vxSerializableEntityState3D> Entities;


        [XmlElement("terrain")]
        public List<vxSerializableTerrainData> Terrains;


        public vxSerializableScene3DData() : base()
        {
            Entities = new List<vxSerializableEntityState3D>();
            Terrains = new List<vxSerializableTerrainData>();
            Enviroment = new vxSerializableEnviroment();
        }

        // Clears the File to take a new file in.
        public override void Clear()
        {
            Entities.Clear();
            Terrains.Clear();
        }
    }
}
