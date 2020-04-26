using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
	/// <summary>
	/// Ribbon control.
	/// </summary>
    public class vxRibbonContextualTabPage : vxRibbonTabPage
    {
        string GroupName;
        Color GroupColor;

        Texture2D gradient;

        public vxRibbonContextualTabPage(vxRibbonControl RibbonControl, string GroupName, string PageTitle, Color GroupColor) : 
        base(RibbonControl, PageTitle)
		{
            this.GroupName = GroupName;
            this.GroupColor = GroupColor;

            gradient = vxInternalAssets.LoadInternalTexture2D("Textures/Defaults/gradient_vertical");
		}

        public override void DrawTab()
        {
            base.DrawTab();

            Rectangle topBounds = Tab.Bounds;
            topBounds.Location -= new Point(0, Tab.Bounds.Height);
            topBounds.Height += topBounds.Height-2;

            Engine.SpriteBatch.Draw(gradient, topBounds, GroupColor * Opacity);

            Point TxtPos = new Point((int)(Tab.Position.X + Tab.Width / 2 - Font.MeasureString(GroupName).X / 2 - Padding.X),
                                     (int)(topBounds.Y + Tab.Height / 2 - Font.MeasureString(GroupName).Y / 2));
            
            Engine.SpriteBatch.DrawString(this.Font, GroupName, TxtPos.ToVector2(), Color.WhiteSmoke * Opacity);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void DrawItems()
        {
            Engine.SpriteBatch.Draw(gradient, Bounds, null, GroupColor * 0.25f,
                                    0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            base.DrawItems();
        }
	}
}
