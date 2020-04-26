using System;
using Microsoft.Xna.Framework;

namespace VerticesEngine.UI.Themes
{

	/// <summary>
	/// Item Theme
	/// </summary>
	public class vxGUIControlTheme
	{
        /// <summary>
        /// The background colour theme.
        /// </summary>
		public vxColourTheme Background = new vxColourTheme();

        /// <summary>
        /// The text colour theme.
        /// </summary>
		public vxColourTheme Text = new vxColourTheme(Color.WhiteSmoke, Color.Black, Color.Black);


		/// <summary>
		/// The border colour theme.
		/// </summary>
		public vxColourTheme Border = new vxColourTheme(Color.Black, Color.Black, Color.Black);

        /// <summary>
        /// Whether or not this theme should have a border.
        /// </summary>
        public bool DoBorder = true;

		public vxGUIControlTheme()
		{

		}

		public vxGUIControlTheme(vxColourTheme Background)
		{
			this.Background = Background;
		}

		public vxGUIControlTheme(vxColourTheme Background, vxColourTheme Text)
		{
			this.Background = Background;
			this.Text = Text;
		}

        public vxGUIControlTheme(vxColourTheme Background, vxColourTheme Text, vxColourTheme Border)
        {
            this.Background = Background;
            this.Text = Text;
            this.Border = Border;
        }

        /// <summary>
        /// Sets the Colors based off the current state.
        /// </summary>
        /// <param name="item">Item.</param>
        public void SetState(vxGUIControlBaseClass item)
        {
            Text.State = item.State;
            Background.State = item.State;
            Border.State = item.State;
        }
	}
}
