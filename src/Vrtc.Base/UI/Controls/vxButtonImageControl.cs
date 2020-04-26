using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
    /// <summary>
    /// Button which has no text, only an Image.
    /// </summary>
    public class vxButtonImageControl : vxGUIControlBaseClass
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

        /// <summary>
        /// Gets or sets the blank draw hover background.
        /// </summary>
        /// <value>The draw hover background.</value>
        public bool DrawHoverBackground;

        /// <summary>
        /// The shadow drop offset.
        /// </summary>
        public int ShadowDrop = 4;

        /// <summary>
        /// The color of the shadow.
        /// </summary>
        public Color ShadowColor = Color.Black * 0.5f;

        public vxButtonImageControl(vxEngine Engine,
            Vector2 position,
            Texture2D buttonImage) :
        this(Engine,
                position,
                buttonImage,
             buttonImage,
                buttonImage.Width,
                buttonImage.Height)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxButtonImageControl"/> class.
        /// </summary>
        /// <param name="Engine">The Vertices Engine Reference.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="buttonImage">Button image.</param>
        /// <param name="hoverImage">Hover image.</param>
        public vxButtonImageControl(vxEngine Engine,
            Vector2 position,
            Texture2D buttonImage,
            Texture2D hoverImage) :
        this(Engine,
                position,
                buttonImage,
                hoverImage,
                buttonImage.Width,
                buttonImage.Height)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxButtonImageControl"/> class.
        /// </summary>
        /// <param name="Engine">The Vertices Engine Reference.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="buttonImage">Button image.</param>
        /// <param name="hoverImage">Hover image.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public vxButtonImageControl(vxEngine Engine,
            Vector2 position,
            Texture2D buttonImage,
            Texture2D hoverImage,
            int width, int height) : base(Engine)
        {
            //Get the current Game vxEngine
            this.Engine = Engine;


            //Set Button Images
            ButtonImage = buttonImage;
            HoverButtonImage = hoverImage;

            //Set Initial Bounding Rectangle
            Width = width;
            Height = height;
            //BorderSize = 2;

            Init(position);
        }


        bool UseSpriteSheet = false;
        public Rectangle MainSpriteSheetLocation;
        public Rectangle HoverSpriteSheetLocation;



        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxButtonImage"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="MainSpriteSheetLocation">Main sprite sheet location.</param>
		public vxButtonImageControl(vxEngine Engine, Vector2 position, Rectangle MainSpriteSheetLocation) :
        this(Engine, position, MainSpriteSheetLocation, MainSpriteSheetLocation, false)
        {
            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxButtonImage"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="MainSpriteSheetLocation">Main sprite sheet location.</param>
        /// <param name="HoverSpriteSheetLocation">Hover sprite sheet location.</param>
		public vxButtonImageControl(vxEngine Engine,
                             Vector2 position, 
                                    Rectangle MainSpriteSheetLocation, Rectangle HoverSpriteSheetLocation,
                                    bool DrawHoverBackground = true) : base(Engine)
        {
            this.Engine = Engine;
            this.DrawHoverBackground = DrawHoverBackground;
            this.MainSpriteSheetLocation = MainSpriteSheetLocation;
            this.HoverSpriteSheetLocation = HoverSpriteSheetLocation;

            UseSpriteSheet = true;

            Init(position);
        }

        void Init(Vector2 position)
        {

            //Set Position
            Position = position;
            OriginalPosition = position;

            //Set Default Colours

            //Default is true
            this.OnInitialHover += Event_InitialHover;
            this.Clicked += OnClicked;

            DrawHoverBackground = !(MainSpriteSheetLocation == HoverSpriteSheetLocation);

            if(DrawHoverBackground)
                Theme.Background = new vxColourTheme(Color.White);
            else
                Theme.Background = new vxColourTheme(Color.White, Color.Gray);

        }

        void OnClicked(object sender, VerticesEngine.UI.Events.vxGuiItemClickEventArgs e)
        {
#if !NO_DRIVER_OPENAL
            PlaySound(vxGUITheme.SoundEffects.MenuConfirm, 0.3f);
#endif
        }

        private void Event_InitialHover(object sender, EventArgs e)
        {
            //If Previous Selection = False and Current is True, then Create Highlite Sound Instsance
#if !NO_DRIVER_OPENAL
            PlaySound(vxGUITheme.SoundEffects.MenuHover, 0.25f);

#endif
        }

        public float Rotation = 0;

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public override void Draw()
        {
            if (IsVisible)
            {
                Theme.SetState(this);

                if (UseSpriteSheet)
                {
                    //Draw Regular Image
                    if (DoBorder)
                    {
                        Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds.GetBorder(BorderSize), MainSpriteSheetLocation,
                                                (IsEnabled ? Color.Black : Color.Black * 0.5f) * Alpha);
                    }

                    if (IsShadowVisible)
                    {
                        Position += new Vector2(ShadowDrop);

                        Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet,Bounds,
                                                MainSpriteSheetLocation,
                                                ShadowColor * Alpha);
                        Position -= new Vector2(ShadowDrop);
                    }


                    Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds,
                                            (HasFocus ? HoverSpriteSheetLocation : MainSpriteSheetLocation),
											Theme.Background.Color * Alpha, Rotation, Vector2.Zero, SpriteEffects.None, 1);

                    //Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds, HoverSpriteSheetLocation, Color_Normal * HoverAlpha);

                    if (IsTogglable && ToggleState)
                    {
                        Engine.SpriteBatch.Draw(vxGUITheme.SpriteSheet, Bounds, 
                                                HoverSpriteSheetLocation, Theme.Background.Color, Rotation, Vector2.Zero, SpriteEffects.None, 1);
                    }
                }
                else
                {
                    //Draw Regular Image
                    if (ButtonImage != null)
                    {
                        if (DoBorder)
                        {
                            Engine.SpriteBatch.Draw(ButtonImage, Bounds.GetBorder(BorderSize)
                                                    , (IsEnabled ? Color.Black : Color.Black * 0.5f) * Alpha);
                        }

                        if (IsShadowVisible)
                        {
                            Engine.SpriteBatch.Draw(ButtonImage, (Bounds.Location + new Point(ShadowDrop)).ToVector2(), ShadowColor * Alpha);
                        }


                        Engine.SpriteBatch.Draw(ButtonImage, Bounds, Theme.Background.Color * Alpha);

                        if (HoverButtonImage != null)
                            Engine.SpriteBatch.Draw(HoverButtonImage, Bounds, Theme.Background.HoverColour * HoverAlpha * Alpha);
                        else
                            Engine.SpriteBatch.Draw(ButtonImage, Bounds, Theme.Background.HoverColour * Alpha);
                    }

                    if (IsTogglable && ToggleState)
                    {

                        if (HoverButtonImage != null)
                            Engine.SpriteBatch.Draw(HoverButtonImage, Bounds, Theme.Background.NormalColour);
                        else
                            Engine.SpriteBatch.Draw(ButtonImage, Bounds, Theme.Background.HoverColour * Alpha);
                    }
                }
            }
        }
    }
}
