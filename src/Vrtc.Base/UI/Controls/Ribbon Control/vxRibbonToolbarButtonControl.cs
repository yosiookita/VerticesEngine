using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
    public class vxRibbonToolbarButtonControl : vxButtonImageControl
    {
        public vxRibbonToolbarButtonControl(vxRibbonControl Ribbon, Texture2D Texture)
            : base(Ribbon.Engine, Vector2.Zero, Texture)
        {
            Ribbon.TitleToolbar.AddItem(this);
            Theme = new vxGUIControlTheme(
                new vxColourTheme(
                    Color.Transparent, Color.DarkOrange, Color.DeepSkyBlue),
                                              new vxColourTheme(
                    Color.Black * 0.75f, Color.Black, Color.Black));

            Width = 24;
            Height = 24;
        }

        public override void Draw()
        {
            //base.Draw();

            if (HasFocus && IsEnabled || ToggleState)
            {
                //Engine.SpriteBatch.Draw(DefaultTexture, Bounds.GetBorder(-1), GetStateColour(ItemTheme.Background) * Alpha);
                Engine.SpriteBatch.Draw(DefaultTexture, Bounds.GetBorder(-2), GetStateColour(Theme.Background) * Alpha * 1.0f);
                Engine.SpriteBatch.Draw(DefaultTexture, Bounds.GetBorder(-3), Color.WhiteSmoke * Alpha * 0.85f);
            }

            Engine.SpriteBatch.Draw(ButtonImage, Bounds.GetBorder(-4), (IsEnabled ? Color.White : Color.White * 0.5f) * Alpha);

        }
    }
}
