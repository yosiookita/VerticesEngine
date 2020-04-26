using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using System.Linq;

//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core.Scenes;
using Virtex.Lib.Vrtc.Core.Cameras.Controllers;
using Virtex.Lib.Vrtc.Core.Input;
using Virtex.Lib.Vrtc.Screens.Menus;
using Virtex.Lib.Vrtc.Physics.BEPU;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries;
using Virtex.Lib.Vrtc.Physics.BEPU.BroadPhaseEntries.MobileCollidables;
using BEPUutilities;
using Virtex.Lib.Vrtc.Physics.BEPU.CollisionRuleManagement;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.GUI.Controls;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Physics.BEPU.Entities.Prefabs;
using Virtex.Lib.Vrtc.Scenes.Sandbox3D;
using Virtex.Lib.Vrtc.Entities.Sandbox3D;
using Virtex.Lib.Vrtc.Graphics;
using Virtex.Lib.Vrtc.Core.Entities;

namespace Virtex.App.VerticesTechDemo
{
    /// <summary>
    /// This is the main class for the game. It holds the instances of the sphere simulator,
    /// the arena, the bsp tree, renderer, GUI (Overlay) and player. It contains the main 
    /// game loop, and provides keyboard and mouse input.
    /// </summary>
    public class TechDemoLevelCastle2 : vxSandboxGamePlay
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
		
        public TechDemoLevelCastle2()
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
            InitialiseLevel();


            ///////////////////////////////////////////////////////////////////////
            //Initialise Camera Code
            ///////////////////////////////////////////////////////////////////////
            #region Set Up Camera

            base.LoadContent();
            Camera.CameraType = CameraType.CharacterFPS;

            character = new CharacterControllerInput(BEPUPhyicsSpace, Camera, Engine);

            //Since this is the character playground, turn on the character by default.
            character.Activate();

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

            DoFog = true;
			FogNear = 20;
			FogFar = Camera.FarPlane / 4;

			Envrio envr = new Envrio(Engine, Engine.vxContentManager.LoadModel("Models/courtyard/td_courtyard"), Vector3.Zero);

            //Envrio envr = new Envrio(Engine, Engine.vxContentManager.LoadModel("Models/castle/mdl_castle"), new Vector3(0, 1, 0));

            //waterItems.Add(new vxWaterEntity(Engine, Vector3.Up, new Vector3(50, 0.25f, 50)));
            

            envr.SpecularIntensity = 1;
            //envr.SpecularIntensity = 100;
            //envr.SpecularPower = 5f;
            //envr.DoFog = false;

            light = new vxLightEntity (Engine, new Vector3 (0, 2, 0), LightType.Point, Color.WhiteSmoke, 2, 1);

            //This is a little convenience method used to extract vertices and indices from a model.
            //It doesn't do anything special; any approach that gets valid vertices and indices will work.
            
			#if !TECHDEMO_PLTFRM_GL
			ModelDataExtractor.GetVerticesAndIndicesFromModel(envr.vxModel.ModelMain, out staticTriangleVertices, out staticTriangleIndices);

            //var staticMesh = new StaticMesh(staticTriangleVertices, staticTriangleIndices, new AffineTransform(Matrix3X3.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi), new Vector3(0, -10, 0)));
            var staticMesh = new StaticMesh(staticTriangleVertices, staticTriangleIndices,
                new AffineTransform(new Vector3(1),
                    Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(0)),
                    new Vector3(0)));
            staticMesh.Sidedness = TriangleSidedness.Counterclockwise;

            BEPUPhyicsSpace.Add(staticMesh);
            BEPUDebugDrawer.Add(staticMesh);
			#endif

            int size = 100;
			BEPUPhyicsSpace.Add(new Box (new Vector3(0, -5, 0), size, 10, size));

            vxTabPage Straights = new vxTabPage(Engine, EntityTabControl, "Items");
            EntityTabControl.AddItem(Straights);

            vxScrollPanel ScrollPanel_GeneralItemsPage = new vxScrollPanel(new Vector2(0, 0),
                Engine.GraphicsDevice.Viewport.Width - 150, Engine.GraphicsDevice.Viewport.Height - 75);

            //Cubes
            ScrollPanel_GeneralItemsPage.AddItem(new vxScrollPanelSpliter(Engine, "Items"));
            ScrollPanel_GeneralItemsPage.AddItem(RegisterNewSandboxItem(WoodenCrate.EntityDescription));
			ScrollPanel_GeneralItemsPage.AddItem(RegisterNewSandboxItem(ConcreteCube.EntityDescription));
			ScrollPanel_GeneralItemsPage.AddItem(RegisterNewSandboxItem(Teapot.EntityDescription));

            //Add the scrollpanel to the slider tab page.
            Straights.AddItem(ScrollPanel_GeneralItemsPage);

            //IndexedCubeTest cube = new IndexedCubeTest(Engine, new Vector3(4, 4, 0));

            
            Teapot t = new Teapot(Engine, new Vector3(4, 4, 0));
            t.SetMesh(Matrix.CreateTranslation(new Vector3(4, 2, 0)), true, true);
            
            ConcreteCube cc = new ConcreteCube(Engine, new Vector3(0, 5, 0));
            cc.SetMesh(Matrix.CreateTranslation(new Vector3(0, 2, 0)), true, true);
            

            ModelObjs mo = new ModelObjs(Engine, new Vector3(-4, 4, 0));
            mo.SetMesh(Matrix.CreateTranslation(new Vector3(0, 2, 8)), true, true);

			Engine.InputManager.ShowCursor = true;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        /*
        public override vxSandboxEntity GetNewEntity(string key)
        {
			vxSandboxEntity returnEntity = null;
            switch (key)
            {
                //Cubes
                case "Virtex.App.VerticesTechDemo.WoodenCrate":
				returnEntity = new WoodenCrate((GameEngine)Engine, Vector3.Zero);
                    break;

                default:
                    returnEntity = base.GetNewEntity(key);
                    break;
			}
			return returnEntity;
        }
        */

        /// <summary>
        /// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            character.Update((float)gameTime.ElapsedGameTime.TotalSeconds, Engine.InputManager.PreviousKeyboardState,
    Engine.InputManager.KeyboardState, Engine.InputManager.PreviousGamePadState, Engine.InputManager.GamePadState);
			light.Position = this.Camera.Position;
            //if (Engine.InputManager.MouseState.MiddleButton == ButtonState.Pressed)
            //    Mouse.SetPosition((int)Engine.Mouse_ClickPos.X, (int)Engine.Mouse_ClickPos.Y);

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
