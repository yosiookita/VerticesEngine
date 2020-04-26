using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Diagnostics;
using System.Linq;

//Virtex vxEngine Declaration
using VerticesEngine.Scenes;
using VerticesEngine.Cameras.Controllers;
using VerticesEngine.Input;
using VerticesEngine.UI.Menus;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUutilities;
using BEPUphysics.CollisionRuleManagement;
using VerticesEngine.Cameras;
using VerticesEngine.Entities;
using VerticesEngine.Utilities;
using BEPUphysics.Entities.Prefabs;
using VerticesEngine.Graphics;
using VerticesEngine.Settings;
using VerticesEngine.Screens.Async;

namespace Virtex.App.VerticesTechDemo
{
	/// <summary>
	/// This is the main class for the game. It holds the instances of the sphere simulator,
	/// the arena, the bsp tree, renderer, GUI (Overlay) and player. It contains the main 
	/// game loop, and provides keyboard and mouse input.
	/// </summary>
    public class IntroBackground2D : vxGameplayScene2D
    {

        public IntroBackground2D():base(vxStartGameMode.GamePlay)
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }

	}
}
