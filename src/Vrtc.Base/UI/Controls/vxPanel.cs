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
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Controls
{
    /// <summary>
    /// Basic Button GUI Control.
    /// </summary>
    public class vxPanel : vxGUIControlBaseClass
    {
        /// <summary>
        /// Gets or sets the texture for this Menu Entry Background.
        /// </summary>
        /// <value>The texture.</value>
        public Texture2D BackgroundTexture;


		public List<vxGUIControlBaseClass> Items = new List<vxGUIControlBaseClass>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxPanel"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		/// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		public vxPanel(vxEngine Engine, Vector2 position, int Width, int Height)
			: this(Engine, new Rectangle((int)position.X, (int)position.Y, Width, Height))
		{
			
		}
		

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxPanel"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		/// <param name="Bounds">Bounds.</param>
		public vxPanel(vxEngine Engine, Rectangle Bounds)
			: base(Engine, new Vector2(Bounds.X, Bounds.Y))
		{
            //Engine
            this.Engine = Engine;

            //Set up Font
            this.Font = vxGUITheme.Fonts.Font;

            Text = "";

            this.Width = Bounds.Width;
            this.Height = Bounds.Height;

            BackgroundTexture = vxInternalAssets.Textures.Blank;

            PositionChanged += OnPositionChanged;

            Theme.Background = new vxColourTheme(Color.Gray);
		}

        /// <summary>
        /// Called on position changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        public virtual void OnPositionChanged(object sender, EventArgs e)
        {
            // Now Draw the Controls for this panel.
            foreach (vxGUIControlBaseClass control in Items)
                control.Position = Position + control.OriginalPosition;
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


		public virtual void Add(vxGUIControlBaseClass control)
		{
			Items.Add(control);

			// Now Change the Position of this 
			control.Position = this.Position + control.OriginalPosition;
		}

		public override void Update()
		{
			base.Update();

            UpdateItems();
		}

        public virtual void UpdateItems()
        {
            // Now Draw the Controls for this panel.
            foreach (vxGUIControlBaseClass control in Items)
                control.Update();
        }

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
		public override void Draw()
        {
			if(DoBorder)
				Engine.SpriteBatch.Draw(BackgroundTexture, Bounds.GetBorder(BorderSize), BorderColour* Opacity);
			
            if(DrawBackground)
                Engine.SpriteBatch.Draw(BackgroundTexture, Bounds, GetStateColour(Theme.Background) * Opacity);
		
        	DrawItems();
		}

        public bool DrawBackground = true;

		public virtual void DrawItems()
		{
			// Now Draw the Controls for this panel.
			foreach (vxGUIControlBaseClass control in Items)
				control.Draw();
		}
    }
}
