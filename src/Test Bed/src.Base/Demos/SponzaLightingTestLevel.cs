using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VerticesEngine.Cameras.Controllers;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using VerticesEngine.Cameras;
using VerticesEngine.UI.Controls;
using VerticesEngine.Utilities;
using BEPUphysics.Entities.Prefabs;
using VerticesEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Entities;
using VerticesEngine.Scenes;
using VerticesEngine.Input;

using System.Reflection;

namespace Virtex.App.VerticesTechDemo
{
	/// <summary>
	/// This is a basic sandbox test level which both shows how to set up a basic sandbox
	/// with a number of test entities.
	/// </summary>
    public class SponzaLightingTestLevel : vxGameplayScene3D
	{
		MotorizedGrabSpring grabber;

		float grabDistance = 1;

        public SponzaLightingTestLevel():base(vxStartGameMode.Editor)
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

		}


        public override vxRenderer3D InitialiseRenderingEngine()
		{
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

			character = new CharacterControllerInput(PhyicsSimulation, Camera, Engine);

			//Since this is the character playground, turn on the character by default.
			character.Activate();


			character.CharacterController.Body.Position = new Vector3(10, 0, 10);
			//Having the character body visible would be a bit distracting.
			character.CharacterController.Body.Tag = "noDisplayObject";


			SimulationStart();

			SimulationStop();

			//Grabbers
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


			// Set up Fog
            //Renderer.IsFogEnabled = false;
            //Renderer.FogNear = 20;
            //Renderer.FogFar = Camera.FarPlane / 10;
			SunEmitter.RotationX += 1.5f;


			Vector3 pos = new Vector3(15, 0, -2);


//Envrio envr = new Envrio(Engine,
//	Engine.ContentManager.LoadModel("Models/courtyard/td_courtyard"),
//	new Vector3(0, 0, 0));
//envr.SpecularIntensity = 0.5f;


Envrio envr = new Envrio(this,
	Engine.ContentManager.LoadModel("Models/sponza/sponza"),
	new Vector3(0, 0, 0));
envr.SpecularIntensity = 0.5f;

			int size = 100;
			Box ground = new Box(new Vector3(0, -5.1f, 0), size, 10, size);

			PhyicsSimulation.Add(ground);




            vxInput.IsCursorVisible = true;
		}


		/// <summary>
		/// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="otherScreenHasFocus"></param>
		/// <param name="coveredByOtherScreen"></param>
		public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			// Update Character
			character.Update((float)gameTime.ElapsedGameTime.TotalSeconds, vxInput.PreviousKeyboardState,
	vxInput.KeyboardState, vxInput.GamePadState, vxInput.GamePadState);


			//Update grabber
			if (vxInput.MouseState.RightButton == ButtonState.Pressed && !grabber.IsGrabbing)
			{
				//Find the earliest ray hit
				RayCastResult raycastResult;
				if (PhyicsSimulation.RayCast(new Ray(Cameras[0].Position, Cameras[0].WorldMatrix.Forward), 500, rayCastFilter, out raycastResult))
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
					grabber.GoalPosition = Cameras[0].Position + Cameras[0].WorldMatrix.Forward * grabDistance;
				}
			}

			else if (vxInput.MouseState.RightButton == ButtonState.Released && grabber.IsUpdating)
			{
				grabber.Release();
				//grabberGraphic.IsDrawing = false;
			}
            vxConsole.WriteInGameDebug(this, "Update");
			base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void DrawGameplayScreen(GameTime gameTime)
		{
			base.DrawGameplayScreen(gameTime);

            vxConsole.WriteInGameDebug(this, "Draw");
		}
		//Testing
		public override void SimulationStart()
		{
            vxInput.IsCursorVisible = false;

            if (SandboxCurrentState == vxEnumSandboxStatus.EditMode)
			{
                SandboxCurrentState = vxEnumSandboxStatus.Running;

				// Set the Camera type to chase Camera
				character.Activate();
				Cameras[0].CameraType = CameraType.CharacterFPS;
			}
			base.SimulationStart();
		}



		public override void SimulationStop()
		{
            vxInput.IsCursorVisible = true;
            if (SandboxCurrentState == vxEnumSandboxStatus.Running)
			{
				//Set Working Plane in its original Position
				//workingPlane.Position = Vector3.Up * WrkngPln_HeightDelta;

                SandboxCurrentState = vxEnumSandboxStatus.EditMode;

				character.Deactivate();
				Camera.CameraType = CameraType.Freeroam;
			}
			base.SimulationStop();
		}
	}
}
