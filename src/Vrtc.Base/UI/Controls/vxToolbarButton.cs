using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{

    /// <summary>
    /// Toolbar Button Controls for the vxToolbar Class.
    /// </summary>
    public class vxToolbarButton : vxGUIControlBaseClass
    {

        /// <summary>
        /// Gets or sets the button image.
        /// </summary>
        /// <value>The button image.</value>
        public Texture2D ButtonImage;

        /// <summary>
        /// Gets or sets the button image.
        /// </summary>
        /// <value>The button image.</value>
        public Texture2D HoverButtonImage;


        bool UseSpriteSheet = false;
        public Rectangle MainSpriteSheetLocation;
        public Rectangle HoverSpriteSheetLocation;

        public vxToolbarButton(vxEngine Engine, Rectangle MainSpriteSheetLocation) :
        this(Engine, MainSpriteSheetLocation,
             new Rectangle(MainSpriteSheetLocation.X,
                           MainSpriteSheetLocation.Y + MainSpriteSheetLocation.Height,
                           MainSpriteSheetLocation.Width,
                           MainSpriteSheetLocation.Height))
        {

        }


        public vxToolbarButton(vxEngine Engine, Rectangle MainSpriteSheetLocation, Rectangle HoverSpriteSheetLocation) :
        base(Engine)
        {
            this.Engine = Engine;

            this.MainSpriteSheetLocation = MainSpriteSheetLocation;
            this.HoverSpriteSheetLocation = HoverSpriteSheetLocation;

            UseSpriteSheet = true;

            Width = MainSpriteSheetLocation.Width;
            Height = MainSpriteSheetLocation.Height;

            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxToolbarButton"/> class. Note the texutres
        /// are loaded by the Games Content manager.
        /// </summary>
        /// <param name="Engine">The Vertices Engine Reference.</param>
        /// <param name="TexturesPath">Path to the textures, note a 'hover texture' must be present with a '_hover' suffix</param>
        public vxToolbarButton(vxEngine Engine, string TexturesPath) : this(Engine, Engine.Game.Content, TexturesPath)
        {

        }

        /// <summary>
		/// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxToolbarButton"/> class.
        /// </summary>
		/// <param name="Engine">The current Vertices vxEngine Instance</param>
		/// <param name="Content">Content Manager too load the Textures with.</param>
        /// <param name="TexturesPath">Path to the textures, note a 'hover texture' must be present with a '_hover' suffix</param>
		public vxToolbarButton(vxEngine Engine, ContentManager Content, string TexturesPath) :
        base(Engine)
        {
            //Get the current Game vxEngine
            this.Engine = Engine;


            //Set Button Images
            if (TexturesPath != "")
            {
                ButtonImage = Content.Load<Texture2D>(TexturesPath);

                try
                {
                    HoverButtonImage = Content.Load<Texture2D>(TexturesPath + "_hover");
                }
                catch
                {
                    HoverButtonImage = Content.Load<Texture2D>(TexturesPath + "_Hover");
                }
                //Set Initial Bounding Rectangle
                Width = ButtonImage.Width;
                Height = ButtonImage.Height;
                Bounds = new Rectangle(0, 0, Width, Height);
            }
            Init();
        }

        void Init()
        {

            //Position is Set by Toolbar
            Position = Vector2.Zero;

            //Setup initial Events to handle mouse sounds
            this.OnInitialHover += Event_OnInitialHover;
            this.Clicked += Event_OnClicked;

            Theme.Background = new vxColourTheme(Color.White);
        }

        private void Event_OnInitialHover(object sender, EventArgs e)
        {
            //If Previous Selection = False and Current is True, then Create Highlite Sound Instsance
#if !NO_DRIVER_OPENAL

            if (IsEnabled)
            {
                PlaySound(vxGUITheme.SoundEffects.MenuHover, 0.3f);
            }
#endif
        }

        void Event_OnClicked(object sender, VerticesEngine.UI.Events.vxGuiItemClickEventArgs e)
        {
#if !NO_DRIVER_OPENAL

            if (IsEnabled)
            {
                PlaySound(vxGUITheme.SoundEffects.MenuConfirm, 0.3f);
            }
#endif


        }

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            Theme.SetState(this);

            if (UseSpriteSheet)
            {
                //Draw Regular Image
                Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds, MainSpriteSheetLocation, Theme.Background.Color);

                if (IsEnabled)
                {
                    //Draw Hover Items
                    //Engine.SpriteBatch.Draw (Engine.Assets.Textures.Blank, BoundingRectangle, Color_Highlight * HoverAlpha);

                    Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds, HoverSpriteSheetLocation, Theme.Background.Color * HoverAlpha);

                    if (IsTogglable && ToggleState)
                    {
                        Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds, HoverSpriteSheetLocation, Theme.Background.NormalColour);
                    }
                }
            }
            else
            {
                //Draw Regular Image
                Engine.SpriteBatch.Draw(ButtonImage, Bounds, Theme.Background.Color);

                if (IsEnabled)
                {

                    //Draw Hover Items
                    //Engine.SpriteBatch.Draw (Engine.Assets.Textures.Blank, BoundingRectangle, Color_Highlight * HoverAlpha);

                    if (HoverButtonImage != null)
                        Engine.SpriteBatch.Draw(HoverButtonImage, Bounds, Theme.Background.NormalColour * HoverAlpha);

                    if (IsTogglable && ToggleState)
                    {

                        if (HoverButtonImage != null)
                            Engine.SpriteBatch.Draw(HoverButtonImage, Bounds, Theme.Background.NormalColour);
                    }
                }
            }
        }
    }
}
