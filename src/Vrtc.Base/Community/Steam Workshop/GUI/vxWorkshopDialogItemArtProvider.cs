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
using VerticesEngine.Community;
using VerticesEngine.Community.Dialogs;
using VerticesEngine.Mathematics;

namespace VerticesEngine.UI.Themes
{
    public class vxWorkshopDialogItemArtProvider : vxArtProviderBase, IGuiArtProvider
	{
        public vxWorkshopDialogItemArtProvider(vxEngine Engine) : base(Engine)
		{
            Theme.Text.DisabledColour = Color.DimGray;
			Theme.Text.SelectedColour = Color.White;
            Theme.Text.HoverColour = Color.Black;

            Theme.Background.DisabledColour = new Color(0.15f, 0.15f, 0.15f, 0.15f);
			Theme.Background.SelectedColour = Color.DeepSkyBlue;
			Theme.Background.HoverColour = Color.DarkOrange;
            //Theme.Border.SelectedColour = Color.WhiteSmoke * 0.5f;


            Padding = new Vector2(4);
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

        public override SpriteFont GetFont()
        {
            return vxGUITheme.Fonts.Font;
        }
        float loadingRot = 0;

        float ReqLoadedAlpha = 0;
        float LoadedAlpha = 0;





        public virtual void Draw(object guiItem)
		{
            var item = (vxWorkshopDialogItem)guiItem;
            loadingRot += 0.1f; 
			float i = 1;

            if (item.IsProcessed)
                ReqLoadedAlpha = 1;

            LoadedAlpha = vxMathHelper.Smooth(LoadedAlpha, ReqLoadedAlpha, 2);

            Theme.SetState(item);

            //SpriteFont SubFont = vxInternalAssets.Fonts.ViewerFont;
            SpriteFont SubFont = vxGUITheme.Fonts.FontSmall;

            //if(item.Enabled == false)
                //Console.WriteLine()

			//Draw Button Background
			SpriteBatch.Draw(DefaultTexture, item.Bounds, Theme.Border.Color);
            SpriteBatch.Draw(DefaultTexture, item.Bounds.GetBorder(-1), Theme.Background.Color * i);

                float w = (item.Height * 600.0f / 380.0f);
            Rectangle buttonRect = new Rectangle((int)(item.Position.X + Padding.X),(int)(item.Position.Y + Padding.Y),
                                            (int)(w - Padding.X * 2),(int)(item.Height - Padding.Y * 2));
			//Draw Icon
			if (item.ButtonImage != null)
			{
                SpriteBatch.Draw(item.ButtonImage, buttonRect.GetBorder(2), Color.Black);
                SpriteBatch.Draw(item.ButtonImage, buttonRect, Color.White * LoadedAlpha);
			}
            else
            {
                
                SpriteBatch.Draw(DefaultTexture, buttonRect.GetBorder(2), Color.Black);
                SpriteBatch.Draw(DefaultTexture, buttonRect, Color.Gray * 0.25f);
                // 957 674 32 32
                SpriteBatch.Draw(vxGUITheme.SpriteSheet, buttonRect.Center.ToVector2(), new Rectangle(957, 674, 32, 32), 
                                 Color.White, loadingRot, Vector2.One * 16, 1.5f, SpriteEffects.None, 1);
            }


            string text = item.Text;
            float width = item.Bounds.Width - (item.Height * 2 + Padding.X * 4);

            float textWidth = Font.MeasureString(text).X;

            if (textWidth > width)
            {
                for (int ci = 0; ci < text.Length; ci++)
                {
                    string txt = text.Substring(0, ci);
                    float subTxtWidth = vxLayout.GetScaledWidth(Font.MeasureString(txt).X);
                    if (subTxtWidth > width)
                    {
                        text = text.Substring(0, ci) + "...";
                        break;
                    }
                }
            }


            //if (item.ToggleState || item.HasFocus)

            Vector2 TitlePos = new Vector2((int)(item.Position.X + w + Padding.X * 2), (int)(item.Position.Y + 8));
            Vector2 DescriptioPos = new Vector2(TitlePos.X, TitlePos.Y + vxLayout.GetScaledHeight(Font.LineSpacing));
            Vector2 sizePos = new Vector2(item.Bounds.Right - vxLayout.GetScaledSize(SubFont.MeasureString(item.Author).X) - Padding.X, item.Position.Y + Padding.Y);

            Color textShadow = ((item.ToggleState || item.HasFocus) ? Color.Black : Color.White);
            if (item.IsEnabled == false)
                textShadow = Color.Black;
            
            SpriteBatch.DrawString(Font, text, TitlePos + new Vector2(2),textShadow * 0.25f, vxLayout.Scale);

            string desp = SubFont.WrapString(item.Description, item.Bounds.Width / 2);
            
			//Draw Text String
            SpriteBatch.DrawString(Font, text,TitlePos, Theme.Text.Color, vxLayout.Scale);
            SpriteBatch.DrawString(SubFont, desp, DescriptioPos, Theme.Text.Color * 0.75f, vxLayout.Scale);
            SpriteBatch.DrawString(SubFont, item.Author, sizePos, Theme.Text.Color, vxLayout.Scale);
          
            //SpriteBatch.DrawString(SubFont, "Subscribed: " + item.IsSubscribed.ToString(),
            //                       new Vector2(TitlePos.X, item.Bounds.Bottom - SubFont.LineSpacing * 2),
            //                       Theme.Text.Color);

            //SpriteBatch.DrawString(SubFont, "Installed:  "+ item.Item.IsInstalled.ToString(),
                                   //new Vector2(TitlePos.X, item.Bounds.Bottom - SubFont.LineSpacing),
                                   //Theme.Text.Color);
            
            // File IO Version
            //string fileVersion = item.FileVersion;
            //SpriteBatch.DrawString(SubFont, fileVersion,
                                          //new Vector2(item.Bounds.Right - SubFont.MeasureString(fileVersion).X - Padding.X,
                                          //            item.Bounds.Bottom - SubFont.MeasureString(fileVersion).Y - Padding.Y),
                                          //Theme.Text.Color * 0.5f);

		}
	}
}
