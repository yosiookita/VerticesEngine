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
    public class vxToolbarArtProvider : vxArtProviderBase, IGuiArtProvider
	{
        public vxToolbarArtProvider(vxEngine Engine):base(Engine)
		{

            //Theme.Background = new vxColourTheme(Color.White);
		}

		public object Clone()
		{
            return this.MemberwiseClone();
		}

		public virtual void Draw(object guiItem)
		{
            vxToolbar toolbar = (vxToolbar)guiItem;
            //Theme.SetState(toolbar);
            toolbar.Bounds = new Rectangle((int)(toolbar.Position.X), (int)(toolbar.Position.Y), 
                Engine.GraphicsDevice.Viewport.Width, 
                (int)(toolbar.Height + Padding.Y));
            
            //Draw Toolbar
            Engine.SpriteBatch.Draw(DefaultTexture,
                                    toolbar.Bounds, Theme.Background.Color * toolbar.HoverAlpha);
        }
	}
}
