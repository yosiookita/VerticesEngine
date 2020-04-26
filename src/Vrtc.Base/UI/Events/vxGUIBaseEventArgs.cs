using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VerticesEngine.UI.Dialogs;

namespace VerticesEngine.UI.Events
{
    public class vxGuiItemClickEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public vxGuiItemClickEventArgs(vxGUIControlBaseClass guiItem)
        {
            _guiItem = guiItem;
        }


        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public vxGUIControlBaseClass GUIitem
        {
            get { return _guiItem; }
        }
        vxGUIControlBaseClass _guiItem;


        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        PlayerIndex playerIndex = PlayerIndex.One;
    }

	public class vxGuiManagerItemAddEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public vxGuiManagerItemAddEventArgs(vxGUIManager guiManager)
		{
			_guiManager = guiManager;
		}


		/// <summary>
        /// Gets the GUI manager.
        /// </summary>
        /// <value>The GUI manager.</value>
		public vxGUIManager GuiManager
		{
			get { return _guiManager; }
		}
		vxGUIManager _guiManager;
	}


	public class vxFileDialogItemClickEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public vxFileDialogItemClickEventArgs(vxFileDialogItem vxbaseGuiItem)
		{
			this.fileDialogItem = vxbaseGuiItem;
		}


		/// <summary>
		/// Gets the index of the player who triggered this event.
		/// </summary>
		public vxFileDialogItem FileDialogItem
		{
			get { return fileDialogItem; }
		}
		vxFileDialogItem fileDialogItem;
	}

	public class vxValueChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the index of the player who triggered this event.
		/// </summary>
		public vxGUIControlBaseClass GUIitem
		{
			get { return _baseGuiItem; }
		}
		vxGUIControlBaseClass _baseGuiItem;

		/// <summary>
		/// Gets the new value.
		/// </summary>
		/// <value>The new value.</value>
		public float NewValue
		{
			get { return _newValue; }
		}
		float _newValue;


		public float PreviousValue
		{
			get { return _previousValue; }
		}
		float _previousValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxValueChangedEventArgs"/> class.
		/// </summary>
		/// <param name="vxbaseGuiItem">Vxbase GUI item.</param>
		/// <param name="newValue">New value.</param>
		/// <param name="previousValue">Previous value.</param>
		public vxValueChangedEventArgs(vxGUIControlBaseClass vxbaseGuiItem, float newValue, float previousValue)
		{
			_baseGuiItem = vxbaseGuiItem;

			_newValue = newValue;
			_previousValue = previousValue;
		}
	}

}
