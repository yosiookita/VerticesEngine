using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using VerticesEngine.Mathematics;
using VerticesEngine.Diagnostics;
using Microsoft.Xna.Framework.Input.Touch;
//using Microsoft.Xna.Framework.GamerServices;
using VerticesEngine.Utilities;
using VerticesEngine.UI.Themes;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Input;
#if __IOS__ || __ANDROID__
using Microsoft.Xna.Framework.GamerServices;
#endif

namespace VerticesEngine.UI.Controls
{
    enum InputJustification
    {
        All,
        Letters,
        Numerical
    }

    /// <summary>
    /// Textbox Control for us in the vxGUI System.
    /// </summary>
    public class vxTextbox : vxGUIControlBaseClass
    {
        //InputJustification InputJustification;

        /// <summary>
        /// Gets or sets the art provider.
        /// </summary>
        /// <value>The art provider.</value>
        public vxTextboxArtProvider ArtProvider;

        public float Caret_Blink = 0;
        public float ReqCaretAlpha = 0;
        public float CaretAlpha = 0;

        /// <summary>
        /// The Font from the Current Art Provider. This is needed to place the Cursor and handle text wrapping.
        /// </summary>
        new SpriteFont Font
        {
            //set { ArtProvider.Font = value; }
            get
            {
                return GetDefaultFont();
            }
        }

        /// <summary>
        /// Occurs when enabled state changed.
        /// </summary>
        public event EventHandler<EventArgs> TextChanged;

        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <returns>The font.</returns>
        public virtual SpriteFont GetDefaultFont()
        {
            if (ArtProvider != null)
                return ArtProvider.Font;
            else
                return vxInternalAssets.Fonts.BaseFont;
        }


        /// <summary>
        /// The Cursor Position.
        /// </summary>
        public Vector2 CaretPosition;

        /// <summary>
        /// The Index the cursor is in in the string.
        /// </summary>
        public int CaretIndex = 0;


        // Key that pressed last frame.
        private Keys pressedKey;

        // Timer for key repeating.
        private float keyRepeatTimer;

        // Key repeat duration in seconds for the first key press.
        private const float keyRepeatStartDuration = 0.3f;

        // Key repeat duration in seconds after the first key press.
        private const float keyRepeatDuration = 0.03f;

        public string TextboxTitle = "";

        public string TextboxDescription = "";

        public bool IsBackgroundTransparent = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxTextbox"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="text">This GUI Items Text.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="TextboxTitle">Textbox title.</param>
        /// <param name="TextboxDescription">Textbox description.</param>
        public vxTextbox(vxEngine Engine, string text, Vector2 position,
                         string TextboxTitle = "Title", string TextboxDescription = "Description") :
        this(Engine, text, position, TextboxTitle, TextboxDescription, 200)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxTextbox"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="text">This GUI Items Text.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="TextboxTitle">Textbox title.</param>
        /// <param name="TextboxDescription">Textbox description.</param>
        /// <param name="length">Length.</param>
        public vxTextbox(vxEngine Engine, string text, Vector2 position,
                         string TextboxTitle, string TextboxDescription, int width) :
        base(Engine)
        {
            //Set the Engine
            this.Engine = Engine;

            //Set Text
            Text = text;

            //Set Textbox Length
            Width = width;

            this.TextboxTitle = TextboxTitle;
            this.TextboxDescription = TextboxDescription;

            //Set Position
            Position = position;

            //Set Justification
            //InputJustification = InputJustification.All;

            //Have this button get a clone of the current Art Provider
            ArtProvider = (vxTextboxArtProvider)vxGUITheme.ArtProviderForTextboxes.Clone();

            //Font = GetDefaultFont();
            
            CaretIndex = text.Length;
            OnTextChanged();


        }


        public override void Select()
        {
            CaretIndex = Text.Length;

            IsSelected = true;
            HasFocus = true;
        }

