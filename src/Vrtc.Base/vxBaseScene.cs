#region File Description
//-----------------------------------------------------------------------------
// vxGameBaseScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using VerticesEngine.Input;
using VerticesEngine.Utilities;


#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using System.IO;
using VerticesEngine.Localization;
using VerticesEngine.Audio.Exceptions;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.UI;
using VerticesEngine.Graphics;
using VerticesEngine.Audio;
#endregion

namespace VerticesEngine
{
    /// <summary>
    /// Enum describes the screen transition state.
    /// </summary>
    public enum ScreenState
    {
		/// <summary>
		/// Transitioning on.
		/// </summary>
        TransitionOn,

		/// <summary>
		/// Screen is active.
		/// </summary>
        Active,

		/// <summary>
		/// Transitioning off.
		/// </summary>
        TransitionOff,

		/// <summary>
		/// The screen is hidden.
		/// </summary>
        Hidden,
    }


    /// <summary>
    /// A scene is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex game and menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as scenes.
    /// </summary>
    public abstract class vxBaseScene : IDisposable
    {
        #region Properties

        /// <summary>
        /// Returns the current Language Pack being used by the Engine.
        /// </summary>
        public vxLanguagePack LanguagePack
        {
            get { return Engine.Language; }
        }

        /// <summary>
        /// This sets the text for this scene. This is handy if you're changing the language during gameplay.
        /// </summary>
        public virtual void OnLocalisationChanged()
        {

        }

        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        public GraphicsDevice GraphicsDevice
        {
            get { return Engine.GraphicsDevice; }
        }


        /// <summary>
        /// Gets the viewport.
        /// </summary>
        /// <value>The viewport.</value>
        public Viewport Viewport
        {
            get { return Engine.GraphicsDevice.Viewport; }
        }


        /// <summary>
        /// Gets the sprite batch.
        /// </summary>
        /// <value>The sprite batch.</value>
        public vxSpriteBatch SpriteBatch
        {
            get { return Engine.SpriteBatch; }
        }



        public bool CanRemoveCompletely = false;

        /// <summary>
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        bool isPopup = false;


        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;


        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;


        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;


        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }


        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenState screenState = ScreenState.TransitionOn;


        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        bool isExiting = false;


        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                       (screenState == ScreenState.TransitionOn ||
                        screenState == ScreenState.Active);
            }
        }

        bool otherScreenHasFocus;


        /// <summary>
        /// Gets the Engine that this screen belongs to.
        /// </summary>
        public vxEngine Engine
        {
            get { return vxEngine.Instance; }
        }


        /// <summary>
        /// Gets a value indicating whether this <see cref="T:VerticesEngine.vxGameBaseScreen"/> is first loop.
        /// </summary>
        /// <value><c>true</c> if is first loop; otherwise, <c>false</c>.</value>
        public bool IsFirstLoop
        {
            get { return _isFirstLoop; }
        }
        bool _isFirstLoop = true;

        /// <summary>
        /// Gets the index of the player who is currently controlling this screen,
        /// or null if it is accepting input from any player. This is used to lock
        /// the game to a specific player profile. The main menu responds to input
        /// from any connected gamepad, but whichever player makes a selection from
        /// this menu is given control over all subsequent screens, so other gamepads
        /// are inactive until the controlling player returns to the main menu.
        /// </summary>
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }

        PlayerIndex? controllingPlayer;

        #endregion

        #region Initialization


        public bool IsInitialised
        {
            get { return isInitialised; }
            set { isInitialised = value; }
        }
        bool isInitialised = false;

        public vxBaseScene()
        {
        }


        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
		public virtual void LoadContent()
        {
            vxInput.InitScene();
        }


        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent()
        {

        }


        #endregion

        #region Update and Draw

        /// <summary>
        /// Firsts the update.
        /// </summary>
        protected virtual void OnFirstUpdate()
        {

        }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (_isFirstLoop)
            {
                _isFirstLoop = false;
                OnFirstUpdate();
            }

            if (isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    vxSceneManager.RemoveScene(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Active;
                }
            }
        }


        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput() { }

        bool _isFirstDraw = true;
        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime) 
        { 

            if (_isFirstDraw)
            {
                _isFirstDraw = false;
                OnFirstDraw(gameTime);
            }
        }

        protected virtual void OnFirstDraw(GameTime gameTime) { }


        #endregion

        #region Public Methods

        /// <summary>
        /// Called when there is a reset or refresh of Graphic settings such as resolution or setting.
        /// </summary>
        public virtual void OnGraphicsRefresh() {

            vxGraphics.InitMainBuffer();
        }

        /// <summary>
        /// Scales an Integer by the Screen Scaler Size. This is used to keep GUI and Item Sizes
        /// consistent across different Screen Sizes and Resolutions.
        /// </summary>
        /// <returns>The scaled size.</returns>
        /// <param name="i">The index.</param>
        public int GetScaledSize(float i)
        {
            return (int)(i * vxLayout.ScaleAvg);
            //return (int)(i * Math.Max(1, ScaleAvg));
        }

        public float GetScalerAvg
        {
            get { return vxLayout.ScaleAvg; }
        }


		public virtual bool PlaySound(vxBaseScene sender, SoundEffect SoundEffect, float Volume = 1, float Pitch = 0)
		{
			try
			{
				// Max the Volume at 1.
				Volume = MathHelper.Min(Volume, 1);

				// Create the Instance of the Sound Effect.
				SoundEffectInstance sndEfctInstnc = SoundEffect.CreateInstance();

				// Set the Volume
				sndEfctInstnc.Volume = Volume * vxAudioManager.SoundEffectVolume;

				// Set the Pitch
				sndEfctInstnc.Pitch = Pitch;

				// Play It
				sndEfctInstnc.Play();

				return true;
			}
			catch (Exception ex)
			{
				vxConsole.WriteException(this,new vxSoundEffectException(sender, SoundEffect, ex));
				return false;
			}
		}

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                vxSceneManager.RemoveScene(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                isExiting = true;
            }
        }

        public void Dispose()
        {
            
        }


        #endregion
    }
}
