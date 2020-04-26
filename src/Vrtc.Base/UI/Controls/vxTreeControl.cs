using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
    /// <summary>
    /// Basic Button GUI Control.
    /// </summary>
    public class vxTreeControl : vxGUIControlBaseClass
    {

		//public List<vxTreeNode> Items = new List<vxTreeNode>();
		public vxTreeNode RootNode;

		public int RunningY = 0;
		public int RunningX = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxPanel"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		public vxTreeControl(vxEngine Engine, Vector2 Position)
			: base(Engine, Position)
		{
            //Engine
            this.Engine = Engine;

			//this.RootNode = RootNode;

            //Set up Font
            this.Font = vxGUITheme.Fonts.Font;

            Text = "";

            this.Width = Bounds.Width;
            this.Height = Bounds.Height;
		}

		public override void Update()
		{
			base.Update();
			if (RootNode != null)
			{
				RootNode.Position = this.Position + new Vector2(16, 0);
				RootNode.Update();
			}
			RunningY = 0;
		}


		public override void Draw()
		{
			base.Draw();

            // get text
			if(RootNode != null)
			RootNode.Draw();
		}

        public vxTreeNode SelectedItem = null;
        public void SelectItem(vxTreeNode node){

            if(SelectedItem != null){
                SelectedItem.ToggleState = false;
            }

            SelectedItem = node;
        }
    }
}
