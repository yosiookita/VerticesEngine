﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using VerticesEngine;
using VerticesEngine.UI.Controls;
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Themes
{
	public class vxSliderArtProvider : vxArtProviderBase, IGuiArtProvider
	{
		/// <summary>
		/// The slider texture.
		/// </summary>
		public Texture2D SliderTexture;

		public vxSliderArtProvider(vxEngine Engine):base(Engine)
		{
			DefaultWidth = 150;
			DefaultHeight = 24;

			DoBorder = true;
            BorderWidth = 2;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.DarkOrange, Color.DarkOrange * 1.2f, Color.DeepSkyBlue),
                new vxColourTheme(Color.Black),
            new vxColourTheme(Color.Black * 0.75f, Color.Black, Color.Black));

			BackgroundImage = DefaultTexture;// Engine.InternalContentManager.Load<Texture2D>("Gui/DfltThm/vxGUITheme/vxButton/Bckgrnd_Nrml");

			SliderTexture = vxInternalAssets.Textures.Blank;
		}


		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public virtual void Draw(object guiItem)
		{
			vxSlider slider = (vxSlider)guiItem;


			// First draw the slider distance marker
			Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank, slider.MarkerRec, Color.Black);


            int border = 1;
            Rectangle borderBounds = new Rectangle(
                slider.Bounds.X - border, 
                slider.Bounds.Y - border, 
                slider.Bounds.Width + 2 * border, 
                slider.Bounds.Height + 2 * border);

            // Now Draw the Button Slider
            Engine.SpriteBatch.Draw(SliderTexture, borderBounds, Theme.Border.Color);
            Engine.SpriteBatch.Draw(SliderTexture, slider.Bounds, Theme.Background.Color);
		}
	}
}

