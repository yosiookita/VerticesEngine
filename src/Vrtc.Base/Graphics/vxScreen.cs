using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using VerticesEngine.Diagnostics;
using VerticesEngine.Graphics;
using VerticesEngine.Utilities;

namespace VerticesEngine
{
    /// <summary>
    /// The Engine Graphical Settings Manager which holds all of settings for engine graphics such as
    /// resolution, fullscreen, vsync as well as other Depth of Field toggle or Cascade Shadow qaulity.
    /// </summary>
    public static class vxScreen
    {
        /// <summary>
        /// Reference to the Engine.
        /// </summary>
        static vxEngine Engine;

        /// <summary>
        /// Gets the graphics device manager.
        /// </summary>
        /// <value>The graphics device manager.</value>
        static GraphicsDeviceManager GraphicsDeviceManager;


        /// <summary>
        /// Gets or sets the resolution from 'Engine.Settings.Graphics.Screen'. NOTE: Apply needs to be called to apply these settings.
        /// </summary>
        /// <value>The resolution.</value>

        //[vxGraphicalSettings("Resolution")]
        public static Point Resolution
        {
            get { return _resolution; }
        }
#if DEBUG
        static Point _resolution = new Point(1000, 600);
#else  
        static Point _resolution = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
#endif

        [vxGraphicalSettings("ResolutionX")]
        public static int Width
        {
            get { return _resolution.X; }
            set { _resolution = new Point(value, _resolution.Y); }
        }

        [vxGraphicalSettings("ResolutionY")]
        public static int Height
        {
            get { return _resolution.Y; }
            set { _resolution = new Point(_resolution.X, value); }
        }

        /// <summary>
        /// Gets or sets a value from 'Engine.Settings.Graphics.Screen' indicating whether this instance is full screen. NOTE: Apply needs to be called to apply these settings.
        /// </summary>
        /// <value><c>true</c> if this instance is full screen; otherwise, <c>false</c>.</value>
        [vxGraphicalSettings("IsFullScreen")]
        public static bool IsFullScreen
        {
            get { return _isFullScreen; }
            set { _isFullScreen = value; }
        }
#if DEBUG
        static bool _isFullScreen = false;
#else
        static bool _isFullScreen = true;
#endif


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:VerticesEngine.Graphics.vxGraphicsSettingsManager"/> is
        /// VS ync on.
        /// </summary>
        /// <value><c>true</c> if is VS ync on; otherwise, <c>false</c>.</value>
        [vxGraphicalSettings("IsVSyncOn")]
        public static bool IsVSyncOn
        {
            get { return _isVSyncOn; }
            set { _isVSyncOn = value; }
        }
        static bool _isVSyncOn = true;


/// <summary>
/// Init the specified engine.
/// </summary>
/// <param name="engine">Engine.</param>
        internal static void Init(vxEngine engine)
        {
            vxConsole.WriteLine("Starting Graphics Settings Manager...");

            Engine = engine;

            GraphicsDeviceManager = Engine.Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;

            vxDebug.CommandUI.RegisterCommand(
                "graref",              // Name of command
                "Refresh the Grapahics Settings",     // Description of command
                delegate (IDebugCommandHost host, string command, IList<string> args)
                {
                    RefreshGraphics();
                });
            

#if __MOBILE__
            SetResolution(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
#endif
        }


        public static void SetResolution(int x, int y)
        {
            SetResolution(new Point(x, y));
        }

        public static void SetResolution(Point point)
        {
            _resolution = point;
            //vxScreen.RefreshGraphics();
        }


        /// <summary>
        /// Applies the Current Resolution and Fullscreen Settings.
        /// </summary>
        public static void RefreshGraphics()
        {
            // Don't set resolution for Mobile.
#if __MOBILE__

            // Set the Window Flags here
#if __ANDROID__
            Microsoft.Xna.Framework.Game.Activity.Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);
            Microsoft.Xna.Framework.Game.Activity.Window.ClearFlags(Android.Views.WindowManagerFlags.ForceNotFullscreen);
#endif

            GraphicsDeviceManager.PreferredBackBufferWidth = vxScreen.Width;
            GraphicsDeviceManager.PreferredBackBufferHeight = vxScreen.Height;

            // Set FullScreen Value
            GraphicsDeviceManager.IsFullScreen = true;
#else

            // Set Resolution
            // *****************************
            GraphicsDeviceManager.PreferredBackBufferWidth = Resolution.X;
            GraphicsDeviceManager.PreferredBackBufferHeight = Resolution.Y;



            // Set FullScreen Value
            GraphicsDeviceManager.IsFullScreen = IsFullScreen;


            // VSync Values
            Engine.Game.IsFixedTimeStep = _isVSyncOn;
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = _isVSyncOn;
    
#endif

            vxConsole.WriteLine("Refreshing Screen With Following Settings:");

            DebugSettingChange(nameof(Resolution), Resolution);
            DebugSettingChange(nameof(IsFullScreen), IsFullScreen);
            DebugSettingChange(nameof(IsVSyncOn), IsVSyncOn);


            // Refresh the Graphics
            GraphicsDeviceManager.ApplyChanges();

            // Now tell all scenes to reset
            Engine.OnGraphicsRefresh();
        }

        static void DebugSettingChange(string name, object setting)
        {
            vxConsole.WriteLine(string.Format("     {0} : {1}", name, setting));
        }

        public static void DebugDump(string area)
        {
            Console.WriteLine("VIEWPORT DUMP: " + area);

            var GraphicsDeviceManager = Engine.Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;


            
            Console.WriteLine("DefaultAdapter");
            Console.WriteLine(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width + "x" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

            Console.WriteLine("CurrentDisplayMode");
            Console.WriteLine(Engine.GraphicsDevice.Adapter.CurrentDisplayMode.Width + "x" + Engine.GraphicsDevice.Adapter.CurrentDisplayMode.Height);


            PresentationParameters pp = GraphicsDeviceManager.GraphicsDevice.PresentationParameters;
            
            Console.WriteLine("BackBufferWidth");
            Console.WriteLine(pp.BackBufferWidth + "x" + pp.BackBufferHeight);


            var w = GraphicsDeviceManager.PreferredBackBufferWidth;
            var h = GraphicsDeviceManager.PreferredBackBufferHeight;

            Console.WriteLine("PreferredBackBuffer");
            Console.WriteLine(w + "x" + h);

            var xB = GraphicsDeviceManager.GraphicsDevice.Viewport.Bounds.Width;
            var yB = GraphicsDeviceManager.GraphicsDevice.Viewport.Bounds.Height;
            Console.WriteLine("Viewport");
            Console.WriteLine(xB + "x" + yB);

        }

        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        public static Texture2D TakeScreenshot()
        {
            return vxGraphics.FinalBackBuffer.Resize(Engine, vxGraphics.FinalBackBuffer.Width, vxGraphics.FinalBackBuffer.Height);
        }
    }
}

