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
	public class vxLabelArtProvider : vxArtProviderBase, IGuiArtProvider
	{
		public vxLabelArtProvider(vxEngine Engine):base(Engine)
		{
            SpriteSheetRegion = new Rectangle(0, 0, 4, 4);

			DefaultWidth = (int)(150 * vxLayout.ScaleAvg);
            DefaultHeight = (int)(24 * vxLayout.ScaleAvg);

			DoBorder = true;
            BorderWidth = 2;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.Black),
                new vxColourTheme(Color.Black),
            new vxColourTheme(Color.Black * 0.75f, Color.Black, Color.Black));

            //Font = vxGUITheme.Fonts.Font;
			//BackgroundImage = Engine.InternalContentManager.Load<Texture2D>("Gui/DfltThm/vxGUITheme/vxButton/Bckgrnd_Nrml");
		}


		public object Clone()
		{
			return this.MemberwiseClone();
		}



		public virtual void Draw(object guiItem)
		{
            vxLabel label = (vxLabel)guiItem;

            if (label.IsShadowVisible)
                Engine.SpriteBatch.DrawString(this.Font, label.Text,
                                                label.Position + label.ShadowOffset,
                                                label.ShadowColour * label.ShadowTransparency,
                                             label.Rotation,
                                             label.Origin,
                                            label.IsScaleFixed ? 1 : label.Scale,
                                            SpriteEffects.None, 0);


            Engine.SpriteBatch.DrawString(this.Font, label.Text, label.Position, Theme.Text.Color,
                                             label.Rotation,
                                             label.Origin,
                                            label.IsScaleFixed ? 1 : label.Scale,
                                            SpriteEffects.None, 0);
        }
	}
}

