using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using VerticesEngine;
using VerticesEngine.UI.Controls;
using VerticesEngine.Utilities;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.UI.Dialogs;

namespace VerticesEngine.UI.Themes
{
	public class vxScrollPanelArtProvider : vxArtProviderBase, IGuiArtProvider
    {
		/// <summary>
		/// A Specific Rasterizer State to preform Rectangle Sizzoring.
		/// </summary>
		public RasterizerState _rasterizerState;

        public Rectangle ScissorRectangle;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Themes.vxScrollPanelArtProvider"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        public vxScrollPanelArtProvider(vxEngine Engine):base(Engine)
		{
			_rasterizerState = new RasterizerState() { ScissorTestEnable = true };

            BackgroundImage = vxInternalAssets.Textures.Blank;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.Black * 0.75f));
        }

        public object Clone()
		{
			return this.MemberwiseClone();
        }

        public void Draw(object guiItem)
		{
			vxScrollPanel panel = (vxScrollPanel)guiItem;


			ScissorRectangle = panel.Bounds;
			
            //Set Scissor Rectangle Size

			try
			{
				//Set up Minimum 
				int x = Math.Min(Math.Max(panel.Bounds.X, 0), Engine.GraphicsDevice.Viewport.Width - 1);
				int y = Math.Min(Math.Max(panel.Bounds.Y, 0), Engine.GraphicsDevice.Viewport.Height - 1);

				int Width = panel.Bounds.Width;
				if (x + panel.Bounds.Width > Engine.GraphicsDevice.Viewport.Width)
					Width = Engine.GraphicsDevice.Viewport.Width - x;
				Width = Math.Max(Width, 1);

				int Height = panel.Bounds.Height;
				if (y + panel.Bounds.Height > Engine.GraphicsDevice.Viewport.Height)
					Height = panel.Bounds.Height - y;

				if (panel.Bounds.Y < 0)
					Height = panel.Bounds.Height + panel.Bounds.Y;

				Height = Math.Max(Height, 1);

				ScissorRectangle =
					new Rectangle(
						x,
						y,
						Width,
						Height);
				//rec = BoundingRectangle;
			}
			catch (Exception ex)
			{
				vxConsole.WriteException(this,ex);
			}


			//Only draw if the rectangle is larger than 2 pixels. This is to avoid
			//artifacts where the minimum size is 1 pixel, which is pointless.
			if (ScissorRectangle.Height > 5 && panel.Position.X + ScissorRectangle.Width / 2 > 0)
			{
				// First End the Main GUImanager Spritebatch
				Engine.SpriteBatch.End();

				// Now Draw Background the background with a new spritebatch
				Engine.SpriteBatch.Begin("UI - Scroll Panel - Internals", SpriteSortMode.Immediate, BlendState.AlphaBlend,
					null, null, _rasterizerState);


                //First Draw The Background
                DrawPanelBackground(panel);


                //Copy the current scissor rect so we can restore it after
                Rectangle OriginalScissorRectangle = Engine.SpriteBatch.GraphicsDevice.ScissorRectangle;

                //Set the current scissor rectangle
                Engine.SpriteBatch.GraphicsDevice.ScissorRectangle = ScissorRectangle;

                // Now draw the panel internals.
                DrawPanelInternals(panel);


				// Now end this special sprite batch
				Engine.SpriteBatch.End();
				
				//Reset scissor rectangle to the saved value
                Engine.SpriteBatch.GraphicsDevice.ScissorRectangle = OriginalScissorRectangle;

				// Finally, restart the base Sprite Batch
				Engine.SpriteBatch.Begin("UI - Post Scroll Panel");
			}
			else
			{
                // Do nothing otherwise
			}
        }

		/// <summary>
		/// Draws the panel background. Override to modify.
		/// </summary>
		/// <param name="panel">Panel.</param>
		public virtual void DrawPanelBackground(vxScrollPanel panel)
		{
            Engine.SpriteBatch.Draw(BackgroundImage, panel.Bounds, Theme.Background.Color * Alpha);
        }

        /// <summary>
        /// Draws the panel internals. Override to modify.
        /// </summary>
        /// <param name="panel">Panel.</param>
        public virtual void DrawPanelInternals(vxScrollPanel panel)
		{
			//Then draw the scroll bar
			panel.ScrollBar.Draw();


			//use for loops, items can be removed while rendereing through the
			//loop. This is generally an issue during networking games when a
			//signal is recieved to remove an item while it's already rendering.
			for (int i = 0; i < panel.Items.Count; i++)
			{
				panel.Items[i].Draw();
			}


            //use for loops, items can be removed while rendereing through the
            //loop. This is generally an issue during networking games when a
            //signal is recieved to remove an item while it's already rendering.
            for (int i = 0; i < panel.Items.Count; i++)
            {
                panel.Items[i].DrawText();
            }

            if (panel.DoBorder)
                panel.DrawBorder();
        }
    }
}
