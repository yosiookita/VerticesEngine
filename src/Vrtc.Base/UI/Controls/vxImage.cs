using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using VerticesEngine.UI.Themes;
using VerticesEngine.Mathematics;
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Controls
{
	/// <summary>
	/// Label Class providing simple one line text as a vxGUI Item.
	/// </summary>
	public class vxImage : vxGUIControlBaseClass
    {
		/// <summary>
		/// The texture.
		/// </summary>
		public Texture2D Texture;

		public float ImageAlpha
		{
			get { return _imageAlpha;}
			set
			{
				_imageAlpha = value;
				ReqImageAlpha = value;
			}
		}
		float _imageAlpha = 1;


		public float ReqImageAlpha = 1;
		public float ReqAlphaStep = 8;


        /// <summary>
        /// The use sprite sheet.
        /// </summary>
		bool UseSpriteSheet = false;

        /// <summary>
        /// The sprite sheet location.
        /// </summary>
		public Rectangle SpriteSheetLocation;


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxImage"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="SpriteSheetLocation">Sprite sheet location for the GUI Sprite Sheet.</param>
        /// <param name="Rectangle">Rectangle.</param>
		public vxImage(vxEngine Engine, Rectangle SpriteSheetLocation, Rectangle Rectangle) : base(Engine, Rectangle.Location.ToVector2())
		{
			this.Engine = Engine;

            UseSpriteSheet = true;

            this.SpriteSheetLocation = SpriteSheetLocation;

			//this.Width = SpriteSheetLocation.Width;
			//this.Height = SpriteSheetLocation.Height;

			this.Bounds = Rectangle;

            Theme = new vxGUIControlTheme(new vxColourTheme(Color.White));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxImage"/> class.
		/// </summary>
		/// <param name="Engine">The Vertices Engine Reference.</param>
		/// <param name="Texture">Texture.</param>
		/// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
		public vxImage(vxEngine Engine, Texture2D Texture, Vector2 position):base(Engine, position)
        {
			this.Engine = Engine;

			this.Texture = Texture;

			this.Width = Texture.Width;
			this.Height = Texture.Height;

			this.Bounds = new Rectangle(
				(int)position.X,
				(int)position.Y,
				this.Width,
				this.Height);

            Theme = new vxGUIControlTheme(new vxColourTheme(Color.White));
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxImage"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="Texture">Texture.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="Width">Width.</param>
        /// <param name="Height">Height.</param>
		public vxImage(vxEngine Engine, Texture2D Texture, Vector2 position, int Width, int Height) : base(Engine, position)
		{
			this.Engine = Engine;

			this.Texture = Texture;

			this.Width = Width;
			this.Height = Height;

			this.Bounds = new Rectangle(
				(int)position.X,
				(int)position.Y,
				this.Width,
				this.Height);

            Theme = new vxGUIControlTheme(new vxColourTheme(Color.White));
		}


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxImage"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="Texture">Texture.</param>
        /// <param name="Rectangle">Rectangle.</param>
		public vxImage(vxEngine Engine, Texture2D Texture, Rectangle Rectangle) : base(Engine, new Vector2(Rectangle.X, Rectangle.Y))
		{
			this.Engine = Engine;

			this.Texture = Texture;

			this.Width = Rectangle.Width;
			this.Height = Rectangle.Height;

			this.Bounds = Rectangle;

            Theme = new vxGUIControlTheme(new vxColourTheme(Color.White));
		}


		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		public override void Draw()
        {
			
            base.Draw();
            _imageAlpha = vxMathHelper.Smooth(_imageAlpha, ReqImageAlpha, ReqAlphaStep);

            if (UseSpriteSheet)
			{
				if (IsShadowVisible)
					Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds.GetOffset(ShadowOffset.ToPoint()), SpriteSheetLocation, ShadowColour * ImageAlpha * ShadowTransparency);

				if (DoBorder)
					Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds.GetBorder(vxLayout.GetScaledSize(BorderSize)), SpriteSheetLocation, BorderColour * ImageAlpha);


				Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds,SpriteSheetLocation, Theme.Background.Color * ImageAlpha);
            }
            else
            {
                if (Texture != null)
                {
                    if (IsShadowVisible)
                        Engine.SpriteBatch.Draw(Texture, Bounds.GetOffset(ShadowOffset.ToPoint()), ShadowColour * ImageAlpha * ShadowTransparency);

                    if (DoBorder)
                        Engine.SpriteBatch.Draw(Texture, Bounds.GetBorder(vxLayout.GetScaledSize(BorderSize)), BorderColour * ImageAlpha);


                    Engine.SpriteBatch.Draw(Texture, Bounds, Theme.Background.Color * ImageAlpha);
                }
            }
        }
    }
}
