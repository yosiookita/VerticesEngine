using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using VerticesEngine;
using VerticesEngine.UI.Controls;
using VerticesEngine.Utilities;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Themes
{
    /// <summary>
    /// The button art provider.
    /// </summary>
	public class vxScrollBarArtProvider : vxArtProviderBase, IGuiArtProvider
	{
		public vxScrollBarArtProvider (vxEngine Engine):base(Engine)
		{
            SpriteSheetRegion = new Rectangle(0, 0, 4, 4);

			DefaultWidth = (int)(150 * vxLayout.ScaleAvg);
            DefaultHeight = (int)(24 * vxLayout.ScaleAvg);

			DoBorder = true;
			BorderWidth = 2;

            Theme = new vxGUIControlTheme(
            new vxColourTheme(Color.DarkOrange, Color.DarkOrange * 1.2f, Color.DeepSkyBlue),
                new vxColourTheme(Color.Black));
		}


		public object Clone()
		{
			return this.MemberwiseClone();
		}

        public virtual void Draw(object guiItem)
        {
            vxScrollBar bar = (vxScrollBar)guiItem;

            Theme.SetState(bar);

            // get the background size
            var rect = new Rectangle(bar.Bounds.X, bar.ParentPanel.Bounds.Y, bar.Bounds.Width, bar.ParentPanel.Height);

            // draw the background
            Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, rect, new Rectangle(), Color.Black * 0.5f);

            // draw the bar
            Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, bar.Bounds, SpriteSheetRegion, Theme.Background.Color);
        }

	}
}

