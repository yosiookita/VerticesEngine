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
public class vxKeyBinding
{
	public string Name = "Key Name";

	public Keys Key;

	public vxKeyBinding(string Name, Keys Key)
	{
		this.Name = Name;
		this.Key = Key;
	}
}
}
