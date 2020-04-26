using System;
using VerticesEngine;
using VerticesEngine.Utilities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using VerticesEngine.UI.Controls;

namespace VerticesEngine.UI.Themes
{
    /// <summary>
    /// Vx GUI theme.
    /// </summary>
    public class vxGUITheme
    {

        static vxEngine Engine;
        static ContentManager Content
        {
            get { return Engine.Game.Content; }
        }
        static string PathTooFiles = "Gui/DfltThm/";

        /// <summary>
        /// The GUI sprite sheet.
        /// </summary>
        public static Texture2D SpriteSheet;

        /*******************************************/
        //					MASTER VALUES
        /*******************************************/

        public static class Fonts
        {
            /// <summary>
            /// Title Font
            /// </summary>
            public static SpriteFont FontTitle;

            /// <summary>
            /// Gets or sets the font.
            /// </summary>
            public static SpriteFont Font;

            /// <summary>
            /// Small Font
            /// </summary>
            public static SpriteFont FontSmall;

            /// <summary>
            /// Tiny Font
            /// </summary>
            public static SpriteFont FontTiny;


            /// <summary>
            /// The font pack. Note that this is not used by the engine internally and
            /// must be intitalised outside of it.
            /// </summary>
            public static vxFontPack FontPack;
        }
        /// <summary>
        /// Gets or sets the default padding for all GUI items
        /// </summary>
        /// <value>The padding.</value>
        public Vector2 Padding = new Vector2(10, 10);

        //Misc
        //public Texture2D SplitterTexture;

        /*******************************************/
        //				ART PROVIDERS
        /*******************************************/
        public static vxButtonArtProvider ArtProviderForButtons;
        public static vxLabelArtProvider ArtProviderForLabels;
        public static vxTabPageTabArtProvider ArtProviderForTabs;
        public static vxTextboxArtProvider ArtProviderForTextboxes;
        public static vxScrollPanelArtProvider ArtProviderForScrollPanel;
        public static vxScrollPanelItemArtProvider ArtProviderForScrollPanelItem;
        public static vxScrollBarArtProvider ArtProviderForScrollBars;
        public static vxFileDialoglItemArtProvider ArtProviderForFileDialogItem;
        public static vxModDialoglItemArtProvider ArtProviderForModDialogItem;
        public static vxWorkshopDialogItemArtProvider ArtProviderForWorkshopDialogItem;
        public static vxFileExplorerItemArtProvider ArtProviderForFileExplorerItem;
        public static vxMenuScreenArtProvider ArtProviderForMenuScreen;
        public static vxMenuItemArtProvider ArtProviderForMenuScreenItems;
        public static vxMessageBoxArtProvider ArtProviderForMessageBoxes;
        public static vxSliderArtProvider ArtProviderForSlider;
        public static vxDialogArtProvider ArtProviderForDialogs;
        public static vxToolbarArtProvider ArtProviderForToolbars;
        public static vxSlidePageTabArtProvider ArtProviderForSlidePageTab;
        public static vxSpinnerControlArtProvider ArtProviderForSpinners;
        public static vxToolTipArtProvider ArtProviderForToolTips;

        /*******************************************/
        //					LABEL
        /*******************************************/
        //public Color vxLabelColorNormal = Color.White;

        /// <summary>
        /// The Sprite Sheet Locations for Common GUI Items such as Arrows
        /// </summary>
        public static class SpriteSheetLoc
        {
            public static Rectangle BlankLoc;
            public static Rectangle ArrowBtnBack;
            public static Rectangle ArrowBtnFwd;
            public static Rectangle ToggleOn;
            public static Rectangle ToggleOff;
        }


        //public vxThemeTextbox vxTextboxes;
        //public vxThemeDialog vxDialogs;
        //public vxLoadingScreenTheme vxLoadingScreen;




        /*******************************************/
        //					Sound Effects
        /*******************************************/
#if !NO_DRIVER_OPENAL

        public static class SoundEffects
        {
            public static SoundEffect MenuHover;
            public static SoundEffect MenuConfirm;
            public static SoundEffect MenuCancel;
        }

#endif

        public static vxGUIControlTheme SelectedItemTheme;

        public static vxGUIControlTheme UnselectedItemTheme;

