using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Virtex.Lib.Vrtc.Core.Cameras.Controllers;
using Virtex.Lib.Vrtc.Physics.BEPU;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries.MobileCollidables;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Physics.BEPU.Entities.Prefabs;
using Virtex.Lib.Vrtc.Scenes.Sandbox3D;
using Virtex.Lib.Vrtc.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Virtex.App.VerticesTechDemo
{
    /// <summary>
    /// This is the main class for the game. It holds the instances of the sphere simulator,
    /// the arena, the bsp tree, renderer, GUI (Overlay) and player. It contains the main 
    /// game loop, and provides keyboard and mouse input.
    /// </summary>
    public class TechDemoLevelCastle : vxSandboxGamePlay
    {
		
        public TechDemoLevelCastle()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }
		vxLightEntity light;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();


            // Reset the Renderer
            Engine.Renderer = new vxRenderer(Engine, 128, 1024);
            Engine.Renderer.LoadContent();


            // Initialise Camera Code
            #region Set Up Camera

            Camera.CameraType = CameraType.CharacterFPS;

            character = new CharacterControllerInput(BEPUPhyicsSpace, Camera, Engine);

            //Since this is the character playground, turn on the character by default.
            character.Activate();


            character.CharacterController.Body.Position = new Vector3(0, 0, 10);
            //Having the character body visible would be a bit distracting.
            character.CharacterController.Body.Tag = "noDisplayObject";

            SimulationStart();
            SimulationStop();

            //
            //Grabbers
            //
            grabber = new MotorizedGrabSpring();
            BEPUPhyicsSpace.Add(grabber);
            rayCastFilter = RayCastFilter;


            #endregion

            // Set up Fog
            DoFog = true;
			FogNear = 20;
			FogFar = Camera.FarPlane / 20;
            SunEmitter.RotationX += 1.5f;


            Vector3 pos = new Vector3(15, 0, -2);

            Envrio envr=new Envrio(Engine,
               Engine.ContentManager.LoadModel("Models/homestead/displaymodel", "textures"),
               pos);


            //waterItems.Add(new vxWaterEntity(Engine, Vector3.Up, new Vector3(50, 0.25f, 50)));
            
            HeatHaze distorter = new HeatHaze(Engine,
                Engine.ContentManager.LoadDistortionModel("Models/homestead/Forge_Distortion"),
                Engine.Game.Content.Load<Texture2D>("Models/homestead/textures/Forge_Heat"),
                pos + new Vector3(-4.4f, 0, +1.3f), -0.75f);
            
            light = new vxLightEntity (Engine, new Vector3 (0, 2, 0), LightType.Point, Color.WhiteSmoke, 5, 1);



            var cube = new ConcreteCube(Engine, new Vector3(0, 1, 0));
            var cube2 = new ConcreteCube(Engine, new Vector3(-5, 2, 0));
            var crate = new WoodenCrate(Engine, new Vector3(5, 2, 0));

            var crate2 = new WoodenCrate(Engine, new Vector3(5, 2, 5));
            //new vxLightEntity(Engine, new Vector3(5, 2, 0), LightType.Point, Color.White, 5, 1);

           // int size = 100;
           // Box baseBox = new Box(new Vector3(0, -5, 0), size, 10, size);
            //BEPUPhyicsSpace.Add(baseBox);

            //var envr = new Envrio(Engine, Engine.vxContentManager.LoadModel("Models/rflctn srfc/ground"), Vector3.Zero);
            

            //This is a little convenience method used to extract vertices and indices from a model.
            //It doesn't do anything special; any approach that gets valid vertices and indices will work.

#if !TECHDEMO_PLTFRM_GL
            //ModelDataExtractor.GetVerticesAndIndicesFromModel(envr.vxModel.ModelMain, out staticTriangleVertices, out staticTriangleIndices);

            //         //var staticMesh = new StaticMesh(staticTriangleVertices, staticTriangleIndices, new AffineTransform(Matrix3X3.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi), new Vector3(0, -10, 0)));
            //         var staticMesh = new StaticMesh(staticTriangleVertices, staticTriangleIndices,
            //             new AffineTransform(new Vector3(1),
            //                 Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(0)),
            //                 new Vector3(0)));
            //         staticMesh.Sidedness = TriangleSidedness.Counterclockwise;

            //         BEPUPhyicsSpace.Add(staticMesh);
            //         BEPUDebugDrawer.Add(staticMesh);
#endif

            int size = 100;
            Box ground = new Box(new Vector3(0, -5, 0), size, 10, size);

            BEPUPhyicsSpace.Add(ground);
            BEPUDebugDrawer.Add(ground);
            
            vxSlideTabPage Straights = new vxSlideTabPage(Engine, EntityTabControl, "Items");
            EntityTabControl.AddItem(Straights);


            vxScrollPanel ScrollPanel_GeneralItemsPage = new vxScrollPanel(Engine, new Vector2(0, 0),
                Engine.GraphicsDevice.Viewport.Width - 150, Engine.GraphicsDevice.Viewport.Height - 75);

            //Cubes
            ScrollPanel_GeneralItemsPage.AddItem(new vxScrollPanelSpliter(Engine, "Items"));
            ScrollPanel_GeneralItemsPage.AddItem(RegisterNewSandboxItem(WoodenCrate.EntityDescription));
			ScrollPanel_GeneralItemsPage.AddItem(RegisterNewSandboxItem(ConcreteCube.EntityDescription));
			ScrollPanel_GeneralItemsPage.AddItem(RegisterNewSandboxItem(Teapot.EntityDescription));

            //Add the scrollpanel to the slider tab page.
            Straights.AddItem(ScrollPanel_GeneralItemsPage);


			Engine.InputManager.ShowCursor = true;
            rand = new Random();
        }

        Random rand;

        /// <summary>
        /// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Update Character
            character.Update((float)gameTime.ElapsedGameTime.TotalSeconds, Engine.InputManager.PreviousKeyboardState,
    Engine.InputManager.KeyboardState, Engine.InputManager.PreviousGamePadState, Engine.InputManager.GamePadState);

            //foreach(Vector4 vec in Engine.Renderer.ShadowSplitTileBounds)
            //    vxConsole.WriteToInGameDebug("Split Bounds: "+ vec);

            //Update grabber
            if (Engine.InputManager.MouseState.RightButton == ButtonState.Pressed && !grabber.IsGrabbing)
            {
                //Find the earliest ray hit
                RayCastResult raycastResult;
                if (BEPUPhyicsSpace.RayCast(new Ray(Camera.Position, Camera.WorldMatrix.Forward), 500, rayCastFilter, out raycastResult))
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

            if (Engine.InputManager.MouseState.RightButton == ButtonState.Pressed && grabber.IsUpdating)
            {
                if (grabDistance < 4)
                {
                    grabDistance = 3;
                    grabber.GoalPosition = Camera.Position + Camera.WorldMatrix.Forward * grabDistance;
                }
            }

            else if (Engine.InputManager.MouseState.RightButton == ButtonState.Released && grabber.IsUpdating)
            {
                grabber.Release();
                //grabberGraphic.IsDrawing = false;
            }
			vxConsole.WriteToInGameDebug ("Update");
            base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

		public override void DrawGameplayScreen (GameTime gameTime)
		{
			base.DrawGameplayScreen (gameTime);

			vxConsole.WriteToInGameDebug ("Draw");
		}
        //Testing
        public override void SimulationStart()
        {
            Engine.InputManager.ShowCursor = false;

            if (SandboxGameState == vxEnumSandboxGameState.EditMode)
            {
                SandboxGameState = vxEnumSandboxGameState.Running;

                // Set the Camera type to chase Camera
                character.Activate();
                Camera.CameraType = CameraType.CharacterFPS;
            }
            base.SimulationStart();
        }
        

        public override void SimulationStop()
        {
            Engine.InputManager.ShowCursor = true;
            if (SandboxGameState == vxEnumSandboxGameState.Running)
            {
                //Set Working Plane in its original Position
                workingPlane.Position = Vector3.Up * WrkngPln_HeightDelta;

                SandboxGameState = vxEnumSandboxGameState.EditMode;

                character.Deactivate();
                Camera.CameraType = CameraType.Freeroam;
            }
            base.SimulationStop();
        }
    }
}
