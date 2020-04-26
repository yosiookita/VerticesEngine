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


namespace Virtex.App.VerticesTechDemo
{
	/// <summary>
	/// This is a FPS demo which is situated at a Mideval Village area which takes
	/// inspiriation from the Witcher and Skyrim.
	/// It's meant to showcase Alpha transparencies and enviroments.
	/// </summary>
	public class FPSVillageTestBed : vxGameplayScene3D
	{


		// Physics Player Input
		public CharacterControllerInput Character;

		#region Picking

		//Motorized Grabber
		protected MotorizedGrabSpring grabber;

		protected float grabDistance;

		//The raycast filter limits the results retrieved from the Space.RayCast while grabbing.
		Func<BroadPhaseEntry, bool> rayCastFilter;
		bool RayCastFilter(BroadPhaseEntry entry)
		{
			if (Character != null)
				return entry != Character.CharacterController.Body.CollisionInformation && entry.CollisionRules.Personal <= CollisionRule.Normal;

			else
				return true;
		}

		#endregion

        public FPSVillageTestBed():base(vxStartGameMode.GamePlay)
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

		}

        public override vxRenderer3D InitialiseRenderingEngine()
		{
			// Reset the Renderer
            return new vxRenderer3D(this)
            {
                ShadowMapSize = 1024,
                ShadowBoundingBoxSize = 128
            };
		}

		public override void InitialiseCameras()
		{
			base.InitialiseCameras();

			Camera.CameraType = CameraType.CharacterFPS;

			Camera.Position = new Vector3(0, 5, -5);

			Character = new CharacterControllerInput(PhyicsSimulation, Camera, Engine);

			//Since this is the character playground, turn on the character by default.
			Character.Activate();

			//Having the character body visible would be a bit distracting.
			Character.CharacterController.Body.Tag = "noDisplayObject";


			// Setup Grabbers
			grabber = new MotorizedGrabSpring();
			PhyicsSimulation.Add(grabber);
			rayCastFilter = RayCastFilter;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		public override void LoadContent()
		{
			base.LoadContent();

			int size = 100;
			Box baseBox = new Box(new Vector3(0, -5, 0), size, 10, size);
			PhyicsSimulation.Add(baseBox);

			Envrio envr = new Envrio(this,
				Engine.ContentManager.LoadModel("Models/homestead/displaymodel", "textures"),
				new Vector3(0, 0, 0));
            envr.DrawWithOriginalMesh = false;
			envr.SpecularIntensity = 0.5f;

		}



		/// <summary>
		/// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="otherScreenHasFocus"></param>
		/// <param name="coveredByOtherScreen"></param>
		public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			Character.Update((float)gameTime.ElapsedGameTime.TotalSeconds, vxInput.PreviousKeyboardState,
	vxInput.KeyboardState, vxInput.GamePadState, vxInput.GamePadState);

			if (this.IsActive)
                vxInput.IsCursorVisible = false;
			else
                vxInput.IsCursorVisible = true;

			//Update grabber
			if (vxInput.MouseState.RightButton == ButtonState.Pressed && !grabber.IsGrabbing)
			{
				//Find the earliest ray hit
				RayCastResult raycastResult;
				if (PhyicsSimulation.RayCast(new Ray(Camera.Position, Camera.WorldMatrix.Forward), 500, rayCastFilter, out raycastResult))
				{
					var entityCollision = raycastResult.HitObject as EntityCollidable;
					//If there's a valid ray hit, then grab the connected object!
					if (entityCollision != null && entityCollision.Entity.IsDynamic)
					{
						Console.WriteLine("GRABBING ITEM: {0}", entityCollision.Entity.GetType().ToString());
						grabber.Setup(entityCollision.Entity, raycastResult.HitData.Location);
						//grabberGraphic.IsDrawing = true;
						grabDistance = raycastResult.HitData.T;
					}
				}
			}

			if (vxInput.MouseState.RightButton == ButtonState.Pressed && grabber.IsUpdating)
			{
				if (grabDistance < 4)
				{
					grabDistance = 3;
					grabber.GoalPosition = Camera.Position + Camera.WorldMatrix.Forward * grabDistance;
				}
			}

			else if (vxInput.MouseState.RightButton == ButtonState.Released && grabber.IsUpdating)
			{
				grabber.Release();
			}

			base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}
	}
}
