//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using VerticesEngine;
//
//namespace VerticesEngine.UI.Themes
//{
//	public class vxMenuEntryTheme : vxBaseItemTheme {
//
//		public int vxMenuItemWidth;
//		public int vxMenuItemHeight;
//
//		public TextJustification TextJustification;
//		public Texture2D vxMenuItemBackground;
//		public Texture2D vxMenuSplitterTexture;
//
//
//        //Title Code
//		public Vector2 TitlePosition;
//		public Vector2 TitlePadding;
//		public Texture2D TitleBackground;
//		public Color TitleBackgroundColor;
//		public Color TitleColor;
//
//		public Vector2 BoundingRectangleOffset;
//
//		public vxMenuEntryTheme(vxEngine Engine) : base(Engine)
//		{
//			TextJustification = TextJustification.Center;
//
//			FineTune = new Vector2 (0, 5);
//
//			vxMenuItemWidth = 100;
//			vxMenuItemHeight = 34;
//
//			TitleColor = Color.White;
//			TitleBackgroundColor = Color.White;
//			TitlePadding = new Vector2 (10, 10);
//			TitlePosition = new Vector2(Engine.GraphicsDevice.Viewport.Width / 2, 80);
//
//			BoundingRectangleOffset = new Vector2(0,0);
//
//            DrawBackgroungImage = true;
//        } 
//	}
//}
//
