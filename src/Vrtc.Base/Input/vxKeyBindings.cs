using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using VerticesEngine.Localization;
using VerticesEngine.Utilities;

namespace VerticesEngine.Input
{

public enum vxKeyBindingID
{
	Forward,
	Left,
	Right,
	Back,

	Jump,
	Croutch,

	Interact1,
	Interact2,	}

/// <summary>
/// Class holding Key Press Type for common movements. This allows players to customise keyboard controls 
/// for different keyboard types.
/// </summary>
public class vxKeyBindings
{
	public vxEngine Engine;

	public Dictionary<object, vxKeyBinding> Bindings = new Dictionary<object, vxKeyBinding>();

	/// <summary>
	/// Initializes a new instance of the <see cref="T:VerticesEngine.Input.KeyBindings"/> class with the 
	/// default QWERTY Keyboard Bindings.
	/// </summary>
	public vxKeyBindings(vxEngine Engine)
	{
		Add(vxKeyBindingID.Forward, new vxKeyBinding("Forward", Keys.W));
		Add(vxKeyBindingID.Back, new vxKeyBinding("Back", Keys.S));
		Add(vxKeyBindingID.Left, new vxKeyBinding("Left", Keys.A));
		Add(vxKeyBindingID.Right, new vxKeyBinding("Right", Keys.D));

		Add(vxKeyBindingID.Jump, new vxKeyBinding("Jump", Keys.Space));
		Add(vxKeyBindingID.Croutch, new vxKeyBinding("Croutch", Keys.LeftControl));

		Add(vxKeyBindingID.Interact1, new vxKeyBinding("Interact1", Keys.Q));
		Add(vxKeyBindingID.Interact2, new vxKeyBinding("Interact2", Keys.E));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:VerticesEngine.Input.KeyBindings"/> class with the specified 
	/// keyboard type.
	/// </summary>
	/// <param name="KeyboardPreset">Keyboard preset.</param>
	public vxKeyBindings(vxEngine Engine, KeyboardTypes KeyboardPreset)
	{
		this.Engine = Engine;

		//string te = Engine.Language[vxLocalisationKey.KeyName_Forward);

		switch (KeyboardPreset)
		{
			case KeyboardTypes.QWERTY:
				Add(vxKeyBindingID.Forward, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Forward], Keys.W));
				Add(vxKeyBindingID.Back, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Back], Keys.S));
				Add(vxKeyBindingID.Left, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Left], Keys.A));
				Add(vxKeyBindingID.Right, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Right], Keys.D));

				Add(vxKeyBindingID.Jump, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Jump], Keys.Space));
				Add(vxKeyBindingID.Croutch, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Croutch], Keys.LeftControl));

				Add(vxKeyBindingID.Interact1, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Interact1], Keys.Q));
				Add(vxKeyBindingID.Interact2, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Interact2], Keys.E));
				break;

			case KeyboardTypes.AZERTY:
				Add(vxKeyBindingID.Forward, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Forward], Keys.Z));
				Add(vxKeyBindingID.Back, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Back], Keys.S));
				Add(vxKeyBindingID.Left, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Left], Keys.Q));
				Add(vxKeyBindingID.Right, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Right], Keys.D));

				Add(vxKeyBindingID.Jump, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Jump], Keys.Space));
				Add(vxKeyBindingID.Croutch, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Croutch], Keys.LeftControl));

				Add(vxKeyBindingID.Interact1, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Interact1], Keys.A));
				Add(vxKeyBindingID.Interact2, new vxKeyBinding(Engine.Language[vxLocalisationKey.KeyName_Interact2], Keys.E));
				break;
		}
	}

	public void Add(object id, vxKeyBinding NewKeyBinding)
	{
		Bindings.Add(id, NewKeyBinding);
	}

	public Keys Get(object id)
	{
		return Bindings[id].Key;
	}

	public vxKeyBinding GetBinding(object id)
	{
		return Bindings[id];
	}	}
}
