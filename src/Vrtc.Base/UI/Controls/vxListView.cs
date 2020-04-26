using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.Input;

namespace VerticesEngine.UI.Controls
{
    public enum vxListViewLayout
    {
        Grid,
        Details,
        List
    }
    public class vxListView : vxGUIControlBaseClass
    {
		//public float Alpha = 0.5f;
        
		public int ScrollBarWidth = 20;

        Viewport viewport_Original;

        public PanelLayout PanelLayout = PanelLayout.List;

        List<vxListViewItem> Items = new List<vxListViewItem>();

        vxListViewScrollBar scrollBar;

        Vector2 PaddingVector = new Vector2(0, 0);

        /// <summary>
        /// Scroll Panael to Hold a Grid or Detail list of Items
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
		public vxListView(vxEngine Engine, Vector2 Position, int Width, int Height):base(Engine)
        {
            this.Width = Width;
            this.Height = Height;
            this.Position = Position;
            this.OriginalPosition = Position;

            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            Padding = new Vector2(5);

            scrollBar = new vxListViewScrollBar(Engine,this,
                new Vector2(
                    Bounds.X + Bounds.Width - Padding.X - 20,
                    Bounds.Y + Padding.Y), 
                Bounds.Height, 
                Bounds.Height)
			{
				BarWidth = ScrollBarWidth,
			};
        }

        public override void Select()
        {
            HasFocus = true;
        }

        //Add an Item to the Scroll Panel
        public void AddItem(vxListViewItem guiItem)
        {
            //int temp_height = 0;

            if (Items.Count > 0)
            {
                vxListViewItem LastGuiItem = Items[Items.Count - 1];

                //First Set the Poition
                guiItem.Position = LastGuiItem.Position + new Vector2(LastGuiItem.Bounds.Width + Padding.X, 0);
            }
            else
            {
                //First Set the Poition
                guiItem.Position = Padding;
            }

            //Check if it is inside the bounding rectangle, if not, move it down one row.
            if (guiItem.Position.X + guiItem.Bounds.Width > this.Position.X + Bounds.Width - Padding.X * 2 - ScrollBarWidth)
            {
                if (Items.Count > 0)
                {
                    vxListViewItem LastGuiItem = Items[Items.Count - 1];
                    guiItem.Position = new Vector2(Padding.X, LastGuiItem.Position.Y + LastGuiItem.Bounds.Height + Padding.Y);
                }

                //There's a chance that This item is the width of the scroll panel, so it 
                //should be set as the minimum between it's width, and the scroll panels width
                guiItem.Width = Math.Min(guiItem.Width, Bounds.Width - (int)Padding.X * 2 - ScrollBarWidth);
             }

            guiItem.OriginalPosition = guiItem.Position;

            Items.Add(guiItem);



            /////////////////////////////////////////
            //        SET SCROLL BAR LENGTH
            /////////////////////////////////////////
            float totalHeight = this.Height;
            float tempPos_min = this.Height - 1;
            float tempPos_max = this.Height;

            //Get The Max and Min Positions of Items to get the overall Height
            foreach (vxGUIControlBaseClass bsGuiItm in Items)
            {
                tempPos_min = Math.Min(bsGuiItm.Position.Y, tempPos_min);
                tempPos_max = Math.Max(bsGuiItm.Position.Y + bsGuiItm.Bounds.Height, tempPos_max);
            }

            scrollBar.ScrollLength = (int)((tempPos_max - tempPos_min));
        }

        /// <summary>
        /// Adds a Range of Values for to the Scroll Panel
        /// </summary>
        /// <param name="xbaseGuiItem"></param>
        public void AddRange(IEnumerable<vxListViewItem> xbaseGuiItem)
        {
            foreach (vxListViewItem guiItem in xbaseGuiItem)
            {
                AddItem(guiItem);
            }
        }

        /// <summary>
        /// Sets the layout of the 
        /// </summary>
        void SetLayout()
        {

        }

		public override void Update()
        {
            base.Update();

            //Recalculate the Bounding rectangle each loop
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            Vector2 offset = new Vector2(viewport_Original.X - Bounds.X,
                viewport_Original.Y - Bounds.Y);

            scrollBar.Position = new Vector2(
                Bounds.X + Bounds.Width - Padding.X - scrollBar.BarWidth,
                Bounds.Y + Padding.Y);

            scrollBar.Update();
            MouseState ms = Mouse.GetState();
            //Only Update Stuff if it has Focus
            MouseState dummyMouseState = new MouseState(
                ms.X - (int)this.Position.X,
                ms.Y - (int)this.Position.Y,
                ms.ScrollWheelValue,
                ms.LeftButton,
                ms.MiddleButton,
                ms.RightButton,
                ms.XButton1,
                ms.XButton2);

                foreach (vxGUIControlBaseClass bsGuiItm in Items)
                {
                    bsGuiItm.Position = PaddingVector + bsGuiItm.OriginalPosition
                        - new Vector2(0, (scrollBar.Percentage * (scrollBar.ScrollLength - this.Height + 2 * Padding.Y)));

                if (this.HasFocus)
                {
                    MouseState prevMS = vxInput.MouseState;
                    vxInput.MouseState = dummyMouseState;
                    bsGuiItm.Update();
                    vxInput.MouseState = prevMS;
                }
                else
                    bsGuiItm.NotHover();
                }

        }

		public override void Draw()
        {
            base.Draw();

			//vxConsole.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            //Save the original Viewport
            viewport_Original = Engine.GraphicsDevice.Viewport;

            //Draw Background
			Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank, Bounds, Color.Black * Alpha);
            
			// Draw the Scroll bar
            scrollBar.Draw();

            //Set Viewport
            try
            {
                int Width = Bounds.Width;
                if (Bounds.X + Bounds.Width > Engine.GraphicsDevice.Viewport.Width)
                    Width = Engine.GraphicsDevice.Viewport.Width - Bounds.X;

                int Height = Bounds.Height;
                if (Bounds.Y + Bounds.Height > Engine.GraphicsDevice.Viewport.Height)
                    Height = Engine.GraphicsDevice.Viewport.Height - Bounds.Y;

                Engine.GraphicsDevice.Viewport = new Viewport(
                    new Rectangle(
                        Math.Min(Math.Max(Bounds.X, 0), Engine.GraphicsDevice.Viewport.Width - 1),
                        Math.Min(Math.Max(Bounds.Y, 0), Engine.GraphicsDevice.Viewport.Height - 1),
                        Width, 
                        Height));
            }
            catch (Exception ex) { vxConsole.WriteLine(ex.Message); }

            //Draw Items
            foreach (vxGUIControlBaseClass bsGuiItm in Items)
            {
                bsGuiItm.Draw();
            }

            //Reset the original Viewport
            Engine.GraphicsDevice.Viewport = viewport_Original;
        }
    }
}
