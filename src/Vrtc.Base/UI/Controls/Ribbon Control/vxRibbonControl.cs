using System;
using Microsoft.Xna.Framework;
using VerticesEngine;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
	/// <summary>
	/// Ribbon control.
	/// </summary>
	public class vxRibbonControl : vxTabControl
	{
		/// <summary>
		/// The default width of the ribbon.
		/// </summary>
		public static int DefaultRibbonWidth = 650;

		/// <summary>
		/// The default height of the ribbon.
		/// </summary>
		public static int DefaultRibbonHeight = 92;

        public Color BackgroundColour = new Color(50, 50, 50, 255);

        public Color ForegroundColour = Color.WhiteSmoke * 0.95f;


        public vxRibbonToolbarLogoButtonControl TitleButton;
        /// <summary>
        /// The title toolbar.
        /// </summary>
        public vxRibbonToolbar TitleToolbar;

        public vxRibbonControl(vxGUIManager GuiManager) :
        this(GuiManager ,Vector2.Zero){}

        public vxRibbonControl(vxGUIManager GuiManager, Vector2 Position):
        base(GuiManager.Engine, new Rectangle((int)(Position.X),(int)(Position.Y+24), 
                                              GuiManager.Engine.GraphicsDevice.Viewport.Width, DefaultRibbonHeight))
        {
			Theme = new vxGUIControlTheme(
			    new vxColourTheme(BackgroundColour, BackgroundColour, BackgroundColour),
			    new vxColourTheme(ForegroundColour, ForegroundColour, ForegroundColour));

            TitleToolbar = new vxRibbonToolbar(GuiManager.Engine, Position);
            GuiManager.Add(TitleToolbar);


            GuiManager.Add(this);

            TitleButton = new vxRibbonToolbarLogoButtonControl(this,
                                                                    vxInternalAssets.LoadInternalTexture2D("TitleScreen/logo/vrtx/vrtx_btn"))
            {
                Width = 42,
                Height = 42,
                Padding = new Vector2(0),
            };

            DefaultRibbonWidth = Engine.GraphicsDevice.Viewport.Width;

		}

        public void AddContextTab(vxTabPageControl page)
        {
            Add(page);
            TitleToolbar.IsTitleShown = false;
        }

        public void RemoveContextTab(vxTabPageControl page)
        {
            Remove(page);
            TitleToolbar.IsTitleShown = true;
        }

		/// <summary>
		/// Ons the selected tab change.
		/// </summary>
		public override void OnSelectedTabChange()
		{
			base.OnSelectedTabChange();

			if (SelectedIndex < Pages.Count)
			{
				vxRibbonTabPage tabPag = (vxRibbonTabPage)Pages[SelectedIndex];

				Width = Math.Max(tabPag.GroupWidth, DefaultRibbonWidth);
			}
		}

        public override void Draw()
        {
            // First draw the Background
            SpriteBatch.Draw(DefaultTexture, Bounds, BackgroundColour * Opacity);

            int tabCount = 0;
            int RunningWidth = 0;

            // Next draw the Tabs for each of the pages
            foreach (vxTabPageControl page in Pages)
            {

                // First Set the Position of the tab
                page.Tab.Position = Position + new Vector2(RunningWidth + TabStartOffset + TabPadding, 0);
                RunningWidth += page.Tab.Width + TabPadding;


                // Always draw the Tab last
                page.DrawTab();

                // Only draw the panel of the tab if it's the Selected Index.
                if (page.Index == SelectedIndex)
                    page.Draw();

                tabCount++;
            }

            TitleButton.Draw();
        }
	}
}
