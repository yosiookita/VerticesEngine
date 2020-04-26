using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Input;
using VerticesEngine.Mathematics;
using VerticesEngine.UI.Themes;
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Controls
{
    public class vxSlideTabPageTab : vxButtonControl
    {
        public new Vector2 Padding
        {
            get { return ArtProvider.Padding; }
            set { ArtProvider.Padding = value; }
        }

        public vxSlideTabPageTab(vxEngine Engine, string text, Vector2 position, int Width, int Height)
            : base(Engine, text, position, Width, Height)
        {
            //  TabText = text;
            //Text = text;

            ArtProvider = (vxSlidePageTabArtProvider)vxGUITheme.ArtProviderForSlidePageTab.Clone();
        }

        public override void Draw()
        {
            //base.Draw();
            ArtProvider.Draw(this);
        }
    }




    public class vxSlidePageTabArtProvider : vxButtonArtProvider
    {
        public vxSlidePageTabArtProvider(vxEngine Engine) : base(Engine)
        {
            Padding = new Vector2(2, 5);
            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.Black * 0.5f, Color.Black * 0.75f, Color.DeepSkyBlue),
                new vxColourTheme(Color.White * 0.75f, Color.White, Color.Black));
        }


        public new object Clone()
        {
            return this.MemberwiseClone();
        }

        public override SpriteFont GetFont()
        {
            return vxGUITheme.Fonts.FontSmall;
        }

        public override void Draw(object guiItem)
        {
            vxSlideTabPageTab tab = (vxSlideTabPageTab)guiItem;

            Theme.SetState(tab);
            
            Vector2 TextSize = Font.MeasureString(tab.Text);

            tab.Height = (int)(TextSize.X + Padding.Y * 2);

            tab.Width = 24;

            //Draw Hover Rectangle
            SpriteBatch.Draw(DefaultTexture, tab.Bounds, Theme.Background.Color);

            //Draw Text
            SpriteBatch.DrawString(Font, tab.Text,
                                          new Vector2(tab.Bounds.X + Padding.Y + TextSize.Y, tab.Bounds.Y + Padding.X),
                                          Theme.Text.Color, MathHelper.PiOver2,
                Vector2.Zero, 1, SpriteEffects.None, 0);

        }
    }





















    /// <summary>
    /// Tab Page
    /// </summary>
    public class vxSlideTabPage : vxGUIControlBaseClass
    {
        /// <summary>
        /// The Tab Control which Manages this page.
        /// </summary>
        public vxSlideTabControl ParentTabControl;

        /// <summary>
        /// The tab.
        /// </summary>
        public vxSlideTabPageTab Tab;

        /// <summary>
        /// Tab Height
        /// </summary>
        public int TabHeight
        {
            get { return ParentTabControl.TabHeight; }
        }

        //int _tabHeight = 100;

        /// <summary>
        /// Tab Width
        /// </summary>
        public int TabWidth
        {
            get { return Tab.Width; }
        }


        //Rectangle Rec_Back = new Rectangle ();

        public Vector2 Position_Original = new Vector2();
        public Vector2 Position_Requested = new Vector2();



        /// <summary>
        /// List of Items owned by this Tab Page
        /// </summary>
        public List<vxGUIControlBaseClass> Items = new List<vxGUIControlBaseClass>();


        //Rotation of Tab, Dependant on ItemOrientation
        //float TabRotation = 0;

        Texture2D TabTexture;
        Texture2D BackTexture;

        Vector2 TabPositionOffset = Vector2.Zero;

        Vector2 ChildElementOffset = Vector2.Zero;

        public Vector2 TabOffset = Vector2.Zero;


        Vector2 OffsetVector
        {
            get
            {
                Vector2 orientation = Vector2.Zero;
                switch (this.ItemOreintation)
                {
                    case vxGUIItemOrientation.Left:
                        orientation = Vector2.UnitX * ParentTabControl.SelectionExtention;
                        break;

                    case vxGUIItemOrientation.Right:
                        orientation = -Vector2.UnitX * ParentTabControl.SelectionExtention;
                        break;
                }
                return orientation;
            }
        }

        vxToggleImageButton PinOpen;

        public vxLabel label;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxSlideTabPage"/> class.
        /// </summary>
        /// <param name="ParentTabControl">Parent tab control.</param>
        /// <param name="tabName">Tab name.</param>
        public vxSlideTabPage(vxSlideTabControl ParentTabControl, string tabName) :
        base(ParentTabControl.Engine)
        {
            this.Font = vxInternalAssets.Fonts.BaseFont;

            this.ParentTabControl = ParentTabControl;

            //Set Tab Name
            Text = tabName;

            //Position is Set By Tab Control
            Position = this.ParentTabControl.Position;
            Position_Original = this.ParentTabControl.Position - new Vector2(this.ParentTabControl.TabHeight, 0);

            Height = 46 + 2;


            TabTexture = vxInternalAssets.Textures.Blank;
            BackTexture = vxInternalAssets.Textures.Blank;
            Padding = new Vector2(5);


            HoverAlphaMax = 1.0f;
            HoverAlphaMin = 0.0f;
            HoverAlphaDeltaSpeed = 10;


            Tab = new vxSlideTabPageTab(Engine, Text, Vector2.Zero, 32, 100);
            Tab.Font = this.Font;
            Tab.Clicked += Tab_Clicked;

            this.Width = ParentTabControl.Width - Tab.Width;
            this.Height = ParentTabControl.Height;

            //Set up Events
            this.ItemOreintationChanged += new EventHandler<EventArgs>(vxTabPage_ItemOreintationChanged);

            label = new vxLabel(Engine, tabName, new Vector2(5));
            label.Font = vxGUITheme.Fonts.FontSmall;
            label.Theme.Text = new vxColourTheme(Color.Black);
            //label.//Colour_Text = Color.Black;
            AddItem(label);

            PinOpen = new vxToggleImageButton(Engine,
                                              new Vector2(Width - 24, 5),
                                              vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/misc/pin_unchecked"),
                                              vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/misc/pin_checked"));

            PinOpen.IsTogglable = true;
            PinOpen.UnFocusAlpha = 1;
            AddItem(PinOpen);

            Close();
        }



        void Tab_Clicked(object sender, Events.vxGuiItemClickEventArgs e)
        {
            ParentTabControl.CloseAllTabs();
            Open();
        }

        void vxTabPage_ItemOreintationChanged(object sender, EventArgs e)
        {
            switch (this.ItemOreintation)
            {
                case vxGUIItemOrientation.Left:

                    //Set Rotatioj
                    //TabRotation = MathHelper.PiOver2;

                    //Set Tab Offsets
                    TabPositionOffset = new Vector2(this.Width, 0) + TabOffset;

                    ChildElementOffset = Padding;

                    //Position is Set By Tab Control
                    //Position = this.ParentTabControl.Position - new Vector2 (this.ParentTabControl.TabHeight, 0);
                    Position_Original = Position;

                    break;

                case vxGUIItemOrientation.Right:

                    //Set Rotatioj
                   // TabRotation = MathHelper.PiOver2;

                    //Set Tab Offsets
                    TabPositionOffset = new Vector2(TabHeight, 0);

                    //ChildElementOffset = 0;//Padding;// new Vector2 (Padding, Padding.Y);

                    //Position is Set By Tab Control
                    Position = this.ParentTabControl.Position - new Vector2(TabHeight, 0);
                    Position_Original = Position;

                    break;
            }

            //Position_Original = this.ParentTabControl.Position - new Vector2(this.ParentTabControl.TabHeight, 0);
        }

        ///// <summary>
        ///// Sets the lengths.
        ///// </summary>
        //public void SetLengths()
        //{
        //    _tabWidth = (int)(Font.MeasureString(Text).X + Padding.X * 2);

        //    //_tabHeight = (int)(Font.MeasureString(Text).Y + Padding.Y * 2);
        //}

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="guiItem">GUI item.</param>
        public void AddItem(vxGUIControlBaseClass guiItem)
        {
            Items.Add(guiItem);

            if (guiItem.Bounds.Right > Bounds.Right)
                guiItem.Width = Width - (int)Padding.X * 2;
        }


        //Clears out the GUI Items
        public void ClearItems()
        {
            Items.Clear();
        }

        /// <summary>
        /// The force select.
        /// </summary>
        bool ForceSelect = false;

        public bool IsOpen{
            get { return _isOpen; }
        }
        bool _isOpen = false;

        /// <summary>
        /// Open this tab page. It won't close untill either Close(); is called, or if the tabpage recieves focus, and then loses focus.
        /// </summary>
        public void Open()
        {
            ForceSelect = true;
            _isOpen = true;
        }

        public void Close()
        {
            if (PinOpen.ToggleState == false)
            {
                //Clear Focus
                HasFocus = false;

                //Clear Force Select just in case it was set.
                ForceSelect = false;

                //Reset Initial Position
                Position_Requested = Position_Original;
                _isOpen = false;
            }
        }
        public int TabVerticleOffset = 0;

        public bool IsToggleControledInternally = true;

        /// <summary>
        /// Updates the GUI Item
        /// </summary>
        public override void Update()
        {
            //SetLengths();

            if (HasFocus == true)
                ForceSelect = false;
            
            //First Set Position based off of Selection Status
            if (ForceSelect)
            {
                Position_Requested = Position_Original + OffsetVector;
            }
            else if (HasFocus == false && IsToggleControledInternally)
            {
                if (vxInput.IsNewMouseButtonPress(MouseButtons.LeftButton) ||
                   vxInput.IsNewTouchPressed())
                    Close();
            }

            // Smooth ou tthe Position
            Position = vxMathHelper.Smooth(Position, Position_Requested, ParentTabControl.ResponseTime);

            // Now set the Tab Position
            Tab.Position = Position + TabPositionOffset + new Vector2(0, TabVerticleOffset);

            vxConsole.WriteInGameDebug(this, Position_Requested);
            vxConsole.WriteInGameDebug(this, Position);


            base.Update();


			//First Set Position
			if (IsOpen)
			{
				for (int i = 0; i < Items.Count; i++)
				{
					vxGUIControlBaseClass bsGuiItm = Items[i];

					bsGuiItm.Position = this.Position + ChildElementOffset + bsGuiItm.OriginalPosition;

					bsGuiItm.Update();

					if (bsGuiItm.HasFocus == true)
						HasFocus = true;

					//Re-eable all child items
					if (Math.Abs(Vector2.Subtract(Position, Position_Requested).Length()) < 10)
					{
						bsGuiItm.IsEnabled = true;
						bsGuiItm.HoverAlphaReq = 1;
					}
					else
					{
						bsGuiItm.IsEnabled = false;
						bsGuiItm.HoverAlphaReq = 0;
					}
				}
			}
            Tab.Update();
        }
        public Color TitleColor = Color.DarkOrange;

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            //Draw Button
            Color ColorReq = Theme.Background.Color;

            if (HasFocus)
                ColorReq = Color.Black;
            
            SpriteBatch.Draw(BackTexture, Bounds, ColorReq * 0.5f);

            //SpriteBatch.Draw(BackTexture, new Rectangle(Bounds.Location, new Point(Bounds.Width, label.Height + 1)), Color.Black);
            //SpriteBatch.Draw(BackTexture, new Rectangle(Bounds.Location, new Point(Bounds.Width, label.Height)), TitleColor);


            Tab.Draw();

			// Draw Items
			if (IsOpen)
			{
				foreach (vxGUIControlBaseClass bsGuiItm in Items)
					bsGuiItm.Draw();
			}

        }


   //     /// <summary>
   //     /// Tabs are drawn by the tab control after everything else so that the are shown over tab of the open pages.
   //     /// </summary>
   //     public void DrawTab()
   //     {

   //         /*
			//int border = 2;
			////Draw Hover Rectangle
			//Engine.SpriteBatch.Draw(TabTexture, new Rectangle(Rec_Tab.X - border, Rec_Tab.Y - border,
			//                                                  Rec_Tab.Width + 2 * border, 
			//                                                  Rec_Tab.Height + 2 * border), Color.Black * HoverAlpha);
			//Engine.SpriteBatch.Draw (TabTexture, Rec_Tab, Colour * HoverAlpha);

			////Draw Edges
			//Engine.SpriteBatch.Draw (TabTexture, new Rectangle (Rec_Tab.X, Rec_Tab.Y - 1, Rec_Tab.Width, 1), Color.White * 0.25f);
			//Engine.SpriteBatch.Draw (TabTexture, new Rectangle (Rec_Tab.X, Rec_Tab.Y + Rec_Tab.Height, Rec_Tab.Width, 1), Color.White * 0.25f);

			////Draw String
			//Engine.SpriteBatch.DrawString (this.Font, Text, TabTextPosition, Color.White, TabRotation,
			//	Vector2.Zero, 1, SpriteEffects.None, 0);
			//*/
        //}

        /// <summary>
        /// When the GUIItem is Selected
        /// </summary>
        public override void Select()
        {
            foreach (vxSlideTabPage tabPage in ParentTabControl.Pages)
                tabPage.HasFocus = false;

            //foreach (vxGUIBaseItem bsGuiItm in Items)
            //bsGuiItm.Enabled = false;

            HasFocus = true;
            base.Select();
        }
    }
}
