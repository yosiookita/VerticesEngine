using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
    /// <summary>
    /// A Seperator Used in the vxScrollPanel.
    /// </summary>
    public class vxScrollPanelSpliter : vxGUIControlBaseClass
    {
        /// <summary>
        /// Gets or sets the button image.fdsf
        /// </summary>
        /// <value>The button image.</value>
        public Texture2D ButtonImage;

		public static Color BackgrounColour = Color.DarkOrange;
		public static bool DoUnderline = true;

        public vxScrollPanelSpliter(
            vxEngine Engine,
			string Text):base(Engine)
        {
            this.Engine = Engine;

            this.Text = Text;

            Position = Vector2.Zero;

            ButtonImage = vxInternalAssets.Textures.Blank;

            this.Width = vxLayout.GetScaledWidth(4096);
            this.Height = vxLayout.GetScaledHeight(36);

            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            Font = vxGUITheme.Fonts.Font;
        }

        public override void DrawText()
        {
			Engine.SpriteBatch.DrawString(Font,
				Text,
				new Vector2(
					Bounds.Location.X + 5,
					Bounds.Location.Y + vxLayout.GetScaledSize(4) + 5),
				Color.Black);
        }

        public override void Draw()
        {

            Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank, Bounds.GetBorder(vxLayout.GetScaledSize(2)), Color.Black);
            Engine.SpriteBatch.Draw(ButtonImage, Bounds, BackgrounColour);


            if (Text != null)
            {
				if(DoUnderline)
                Engine.SpriteBatch.Draw(
                    vxInternalAssets.Textures.Blank,
                    new Rectangle(
                        Bounds.Location.X,
                        Bounds.Location.Y + vxLayout.GetScaledSize(4) + (int)vxInternalAssets.Fonts.MenuFont.MeasureString(Text).Y,
                        Bounds.Width,
                        1),
                    Color.Black * 0.5f);
            }
        }
    }
}
