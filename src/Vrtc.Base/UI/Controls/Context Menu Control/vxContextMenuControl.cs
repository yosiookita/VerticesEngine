﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;
using VerticesEngine.UI.Controls;
using System.Collections.Generic;
using VerticesEngine.Input;

namespace VerticesEngine.UI.Dialogs
{
	/// <summary>
	/// File Chooser Dialor Item.
	/// </summary>
	public class vxContextMenuControl : vxPanel
	{
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxContextMenuControl"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        public vxContextMenuControl(vxEngine Engine): base(Engine, Vector2.Zero, 2,2)
		{
			Padding = new Vector2(4);

            PositionChanged += OnPositionChanged;

            Width = 150;
            Height = 24;


            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.Black * 0.5f));

            Hide();
		}



        public vxContextMenuItem AddItem(string text)
        {
            return new vxContextMenuItem(this, text);
        }


        public void AddItem(vxContextMenuItem item)
        {
            Items.Add(item);

            int runningHeight = 0;
            foreach (var itm in Items)
            {
                itm.Position = new Vector2(0, runningHeight);
                itm.OriginalPosition = itm.Position;
                runningHeight += itm.Height;
            }
            Height = runningHeight;

        }

        public void AddSplitter()
        {
            var item = new vxContextMenuSplitter(this);

            Items.Add(item);

            int runningHeight = 0;
            foreach (var itm in Items)
            {
                itm.Position = new Vector2(0, runningHeight);
                itm.OriginalPosition = itm.Position;
                runningHeight += itm.Height;
            }
            Height = runningHeight;
        }

        public void Show()
        {
            Position = vxInput.Cursor;
            IsVisible = true;
            // Now Draw the Controls for this panel.
            foreach (vxGUIControlBaseClass control in Items)
                control.HasFocus = false;
        }

        public void Hide()
        {
            IsVisible = false;


            // Now Draw the Controls for this panel.
            foreach (vxGUIControlBaseClass control in Items)
                control.HasFocus = false;
            
            HasFocus = false;
        }

        public override void Update()
        {
            if (IsVisible)
            {
                base.Update();

                if (vxInput.IsNewLeftMouseClick && HasFocus == false)
                    Hide();
                
            }
        }

        public override void Draw()
        {
            if (IsVisible)
                base.Draw();
        }
	}
}
