using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Controls
{
	public class vxTabPageTab : vxButtonControl
	{
		vxTabPageControl TabPage;

		/// <summary>
		/// Gets or sets the art provider.
		/// </summary>
		/// <value>The art provider.</value>
		public new vxTabPageTabArtProvider ArtProvider;

		public static int TabWidth = 96;
		public static int TabHeight = 24;

		public vxTabPageTab(vxEngine Engine, string PageTitle, vxTabPageControl TabPage)
			: base(Engine, PageTitle, Vector2.Zero)
		{
			this.TabPage = TabPage;

			Width = TabWidth;
			Height = TabHeight;

			DoSelectionBorder = false;

			//Have this button get a clone of the current Art Provider
			ArtProvider = (vxTabPageTabArtProvider)vxGUITheme.ArtProviderForTabs.Clone();
			ArtProvider.SetDefaults();
		}

		public override void Draw()
		{
			ArtProvider.Theme.Background.SelectedColour = (TabPage.Index == TabPage.TabControl.SelectedIndex) ? Color.White : Color.Gray;

			//Now get the Art Provider to draw the scene
			this.ArtProvider.Draw(this);

			DrawToolTip();
		}

	}



	/// <summary>
	/// Tab page control.
	/// </summary>
	public class vxTabPageControl : vxPanel
	{
		/// <summary>
		/// The owning tab control.
		/// </summary>
		public vxTabControl TabControl;

		/// <summary>
		/// The tab for this page.
		/// </summary>
		public vxTabPageTab Tab;

		/// <summary>
		/// Is this Tab Page active.
		/// </summary>
		public bool IsActive = false;

		public bool IsTabSelected = false;

        /// <summary>
        /// The index of the page within the Tab Control.
        /// </summary>
        public new int Index = 0;


		/// <summary>
		/// Initializes a new instance of the <see cref="T:Virtex.Lib.Iris.Gui.Controls.vxTabPageControl"/> class.
		/// </summary>
		/// <param name="TabControl">Tab control.</param>
		/// <param name="PageTitle">Page title.</param>
		public vxTabPageControl(vxTabControl TabControl, string PageTitle):base(TabControl.Engine, TabControl.Bounds)
		{
			this.TabControl = TabControl;
			Tab = new vxTabPageTab(Engine, PageTitle, this);
			Tab.Clicked += Tab_Clicked;


			Tab.Theme = vxGUITheme.UnselectedItemTheme;
		}

        public vxTabPageControl(vxEngine Engine, string PageTitle) : 
            base(Engine, new Rectangle(0,0,250,100))
        {
            //this.TabControl = TabControl;
            Tab = new vxTabPageTab(Engine, PageTitle, this);
            Tab.Clicked += Tab_Clicked;

            Tab.Theme = vxGUITheme.UnselectedItemTheme;
        }

        /// <summary>
        /// Selects the tab.
        /// </summary>
        public virtual void SelectTab()
		{
            Tab.Theme = vxGUITheme.SelectedItemTheme;
            TabControl.SelectedIndex = this.Index;
			TabControl.OnSelectedTabChange();
			IsTabSelected = true;

		}


		/// <summary>
		/// Uns the select tab.
		/// </summary>
		public virtual void UnSelectTab()
		{
            Tab.Theme = vxGUITheme.UnselectedItemTheme;
			IsTabSelected = false;
		}



		/// <summary>
		/// Fired when the Tab is clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Tab_Clicked(object sender, vxGuiItemClickEventArgs e)
		{
			// Unselect Previous Tab
			TabControl.Pages[TabControl.SelectedIndex].UnSelectTab();

			// Set the New Selected Index
			TabControl.SelectedIndex = Index;

			// Now Select The New Tab
			TabControl.Pages[TabControl.SelectedIndex].SelectTab();
		}


		public override void Draw()
		{
			// First Reset the Panel Position
			Position = TabControl.Position + new Vector2(0, Tab.Height);

			//Colour = Color.Transparent;

			base.Draw();
		}

        public virtual void DrawTab()
        {
            Tab.Draw();
        }

	}
}
