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
    public class vxSerializableSceneBaseData
    {
        [XmlAttribute("filerev")]
        public int FileReversion = 1;


        // Level Title
        [XmlElement("title")]
        public string LevelTitle = "Enter A Title";

        // Level Description
        [XmlElement("desc")]
        public string LevelDescription="Enter A Description";


        public vxSerializableSceneBaseData()
        {
           
        }

        // Clears the File to take a new file in.
        public virtual void Clear()
        {
            
        }
    }
}
