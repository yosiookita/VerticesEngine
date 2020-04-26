using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;

namespace VerticesEngine.UI
{
    /// <summary>
    /// This loads and holds a Font Pack for a number of different font sizes.
    /// It wil hold sizes of 12, 16, 20, 24 and 36. You must have your fonts in the path you
    /// sent to the constructor labeled as 'font_12' 'font_16' etc...
    /// </summary>
    public class vxFontPack
    {
        public SpriteFont Size12;
        public SpriteFont Size16;
        public SpriteFont Size20;
        public SpriteFont Size24;
        public SpriteFont Size36;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MGTemplate.Base.vxFont"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="path">The path to the folder containing the font packs. Files must be written as font_12.xnb, font_16.xnb etc... from 12 to 36</param>
        public vxFontPack(vxEngine Engine, string path)
        {
            Size12 = Engine.Game.Content.Load<SpriteFont>(Path.Combine(path, "font_12"));
            Size16 = Engine.Game.Content.Load<SpriteFont>(Path.Combine(path, "font_16"));
            Size20 = Engine.Game.Content.Load<SpriteFont>(Path.Combine(path, "font_20"));
            Size24 = Engine.Game.Content.Load<SpriteFont>(Path.Combine(path, "font_24"));
            Size36 = Engine.Game.Content.Load<SpriteFont>(Path.Combine(path, "font_36"));
        }
    }
}
