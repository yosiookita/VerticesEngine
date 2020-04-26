using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
	public enum vxEnumButtonSize
	{
		Small,
		Big
	}

	public enum vxEnumTextHorizontalJustification
	{
		Left,
		Center,
		Right
	}

    /// <summary>
    /// Basic Button GUI Control.
    /// </summary>
    public class vxButtonControl : vxGUIControlBaseClass
    {
        /// <summary>
        /// Gets or sets the art provider.
        /// </summary>
        /// <value>The art provider.</value>
        public vxButtonArtProvider ArtProvider;

        /// <summary>
        /// Gets or sets the texture for this Menu Entry Background.
        /// </summary>
        /// <value>The texture.</value>
        public Texture2D BackgroundTexture;

        /// <summary>
        /// The icon for this button.
        /// </summary>
        public Texture2D Icon;




        public vxEnumButtonSize ButtonSize = vxEnumButtonSize.Small;

        public vxEnumTextHorizontalJustification TextHorizontalJustification = vxEnumTextHorizontalJustification.Center;

        /// <summary>
        /// Should the width be set by using the min width or the length of the text?
        /// </summary>
        public bool UseDefaultWidth = true;


        public vxButtonControl(vxEngine Engine, object localTextKey, Vector2 Position, int Width, int Height)
            :this(Engine, Engine.Language[localTextKey], Position, Width, Height)
        {
            LocalisationKey = localTextKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxButton"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="Text">Text.</param>
        /// <param name="Position">Position.</param>
        /// <param name="Width">Width.</param>
        /// <param name="Height">Height.</param>
        public vxButtonControl(vxEngine Engine, string Text, Vector2 Position, int Width, int Height) :
        this(Engine, Text, Position)
        {
            this.Width = (int)(Width * vxLayout.ScaleAvg);
            this.Height = (int)(Height * vxLayout.ScaleAvg);

            UseDefaultWidth = false;
        }

        public vxButtonControl(vxEngine Engine, object localTextKey, Vector2 Position)
            :this(Engine, Engine.Language[localTextKey], Position)
        {
            LocalisationKey = localTextKey;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxButtonControl"/> class.
		/// </summary>
		/// <param name="Engine">The Vertices Engine.</param>
		/// <param name="Text">BUtton Text.</param>
        /// <param name="Position">Button Position.</param>
		public vxButtonControl(vxEngine Engine, string Text, Vector2 Position)
            : base(Engine, Position)
        {
            //Text
            this.Text = Text;

            //Engine
            this.Engine = Engine;

            //Set up Font
            Font = vxGUITheme.Fonts.Font;

            DoBorder = true;
            
			//Have this button get a clone of the current Art Provider
			ArtProvider = (vxButtonArtProvider)vxGUITheme.ArtProviderForButtons.Clone ();
            ArtProvider.SetDefaults();

			Width = (int)(Math.Max(ArtProvider.DefaultWidth, (int)(this.Font.MeasureString(Text).X + Padding.X * 2)) * vxLayout.ScaleAvg);


			OnInitialHover += this_OnInitialHover;
			Clicked += this_Clicked;
		}

		private void this_OnInitialHover(object sender, EventArgs e)
        {
			//If Previous Selection = False and Current is True, then Create Highlite Sound Instsance
#if !NO_DRIVER_OPENAL
			PlaySound(vxGUITheme.SoundEffects.MenuHover, 0.3f);
#endif
		}

		void this_Clicked (object sender, VerticesEngine.UI.Events.vxGuiItemClickEventArgs e)
		{
            #if !NO_DRIVER_OPENAL
			PlaySound(vxGUITheme.SoundEffects.MenuConfirm, 0.3f);
			#endif
		}

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public override void Draw()
        {
            //Now get the Art Provider to draw the scene
            this.ArtProvider.Draw(this);

            base.Draw();
        }
    }
}
