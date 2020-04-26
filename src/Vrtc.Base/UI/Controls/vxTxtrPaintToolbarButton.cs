﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using VerticesEngine.Mathematics;

using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Controls
{
    
	/// <summary>
	/// Toolbar Button Controls which holds an image of the curent Texture Painting Texture.
	/// </summary>
    public class vxTxtrPaintToolbarButton : vxToolbarButton
    {
        vxTerrainManager TerrainManager;

        Texture2D TxtrPaintTexture
        {
            get { return (TerrainManager.Textures[TexturePaintIndex]); }
        }

        public int TexturePaintIndex;

        /// <summary>
		/// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxToolbarButton"/> class.
        /// </summary>
        /// <param name="vxEngine">The current Vertices vxEngine Instance</param>
		/// <param name="Content">Content Manager too load the Textures with.</param>
        /// <param name="TexturesPath">Path to the textures, note a 'hover texture' must be present with a '_hover' suffix</param>
		public vxTxtrPaintToolbarButton(vxEngine Engine, vxTerrainManager TerrainManager, int TexturePaintIndex):
		base(Engine, "")
        {
            //Get the current Game vxEngine
            this.Engine = Engine;

            this.TerrainManager = TerrainManager;

            this.TexturePaintIndex = TexturePaintIndex;

            //Position is Set by Toolbar
            Position = Vector2.Zero;

            IsTogglable = true;

            //Set Button Images
			ButtonImage = vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/tlbr/terrain/txtrpaint_overlay");

            //Set Initial Bounding Rectangle
            Width = 48;
            Height = Width;// ButtonImage.Height;
            Bounds = new Rectangle(0, 0, Width, Height);

            SetToolTip("Set Terrain Texture Painting to this Texture.");
		}


        Color ColorState(Color NormalCol, Color HighLiteCol, Color SelectedCol)
        {
            if (IsEnabled == false)
                return Color.Gray * 0.5f;

            // Has Focus, but is not Toggled
            else if (HasFocus && (IsTogglable && ToggleState) == false)
                return HighLiteCol;

            // Regardless If it's Toggled
            else if ((IsTogglable && ToggleState))
                return SelectedCol;

            // Meets no criteria, then just return the normal colour.
            else
                return NormalCol;
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		public override void Draw()
        {
            HoverAlpha = vxMathHelper.Smooth(HoverAlpha, HoverAlphaReq, HoverAlphaDeltaSpeed);
            
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds.GetBorder(-7), ColorState(Color.Black, Color.DeepSkyBlue, Color.Orange));
            Engine.SpriteBatch.Draw(TxtrPaintTexture, Bounds.GetBorder(-9), (IsEnabled ? Color.White : Color.Gray));
            Engine.SpriteBatch.Draw(ButtonImage, Bounds, ColorState(Color.White, Color.SkyBlue, Color.Orange));
            base.DrawToolTip();

        }
    }
}