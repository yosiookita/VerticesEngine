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
    public class IntroBackground : vxGameplayScene3D
    {

		//
		//Player
		//
		public CharacterControllerInput character;



		#region Picking

		//Motorized Grabber
		protected MotorizedGrabSpring grabber;
		protected float grabDistance;

		//Load in mesh data and create the collision mesh.
		Vector3[] staticTriangleVertices;
		int[] staticTriangleIndices;

		//The raycast filter limits the results retrieved from the Space.RayCast while grabbing.
		Func<BroadPhaseEntry, bool> rayCastFilter;
		bool RayCastFilter(BroadPhaseEntry entry)
		{
			if (character != null)
				return entry != character.CharacterController.Body.CollisionInformation && entry.CollisionRules.Personal <= CollisionRule.Normal;

			else
				return true;
		}

		#endregion

        public IntroBackground():base(vxStartGameMode.GamePlay)
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }

        public override vxRenderer3D InitialiseRenderingEngine()
		{
			// Reset the Renderer
			//return new vxRenderer(this, 128, 1024);
            return new vxRenderer3D(this)
            {
                ShadowMapSize = 1024,
                ShadowBoundingBoxSize = 128
            };
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		public override void LoadContent()
        {
				base.LoadContent();
			      

            ///////////////////////////////////////////////////////////////////////
            //Initialise Camera Code
            ///////////////////////////////////////////////////////////////////////
            #region Set Up Camera


            Camera.CameraType = CameraType.Orbit;
			Camera.OrbitTarget = new Vector3(0,1.5f,0);
			Camera.OrbitZoom = 2375;

            

			//
			//Grabbers
			//
			grabber = new MotorizedGrabSpring();
			PhyicsSimulation.Add(grabber);
			rayCastFilter = RayCastFilter;

			IsPausable = false;

            SunEmitter.RotationX += 1.5f;

            #endregion

            //Renderer.IsFogEnabled = true;
            //Renderer.FogNear = 20;
            //Renderer.FogFar = 50;

			//Envrio envr = new Envrio(Engine,
			//	Engine.ContentManager.LoadModel("Models/homestead/displaymodel", "textures"),
			//	new Vector3(0, 0, 0));
   //         envr.DrawWithOriginalMesh = false;
			//envr.SpecularIntensity = 1;
            //envr.SpecularPower = 0.25f;
            //envr.EmissiveColour = Color.White;

            SkyBox.SkyboxTextureCube = Engine.Game.Content.Load<TextureCube>("Textures/skyboxes/Terra/skybox");
            vxEntity3D spitfire = new vxEntity3D(
           this,
                Engine.ContentManager.LoadModel("Models/Spitfire/Spitfire", "txtrs"),
               new Vector3(0, 0, 0));

            spitfire.WorldTransform *= Matrix.CreateRotationX(MathHelper.PiOver2);
            spitfire.WorldTransform *= Matrix.CreateRotationY(MathHelper.Pi);
            spitfire.WorldTransform *= Matrix.CreateTranslation(new Vector3(0, 1.5f, 0));

            //spitfire.ReflectionMap = DefaultTexture;
            spitfire.ReflectionIntensity = 1.5f;
            spitfire.FresnelBias = 0.051f;
            //spitfire.FresnelBias = 1;
            spitfire.FresnelPower = 3;
            spitfire.DiffuseIntensity = 1;
            spitfire.AmbientLightIntensity = 1;


            WorkingPlane.IsVisible = false;
        }

        float angle = 0;

        int cnt = 0;
		/// <summary>
		/// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
		/// </summary>
		/// <param name="gameTime"></param>
		/// <param name="otherScreenHasFocus"></param>
		/// <param name="coveredByOtherScreen"></param>
		public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
            //cnt++;

            if(cnt == 2)
            {
                vxLoadingScreen.Load(Engine, true, PlayerIndex.One, new BasicSandboxTestLevel());

                //vxLoadingScreen.Load(Engine, true, PlayerIndex.One, new TechDemoSandboxSampleLevel());

                //vxLoadingScreen.Load(Engine, true, PlayerIndex.One, new TechDemoLevelSSRDemo());
                
				//vxLoadingScreen.Load(Engine, true, PlayerIndex.One, new ModelViewer(Engine));
            }

            vxConsole.WriteInGameDebug(this, Camera.OrbitZoom);
            if (vxInput.MouseState.MiddleButton == ButtonState.Pressed) {
				angle = Camera.Yaw;
			} else {
				angle += 0.001f;
				//d.World = Matrix.CreateScale(1.0f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(angle);
				Camera.Yaw = angle;
			}

            base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void DrawScene(GameTime gameTime)
		{
			base.DrawScene(gameTime);
		}
	}
}