        public override void NotHover()
        {
            base.NotHover();
        }

#if __IOS__ || __ANDROID__
        bool IsGuideUp = false;
#endif
        public override void Update()
        {
            MouseState mouseState = vxInput.MouseState;

            Vector2 cursor = vxInput.Cursor;

            //if (cursor.X > Bounds.Left && cursor.X < Bounds.Right &&
            //	cursor.Y < Bounds.Bottom && cursor.Y > Bounds.Top)
            if (Bounds.Contains(cursor))
            {
#if __IOS__ || __ANDROID__
				if (vxInput.TouchCollection.Count > 0)
				{

					//Only Fire Select Once it's been released
					if (vxInput.TouchCollection[0].State == TouchLocationState.Pressed)
						Select();
					//Hover if and only if Moved is selected
					else if (vxInput.TouchCollection[0].State == TouchLocationState.Moved)
						Hover();
				}
#else

                if (mouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released)
                    Select();
                else if (IsSelected == false)
                    Hover();
#endif
            }

#if __IOS__ || __ANDROID__

			else if (vxInput.TouchCollection.Count > 0)
			{
				if (vxInput.TouchCollection[0].State == TouchLocationState.Pressed ||
					 vxInput.TouchCollection[0].State == TouchLocationState.Moved ||
				IsSelected == false)
				{
					NotHover();
				}
			}
#else
            else if (mouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released ||
                IsSelected == false)
            {
                NotHover();
                IsSelected = false;
            }
#endif

            //Set State for next Loop
            //PreviousMouseState = mouseState;

            if (IsSelected == true && IsEnabled)
            {
                Caret_Blink++;

                if (Caret_Blink < 30)
                {
                    ReqCaretAlpha = 1;
                }
                else if (Caret_Blink < 60)
                {
                    ReqCaretAlpha = 0;
                }
                else
                    Caret_Blink = 0;

                // Use the internet system keyboards for Mobile Platforms
#if __IOS__ || __ANDROID__
				if (IsGuideUp == false)
				{
					IsGuideUp = true;
					Guide.BeginShowKeyboardInput(PlayerIndex.One, TextboxTitle, TextboxDescription, Text, HandleTextAsyncCallback, null);

				}
#else
                ProcessKeyInputs(0.0167f);
#endif

            }
            else
            {
                ReqCaretAlpha = 0;
                Caret_Blink = 0;
            }


            CaretAlpha = ReqCaretAlpha;//vxMathHelper.Smooth(CaretAlpha, ReqCaretAlpha, 4);

            //Make sure the cursor index doesn't go past the text length
            CaretIndex = MathHelper.Clamp(CaretIndex, 0, Text.Length);

        }




        public int DisplayTextStart
        {
            get { return _displayTextStart; }
            set
            {
                _displayTextStart = MathHelper.Clamp(value, 0, Text.Length);
            }

        }
        int _displayTextStart = 0;

        /// <summary>
        /// The lines of text.
        /// </summary>
        public string[] Lines;

        /// <summary>
        /// The index of the current line.
        /// </summary>
        public int CurrentLineIndex = 0;

        public string DisplayText
        {
            get { return _displayText; }
        }
        string _displayText;

        /// <summary>
        /// The text position offset.
        /// </summary>
        public Vector2 TextPositionOffset = new Vector2();

        void HandleTextAsyncCallback(IAsyncResult ar)
        {
            //Console.WriteLine(ar.IsCompleted);
            try
            {
#if __IOS__ || __ANDROID__
                string newText = Guide.EndShowKeyboardInput(ar);
                if (newText != null)
                    Text = newText;
                
            IsGuideUp = false;
#endif
            }
            catch { }
            IsSelected = false;
        }

        public bool IsMultiLine = false;

        public override void Draw()
        {
            //Now get the Art Provider to draw the scene
            this.ArtProvider.Draw(this);

            base.Draw();
        }

        public override void OnLayoutInvalidated()
        {
            OnTextChanged();

            base.OnLayoutInvalidated();
        }


