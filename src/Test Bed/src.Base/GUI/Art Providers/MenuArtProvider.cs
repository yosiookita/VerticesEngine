using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using VerticesEngine.UI.Themes;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI;

namespace Virtex.App.VerticesTechDemo
{
    public class MenuItemArtProvider : vxMenuItemArtProvider
    {
        public MenuItemArtProvider(vxEngine Engine) : base(Engine)
        {
            //BackgroundColour = Color.DarkOrange;

            //BackgroundHoverColour = Color.DeepSkyBlue;

            //TextColour = Color.White;

            //TextHoverColour = Color.White;

            Theme = new vxGUIControlTheme(
            new vxColourTheme(Color.DarkOrange, Color.DeepSkyBlue),
                new vxColourTheme(Color.White));
            
            Padding = new Vector2(15, 15) * vxLayout.ScreenSizeScaler;
        }



        public override void Draw(object guiItem)
        {
            //First Cast the GUI Item to be a Menu Entry
            vxMenuEntry menuEntry = (vxMenuEntry)guiItem;

            Theme.SetState(menuEntry);

            // Set the font type
            SpriteFont font = vxGUITheme.Fonts.FontPack.Size12;

            // get the text size
            Vector2 Size = font.MeasureString(menuEntry.Text) * vxLayout.ScreenSizeScaler;

            // calculate the bounds from the size
            menuEntry.Bounds = new Rectangle(
                (int)(menuEntry.Position.X - Size.X / 2 - Padding.X),
                (int)(menuEntry.Position.Y - Size.Y / 2 - Padding.Y),
                (int)(Size.X + 2 * Padding.X),
                (int)(Size.Y + 2 * Padding.Y));

            //Set Opacity from Parent Screen Transition Alpha
            menuEntry.Opacity = menuEntry.ParentScreen.TransitionAlpha;

            //Do a last second null check.
            if (menuEntry.Texture == null)
                menuEntry.Texture = DefaultTexture;

            // Draw the background
            if (DrawBackgroungImage){

                // first draw the bacground edge
                Engine.SpriteBatch.Draw(menuEntry.Texture, menuEntry.Bounds.GetBorder(-1), Color.Black * menuEntry.Opacity);

                // now draw the main background
                Engine.SpriteBatch.Draw(menuEntry.Texture, menuEntry.Bounds.GetBorder(-4), Theme.Background.Color * menuEntry.Opacity);
            }
                

            // draw the menu text

            // draw the shadow
            Engine.SpriteBatch.DrawString(
                font,
                menuEntry.Text,
                menuEntry.Position + Vector2.One * 2,
                (menuEntry.HasFocus ? Color.Black : Color.Black * 0.75f) * menuEntry.Opacity,
            0,
                font.MeasureString(menuEntry.Text) / 2,
            vxLayout.ScreenSizeAvgScaler,
            SpriteEffects.None,
            1);

            // draw the main text
            Engine.SpriteBatch.DrawString(
                font,
                menuEntry.Text,
                menuEntry.Position,
                Theme.Text.Color * menuEntry.Opacity,
            0,
                font.MeasureString(menuEntry.Text) / 2,
                vxLayout.ScreenSizeAvgScaler,
            SpriteEffects.None,
            1);
        }
    }
}
