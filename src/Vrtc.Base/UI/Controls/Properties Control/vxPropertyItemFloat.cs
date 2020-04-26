using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using VerticesEngine.UI.Themes;
using VerticesEngine.Mathematics;

namespace VerticesEngine.UI.Controls
{
    public class vxPropertyControlFloatTextbox : vxPropertyControlTextbox
    {
        public vxPropertyControlFloatTextbox(vxPropertyItemBaseClass Property, string InitialValue) :
        base(Property, InitialValue)
        {

        }

        public override string FilterTextInput(string input)
        {
            float result;
            if (float.TryParse(input, out result))
            {
                return input;
            }
            else
            {
                return PreviousText;
            }
        }
    }

	public class vxPropertyItemFloat : vxPropertyItemBaseClass
    {
        public vxPropertyItemFloat(vxPropertyGroup propertyGroup, PropertyInfo PropertyInfo, List<object> TargetObjects) :
        base(propertyGroup, PropertyInfo, TargetObjects)
        {
			
        }

        public override vxGUIControlBaseClass CreatePropertyInputControl()
        {
            Value = Value == null ? "" : Value;
            return new vxPropertyControlFloatTextbox(this, Value);
        }
    }

}
