using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Events;
using Microsoft.Xna.Framework.Audio;
using Virtex.Lib.Vrtc.GUI.Themes;
using System.Collections.Generic;
using System.Reflection;

namespace Virtex.Lib.Vrtc.GUI.Controls
{
    public class vxPropertyItemBoundingBox : vxPropertyItemBaseClass
	{
        BoundingBox BoundingBox;

        public Vector3 Max
        {
            get { return BoundingBox.Max; }
            set { BoundingBox.Max = value; }
        }

        public Vector3 Min
        {
            get { return BoundingBox.Min; }
            set { BoundingBox.Min = value; }
        }


        public void SetValue()
        {
            Console.WriteLine(BoundingBox);
                try
                {
                    PropertyInfo.SetValue(TargetObject, BoundingBox);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
        }

        public vxPropertyItemBoundingBox(vxPropertyGroup propertyGroup, PropertyInfo PropertyInfo, object TargetObject) :
		base(propertyGroup, PropertyInfo, TargetObject)
		{
            BoundingBox = (BoundingBox)PropertyInfo.GetValue(TargetObject);

            PropertyInfo maxProperty = this.GetType().GetProperty("Max");
            PropertyInfo minProperty = this.GetType().GetProperty("Min");

            Items.Add(new vxPropertyItemVector3(propertyGroup, maxProperty, this));
            Items.Add(new vxPropertyItemVector3(propertyGroup, minProperty, this));
		}
	}
}
