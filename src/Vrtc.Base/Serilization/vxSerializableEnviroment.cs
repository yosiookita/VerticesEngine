using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using VerticesEngine;

namespace VerticesEngine.Serilization
{

    /// <summary>
    /// This holds the Serializable data of a Enviroment Effects (i.e. Time of Day, Fog, Water etc...)
    /// </summary>
    public class vxSerializableEnviroment
    {
        // Time of day for this scene
        [XmlAttribute("TimeOfDay")]
        public TimeOfDay TimeOfDay = TimeOfDay.Day;

        [XmlElement("SunRotation")]
        public Vector2 SunRotations = new Vector2(1,0);

        // Fog State
        //[XmlElement("Fog")]
        //public vxSerializableFogState Fog;


        public vxSerializableEnviroment()
        {
            //Fog = new vxSerializableFogState();
        }
    }
}
