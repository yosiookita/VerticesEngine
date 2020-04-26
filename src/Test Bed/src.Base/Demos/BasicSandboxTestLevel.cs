using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VerticesEngine.Controllers;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using VerticesEngine.UI.Controls;
using VerticesEngine.Utilities;
using BEPUphysics.Entities.Prefabs;
using VerticesEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Input;
using VerticesEngine.UI.Dialogs;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using VerticesEngine.Util;
using VerticesEngine.Mathematics;
using VerticesEngine;

namespace Virtex.App.VerticesTechDemo
{
    public enum SandboxCategories
    {
        GridItems,
        RealWorldItems
    }


    public enum SandboxSubCategories
    {
        GridItems,
        ConstructionItems,
        NaturalItems,
    }

    /// <summary>
    /// This is a basic sandbox test level which both shows how to set up a basic sandbox
    /// with a number of test entities.
    /// </summary>
    public class BasicSandboxTestLevel : vxGameplayScene3D
	{
		MotorizedGrabSpring grabber;
        
		float grabDistance = 1;

        public BasicSandboxTestLevel()
            : base(vxStartGameMode.Editor)
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
                ShadowBoundingBoxSize = 256
            };
        }


        public override void InitialiseCameras()
		{
			base.InitialiseCameras();
			Camera.CameraType = CameraType.CharacterFPS;

			character = new CharacterControllerInput(PhyicsSimulation, Camera, Engine);

			//Since this is the character playground, turn on the character by default.
			character.Activate();


			character.CharacterController.Body.Position = new Vector3(-50, 50, 50);
           // Camera.Position = new Vector3(100, 100, 100);
            Camera.Yaw = -MathHelper.PiOver4;
            Camera.Pitch = -MathHelper.PiOver4 * 2 / 3;
            //Having the character body visible would be a bit distracting.
            character.CharacterController.Body.Tag = "noDisplayObject";


			SimulationStart();

			SimulationStop();

			//Grabbers
			grabber = new MotorizedGrabSpring();
			PhyicsSimulation.Add(grabber);
			rayCastFilter = RayCastFilter;

		}
        DrivableVehicle vehicle;
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            Console.WriteLine("loading...");
			base.LoadContent();

            Console.WriteLine("inital");


            SkyBox.SkyboxTextureCube = Engine.Game.Content.Load<TextureCube>("Textures/skyboxes/Terra/skybox");
            
			SunEmitter.RotationX += 1.5f;




            Vector3 pos = new Vector3(15, 0, -2);

            ArenaEntity arena = new ArenaEntity(
            this,
                Engine.ContentManager.LoadModel("Models/arena/arena", "txtrs"),
                new Vector3(0, 0, 0));
            arena.EmissiveColour = new Color(0, 150, 255);
            arena.DoSSR = true;

			//ReflectionSurface surface = new ReflectionSurface(Engine,
                                                              //Engine.ContentManager.LoadModel("Models/arena/arena", "txtrs"), new Vector3(0, 0, 0));
            //Items.Add(surface);
            new ConcreteCube(this, new Vector3(5, 3, 0));
            vehicle = new DrivableVehicle(this, new Vector3(0, 30, 0));


