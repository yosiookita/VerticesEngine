using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
	/// <summary>
	/// List view item base class.
	/// </summary>
    public class vxListViewItem : vxGUIControlBaseClass
    {
        int ButtonHeight = 64;
        public int ButtonWidth = 512;

       /// <summary>
       /// Initializes a new instance of the <see cref="VerticesEngine.UI.vxListViewItem"/> class.
       /// </summary>
       /// <param name="Engine">The Vertices Engine Reference.</param>
       /// <param name="text">This GUI Items Text.</param>
		public vxListViewItem(vxEngine Engine, string Text):base(Engine)
        {
			Padding = new Vector2(4);
            
            this.Engine = Engine;

			this.Font = vxGUITheme.Fonts.Font;
            
            this.Text = Text;

			ButtonHeight = (int)(Font.MeasureString(Text).Y + 2 * Padding.Y);
            Bounds = new Rectangle(0, 0, 400, ButtonHeight);


            Width = 3000;
        }



        
		public override void Draw()
        {
            base.Draw();

            //
            //Update Rectangle
            //
            Bounds = new Rectangle((int)(Position.X), (int)(Position.Y),
                ButtonWidth, ButtonHeight);
            
            //
            //Draw Button

            Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank, Bounds, Theme.Background.Color);
            
			Engine.SpriteBatch.DrawString(this.Font, Text,
                new Vector2(
                    (int)(Position.X + Padding.X), 
                    (int)(Position.Y + Padding.Y)),
                Theme.Text.Color);
        }
    }
}
