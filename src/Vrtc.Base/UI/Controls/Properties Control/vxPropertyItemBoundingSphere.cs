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
    public class vxPropertyItemBoundingSphere : vxPropertyItemBaseClass
	{
        BoundingSphere BoundingSphere;

        public Vector3 Center
        {
            get { return BoundingSphere.Center; }
            set { BoundingSphere.Center = value; }
        }

        public float Radius
        {
            get { return BoundingSphere.Radius; }
            set { BoundingSphere.Radius = value; }
        }


        public void SetValue()
        {
                try
                {
                PropertyInfo.SetValue(TargetObject, BoundingSphere);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
        }

        public vxPropertyItemBoundingSphere(vxPropertyGroup propertyGroup, PropertyInfo PropertyInfo, object TargetObject) :
		base(propertyGroup, PropertyInfo, TargetObject)
		{
            BoundingSphere = (BoundingSphere)PropertyInfo.GetValue(TargetObject);

            PropertyInfo centerProperty = this.GetType().GetProperty("Center");
            PropertyInfo radiusProperty = this.GetType().GetProperty("Radius");

            Items.Add(new vxPropertyItemVector3(propertyGroup, centerProperty, this));
            Items.Add(new vxPropertyItemBaseClass(propertyGroup, radiusProperty, this));
		}
	}
}
