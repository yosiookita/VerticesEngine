using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using VerticesEngine.UI.Controls;

namespace VerticesEngine.UI.Themes
{
    /// <summary>
    /// The button art provider.
    /// </summary>
	public class vxButtonArtProvider : vxArtProviderBase, IGuiArtProvider
	{
		public vxButtonArtProvider (vxEngine Engine):base(Engine)
		{
            SpriteSheetRegion = new Rectangle(0, 0, 4, 4);

			DefaultWidth = vxLayout.GetScaledHeight(192);
            DefaultHeight = vxLayout.GetScaledHeight(42);

			DoBorder = true;
            BorderWidth = 2;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.DarkOrange, Color.DarkOrange * 1.2f, Color.DeepSkyBlue),
                new vxColourTheme(Color.Black),
            new vxColourTheme(Color.Black * 0.75f, Color.Black, Color.Black));
		}


		public object Clone()
		{
			return this.MemberwiseClone();
		}

        public virtual void SetDefaults()
        {
            DefaultWidth = vxLayout.GetScaledHeight(225);
            DefaultHeight = vxLayout.GetScaledHeight(36);

        }



		public virtual void Draw(object guiItem)
		{
            vxButtonControl button = (vxButtonControl)guiItem;

            Theme.SetState(button);

            //Update Rectangle
            if (button.UseDefaultWidth)
            {
                //Set Width and Height
                button.Width = (int)(Math.Max(this.DefaultWidth, (int)(this.Font.MeasureString(button.Text).X + Padding.X * 2))*vxLayout.ScaleAvg);
			    button.Height = (int)(Math.Max(this.DefaultHeight, (int)(this.Font.MeasureString(button.Text).Y + Padding.Y * 2))* vxLayout.ScaleAvg);


                button.Bounds = new Rectangle(
                    (int)(button.Position.X - Padding.X),
                    (int)(button.Position.Y - Padding.Y / 2),
                    button.Width, button.Height);
            }

			float GUIAlpha = 1;
			if (button.UIManager != null)
				GUIAlpha = button.UIManager.Alpha;


			if(button.DoBorder)
                Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, button.BorderBounds, SpriteSheetRegion, button.BorderColour * Opacity * GUIAlpha);

            Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, button.Bounds, SpriteSheetRegion, Theme.Background.Color * Opacity * GUIAlpha);

			Engine.SpriteBatch.DrawString(this.Font, button.Text,
				new Vector2(
					button.Position.X + button.Width / 2 - this.Font.MeasureString(button.Text).X / 2* vxLayout.ScaleAvg - Padding.X,
					button.Position.Y + button.Height / 2 - this.Font.MeasureString(button.Text).Y / 2* vxLayout.ScaleAvg),
				Theme.Text.Color * Opacity* GUIAlpha,
                                         0,
                                         Vector2.Zero,
                                         vxLayout.ScaleAvg,
                                         SpriteEffects.None,
                                         1);
		}
	}
}

