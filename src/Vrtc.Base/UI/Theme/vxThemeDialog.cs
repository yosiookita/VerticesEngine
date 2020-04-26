using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Core;

namespace Virtex.Lib.Vrtc.GUI.Themes
{
	public class vxThemeDialog : vxBaseItemTheme {

        public int Header_Width;
        public int Header_Height;

        public Color Header_TextColour;
        public Color Header_TextHover;

        public Color Header_BackgroundColour;
        public Color Header_BackgroundHoverColour;

        public Vector2 Header_Padding;
        public Vector2 Header_FineTune;

        public Texture2D Header_BackgroundImage;

        public int Header_BorderWidth;
        public bool Header_DoBorder;

        public TextJustification TextJustification;

        public vxThemeDialog(vxEngine Engine) :base(Engine)
		{
            Width = 150;
            Height = 24;

            TextJustification = TextJustification.Center;

            BackgroundColour = Color.LightGray;
            BackgroundHoverColour = Color.White;

            TextColour = Color.White;
            TextHover = Color.White;


            Header_BackgroundImage = Engine.InternalAssets.Textures.Blank;

            Header_Padding = new Vector2(10, 10);
            Header_FineTune = new Vector2(0);

            int s = 35;
            Header_TextColour = new Color(s, s, s, 255);
            Header_TextHover = Color.Black;

            Header_BackgroundColour = Color.Gray;
            Header_BackgroundHoverColour = Color.DarkOrange;

            Header_BorderWidth = 1;
            Header_DoBorder = false;
        }
    }
}

