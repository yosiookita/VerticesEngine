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
    public class vxRibbonDropdownControl : vxComboBox
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxLabel"/> class.
		/// </summary>
		/// <param name="Engine">The Vertices Engine Reference.</param>
		/// <param name="text">This GUI Items Text.</param>
		/// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        public vxRibbonDropdownControl(vxRibbonControlGroup RibbonControlGroup, string text, string Value) : 
        base(RibbonControlGroup.Engine, text, Vector2.Zero)
        {
            UIManager = RibbonControlGroup.UIManager;
            ////Colour_Text = Engine.GUITheme.vxLabelColorNormal;
            ItemPadding = 3;
            TextJustification = vxEnumTextHorizontalJustification.Left;
            this.Font = vxInternalAssets.Fonts.ViewerFont;
            Width = 100;
            Height = 20;
            Theme.Background = new vxColourTheme(
                Color.Transparent,
                RibbonControlGroup.RibbonTabPage.RibbonControl.ForegroundColour,
                Color.DarkOrange);
            Theme.Text = new vxColourTheme(
                Color.Black,
                Color.Black,
                Color.Black);
            Theme.Border = new vxColourTheme(
                Color.Transparent,
                Color.DeepSkyBlue,
                Color.DarkOrange);
            
            //DoBorder = false;

            RibbonControlGroup.Add(this);
        }

        public override void Draw()
        {
            base.Draw();
            base.DrawText();
        }
    }
}
