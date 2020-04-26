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
using VerticesEngine.Mathematics;

namespace VerticesEngine.UI.Controls
{
    

    /// <summary>
    /// Basic Button GUI Control.
    /// </summary>
    public class vxNotification : vxGUIControlBaseClass
    {

        /// <summary>
        /// Gets or sets the texture for this Menu Entry Background.
        /// </summary>
        /// <value>The texture.</value>
        public Texture2D BackgroundTexture;

		/// <summary>
		/// The icon for this button.
		/// </summary>
		public Texture2D Icon;



		/// <summary>
		/// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxButtonControl"/> class.
		/// </summary>
		/// <param name="Engine">The Vertices Engine.</param>
		/// <param name="Text">BUtton Text.</param>
		/// <param name="position">Button Position.</param>
        public vxNotification(vxEngine Engine, string Text, Color state)
            : base(Engine, Vector2.Zero)
        {
            //Text
            this.Text = Text;

            //Engine
            this.Engine = Engine;

            //Set up Font
            Font = vxGUITheme.Fonts.Font;

            DoBorder = true;

            this.state = state;
		}

        Color state;

        int count = 0;

        float Offset = 0;

        float ReqOffset = 1;

        public int TimeShown = 120;

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public override void Draw()
        {
            //Now get the Art Provider to draw the scene

            base.Draw();

            Height = 32;
            int border = 15;

            count++;

            if (count > TimeShown)
                ReqOffset = -1;

            //if (Offset < 0.5f)
                //UIManager.Remove(this);

            Offset = vxMathHelper.Smooth(Offset, ReqOffset, 8);

            if (vxNotificationManager.IsOnBottom)
            {

                Bounds = new Rectangle(Viewport.Width * 10 / border,
                                      Viewport.Height - Height,
                                       Viewport.Width / (100 - border * 2),
                                       Height);


                Bounds = new Rectangle(0,
                                       (int)(Viewport.Height - Height * Offset),
                                       Viewport.Width,
                                       Height);
            }
            else
            {


                Bounds = new Rectangle(0,
                                       (int)(Height * (Offset - 1)),
                                       Viewport.Width,
                                       Height);
            }

            // draw base
            SpriteBatch.Draw(DefaultTexture, Bounds, new Color(new Vector3(0.15f)));
            SpriteBatch.Draw(DefaultTexture, new Rectangle(Bounds.Location + new Point(4), new Point(24)), state);
            SpriteBatch.DrawString(Font, Text, Bounds.Location.ToVector2()+new Vector2(32,5), Color.White);
        }
    }
}
