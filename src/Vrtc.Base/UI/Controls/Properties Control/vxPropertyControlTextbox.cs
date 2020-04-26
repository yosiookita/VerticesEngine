using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Events;
using Microsoft.Xna.Framework.Audio;
using VerticesEngine.UI.Themes;
using System.Collections.Generic;
using VerticesEngine.Input;

namespace VerticesEngine.UI.Controls
{
    /// <summary>
    /// Property item group.
    /// </summary>
    public class vxPropertyControlTextbox : vxTextbox
    {
        vxPropertyItemBaseClass Property;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxPropertyTextbox"/> class.
        /// </summary>
        /// <param name="Property">Property.</param>
        /// <param name="Text">Text.</param>
		public vxPropertyControlTextbox(vxPropertyItemBaseClass Property, string Text) :
        base(Property.Engine, Text, Vector2.Zero)
        {
            Padding = new Vector2(0, 1);
            this.Property = Property;
            //Set Text
            this.Text = Text;

            this.IsEnabled = Property.IsReadOnly;

            Font = Property.Font;

            Height = Property.GetHeight();
            Width = 3000;


            Theme.Text = new vxColourTheme(Color.White * 0.75f, Color.White, Color.Black);
        }

        public override SpriteFont GetDefaultFont()
        {
            return vxGUITheme.Fonts.FontSmall;
        }

        public override void Update()
        {
            IsUpdateable = false;

            base.Update();
            if (IsSelected)
            {
                if (vxInput.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    string text = Property.PropertyBox.Text;
                    float result;
                    if (float.TryParse(text, out result))
                    {
                        Property.SetValue(result);
                        IsSelected = false;
                    }

                }
            }
        }


        // Don't draw the background
        public override void Draw()
        {
            //base.DrawText();
            Engine.SpriteBatch.DrawString(Font, DisplayText, Position,
                                          (GetStateColour(Theme.Text)) * TransitionAlpha, vxLayout.Scale);

            //Draw Caret Seperately
            if (IsEnabled && CaretAlpha > 0)
                Engine.SpriteBatch.DrawString(Font, "I", CaretPosition, Color.Black * CaretAlpha, vxLayout.Scale);
        }
    }


    public class PropertiesTextboxArtProvider : vxTextboxArtProvider
    {
        public override string Caret
        {
            get { return "I"; }
        }

        public PropertiesTextboxArtProvider(vxEngine Engine) : base(Engine)
        {
            DefaultWidth = 150;
            DefaultHeight = 24;

            DoBorder = true;
            BorderWidth = 2;

            //TextColour = Color.Black;
            //Theme.Background.NormalColour = ChaoticUI.Colors.BlueprintBlue * 1.05f;
            //Theme.Background.HoverColour = ChaoticUI.Colors.BlueprintBlue * 1.2f;
            //Theme.Background.SelectedColour = ChaoticUI.Colors.BlueprintBlue * 1.52f;

            Theme.Text.DisabledColour = Color.WhiteSmoke * 0.75f;
            Theme.Text.NormalColour = Color.WhiteSmoke;
            Theme.Text.HoverColour = Color.WhiteSmoke;
            Theme.Text.SelectedColour = Color.Black;

            //Theme.Border = new vxColourTheme(ChaoticUI.Colors.MinorConstructionLine,
            //                                 ChaoticUI.Colors.MajorConstructionLine,
            //                                 Color.Black);
            //Font = vxGUITheme.Fonts.FontTiny;
            BackgroundImage = DefaultTexture;
            Padding = new Vector2(5);
        }

        public override SpriteFont GetFont()
        {
            return vxGUITheme.Fonts.FontSmall;
        }

        public override void Draw(object guiItem)
        {
            vxTextbox textbox = (vxTextbox)guiItem;
            Theme.SetState(textbox);

            //Font = vxGUITheme.Fonts.Font;

            //DisplayText = textbox.Text;
            //First Set the Bounding Rectangle for the Textbox
            Bounds = new Rectangle(
                (int)(textbox.Position.X - Padding.X),
                (int)(textbox.Position.Y - Padding.Y / 2),
                (int)(textbox.Width + Padding.X),
                (int)(textbox.Height + Padding.Y));

            // Now set the Height for a Multiline Box
            //if (textbox.IsMultiLine)
            Bounds.Height = (int)(Font.LineSpacing + Padding.Y / 2);// Math.Max((int)(Font.LineSpacing + Padding.Y / 2), textbox.Height);
                                                                    //else
                                                                    //Bounds.Height = (int)(Font.LineSpacing + Padding.Y);

            // Now set the Border Rectangle.
            Rectangle BackRectangle = Bounds.GetBorder(1);

            // Finally, set the Text position based on the mid of the text box.
            //Vector2 TextPosition = Bounds.Location.ToVector2() + new Vector2(0, Bounds.Height / 2 - Font.LineSpacing / 2);

            //// If it's multiline, then set it based on the position and padding.
            ////if (textbox.IsMultiLine)
            //TextPosition = textbox.Position + textbox.TextPositionOffset;

            try
            {
                //if (textbox.Text != "")
                //    TextPosition.X += vxLayout.GetScaledWidth(Font.MeasureString(textbox.Text.Substring(0, textbox.DisplayTextStart + 1)).X);

                string text = textbox.DisplayText;

                // if the text is longer than the width, then let's shrink it
                while (vxLayout.GetScaledWidth(Font.MeasureString(text).X) > Bounds.Width - Padding.X)
                {
                    text = text.Substring(0, text.Length - 1);
                }

                //Engine.SpriteBatch.DrawString(Font, text, Vector2.One  textbox.Position + Vector2.One * 2, (textbox.HasFocus ? Color.Black * 0.5f : Color.Black * 0.25f) * textbox.TransitionAlpha, vxLayout.Scale);

                Engine.SpriteBatch.DrawString(Font, text, textbox.Position, Theme.Text.Color * textbox.TransitionAlpha, vxLayout.Scale);
            }
            catch
            {

            }

            //Draw Caret Seperately
            //Engine.SpriteBatch.DrawString(Font, Caret, textbox.CaretPosition + Vector2.UnitX * 2 + Vector2.One * 2, Color.Black * 0.5f * textbox.CaretAlpha, vxLayout.Scale);
            Engine.SpriteBatch.DrawString(Font, Caret, textbox.CaretPosition + Vector2.UnitX * 2, Color.White * textbox.CaretAlpha, vxLayout.Scale);
        }
    }
}
