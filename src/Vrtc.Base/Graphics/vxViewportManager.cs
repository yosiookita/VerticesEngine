using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// Viewport manager which really only reports different facts about Cameras and Viewports being used.
    /// It also draws the Split Screen borders, so if you want a different look, then override this class.
    /// </summary>
    public class vxViewportManager
	{
		readonly vxEngine Engine;

		/// <summary>
		/// The main viewport.
		/// </summary>
		public Viewport MainViewport;


		/// <summary>
		/// Gets the number of viewports.
		/// </summary>
		/// <value>The number of viewports.</value>
		public int NumberOfViewports
		{
            get { return Scene.Cameras.Count; }
		}


		/// <summary>
		/// The viewports.
		/// </summary>
		public readonly List<Viewport> Viewports;

        vxGameplayScene3D Scene;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.Graphics.vxViewportManager"/> class.
		/// </summary>
        /// <param name="Scene">Scene.</param>
		public vxViewportManager(vxGameplayScene3D Scene)
		{
            this.Scene = Scene;
			this.Engine = Scene.Engine;

			MainViewport = Engine.GraphicsDevice.Viewport;

			Viewports = new List<Viewport>();

            foreach (vxCamera3D camera in Scene.Cameras)
				Viewports.Add(camera.Viewport);
		}

        public void ResetMainViewport()
        {
            MainViewport = Engine.GraphicsDevice.Viewport;
        }

		/// <summary>
		/// Resets the viewport to Fullscreen.
		/// </summary>
		public void ResetViewport()
		{
			// Reset the Viewport
			Engine.GraphicsDevice.Viewport = MainViewport;
		}
	}
}