//            new SciFiCompositieCrate(this, new Vector3(3, 3, 0));

            //            new ConcreteCube(this, new Vector3(-7, 2, -5));

            //            new WoodenCrate(this, new Vector3(5, 2, 2));
            //            new WoodenCrate(this, new Vector3(5, 1, 5));


            //            //for (int i = 0; i < 10; i++)
            //            //    for (int j = 0; j < 10; j++)
            //            //new CompositieCrate(Engine, new Vector3(i, 20, j));
            //            new CompositieCrate(this, new Vector3(5, 3, 4));
            //            new CompositieCrate(this, new Vector3(7, 2, 7));

            //            new SciFiCompositieCrate(this, new Vector3(-7, 2, 5));
            //            new SciFiCompositieCrate(this, new Vector3(-7, 2, 7));
            //            new SciFiCompositieCrate(this, new Vector3(-7, 2, 9));


            //            new GridBallx1(this, new Vector3(5, 2, 5));
            //            new GridCubex2(this, new Vector3(3, 4, 5));
            //            new GridCubex2(this, new Vector3(9, 3, 3));

            //            new AOBlocks(this, new Vector3(9, 10, 3));

            //            //var water = new vxWaterEntity(this, new Vector3(0, 0.25f, 0), Vector3.One*200);

            //            Console.WriteLine("entities");

            //            var tet1 = new GridTetrax2(this, new Vector3(10, 3, 0));
            //            tet1.SpecularPower = 0.25f;
            //            var tet2 = new GridTetrax2(this, new Vector3(15, 3, 0));
            //            tet2.SpecularPower = 0.5f;
            //            var tet3 = new GridTetrax2(this, new Vector3(12, 3, 7));
            //            tet3.SpecularPower = 1;

            //            float h = 2.5f;
            //            float pow = 0.5f;
            //            //vxModel sphereModel = Engine.ContentManager.LoadModel(GridBallx1.EntityDescription.FilePath);
            //            var sph1 = new GridBallx2(this,
            //                                      new Vector3(20, h, 0));
            //            sph1.SpecularPower = pow;
            //            sph1.SpecularIntensity = 1;

            //            var sph2 = new GridBallx2(this,
            //                                      new Vector3(22, h, 0));
            //            sph2.SpecularPower = pow;
            //            sph2.SpecularIntensity = 0.5f;

            //            var sph3 = new GridBallx2(this,
            //                                      new Vector3(24, h, 0));
            //            sph3.SpecularPower = pow;

            //var pool = new ArenaEntity(this,
            //                          Engine.ContentManager.LoadModel("Models/grid/grid_pool/grid_pool"),
            //                new Vector3(0, 0, 0));


            //            //var water = new vxWaterEntity(Engine,  new Vector3(0,2,0), Vector3.One * 40);
            //            var plank = new TrackEntity(this,
            //                new Vector3(-50, 2, 0));

            vxInput.IsCursorVisible = true;


            Console.WriteLine("Loaded");
		}


        bool isInVehicleMode = false;

        public override void UpdateCameraChaseTarget()
        {
            base.UpdateCameraChaseTarget();

            if(Camera.CameraType == CameraType.ChaseCamera)
            {
                
                Camera.ChasePosition = vxMathHelper.Smooth(Camera.ChasePosition, vehicle.WorldTransform.Translation, 1.2f);
                Camera.ChaseDirection = vxMathHelper.Smooth(Camera.ChaseDirection, vehicle.WorldTransform.Forward, 12);
                Camera.Up = vxMathHelper.Smooth(Camera.Up, Vector3.Up, 12);
                Camera.DesiredPositionOffset = new Vector3(0, 5, 12);
                
            }
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
			character.Update((float)gameTime.ElapsedGameTime.TotalSeconds, vxInput.PreviousKeyboardState, vxInput.KeyboardState, vxInput.GamePadState, vxInput.GamePadState);

            if (SandboxCurrentState == vxEnumSandboxStatus.Running)
            {
                // handle Vehicle Mode 
                if(vxInput.IsNewKeyPress(Keys.V))
                {
                    isInVehicleMode = !isInVehicleMode;

                    if (isInVehicleMode)
                    {
                        character.Deactivate();
                        vehicle.Activate();
                        Camera.CameraType = CameraType.ChaseCamera;
                    }
                    else
                    {
                        character.Activate();
                        vehicle.Deactivate();
                        Camera.CameraType = CameraType.CharacterFPS;
                    }
                }

                if (isInVehicleMode)
                {

                }
                else
                {
                    if (vxInput.IsNewMouseButtonPress(MouseButtons.LeftButton))
                    {
                        var ball = new GridBallx1(this, Camera.Position + Camera.WorldMatrix.Forward * 2);
                        ball.OnSandboxStatusChanged(true);
                        ball.PhysicsCollider.ApplyImpulse(Camera.Position,
                                                 Camera.WorldMatrix.Forward * 100 * ball.PhysicsCollider.Mass);

                    }


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
                                Console.WriteLine("GRABBING ITEM: {0}", entityCollision.Entity.GetType());
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
                }
            }
            vxConsole.WriteInGameDebug(this, "Update");
			base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void DrawGameplayScreen(GameTime gameTime)
		{
			base.DrawGameplayScreen(gameTime);

            vxConsole.WriteInGameDebug(this, "Draw");
		}

        public override void DrawHUD()
        {
            base.DrawHUD();

            if (SandboxCurrentState == vxEnumSandboxStatus.Running)
            {
                int sq = 3;
                Rectangle rec = new Rectangle(
                Viewport.Width / 2 - sq / 2, Viewport.Height / 2 - sq / 2, sq, sq);
                //SpriteBatch.Begin();
                SpriteBatch.Draw(DefaultTexture, rec.GetBorder(1), Color.Black);
                SpriteBatch.Draw(DefaultTexture, rec, vxInput.IsMouseButtonPressed(MouseButtons.RightButton) ? Color.DeepSkyBlue : Color.White);
                //SpriteBatch.End();
            }
        }

		//Testing
		public override void SimulationStart()
		{
            IsEncodedIndexNeeded = false;
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
            IsEncodedIndexNeeded = true;
            vxInput.IsCursorVisible = true;
            isInVehicleMode = false;
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