        public vxGUIControlTheme DropDownItemTheme;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Themes.vxGUITheme"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        public vxGUITheme(vxEngine engine)
        {
            Engine = engine;

            Padding = vxLayout.Scale * Padding;

            SpriteSheet = vxInternalAssets.Textures.Blank;

            //SetDefaultTheme();
            Fonts.Font = vxInternalAssets.Fonts.MenuFont;
            Fonts.FontTitle = vxInternalAssets.Fonts.MenuTitleFont;
            Fonts.FontSmall = vxInternalAssets.Fonts.ViewerFont;



            // Sound Effects
            /*******************************************/
            SoundEffects.MenuHover = vxInternalAssets.SoundEffects.MenuClick; //Engine.InternalContentManager.Load<SoundEffect>("Gui/DfltThm/vxGUITheme/SndFx/Menu/Click/Menu_Click");
            SoundEffects.MenuConfirm = vxInternalAssets.SoundEffects.MenuConfirm;// Engine.InternalContentManager.Load<SoundEffect>("Gui/DfltThm/vxGUITheme/SndFx/Menu/MenuConfirm");
            SoundEffects.MenuCancel = vxInternalAssets.SoundEffects.MenuError;//Engine.InternalContentManager.Load<SoundEffect>("Gui/DfltThm/vxGUITheme/SndFx/Menu/MenuError");


            Color bckgrndCol = new Color(50, 50, 50, 255);
            Color foregrndCol = Color.DeepSkyBlue;

            SelectedItemTheme = new vxGUIControlTheme(
            new vxColourTheme(foregrndCol, foregrndCol, foregrndCol),
                new vxColourTheme(bckgrndCol, bckgrndCol, bckgrndCol));


            UnselectedItemTheme = new vxGUIControlTheme(
            new vxColourTheme(bckgrndCol, bckgrndCol * 1.75f, bckgrndCol),
            new vxColourTheme(foregrndCol, foregrndCol * 1.75f, foregrndCol));


            Color dDIThmBack = Color.Black * 0.75f;
            DropDownItemTheme = new vxGUIControlTheme(
            new vxColourTheme(dDIThmBack, Color.DarkOrange, dDIThmBack),
            new vxColourTheme(foregrndCol, Color.Black, foregrndCol));
        }


        public void SetDefaultTheme()
        {
            //Initialise Art Providers
            ArtProviderForButtons = new vxButtonArtProvider(Engine);
            ArtProviderForLabels = new vxLabelArtProvider(Engine);
            ArtProviderForTabs = new vxTabPageTabArtProvider(Engine);
            ArtProviderForTextboxes = new vxTextboxArtProvider(Engine);
            ArtProviderForScrollPanel = new vxScrollPanelArtProvider(Engine);
            ArtProviderForScrollPanelItem = new vxScrollPanelItemArtProvider(Engine);
            ArtProviderForScrollBars = new vxScrollBarArtProvider(Engine);
            ArtProviderForFileDialogItem = new vxFileDialoglItemArtProvider(Engine);
            ArtProviderForModDialogItem = new vxModDialoglItemArtProvider(Engine);
            ArtProviderForWorkshopDialogItem = new vxWorkshopDialogItemArtProvider(Engine);
            ArtProviderForFileExplorerItem = new vxFileExplorerItemArtProvider(Engine);
            ArtProviderForMenuScreen = new vxMenuScreenArtProvider(Engine);
            ArtProviderForMenuScreenItems = new vxMenuItemArtProvider(Engine);
            ArtProviderForMessageBoxes = new vxMessageBoxArtProvider(Engine);
            ArtProviderForSlider = new vxSliderArtProvider(Engine);
            ArtProviderForDialogs = new vxDialogArtProvider(Engine);
            ArtProviderForToolbars = new vxToolbarArtProvider(Engine);
            ArtProviderForSlidePageTab = new vxSlidePageTabArtProvider(Engine);
            ArtProviderForSpinners = new vxSpinnerControlArtProvider(Engine);
            ArtProviderForToolTips = new vxToolTipArtProvider(Engine);
        }

        /// <summary>
        /// Loads the gui sprite sheet.
        /// </summary>
        /// <param name="path">Path.</param>
        public static void LoadSpriteSheet(bool IsXNB = true, string path = "Textures/Gui/GUISpriteSheet")
        {
            // is the sprite sheet a compiled XNB?
            if (IsXNB)
            {
                SpriteSheet = Engine.Game.Content.Load<Texture2D>(path);
            }
            // sometimes the Spritesheet may be supplied as a 'png' file to cutdown on file space as a 1024x1024 image can be
            // ~300 kb as a png and 4,000 kb as an 'xnb'.
            else
            {
#if __ANDROID__
                Stream fileStream = Game.Activity.Assets.Open("Content/" + path + ".png");
                SpriteSheet = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
#else
                using (var fileStream = new System.IO.FileStream("Content/" + path + ".png", System.IO.FileMode.Open))
                {
                    SpriteSheet = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
                }
#endif
			}
        }


        /// <summary>
        /// Loads the texture.
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="path">Path.</param>
        public static Texture2D LoadTexture(ContentManager contentManager, string path)
		{
			try{
                return contentManager.Load<Texture2D>(PathTooFiles + path);
			}
			catch(Exception ex){
				vxConsole.WriteException ("vxGUITheme",ex);

				return vxInternalAssets.Textures.DefaultDiffuse;
			}
		}
	}

}