        protected void HandleCursorPosition()
        {
            string leftText = _displayText;

            if (ArtProvider != null)
            {
                float cursorHalfWidth = vxLayout.GetScaledWidth(Font.MeasureString(ArtProvider.Caret).X/2);

                // clamp the cursor index
                CaretIndex = MathHelper.Clamp(CaretIndex, 0, Text.Length);

                leftText = Text.Substring(DisplayTextStart, CaretIndex - DisplayTextStart);
                CaretPosition.Y = Position.Y;
                CaretPosition.X = Position.X + vxLayout.GetScaledWidth(Font.MeasureString(leftText).X) - cursorHalfWidth;
            }
        }

        
        /// <summary>
        /// Fired whenerer the text changed (or a key is pressed).
        /// </summary>
        public override void OnTextChanged()
        {
            // Get Cursor Position

            // Fire the Text Change Event
            if (TextChanged != null)
                TextChanged(this, new EventArgs());

            // First set the Display text as the text
            _displayText = Text;

            HandleCursorPosition();


            // first, is the text wider than the width then chop display end values
            if (Width > 0)
            {
                // The Caret is beyond the right side
                if (CaretPosition.X > Bounds.Right)
                {
                    DisplayTextStart += 5;
                }
                else if (CaretPosition.X < Bounds.Left)
                {
                    DisplayTextStart -= 10;
                }

                _displayText = Text.Substring(DisplayTextStart);
                HandleCursorPosition();
                

            }
            /*
            // First set the Display text as the text
            _displayText = Text;
            float wd = Width;


            // first, is the text wider than the width
            if (Font.MeasureString(Text).X > Width)
            {



                int ind = MathHelper.Clamp(CursorIndex - DisplayTextStart, 0, Text.Length); ;
                leftText = Text.Substring(DisplayTextStart, ind);
                CursorPosition.Y = Position.Y;
                CursorPosition.X = Position.X + Font.MeasureString(leftText).X - cursorHalfWidth;

                //DisplayTextStart = MathHelper.Clamp(DisplayTextStart, 0, Text.Length);

                _displayText = Text.Substring(DisplayTextStart, ind);

                int dispLength = Text.Length - DisplayTextStart;
                _displayText = Text.Substring(DisplayTextStart, dispLength);
                int i = 0;
                // now check if the display text is wider than the bounds width
                while(Font.MeasureString(_displayText).X - cursorHalfWidth > Math.Max(Width, 2 * cursorHalfWidth))
                {
                    i++;
                    dispLength = Text.Length - DisplayTextStart - i;
                    dispLength = Math.Max(dispLength, 0);
                    _displayText = Text.Substring(DisplayTextStart, dispLength);
                }
            }
            else
            {
                _displayText = Text;
                DisplayTextStart = 0;
            }
            */
        }

        bool NewKey = false;
        /// <summary>
        /// Hand keyboard input.
        /// </summary>
        /// <param name="dt"></param>
        public void ProcessKeyInputs(float dt)
        {
            NewKey = false;
            KeyboardState keyState = vxInput.KeyboardState;
            Keys[] keys = keyState.GetPressedKeys();

            bool shift = keyState.IsKeyDown(Keys.LeftShift) ||
                keyState.IsKeyDown(Keys.RightShift);

            foreach (Keys key in keys)
            {
                if (!IsKeyPressed(key, dt)) continue;

                char ch;
                if (KeyboardUtils.KeyToString(key, shift, out ch))
                {
                    // Handle typical character input.
                    Text = Text.Insert(CaretIndex, new string(ch, 1));
                    CaretIndex++;
                }
                else
                {
                    switch (key)
                    {
                        case Keys.Back:
                            if (CaretIndex > 0)
                                Text = Text.Remove(--CaretIndex, 1);
                            break;
                        case Keys.Delete:
                            if (CaretIndex < Text.Length)
                                Text = Text.Remove(CaretIndex, 1);
                            break;
                        case Keys.Left:
                            if (CaretIndex > 0)
                                CaretIndex--;
                            break;
                        case Keys.Right:
                            if (CaretIndex < Text.Length)
                                CaretIndex++;
                            break;
                        case Keys.Up:
                            
                            //CaretIndex -= Lines[CurrentLineIndex].Length;
                            break;
                        case Keys.Down:
                            //CaretIndex += Lines[CurrentLineIndex].Length;
                            break;
                    }
                }
                NewKey = true;


            }

            // Raise the 'TextChanged' event if theres any keys pressed, this chatches
            // not only character additions or removals, but also cusor traversal
            if (NewKey)
                OnTextChanged();
        }

        /// <summary>
        /// Pressing check with key repeating.
        /// </summary>
        /// <returns><c>true</c> if this instance is key pressed the specified key dt; otherwise, <c>false</c>.</returns>
        /// <param name="key">Key.</param>
        /// <param name="dt">Dt.</param>
        bool IsKeyPressed(Keys key, float dt)
        {
            // Treat it as pressed if given key has not pressed in previous frame.
            if (vxInput.PreviousKeyboardState.IsKeyUp(key))
            {
                keyRepeatTimer = keyRepeatStartDuration;
                pressedKey = key;
                return true;
            }

            // Handling key repeating if given key has pressed in previous frame.
            if (key == pressedKey)
            {
                keyRepeatTimer -= dt;
                if (keyRepeatTimer <= 0.0f)
                {
                    keyRepeatTimer += keyRepeatDuration;
                    return true;
                }
            }

            return false;
        }
    }
}
