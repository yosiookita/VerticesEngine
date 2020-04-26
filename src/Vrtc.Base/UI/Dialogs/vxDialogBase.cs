using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VerticesEngine;
using VerticesEngine.UI;
using VerticesEngine.Utilities;
using VerticesEngine.Input.Events;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class vxDialogBase : vxBaseScene
    {
        #region Fields


        /// <summary>
        /// Message Box Title
        /// </summary>
        public string Title;


		/// <summary>
		/// The given Art Provider of the Menu Entry. 
		/// </summary>
		public vxDialogArtProvider ArtProvider { get; internal set; }

        /// <summary>
        /// Gets the bounds of the art providers bounding GUI rectangle.
        /// </summary>
        /// <value>The bounds.</value>
        public Rectangle Bounds
        {
            get { return ArtProvider.GUIBounds; } 
        }

        /// <summary>
        /// The Internal GUI Manager so the Dialog Box can handle it's own items.
        /// </summary>
        public vxGUIManager InternalGUIManager {get; set;}
        
        public vxButtonControl OKButton;

        public vxButtonControl ApplyButton;

        public vxButtonControl CancelButton;

        //public SpriteBatch SpriteBatch;

        public SpriteFont Font
        {
            get { return ArtProvider.Font; }
        }

        public bool IsCustomButtonPosition = false;
        // Center the message text in the viewport.


		//List<vxScrollPanelItem> List_Items = new List<vxScrollPanelItem>();

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        //public event EventHandler<PlayerIndexEventArgs> Apply;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization

        public readonly vxEnumButtonTypes ButtonTypes;

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
		public vxDialogBase(string Title, vxEnumButtonTypes ButtonTypes)
        {
            this.Title = Title;

            this.ButtonTypes = ButtonTypes;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }
        #endregion

        public Vector2 viewportSize;



		/// <summary>
		/// Gets the size of the boundary.
		/// </summary>
		/// <returns>The boundary size.</returns>
		public virtual Vector2 GetBoundarySize()
		{
			return new Vector2(Engine.GraphicsDevice.Viewport.Width, Engine.GraphicsDevice.Viewport.Height);
		}

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            InternalGUIManager = new vxGUIManager(Engine);

			this.ArtProvider = (vxDialogArtProvider)vxGUITheme.ArtProviderForDialogs.Clone();

			//And just so that all is set up properly, resize anything based off of current resolution scale.
			ArtProvider.SetBounds();

			viewportSize = GetBoundarySize();

            ApplyButton = new vxButtonControl(Engine, "Apply", new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));
            OKButton = new vxButtonControl(Engine, "OK", new Vector2(viewportSize.X / 2 - 115, viewportSize.Y / 2 + 20));
            CancelButton = new vxButtonControl(Engine, "Cancel", new Vector2(viewportSize.X / 2 + 15, viewportSize.Y / 2 + 20));

            ApplyButton.Clicked += OnApplyButtonClicked;
            OKButton.Clicked += OnOKButtonClicked;
            CancelButton.Clicked += OnCancelButtonClicked;


            if (ButtonTypes != vxEnumButtonTypes.None)
            {
                if (ButtonTypes == vxEnumButtonTypes.OkApplyCancel)
                    InternalGUIManager.Add(ApplyButton);

                InternalGUIManager.Add(OKButton);

                if (ButtonTypes != vxEnumButtonTypes.Ok)
                    InternalGUIManager.Add(CancelButton);
            }

            //spriteBatch = Engine.SpriteBatch;
			//font = vxGUITheme.Fonts.Font;

            vxGUITheme.ArtProviderForButtons.SetDefaults();

        }

		public virtual void OnApplyButtonClicked(object sender, vxGuiItemClickEventArgs e)
		{
			// Raise the accepted event.
			if (Accepted != null)
                Accepted(this, new PlayerIndexEventArgs(ControllingPlayer.Value));
		}

		public virtual void OnOKButtonClicked(object sender, vxGuiItemClickEventArgs e)
		{
			// Raise the accepted event, then exit the dialog.
			OnApplyButtonClicked(sender, e);

			ExitScreen();

		}

        public virtual void OnCancelButtonClicked(object sender, vxGuiItemClickEventArgs e)
        {
			// Raise the cancelled event, then exit the message box.
			if (Cancelled != null)
				Cancelled(this, new PlayerIndexEventArgs(ControllingPlayer.Value));
			
            ExitScreen();
        }

        public virtual void OnButtonPositionSet()
        {
            if (IsCustomButtonPosition == false)
            {
                Rectangle GUIBounds = ArtProvider.GUIBounds;

                ApplyButton.Position = new Vector2(GUIBounds.Right, GUIBounds.Bottom + ArtProvider.ButtonBuffer)
                    - new Vector2(
                        ApplyButton.Width + OKButton.Width + CancelButton.Width + ArtProvider.Padding.X * 3,
                        ApplyButton.Height + ArtProvider.Padding.Y);

                OKButton.Position = new Vector2(GUIBounds.Right, GUIBounds.Bottom + ArtProvider.ButtonBuffer)
                    - new Vector2(
                        OKButton.Width + CancelButton.Width + ArtProvider.Padding.X * 2,
                        OKButton.Height + ArtProvider.Padding.Y);

                CancelButton.Position = new Vector2(GUIBounds.Right, GUIBounds.Bottom + ArtProvider.ButtonBuffer)
                    - new Vector2(
                        CancelButton.Width + ArtProvider.Padding.X,
                        CancelButton.Height + ArtProvider.Padding.Y);
            }
        }
        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            foreach (vxGUIControlBaseClass item in InternalGUIManager.Items)
            {
                item.TransitionAlpha = TransitionAlpha;
                item.ScreenState = ScreenState;
            }

            OnButtonPositionSet();

            //Update GUI Manager
            if(otherScreenHasFocus == false)
                InternalGUIManager.Update();
        }

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            vxSceneManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

			SpriteBatch.Begin("UI - Dialog");

			//First Draw the Dialog Art Provider
			ArtProvider.Draw(this);

            //Draw the GUI
            InternalGUIManager.DrawByOwner();

			SpriteBatch.End();
		}

		public virtual void SetArtProvider(vxDialogArtProvider NewArtProvider)
		{
			ArtProvider = (vxDialogArtProvider)NewArtProvider.Clone();
		}

        #endregion
    }
}
