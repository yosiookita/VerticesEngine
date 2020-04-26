using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;

namespace VerticesEngine.UI.Controls
{
	/// <summary>
	/// Tab Control which managers Tab Pages.
	/// </summary>
    public class vxSlideTabControl : vxGUIControlBaseClass
    {
        /// <summary>
        /// List of Tab Pages
        /// </summary>
        public List<vxSlideTabPage> Pages = new List<vxSlideTabPage>();

        /// <summary>
        /// Space in between tabs
        /// </summary>
        int PageOffset = 1;

        /// <summary>
        /// The response time. Higher Number is slower.
        /// </summary>
        public int ResponseTime = 3;

        /// <summary>
        /// The Amount that the page entends out
        /// </summary>
        public int SelectionExtention
        {
            get{return this.Width;}   
        }

		/// <summary>
		/// The height of the tab stub.
		/// </summary>
        public int TabHeight = 24;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxSlideTabControl"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		/// <param name="offSetPosition">Off set position.</param>
		/// <param name="GUIOrientation">GUIO rientation.</param>
        public vxSlideTabControl(vxEngine Engine, int Width, int Height, 
		                    Vector2 offSetPosition, vxGUIItemOrientation GUIOrientation):
		base(Engine)
        {
            this.Engine = Engine;

            this.Width = Width;
            this.Height = Height;


            ItemOreintation = GUIOrientation;
            switch (GUIOrientation)
            {
                case vxGUIItemOrientation.Left:

                    Position = offSetPosition - new Vector2(this.Width, 0);
                    break;

                case vxGUIItemOrientation.Right:

                    Position = new Vector2(Engine.GraphicsDevice.Viewport.Width, 0) + offSetPosition;
                    TabHeight *= -1;
                    break;
            }
			OriginalPosition = Position;
            HoverAlphaMax = 0.75f;
            HoverAlphaMin = 0.5f;
            HoverAlphaDeltaSpeed = 10;
        }
        //int gap = 35;

		/// <summary>
		/// Adds a vxTabPage.
		/// </summary>
		/// <param name="guiItem">GUI item.</param>
        public void AddItem(vxSlideTabPage guiItem)
        {
            int tempPosition = 0;
            //First Set Position
            foreach (vxSlideTabPage page in Pages)
            {
                tempPosition += page.TabWidth + PageOffset;
            }

            guiItem.TabOffset = new Vector2(0, tempPosition);
            guiItem.ItemOreintation = this.ItemOreintation;


            Pages.Add(guiItem);
        }

		/// <summary>
		/// Closes all tabs.
		/// </summary>
        public void CloseAllTabs()
        {
            foreach (vxSlideTabPage tabPage in Pages)
            {
				tabPage.Close ();
            }
            this.HasFocus = false;
        }

		/// <summary>
		/// Updates the GUI Item
		/// </summary>
		public override void Update()
        {
            base.Update();
            int runningTabHeight = 0;
            foreach (var page in Pages)
            {
                page.TabVerticleOffset = runningTabHeight;
                runningTabHeight += page.Tab.Height + (int)page.Tab.Padding.Y;

                page.Update();
                if (page.HasFocus == true)
                    HasFocus = true;
            }
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		public override void Draw()
        {
            base.Draw();

            //Draw Button
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds, Color.Black * HoverAlpha);

            //Draw Pages
            foreach (vxSlideTabPage page in Pages)
                page.Draw();

            //Draw Tabs Ontop
            //foreach (vxSlideTabPage bsGuiItm in Pages)
                //bsGuiItm.DrawTab();
            

            Engine.SpriteBatch.Draw(DefaultTexture,
                new Rectangle((int)(Position.X) - TabHeight, (int)(Position.Y), this.Width, this.Height),
                Color.DarkOrange);
             
        }
    }
}
