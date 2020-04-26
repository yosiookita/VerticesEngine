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
	/// <summary>
	/// Label Class providing simple one line text as a vxGUI Item.
	/// </summary>
    public class vxPropertyComboBox : vxComboBox
    {
		/// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxPropertyComboBox"/> class.
        /// </summary>
        /// <param name="Property">Property.</param>
        /// <param name="text">Text.</param>
        /// <param name="Value">Value.</param>
        public vxPropertyComboBox(vxPropertyItemBaseClass Property, string text, string Value) : 
        base(Property.Engine, text, Vector2.Zero)
        {
            ////Colour_Text = Engine.GUITheme.vxLabelColorNormal;
            ItemPadding = 3;
            TextJustification = vxEnumTextHorizontalJustification.Left;
            this.Font = vxInternalAssets.Fonts.ViewerFont;

            Theme.Background = new vxColourTheme(
            Color.Transparent,
                Color.DarkOrange,
                Color.DarkOrange);
            Theme.Text = new vxColourTheme(
            Color.White * 0.75f,
                Color.White,
                Color.White);
            
            DoBorder = false;
        }
    }

    public class vxPropertyItemChoices : vxPropertyItemBaseClass
    {
        vxPropertyComboBox ComboBoxControl;

        public vxPropertyItemChoices(vxPropertyGroup propertyGroup, PropertyInfo PropertyInfo, List<object> TargetObjects) :
        base(propertyGroup, PropertyInfo, TargetObjects)
        {
           
        }

		public override vxGUIControlBaseClass CreatePropertyInputControl()
		{
			string InitValue = GetPropertyValueAsString();

			ComboBoxControl = new vxPropertyComboBox(this, InitValue, InitValue);

			ComboBoxControl.UIManager = Engine.CurrentScene.UIManager;

            ComboBoxControl.Font = Font;

			foreach (var item in Enum.GetValues(PropertyInfo.PropertyType))
				ComboBoxControl.AddItem(item.ToString());


			ComboBoxControl.SelectionChanged += ComboBoxControl_SelectionChanged;

			ComboBoxControl.Height = 16;

			return ComboBoxControl;
		}

		void ComboBoxControl_SelectionChanged(object sender, Events.vxComboBoxSelectionChangedEventArgs e)
		{
			SetValue(e.SelectedIndex);
		}
    }
}
