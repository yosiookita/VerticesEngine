using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using VerticesEngine.Graphics;
using VerticesEngine.UI.Controls;

namespace VerticesEngine
{
    public class vxBaseContainer : vxGameObject, IEnumerable
    {
        public vxBaseContainer() { }

        public vxGameObject this[int i]
        {
            get
            {
                return Children[i];
            }
            set
            {
                Children[i] = value;
            }
        }


        public IEnumerator GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }



    public enum vxInspectorCategory
    {
        BasicProperties,
        EntityProperties,
        EntityTransformProperties,
        ModelProperties,
        GraphicalProperies,
    }


    public enum vxInspectorShaderCategory
    {
        AntiAliasing,
        BlurShader,
        Bloom,
        MotionBlur,
        DeferredRenderer,
        DepthOfField,
        Distortion,
        EdgeDetection,
        GodRays,
        Lighting,
        ShadowMapping,
        ScreenSpaceAmbientOcclusion,
        ScreenSpaceReflections

    }

    /// <summary>
    /// This is the base class for all items in the Vertices Engine. It allows access to basic variables such as the 
    /// Engine and the GraphicsDevice.
    /// </summary>
    public class vxGameObject : IDisposable
	{
		/// <summary>
		/// The Vertices Engine Reference.
		/// </summary>
		public vxEngine Engine;

        /// <summary>
        /// Is this Game Object currently visible? Note that an Object can be Enabled, but not visible
        /// </summary>
        public bool IsVisible = true;

        /// <summary>
        /// Is this Game Object Enabled currently
        /// </summary>
		public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;

                if (_isEnabled)
                {
                    OnEnabled();
                }
                else
                {
                    OnDisabled();
                }

