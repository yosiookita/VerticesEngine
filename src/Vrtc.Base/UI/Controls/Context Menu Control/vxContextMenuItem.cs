using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;
using VerticesEngine.UI.Controls;
using System.Collections.Generic;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// File Chooser Dialor Item.
    /// </summary>
    public class vxContextMenuItem : vxButtonControl
    {
        vxContextMenuControl Menu;

        Vector2 TextPosition = new Vector2();

        public new Texture2D Icon;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxContextMenuItem"/> class.
        /// </summary>
        /// <param name="menu">Menu.</param>
        /// <param name="Text">Text.</param>
        public vxContextMenuItem(vxContextMenuControl menu, string Text, Texture2D Icon = null) : base(menu.Engine, Text, Vector2.Zero)
        {
            Padding = new Vector2(4);

            this.Icon = Icon;
            Menu = menu;
            Font = vxInternalAssets.Fonts.ViewerFont;
            Width = menu.Width;//(int)(Font.MeasureString(Text).X + Padding.X * 2);
            Height = 20;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.Transparent, Color.DeepSkyBlue, Color.DarkOrange));

            Clicked += delegate { Menu.Hide(); };

            menu.AddItem(this);
        }

        public override void OnTextChanged()
        {
            base.OnTextChanged();

            OnItemPositionChange();
        }

        public override void OnItemPositionChange()
        {
            base.OnItemPositionChange();

            TextPosition = Position + new Vector2(22, Height / 2);
        }


        public override void Draw()
        {
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds, GetStateColour(Theme.Background)*1.05f);
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds.GetBorder(-1), GetStateColour(Theme.Background));

            Engine.SpriteBatch.DrawString(Font, Text, TextPosition,
                                          GetStateColour(Theme.Text), 0, Vector2.UnitY * (TextSize.Y / 2-3), 1, SpriteEffects.None, 1);


            //Draw Icon
            if (Icon != null)
            {
                Engine.SpriteBatch.Draw(Icon, new Rectangle((int)(Position.X+2), (int)(Position.Y+2), 16, 16), Color.White);
            }
        }
    }

    public class vxContextMenuSplitter : vxButtonControl
    {
        vxContextMenuControl Menu;

        Rectangle Splitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxContextMenuItem"/> class.
        /// </summary>
        /// <param name="menu">Menu.</param>
        public vxContextMenuSplitter(vxContextMenuControl menu) : base(menu.Engine, "", Vector2.Zero)
        {
            Padding = new Vector2(4);

            Menu = menu;
            Font = vxInternalAssets.Fonts.ViewerFont;
            Width = menu.Width;
            Height = 5;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.Gray * 0.5f));
        }

        public override void OnLayoutInvalidated()
        {
            base.OnLayoutInvalidated();

            Splitter = new Rectangle(Bounds.X+4, Bounds.Y, Bounds.Width-8, 1);
        }

        public override void OnTextChanged()
        {
            base.OnTextChanged();

            OnItemPositionChange();
        }
                

        public override void Draw()
        {
            Engine.SpriteBatch.Draw(DefaultTexture, Splitter, GetStateColour(Theme.Background));

        }
    }
}
