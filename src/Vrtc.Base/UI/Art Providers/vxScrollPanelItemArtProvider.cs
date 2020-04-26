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
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.UI.Dialogs;

namespace VerticesEngine.UI.Themes
{
	public class vxScrollPanelItemArtProvider : vxArtProviderBase, IGuiArtProvider
	{
		public vxScrollPanelItemArtProvider(vxEngine Engine) : base(Engine)
		{
            Padding = new Vector2(4);


            Theme = new vxGUIControlTheme(
                new vxColourTheme(new Color(0.15f, 0.15f, 0.15f, 0.5f), Color.DarkOrange),
                new vxColourTheme(Color.LightGray));
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

        int iconOffset = 0;
		public void Draw(object guiItem)
		{
			vxScrollPanelItem item = (vxScrollPanelItem)guiItem;

            Theme.SetState(item);

			//Draw Button Background
			Engine.SpriteBatch.Draw(DefaultTexture, item.Bounds, Color.Black);
            Engine.SpriteBatch.Draw(DefaultTexture, item.Bounds.GetBorder(-1), Theme.Background.Color);

			//Draw Icon
			if (item.ButtonImage != null)
			{
				Engine.SpriteBatch.Draw(item.ButtonImage, new Rectangle(
                    (int)(item.Position.X + Padding.X), 
                    (int)(item.Position.Y + Padding.Y),
                    (int)(item.Height - Padding.X * 2), 
                    (int)(item.Height - Padding.Y * 2)), 
                                        Color.LightGray);

                iconOffset = item.Height;
			}

			//Draw Text String
			Engine.SpriteBatch.DrawString(vxGUITheme.Fonts.Font, item.Text,
                                          new Vector2(
                                              (int)(item.Position.X + iconOffset + Padding.X * 2), 
                                              (int)(item.Position.Y + 8)),
				Theme.Text.Color);
		}
	}
}