                InternalOnEnabledChanged(_isEnabled);
            }
        }
        private bool _isEnabled = true;

        /// <summary>
        /// Called on Component Enabled
        /// </summary>
        public virtual void OnEnabled() { }


        /// <summary>
        /// Called on Component Disabled
        /// </summary>
        public void OnDisabled() { }


        [vxShowInInspectorAttribute("Element ID", vxInspectorCategory.BasicProperties, "The Elements ID in the system.", isReadOnly:true, isDebugOnly:true)]
		public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        string _id = "";



        protected virtual bool HasId()
        {
            return true;
        }

		public List<vxGameObject> Children = new List<vxGameObject>();

		public vxGameObject Parent;


        /// <summary>
        /// Gets the game.
        /// </summary>
        /// <value>The game.</value>
		public vxGame Game
		{
			get { return Engine.Game; }
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
            set { Engine.GraphicsDevice.Viewport = value; }
		}


        /// <summary>
        /// Gets the sprite batch.
        /// </summary>
        /// <value>The sprite batch.</value>
		public vxSpriteBatch SpriteBatch
		{
			get { return Engine.SpriteBatch; }
		}



        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public vxSettings Settings
        {
            get { return Engine.Settings; }
        }




        /// <summary>
        /// Gets the default texture.
        /// </summary>
        /// <value>The default texture.</value>
        public Texture2D DefaultTexture
        {
            get { return vxInternalAssets.Textures.Blank; }
        }

        #region - Sandbox State Fields and Properties -

        /// <summary>
        /// State of the Entity which is triggered by the simulation.
        /// </summary>
        public vxEnumSandboxStatus SandboxState = vxEnumSandboxStatus.EditMode;


        /// <summary>
		/// Gets or sets the selection state.
		/// </summary>
		/// <value>The state of the selection.</value>
		public vxSelectionState SelectionState
        {
            get { return _selectionState; }
            set
            {
                PreviousSelectionState = _selectionState;
                _selectionState = value;
                OnSelectionStateChange();
            }
        }
        vxSelectionState _selectionState;

        vxSelectionState PreviousSelectionState = vxSelectionState.None;

        public bool OnlySelectInSandbox = false;

        /// <summary>
        /// Called when Selected
        /// </summary>
        public virtual void OnSelected() { }


        /// <summary>
        /// Called when Unselected
        /// </summary>
        public virtual void OnUnSelected() { }

        /// <summary>
        /// Called when the selection state changes.
        /// </summary>
        protected virtual void OnSelectionStateChange()
        {
            // If the Level is in Edit mode, then set up Selection
            if (SandboxState == vxEnumSandboxStatus.Running && OnlySelectInSandbox)
            {
                SelectionState = vxSelectionState.None;
            }
            else
            {
                if (SelectionState == vxSelectionState.Selected &&
                    PreviousSelectionState != vxSelectionState.Selected)
                {
                    OnSelected();
                    // Raise the 'SelectionStateSelected' event.
                    if (Selected != null)
                        Selected(this, new EventArgs());
                }

                if (PreviousSelectionState == vxSelectionState.Selected &&
                    SelectionState != vxSelectionState.Selected)
                {
                    OnUnSelected();
                    // Raise the 'SelectionStateUnSelected' event.
                    if (UnSelected != null)
                        UnSelected(this, new EventArgs());
                }

                PreviousSelectionState = SelectionState;
            }
        }


        /// <summary>
        /// Event Fired when the Items Selection stat Changes too Hovered
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Event Fired when the Items Selection stat Changes too unselected (or unhovered)
        /// </summary>
        public event EventHandler<EventArgs> UnSelected;

        #endregion


        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <returns>The title.</returns>
        public virtual string GetTitle()
        {
            return this._id;
        }

        internal void GetProperties(vxPropertiesControl propertyControl)
        {
            // first get all Attributes for this Item
            propertyControl.AddPropertiesFromType(GetType());

        }



        /// <summary>
        /// The tree node for this graphical Element.
        /// </summary>
        public vxTreeNode TreeNode
        {
            get
            {
                if (_treeNode == null)
                    _treeNode = new vxTreeNode(Engine, null, Id);
                _treeNode.Entity = this;
                return _treeNode;
            }
            set { _treeNode = value; }
        }
        vxTreeNode _treeNode;


		public virtual Texture2D GetTreeIcon()
		{
			return vxInternalAssets.Textures.Blank;
		}

        /// <summary>
        /// Gets the icon for this game object. Override this to provide per-entity cusomtization.
        /// </summary>
        /// <returns>The icon.</returns>
        public virtual Texture2D GetIcon(int w, int h)
        {
            return DefaultTexture;
        }


        public virtual void AddToNode(vxTreeControl control, vxTreeNode node)
		{
			if(_treeNode == null)
				_treeNode = new vxTreeNode(Engine, control, Id);

            _treeNode.Entity = this;

			_treeNode.Clear();

			// first add it to the node
			node.Add(_treeNode); 
			_treeNode.IsExpanded = true;

			//TreeNode.TreeControl = node.TreeControl;

			foreach (vxGameObject item in Children)
			{
				// now recursivecly add all others
				item.AddToNode(control, _treeNode);
			}
		}

        /// <summary>
        /// Creates a new vxGameObject
        /// </summary>
        /// <param name="Engine"></param>
        public vxGameObject()
        {
            this.Engine = vxEngine.Instance;

			string key = this.GetType().ToString();

			// break down key for id
			if (key.Contains("."))
                _id = key.Substring(key.LastIndexOf(".") + 1) + ".";

			int i = 0;

            // set the ID. 
            if (HasId())
            {
                while (vxGameObject.NameRegister.Contains(_id + i))
                    i++;

                _id += i;
                vxGameObject.NameRegister.Add(Id);
            }
            else
            {
                //Console.WriteLine(this.GetType());
            }
        }

		public void Add(vxGameObject item)
		{
			Children.Add(item);
		}


		public void Remove(vxGameObject item)
		{
			Children.Remove(item);
		}

        /// <summary>
        /// Dispposes this Object
        /// </summary>
		public virtual void Dispose(){}

        /// <summary>
        /// Called when there is a reset or refresh of Graphic settings such as resolution or setting.
        /// </summary>
        public virtual void OnGraphicsRefresh() { }


        #region -- Internal Engine Methods --


        /// <summary>
        /// The name register.
        /// </summary>
        public static List<string> NameRegister = new List<string>();


        /// <summary>
        /// Called internally by the engine when enabled changes
        /// </summary>
        /// <param name="value"></param>
        internal virtual void InternalOnEnabledChanged(bool value) { }

        #endregion
    }
}
