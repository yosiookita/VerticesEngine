using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using VerticesEngine.Localization;
using VerticesEngine.Utilities;
using VerticesEngine;

namespace VerticesEngine.Input
{
    /// <summary>
    /// An enum of all available mouse buttons.
    /// </summary>
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    }

    public enum KeyboardTypes
    {
        QWERTY,
        AZERTY,
        CUSTOM
    }

    /// <summary>
    /// The central Input Manager Class for the Vertices Engine. This handles all types of input, from Keyboard, Mouse, GamePad
    /// touch and gesture support.
    /// </summary>
    public static class vxInput
    {
        #region -- Settings --


        public static class MouseSettings
        {
            [vxInputSettingsAttribute("Mouse.Sensitivity")]
            public static float Sensitivity = 1.0f;

            [vxInputSettingsAttribute("Mouse.IsXInverted")]
            public static bool IsXInverted = false;


            [vxInputSettingsAttribute("Mouse.IsYInverted")]
            public static bool IsYInverted = false;
        }


        public static class GamePadSettings
        {
            //Controls
            [vxInputSettingsAttribute("GamePad.Sensitivity")]
            public static float Sensitivity = 1.0f;

            [vxInputSettingsAttribute("GamePad.IsXInverted")]
            public static bool IsXInverted = false;


            [vxInputSettingsAttribute("GamePad.IsYInverted")]
            public static bool IsYInverted = false;
        }


        public static class KeyboardSettings
        {
            [vxInputSettingsAttribute("Key.Movement.Forwards")]
            public static Keys Forward = Keys.W;

            [vxInputSettingsAttribute("Key.Movement.Backwards")]
            public static Keys Backwards = Keys.S;

            [vxInputSettingsAttribute("Key.Movement.Left")]
            public static Keys Left = Keys.A;

            [vxInputSettingsAttribute("Key.Movement.Right")]
            public static Keys Right = Keys.D;

            [vxInputSettingsAttribute("Key.Movement.Jump")]
            public static Keys Jump = Keys.Space;

            [vxInputSettingsAttribute("Key.Movement.Crouch")]
            public static Keys Crouch = Keys.LeftControl;

        }
        #endregion


        private static readonly List<GestureSample> _gestures = new List<GestureSample>();

        private static bool _handleVirtualStick;

        /// <summary>
        /// Gets or sets a value indicating whether the custom cusor in <see cref="T:VerticesEngine.Input.vxvxInput"/> is
        /// visible.
        /// </summary>
        /// <value><c>true</c> if is cusor visible; otherwise, <c>false</c>.</value>
        public static bool IsCursorVisible;

        /// <summary>
        /// Gets or sets the cursor sprite.
        /// </summary>
        /// <value>The cursor sprite.</value>
        public static Texture2D CursorSprite;


        public static Texture2D CursorSpriteClicked;

        /// <summary>
        /// The cursor sprite scale.
        /// </summary>
        public static float CursorSpriteScale = 1;

        /// <summary>
        /// The cursors pixel. This is mainly used for GUI interesection.
        /// </summary>
        public static Rectangle CursorPixel = new Rectangle(0, 0, 1, 1);

        /// <summary>
        /// Gets or sets the cursor rotation.
        /// </summary>
        /// <value>The cursor rotation.</value>
        public static float CursorSpriteRotation = 0;

        /// <summary>
        /// The cursor sprite colour.
        /// </summary>
        public static Color CursorSpriteColour = Color.White;

        /// <summary>
        /// The cursor sprite colour on click.
        /// </summary>
        public static Color CursorSpriteColourOnClick = Color.White;


        /// <summary>
        /// Does the cursor rotate
        /// </summary>
        public static bool DoCursorSpriteRotation = false;

#if WINDOWS_PHONE
		private VirtualStick _phoneStick;
		private VirtualButton _phoneA;
		private VirtualButton _phoneB;
#endif

        private static vxEngine _engine;
        private static Viewport _viewport;


        /// <summary>
        /// Gets the state of the game pad for Player One.
        /// </summary>
        /// <value>The state of the game pad.</value>
        public static GamePadState GamePadState
        {
            get { return GamePadStates[0]; }
        }


        /// <summary>
        /// A collection of game pad states based off of how many players are allowed for this specific game.
        /// </summary>
        public static List<GamePadState> GamePadStates = new List<GamePadState>();

        public static List<GamePadState> PreviousGamePadStates = new List<GamePadState>();

        public static vxKeyBindings KeyBindings;

        public static KeyboardState KeyboardState { get; private set; }

        public static TouchCollection TouchCollection { get; private set; }

        public static TouchCollection PreviousTouchCollection { get; private set; }

        public static MouseState MouseState;

        public static GamePadState VirtualState { get; private set; }


        /// <summary>
        /// Gets the state of the previous game pad state for Player One.
        /// </summary>
        /// <value>The state of the previous game pad.</value>
        public static GamePadState GetPreviousGamePadState()
        {
            return PreviousGamePadStates[0]; 
        }

        public static KeyboardState PreviousKeyboardState { get; private set; }

        public static MouseState PreviousMouseState { get; private set; }

        /// <summary>
        /// Gets the Change in Scroll wheel position since the last update
        /// </summary>
        public static int ScrollWheelDelta { get; private set; }
        private static int PreviousScrollWheel;

        public static GamePadState PreviousVirtualState { get; private set; }

        //public bool ShowCursor
        //{
        //	get { return _cursorIsVisible; }
        //	set { _cursorIsVisible = value; }
        //}

        public static bool EnableVirtualStick;


        /// <summary>
        /// Gets or sets the cursor position.
        /// </summary>
        /// <value>The cursor.</value>
        public static Vector2 Cursor
        {
            get { return _cursor; }
            set
            {
                _cursor = value;

                // set mouse position
                Mouse.SetPosition((int)_cursor.X, (int)_cursor.Y);
            }
        }
        static Vector2 _cursor;

        static public Vector2 PreviousCursor;

        /// <summary>
        /// The mouse click position.
        /// </summary>
        public static Vector2 MouseClickPos = new Vector2();

        public static bool IsCursorMoved { get; private set; }

        public static bool IsCursorValid { get; private set; }

        public static int NumberOfGamePads = 1;

        /// <summary>
        ///   Constructs a new input state.
        /// </summary>
        internal static void Init(vxEngine Engine)
        {
            KeyBindings = new vxKeyBindings(Engine);

            KeyboardState = new KeyboardState();
            //GamePadState = new GamePadState();
            MouseState = new MouseState();
            VirtualState = new GamePadState();


            PreviousKeyboardState = new KeyboardState();
            //PreviousGamePadState = new GamePadState();
            PreviousMouseState = new MouseState();
            PreviousVirtualState = new GamePadState();

            _engine = Engine;


            // Handle Cursor Abilities
            IsCursorVisible = false;
            IsCursorMoved = false;

#if __MOBILE__
            IsCursorValid = false;
#else
			IsCursorValid = true;
#endif
            _cursor = Vector2.Zero;

            _handleVirtualStick = false;

            NumberOfGamePads = (int)MathHelper.Clamp(Engine.Game.Config.MaxNumberOfPlayers, 1, 4);

            for (int i = 0; i < NumberOfGamePads; i++)
            {
                GamePadStates.Add(new GamePadState());
                PreviousGamePadStates.Add(new GamePadState());
            }
        
            CursorSprite = vxInternalAssets.LoadInternalTexture2D("Textures/gui/cursor/Cursor");
            CursorSpriteClicked = vxInternalAssets.LoadInternalTexture2D("Textures/gui/cursor/Cursor");

#if WINDOWS_PHONE
			// virtual stick content
			_phoneStick = new VirtualStick(_manager.Content.Load<Texture2D>("Common/socket"),
			_manager.Content.Load<Texture2D>("Common/stick"), new Vector2(80f, 400f));

			Texture2D temp = _manager.Content.Load<Texture2D>("Common/buttons");
			_phoneA = new VirtualButton(temp, new Vector2(695f, 380f), new Rectangle(0, 0, 40, 40), new Rectangle(0, 40, 40, 40));
			_phoneB = new VirtualButton(temp, new Vector2(745f, 360f), new Rectangle(40, 0, 40, 40), new Rectangle(40, 40, 40, 40));
#endif
            _viewport = _engine.GraphicsDevice.Viewport;
            TouchPanel.EnabledGestures = GestureType.Tap;

            PreviousScrollWheel = MouseState.ScrollWheelValue;
            IsCursorVisible = true;
        }

#if __MOBILE__
        static Vector2 pos1;
        static Vector2 pos2;

        //static Vector2 pos1_strt;
        //static Vector2 pos2_strt;

        // Pinch Zoom Variables
        //static float initialZoom = 0;

        static float initialPinchDist = 0;

        static bool pinchBegin = true;


        // Touch Panning Variables
        //static bool panBegin = true;
        static Vector2 previousPan = Vector2.Zero;

        //static bool _isNewTouchDown = false;

        //static bool _isNewTouchRelease = false;
#endif

        /// <summary>
        /// The initial mouse down position.
        /// </summary>
        public static Point MouseDownPosition;


        public static Vector2 DragDistance
        {
            get { return _dragDistance; }
        }
        static Vector2 _dragDistance = Vector2.Zero;

        /// <summary>
        ///   Reads the latest state of the keyboard and gamepad and mouse/touchpad.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            // Save Previous Input States
            // ***********************************************************
            PreviousKeyboardState = KeyboardState;
            PreviousMouseState = MouseState;
            PreviousCursor = Cursor;
            PreviousTouchCollection = TouchCollection;
            for (int i = 0; i < NumberOfGamePads; i++)
                PreviousGamePadStates[i] = GamePadStates[i];

            if (_handleVirtualStick)
                PreviousVirtualState = VirtualState;


            // Now Get Current Input States
            // ***********************************************************
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            TouchCollection = TouchPanel.GetState();
            for (int i = 0; i < NumberOfGamePads; i++)
                GamePadStates[i] = GamePad.GetState((PlayerIndex)i);




#if __MOBILE__
            // Handle Touch Controls. This is because not every platform reliablly fires TouchState.Pressed. 
            // So instead, cound the touch collection between frames.
            //_isNewTouchDown = false;
            //_isNewTouchRelease = false;

            //if (PreviousTouchCollection.Count == 0 && TouchCollection.Count > 0)
            //    _isNewTouchDown = true;
            //else if (PreviousTouchCollection.Count > 0 && TouchCollection.Count == 0)
            //    _isNewTouchRelease = true;
#endif

            if (IsNewMouseButtonPress(MouseButtons.LeftButton) || MouseState.LeftButton == ButtonState.Released)
            {
                MouseDownPosition = MouseState.Position;
            }

            if (_handleVirtualStick)
            {
#if XBOX
				VirtualState = GamePad.GetState(PlayerIndex.One);
#elif WINDOWS
				VirtualState = GamePad.GetState(PlayerIndex.One).IsConnected ? GamePad.GetState(PlayerIndex.One) : HandleVirtualStickWin();
#elif WINDOWS_PHONE
				VirtualState = HandleVirtualStickWP7();
#endif
            }

            _gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                _gestures.Add(TouchPanel.ReadGesture());
            }

            // Update cursor
            Vector2 oldCursor = Cursor;
            if (GamePadState.IsConnected && GamePadState.ThumbSticks.Left != Vector2.Zero)
            {
                Vector2 temp = GamePadState.ThumbSticks.Left;
                _cursor += temp * new Vector2(300f, -300f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Mouse.SetPosition((int)Cursor.X, (int)Cursor.Y);
            }
#if __IOS__ || __ANDROID__
            else
            {
                if (TouchCollection.Count > 0)
                {
                    //Only Fire Select Once it's been released
                    if (TouchCollection.Count == 1)
                    {
                        if (TouchCollection[0].State == TouchLocationState.Moved || TouchCollection[0].State == TouchLocationState.Pressed)
                        {
                            Cursor = TouchCollection[0].Position;
                        }
                    }

                    // Handle Pinch Zoom and Panning
                    if (TouchCollection.Count == 2)
                    {
                        // First get the currnet location of the touch positions
                        pos1 = TouchCollection[0].Position;
                        pos2 = TouchCollection[1].Position;

                        // if its the first loop with this pinch, then set the initial condisitons
                        if (pinchBegin == true)
                        {
                            pinchBegin = false;
                            initialPinchDist = Vector2.Subtract(pos2, pos1).Length();

                            // Get Average Position for panning
                            previousPan = (pos1 + pos2) / 2;
                            Cursor = previousPan;
                        }

                        // if not, then set the zoom based off of the pinch
                        else
                        {
                            float CamZoomDelta = Vector2.Subtract(pos2, pos1).Length() - initialPinchDist;

                            //_engine.Current2DSceneBase.Camera.Zoom += CamZoomDelta / 750;
                            initialPinchDist = Vector2.Subtract(pos2, pos1).Length();

                            Vector2 CamMov = previousPan - (pos1 + pos2) / 2;
                            
                            if (_engine.CurrentScene is vxGameplayScene2D)
                            {
                                ((vxGameplayScene2D)_engine.CurrentScene).Cameras[0].Zoom += CamZoomDelta / 750;
                                ((vxCamera2D)(_engine.CurrentScene).Cameras[0]).MoveCamera(CamMov / 20);
                            }
                            
                            //_engine.Current2DSceneBase.Camera.MoveCamera(CamMov / 20);
                            previousPan = (pos1 + pos2) / 2;
                            Cursor = previousPan;
                        }
                    }
                    else
                    {
                        pinchBegin = true;
                    }
                }
            }

#else
			else
			{
                _cursor = new Vector2(MouseState.X, MouseState.Y);// - _manager.Game.Window.Position.ToVector2();
			}
#endif



            // Clamp The Cursor Position
            _cursor = new Vector2(
                MathHelper.Clamp(_cursor.X, 0f, _engine.GraphicsDevice.Viewport.Width),
                MathHelper.Clamp(_cursor.Y, 0f, _engine.GraphicsDevice.Viewport.Height)
            );

            if (IsCursorValid && oldCursor != Cursor)
                IsCursorMoved = true;
            else
                IsCursorMoved = false;

            //#if WINDOWS
            IsCursorValid = _viewport.Bounds.Contains(MouseState.X, MouseState.Y);
#if __MOBILE__
            IsCursorValid = MouseState.LeftButton == ButtonState.Pressed;
#endif
            if (DoCursorSpriteRotation)
                CursorSpriteRotation += 0.0167f;

            CursorPixel.Location = Cursor.ToPoint();

            ScrollWheelDelta = MouseState.ScrollWheelValue - PreviousScrollWheel;
            PreviousScrollWheel = MouseState.ScrollWheelValue;



            // Is it the First Down?
            if (IsNewMainInputDown())
            {
                InitialDownLoc = Cursor;
                IsDragging = false;
            }

            else if (IsNewMainInputUp())
            {
                //IsDragging = false;
            }

            if (IsMainInputDown() && IsDragging == false)
            {
                float dif = (Cursor - InitialDownLoc).Length();
                if (dif > 5)
                    IsDragging = true;
            }

            if (upCount > 30)
            {
                IsInit = true;
            }
            else
            {
                upCount++;
            }

            // Add The Difference to the Drag Vector
            //if (PreviousTouchCollection.Count > 0 && TouchCollection.Count > 0)
            //    _dragDistance += PreviousTouchCollection[0].Position - TouchCollection[0].Position;
            //else
                //_dragDistance = Vector2.Zero;

            _dragDistance = (Cursor - InitialDownLoc);
        }
        static int upCount = 0;

        /// <summary>
        /// Input is Reinitialised at the start of each scene.
        /// </summary>
        public static void InitScene()
        {
            IsInit = false;
            upCount = 0;
        }

        // 
        public static bool IsInit = false;

        /// <summary>
        /// Is the Mouse Dragging. If yes, then don't let the GUI take input.
        /// </summary>
        public static bool IsDragging = false;

        static Vector2 InitialDownLoc = new Vector2();

        /// <summary>
        /// Whether or not the engine is initialised enough to draw the cursor. The default is this isn't set to true
        /// until after the global LoadAssets screen is called.
        /// </summary>
        public static bool IsCusorInitialised = false;

        public static bool DoBoxSelect = false;

        public static bool IsNewLeftMouseClick
        {
            get { return (IsNewMouseButtonPress(MouseButtons.LeftButton) && !DoBoxSelect); }
        }

        /// <summary>
        /// Draw this instance.
        /// </summary>
        internal static void Draw()
        {
            //_cursorIsVisible = true;
            if (IsCursorVisible && IsCusorInitialised == true)
            {
                if (MouseState.LeftButton == ButtonState.Pressed && DoBoxSelect)
                {
                    Rectangle MouseBoxSelection = new Rectangle(
                    MouseDownPosition,
                        MouseState.Position - MouseDownPosition);

                    _engine.SpriteBatch.Draw(
                    vxInternalAssets.Textures.Blank,
                    MouseBoxSelection,
                        Color.DeepSkyBlue * 0.5f);
                }
                _engine.SpriteBatch.Draw(
                        MouseState.LeftButton == ButtonState.Pressed ? CursorSpriteClicked : CursorSprite,
                    Cursor,
                    null,
                    MouseState.LeftButton == ButtonState.Pressed ? CursorSpriteColourOnClick : CursorSpriteColour,
                    CursorSpriteRotation, 
                    new Vector2(CursorSprite.Width / 2, CursorSprite.Height / 2),
                    CursorSpriteScale,
                    SpriteEffects.None,
                    0f);

                //_engine.SpriteBatch.End();

            }
#if WINDOWS_PHONE
			if (_handleVirtualStick)
			{
			_manager.SpriteBatch.Begin();
			_phoneA.Draw(_manager.SpriteBatch);
			_phoneB.Draw(_manager.SpriteBatch);
			_phoneStick.Draw(_manager.SpriteBatch);
			_manager.SpriteBatch.End();
			}
#endif
        }



        private static GamePadState HandleVirtualStickWin()
        {
            Vector2 leftStick = Vector2.Zero;
            List<Buttons> buttons = new List<Buttons>();

            if (KeyboardState.IsKeyDown(Keys.A))
                leftStick.X -= 1f;
            if (KeyboardState.IsKeyDown(Keys.S))
                leftStick.Y -= 1f;
            if (KeyboardState.IsKeyDown(Keys.D))
                leftStick.X += 1f;
            if (KeyboardState.IsKeyDown(Keys.W))
                leftStick.Y += 1f;
            if (KeyboardState.IsKeyDown(Keys.Space))
                buttons.Add(Buttons.A);
            if (KeyboardState.IsKeyDown(Keys.LeftControl))
                buttons.Add(Buttons.B);
            if (leftStick != Vector2.Zero)
                leftStick.Normalize();

            //return new GamePadState(leftStick, Vector2.Zero, 0f, 0f, buttons.ToArray());
            return new GamePadState();
        }

        private static GamePadState HandleVirtualStickWP7()
        {
            List<Buttons> buttons = new List<Buttons>();
            Vector2 stick = Vector2.Zero;

#if WINDOWS_PHONE
			_phoneA.Pressed = false;
			_phoneB.Pressed = false;
			TouchCollection touchLocations = TouchPanel.GetState();
			foreach (TouchLocation touchLocation in touchLocations)
			{
			_phoneA.Update(touchLocation);
			_phoneB.Update(touchLocation);
			_phoneStick.Update(touchLocation);
			}
			if (_phoneA.Pressed)
			{
			buttons.Add(Buttons.A);
			}
			if (_phoneB.Pressed)
			{
			buttons.Add(Buttons.B);
			}
			stick = _phoneStick.StickPosition;
#endif
            //return new GamePadState(stick, Vector2.Zero, 0f, 0f, buttons.ToArray());
            return new GamePadState();
        }

        /// <summary>
        ///   Helper for checking if a key was newly pressed during this update.
        /// </summary>
        public static bool IsNewKeyPress(Keys key)
        {
            return (KeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key));
        }

        public static bool IsNewKeyRelease(Keys key)
        {
            return (PreviousKeyboardState.IsKeyDown(key) && KeyboardState.IsKeyUp(key));
        }

        public static bool IsKeyDown(Keys key)
        {
            return (KeyboardState.IsKeyDown(key));
        }


        /// <summary>
        /// Returns the boolean of whether of GamePad Button has been newly Pressed. The default is for Player One, but you can pass the PlayerIndex as an 
        /// optional Variable.
        /// </summary>
        /// <returns><c>true</c>, if new button release was used, <c>false</c> otherwise.</returns>
        /// <param name="button">Button to check if it's newly been pressed.</param>
        /// <param name="PlayerIndex">Player index.</param>
        public static bool IsNewButtonPressed(Buttons button, PlayerIndex PlayerIndex = PlayerIndex.One)
        {
            return (GamePadStates[(int)PlayerIndex].IsButtonDown(button) && PreviousGamePadStates[(int)PlayerIndex].IsButtonUp(button));
        }


        /// <summary>
        /// Returns the boolean of whether of GamePad Button has been newly Released. The default is for Player One, but you can pass the PlayerIndex as an 
        /// optional Variable.
        /// </summary>
        /// <returns><c>true</c>, if new button release was used, <c>false</c> otherwise.</returns>
        /// <param name="button">Button to check if it's newly been pressed.</param>
        /// <param name="PlayerIndex">Player index.</param>
        public static bool IsNewButtonReleased(Buttons button, PlayerIndex PlayerIndex = PlayerIndex.One)
        {
            return (PreviousGamePadStates[(int)PlayerIndex].IsButtonDown(button) && GamePadStates[(int)PlayerIndex].IsButtonUp(button));
        }


        /// <summary>
        /// Returns the boolean of whether of GamePad Button is Pressed. The default is for Player One, but you can pass the PlayerIndex as an 
        /// optional Variable.
        /// </summary>
        /// <returns><c>true</c>, if the button press was used, <c>false</c> otherwise.</returns>
        /// <param name="button">Button to check if it's pressed.</param>
        /// <param name="PlayerIndex">Player index.</param>
        public static bool IsButtonPressed(Buttons button, PlayerIndex PlayerIndex = PlayerIndex.One)
        {
            return GamePadStates[(int)PlayerIndex].IsButtonDown(button);
        }


        /// <summary>
        /// Returns the boolean of whether of GamePad Button is Released. The default is for Player One, but you can pass the PlayerIndex as an 
        /// optional Variable.
        /// </summary>
        /// <returns><c>true</c>, if the button release was used, <c>false</c> otherwise.</returns>
        /// <param name="button">Button to check if it's released.</param>
        /// <param name="PlayerIndex">Player index.</param>
        public static bool IsButtonReleased(Buttons button, PlayerIndex PlayerIndex = PlayerIndex.One)
        {
            return GamePadStates[(int)PlayerIndex].IsButtonUp(button);
        }

        /// <summary>
        ///   Helper for checking if a mouse button was newly pressed during this update.
        /// </summary>
        public static bool IsNewMouseButtonPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (MouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (MouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (MouseState.MiddleButton == ButtonState.Pressed && PreviousMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (MouseState.XButton1 == ButtonState.Pressed && PreviousMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (MouseState.XButton2 == ButtonState.Pressed && PreviousMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }


        /// <summary>
        /// Checks if the requested mouse button is released.
        /// </summary>
        /// <param name="button">The button.</param>
        public static bool IsNewMouseButtonRelease(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (PreviousMouseState.LeftButton == ButtonState.Pressed && MouseState.LeftButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (PreviousMouseState.RightButton == ButtonState.Pressed && MouseState.RightButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (PreviousMouseState.MiddleButton == ButtonState.Pressed && MouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (PreviousMouseState.XButton1 == ButtonState.Pressed && MouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (PreviousMouseState.XButton2 == ButtonState.Pressed && MouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Ises the mouse button pressed.
        /// </summary>
        /// <returns><c>true</c>, if mouse button pressed was ised, <c>false</c> otherwise.</returns>
        /// <param name="button">Button.</param>
        public static bool IsMouseButtonPressed(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (MouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (MouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (MouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.ExtraButton1:
                    return (MouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.ExtraButton2:
                    return (MouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is a New Screen Touch pressed
        /// </summary>
        /// <returns><c>true</c>, if new touch pressed was ised, <c>false</c> otherwise.</returns>
        public static bool IsNewTouchPressed()
        {
            return (PreviousTouchCollection.Count == 0 && TouchCollection.Count > 0);
        }

        /// <summary>
        /// Is a New Screen Touch released
        /// </summary>
        /// <returns><c>true</c>, if new touch released was ised, <c>false</c> otherwise.</returns>
        public static bool IsNewTouchReleased()
        {
            if (TouchCollection.Count == 0 && PreviousTouchCollection.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        ///   Checks for a "menu select" input action.
        /// </summary>
        public static bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter) || IsNewButtonPressed(Buttons.A) || IsNewButtonPressed(Buttons.Start) || IsNewMouseButtonPress(MouseButtons.LeftButton) || IsTouchReleased();
        }

        public static bool IsMenuPressed()
        {
            return KeyboardState.IsKeyDown(Keys.Space) || KeyboardState.IsKeyDown(Keys.Enter) || GamePadState.IsButtonDown(Buttons.A) || GamePadState.IsButtonDown(Buttons.Start) || MouseState.LeftButton == ButtonState.Pressed || IsTouchReleased();
        }

        public static bool IsMenuReleased()
        {
            return IsNewKeyRelease(Keys.Space) || IsNewKeyRelease(Keys.Enter) || IsNewButtonReleased(Buttons.A) || IsNewButtonReleased(Buttons.Start) || IsNewMouseButtonRelease(MouseButtons.LeftButton);
        }

        /// <summary>
        /// Is there at least one touch in the touch collection.
        /// </summary>
        /// <returns><c>true</c>, if touch pressed was ised, <c>false</c> otherwise.</returns>
		public static bool IsTouchPressed()
        {
            if (TouchCollection.Count > 0)
            {
                return (TouchCollection[0].State != TouchLocationState.Released);
            }
            else
                return false;
        }

        public static bool IsTouchReleased()
        {
            if (TouchCollection.Count > 0)
            {
                return (TouchCollection[0].State == TouchLocationState.Released);
            }
            else
                return false;
        }

        /// <summary>
        /// Cross Platform Main Input Check. Checks for New Left Click Down, New Button A Press or New Touch Pressed.
        /// </summary>
        /// <returns><c>true</c>, if new main input down was ised, <c>false</c> otherwise.</returns>
        public static bool IsNewMainInputDown()
        {
            return (IsNewButtonPressed(Buttons.A) ||
                    IsNewMouseButtonPress(MouseButtons.LeftButton)) ||
                    IsNewTouchPressed();
            //TouchCollection.Count > 0 && TouchCollection[0].State == TouchLocationState.Pressed;
        }


        /// <summary>
        /// Cross Platform Main Input Check. Checks for Left Click Down, Button A Press or Touch Pressed.
        /// </summary>
        /// <returns><c>true</c>, if main input down was ised, <c>false</c> otherwise.</returns>
        public static bool IsMainInputDown()
        {
            return (IsButtonPressed(Buttons.A) ||
                    MouseState.LeftButton == ButtonState.Pressed ||
                    IsTouchPressed());
            //TouchCollection.Count > 0 && TouchCollection[0].State == TouchLocationState.Pressed;
        }

        /// <summary>
        /// Cross Platform Main Input Check Up. Checks for New Left Click Release, New Button A Release or New Touch Release.
        /// </summary>
        /// <returns><c>true</c>, if new main input down was ised, <c>false</c> otherwise.</returns>
        public static bool IsNewMainInputUp()
        {
            return (IsNewButtonReleased(Buttons.A) ||
                     IsNewMouseButtonRelease(MouseButtons.LeftButton)) ||
                    IsNewTouchReleased();
            //TouchCollection.Count > 0 && TouchCollection[0].State == TouchLocationState.Released);
        }


        public static bool IsMainInputUp()
        {
            return (IsButtonReleased(Buttons.A) ||
                    MouseState.LeftButton == ButtonState.Released ||
                    IsTouchReleased());
            //TouchCollection.Count > 0 && TouchCollection[0].State == TouchLocationState.Pressed;
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public static bool IsMenuUp()
        {
            //PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up) ||
                   IsNewButtonPressed(Buttons.DPadUp) ||
                   IsNewButtonPressed(Buttons.LeftThumbstickUp);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public static bool IsMenuDown()
        {
           // PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down) ||
                   IsNewButtonPressed(Buttons.DPadDown) ||
                   IsNewButtonPressed(Buttons.LeftThumbstickDown);
        }

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public static bool IsPauseGame()
        {
           // PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPressed(Buttons.Back) ||
                   IsNewButtonPressed(Buttons.Start);
        }

        /// <summary>
        ///   Checks for a "menu cancel" input action.
        /// </summary>
        public static bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPressed(Buttons.Back);
        }
    }
}

