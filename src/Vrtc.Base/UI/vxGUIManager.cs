using System.Collections.Generic;
using System.Linq;

namespace VerticesEngine.UI
{
    /// <summary>
    /// GUI Manager for a given Game Scene. This Handles all GUI Items within a given scene.
    /// </summary>
    public class vxGUIManager
    {
		public vxEngine Engine;

        /// <summary>
        /// The collection of GUI items in this GUI Manager.
        /// </summary>
        public List<vxGUIControlBaseClass> Items = new List<vxGUIControlBaseClass>();

		/// <summary>
		/// Does the GUI have focus.
		/// </summary>
        public bool HasFocus = false;


		/// <summary>
		/// This item is the current item with focus.
		/// </summary>
		public vxGUIControlBaseClass FocusedItem;

		/// <summary>
		/// Gets or sets the alpha of the GUI Manager.
		/// </summary>
		/// <value>The alpha.</value>
		public float Alpha
		{
			get { return _alpha; }
			set { _alpha = value; }
		}
		float _alpha = 1;


       /// <summary>
       /// Initializes a new instance of the <see cref="VerticesEngine.UI.vxGUIManager"/> class.
       /// </summary>
        public vxGUIManager(vxEngine Engine)
        {
			this.Engine = Engine;
        }

		/// <summary>
		/// Adds a vxGUI Item to thie GUI Manager.
		/// </summary>
		/// <param name="item"></param>
		public void Add(vxGUIControlBaseClass item)
		{
            item.Index = Items.Count();
			Items.Add(item);
			item.OnGUIManagerAdded(this);
        }

        /// <summary>
        /// Clears this UIManager of all GUI Items.
        /// </summary>
        public void Clear()
        {
            Items.Clear();   
        }

        /// <summary>
        /// Remove the specified item.
        /// </summary>
        /// <returns>The remove.</returns>
        /// <param name="item">Item.</param>
        public void Remove(vxGUIControlBaseClass item)
		{
			Items.Remove(item);
		}


		/// <summary>
		/// Adds a Range of vxGUI Items to thie GUI Manager.
		/// </summary>
		/// <param name="item">Xbase GUI item.</param>
        public void AddRange(IEnumerable<vxGUIControlBaseClass> item)
        {
            Items.AddRange(item);
        }

        /// <summary>
        /// Tells the GUI Manager too update each of the Gui Items
        /// </summary>
        public void Update()
        {
			// The GUI Manager Draws and Updates it's items from the back forwards. It
			// only allows one item to have focus, which is the most forward item with the mouse
			// over it.
            HasFocus = false;

            if (this.FocusedItem == null)
            {
                for(int i = 0; i < Items.Count; i++)
                {
                    vxGUIControlBaseClass guiItem = Items[i];
                    guiItem.GUIIndex = i;
					//guiItem.Alpha = this.Alpha;
                    guiItem.Update();

                    if (guiItem.HasFocus == true)
                        HasFocus = true;
                }
            }
            else
            {
                // If there's a focused item, then the UIManager has Focus
                this.FocusedItem.Update();
                HasFocus = true;
            }
        }

        /// <summary>
        /// Tells the GUI Manager too Draw the Gui Items
        /// </summary>
        public void Draw()
        {
			// The GUI Manager tries to draw all of the GUI items in one SpriteBatch call.
			// Any items which require special SpriteBatch calls will need to End the call,
			// do what they need, then do a special call after wards.
			Engine.SpriteBatch.Begin("GUI - Internal Manager Draw");
			DrawByOwner();
			Engine.SpriteBatch.End();
        }

		/// <summary>
		/// Draws the GUI manager using a pre-started SpriteBatch the by owner.
		/// </summary>
		public void DrawByOwner()
		{
			foreach (vxGUIControlBaseClass guiItem in Items)
			{
				guiItem.Alpha = Alpha;
				guiItem.Draw();
			}

			foreach (vxGUIControlBaseClass guiItem in Items)
			{
				guiItem.DrawText();
			}

			if (this.FocusedItem != null)
			{
				this.FocusedItem.Draw();
				this.FocusedItem.DrawText();
			}
		}
    }
}
