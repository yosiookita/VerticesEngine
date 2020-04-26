using System;
using System.Collections.Generic;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI;
using Virtex.Lib.Vrtc.GUI.Controls;

namespace Virtex.Lib.Vrtc.Graphics
{

	/// <summary>
	/// The base graphical element class for the Vertices Engine which acts as a inhertiable object controlling
	/// properties, selection, highliting etc...
	/// </summary>
	public class vxGraphicalElement
	{
		/// <summary>
		/// Gets or sets the selection state.
		/// </summary>
		/// <value>The state of the selection.</value>
		vxEnumSelectionState SelectionState
		{
			get { return _selectionState; }
			set
			{
				_selectionState = value;
				OnSelectionStateChange();
			}
		}
		vxEnumSelectionState _selectionState;

		/// <summary>
		/// Called when the selection state changes.
		/// </summary>
		public virtual void OnSelectionStateChange()
		{

		}


		/// <summary>
		/// The properties for this element.
		/// </summary>
		public List<vxProperties> Properties = new List<vxProperties>();

		/// <summary>
		/// The tree node for this graphical Element.
		/// </summary>
		public vxTreeNode TreeNode
		{
			get
			{
				if (_treeNode == null)
					return new vxTreeNode(Engine, null, "node");
				else
					return _treeNode;
			}
			set { _treeNode = value; }
		}
		vxTreeNode _treeNode;

		/// <summary>
		/// Sets the properties for this element.
		/// </summary>
		public virtual void SetProperties()
		{

		}

		/// <summary>
		/// The engine.
		/// </summary>
		public vxEngine Engine;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Virtex.Lib.Vrtc.Graphics.vxGraphicalElement"/> class.
		/// </summary>
		public vxGraphicalElement(vxEngine Engine)
		{
			this.Engine = Engine;
		}
	}


	/// <summary>
	/// Property Base Class
	/// </summary>
	public class vxProperties
	{
		/// <summary>
		/// The type of the property.
		/// </summary>
		public readonly vxEnumPropertyType PropertyType;



		/// <summary>
		/// Initializes a new instance of the <see cref="T:Virtex.Lib.Vrtc.Graphics.vxProperties"/> class.
		/// </summary>
		/// <param name="PropertyType">Property type.</param>
		public vxProperties(vxEnumPropertyType PropertyType)
		{
			this.PropertyType = PropertyType;
		}
	}
}
