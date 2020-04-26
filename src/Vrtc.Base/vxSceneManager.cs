using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace VerticesEngine
{
    /// <summary>
    /// The Scene Manager which handles Scene Loading and drawing
    /// </summary>
    public static class vxSceneManager
    {
        /// <summary>
        /// Screen List
        /// </summary>
        public static List<vxBaseScene> SceneCollection = new List<vxBaseScene>();

        /// <summary>
        /// Screens to Update List
        /// </summary>
        static List<vxBaseScene> scenesToUpdate = new List<vxBaseScene>();

        static vxEngine Engine;



        /// <summary>
        /// The color to fade the back buffer to.
        /// </summary>
        public static Color FadeToBackBufferColor = Color.Black;

        /// <summary>
        /// The color of the loading screen background.
        /// </summary>
        public static Color LoadingScreenBackColor = Color.Black;

        /// <summary>
        /// The color of the loading screen text.
        /// </summary>
        public static Color LoadingScreenTextColor = Color.White;

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public static bool TraceEnabled;

        internal static void Init(vxEngine engine)
        {
            Engine = engine;
        }

        internal static void LoadContent()
        {

            // Tell each of the screens to load their content.
            foreach (vxBaseScene screen in SceneCollection)
            {
                screen.LoadContent();
            }

        }

        internal static void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (vxBaseScene screen in SceneCollection)
            {
                screen.UnloadContent();
            }
        }


        internal static void Update(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            scenesToUpdate.Clear();

            for (int i = 0; i < SceneCollection.Count; i++)
            {
                vxBaseScene screen = SceneCollection[i];
                scenesToUpdate.Add(screen);

                if (screen.CanRemoveCompletely == true)
                    screen.Dispose();
            }

            bool otherScreenHasFocus = !Engine.Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (scenesToUpdate.Count > 0)
            {

                // Pop the topmost screen off the waiting list.
                vxBaseScene screen = scenesToUpdate[scenesToUpdate.Count - 1];

                scenesToUpdate.RemoveAt(scenesToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput();

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (TraceEnabled)
                TraceScreens();
        }


        internal static void Draw(GameTime gameTime)
        {
            for (int s = 0; s < SceneCollection.Count; s++)
            {
                if (SceneCollection[s].ScreenState == ScreenState.Hidden)
                    continue;

                SceneCollection[s].Draw(gameTime);
            }
        }


        #region Public Methods

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public static void AddScene(vxBaseScene screen)
        {
            AddScene(screen, PlayerIndex.One);
        }


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public static void AddScene(vxBaseScene screen, PlayerIndex? controllingPlayer)
        {
            screen.IsInitialised = true;
            screen.ControllingPlayer = controllingPlayer;

            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (Engine.IsEngineInitialised)
            {
                screen.LoadContent();
            }

            SceneCollection.Add(screen);

        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use vxGameBaseScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public static void RemoveScene(vxBaseScene screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (Engine.IsEngineInitialised)
            {
                screen.UnloadContent();
            }

            SceneCollection.Remove(screen);
            scenesToUpdate.Remove(screen);
            screen.Dispose();

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (SceneCollection.Count > 0)
            {
            }
        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public static vxBaseScene[] GetScreens()
        {
            return SceneCollection.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public static void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = Engine.GraphicsDevice.Viewport;

            Engine.SpriteBatch.Begin("UI.FadeBackBuffer");

            Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank, viewport.Bounds,
                FadeToBackBufferColor * alpha);

            Engine.SpriteBatch.End();
        }


        #endregion

        #region Utility Functions

        internal static void OnGraphicsRefresh()
        {
            // Now tell all scenes to reset
            foreach (var scene in GetScreens())
            {
                scene.OnGraphicsRefresh();
            }
        }

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        static void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (vxBaseScene screen in SceneCollection)
                screenNames.Add(screen.GetType().Name);
        }

        #endregion
    }
}
