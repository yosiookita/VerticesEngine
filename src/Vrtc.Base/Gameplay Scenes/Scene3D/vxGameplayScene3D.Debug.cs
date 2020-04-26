

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using VerticesEngine.Diagnostics;

#endregion

namespace VerticesEngine
{
    public partial class vxGameplayScene3D : vxGameplaySceneBase
    {

        //int lineDist;
        //SpriteFont debugfont;

        public RasterizerState rs_wire = new RasterizerState() { FillMode = FillMode.WireFrame, };
        public RasterizerState rs_solid = new RasterizerState() { FillMode = FillMode.Solid, };
        /*
		public void DrawDebug(vxCamera3D Camera)
		{
			lineDist = vxInternalAssets.Fonts.DebugFont.LineSpacing + 2;
			debugfont = vxInternalAssets.Fonts.DebugFont;

			// If needed, Render Debug Code at the end
			switch (SceneDebugDisplayMode)
			{
				// Draws a Tesselated View of the Current Scene
				// *****************************************************************************
				case vxEnumSceneDebugMode.MeshTessellation:

					Engine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.LightGray, 1.0f, 0);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;

					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

					// First Draw it without the Wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderDebugWire(Camera, false);

                    

                    Engine.GraphicsDevice.RasterizerState = rs_wire;

					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
					//Engine.GraphicsDevice.BlendState = BlendState.Additive;

					// Then set the fillmode to wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderDebugWire(Camera, true);

					Engine.GraphicsDevice.RasterizerState = rs_solid;

					//Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
					//Engine.GraphicsDevice.BlendState = BlendState.Additive;

					// Draw Debug Text
					AddDebugString("Mesh Tessellation");
					AddDebugString("Number of Primitives Drawn: " + Engine.GlobalPrimitiveCount);
					DrawDebugStrings();

					break;


				// Draw a Wire Frame Mesh of the Current View
				// *****************************************************************************
				case vxEnumSceneDebugMode.TexturedWireFrame:

					Engine.GraphicsDevice.Clear(Color.Black);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;
					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

					Engine.GraphicsDevice.RasterizerState = rs_wire;

					SkyBox.Enabled = false;
					// First Draw it without the Wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderDebugWire(Camera,true, true);
						//entity.RenderMesh(Camera);
					

					SkyBox.Enabled = true;
					Engine.GraphicsDevice.RasterizerState = rs_solid;


					// Draw Debug Text
					AddDebugString("Textured Wireframe");
					DrawDebugStrings();

					break;


				// Draw a Wire Frame Mesh of the Current View
				// *****************************************************************************
				case vxEnumSceneDebugMode.WireFrame:

					Engine.GraphicsDevice.Clear(Color.Gray);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;
					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

					Engine.GraphicsDevice.RasterizerState = rs_wire;
					float col = 0.2f;
					// First Draw it without the Wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderDebugWire(Camera, true, new Color(col, col, col, 1));

					Engine.GraphicsDevice.RasterizerState = rs_solid;

					// Draw Debug Text
					AddDebugString("Wireframe");
					DrawDebugStrings();


					break;

				// Draws the Normal Map
				// *****************************************************************************
				case vxEnumSceneDebugMode.NormalMap:

					Engine.GraphicsDevice.Clear(Color.Black);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;
					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

					Engine.SpriteBatch.Begin("Debug - Normal Map");
					Engine.SpriteBatch.Draw(RenderTargets.NormalMap, Engine.GraphicsDevice.Viewport.Bounds, Color.White);
					Engine.SpriteBatch.End();

					// Draw Debug Text
					AddDebugString("Normal Map");
					DrawDebugStrings();

					break;

				// Draws the Depth Map
				// *****************************************************************************
				case vxEnumSceneDebugMode.DepthMap:

					Engine.GraphicsDevice.Clear(Color.Black);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;
					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


					Engine.SpriteBatch.Begin("Drawing Depth Map To Scene", 0, BlendState.Opaque, SamplerState.PointClamp, null, null);
					Engine.SpriteBatch.Draw(RenderTargets.DepthMap, Engine.GraphicsDevice.Viewport.Bounds, Color.White);

					Engine.SpriteBatch.End();

					// Draw Debug Text
					AddDebugString("Depth Map");
					DrawDebugStrings();
					break;


				// Draws the Light Map
				// *****************************************************************************
				case vxEnumSceneDebugMode.LightMap:

					Engine.GraphicsDevice.Clear(Color.Black);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;
					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

					Engine.SpriteBatch.Begin("Debug - Light Map");
					Engine.SpriteBatch.Draw(RenderTargets.LightMap, Engine.GraphicsDevice.Viewport.Bounds, Color.White);
					Engine.SpriteBatch.End();


					Engine.GraphicsDevice.RasterizerState = rs_wire;
					float coll = 0.35f;
					// First Draw it without the Wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderDebugWire(Camera, true, new Color(coll, coll, coll, 1));

					Engine.GraphicsDevice.RasterizerState = rs_solid;

					// Draw Debug Text
					AddDebugString("Scene Light Map");
					AddDebugString("Number of Light Entities: " + LightItems.Count);
					DrawDebugStrings();

					break;


				// Draws the Cascade Shadow Map Debug
				// *****************************************************************************

				case vxEnumSceneDebugMode.BlankShadow:

					Engine.GraphicsDevice.Clear(Color.DarkSlateGray);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;
					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

					//Console.WriteLine("BlankShadow");
					// First Draw it without the Wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderMeshShadowDebug(Camera, false, true);

					// Draw Debug Text
					AddDebugString("Cascade Shadow Map Breakdown - " + SceneDebugDisplayMode);
					AddDebugString("Number of Shadow Cascade Shadow Maps: " + (int)Engine.Settings.Graphics.Shadows.Quality);
					AddDebugString("Shadow Map Texture Size: " + RenderTargets.ShadowMap.Width + "x" + RenderTargets.ShadowMap.Height + " pxs");
					AddDebugString("Shadow Zone Size: " + Renderer.ShadowBoundingBoxSize.ToString());
					DrawDebugStrings();



					break;

				case vxEnumSceneDebugMode.SplitColors:
				case vxEnumSceneDebugMode.BlockPattern:

					Engine.GraphicsDevice.Clear(Color.DarkSlateGray);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;
					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


					// First Draw it without the Wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderMeshShadowDebug(Camera, (SceneDebugDisplayMode == vxEnumSceneDebugMode.BlockPattern));



					Engine.GraphicsDevice.RasterizerState = rs_wire;

					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

					// Then set the fillmode to wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderDebugWire(Camera, true, Color.Gray * 0.25f);

					Engine.GraphicsDevice.RasterizerState = rs_solid;

					// Draw Debug Text
					AddDebugString("Cascade Shadow Map Breakdown - " + SceneDebugDisplayMode);
					AddDebugString("Number of Shadow Cascade Shadow Maps: " + (int)Engine.Settings.Graphics.Shadows.Quality);
					AddDebugString("Shadow Map Texture Size: " + RenderTargets.ShadowMap.Width + "x" + RenderTargets.ShadowMap.Height + " pxs");
					AddDebugString("Shadow Zone Size: " + Renderer.ShadowBoundingBoxSize.ToString());
					DrawDebugStrings();
					break;




                // Draw a Wire Frame Mesh of the Current View
                // *****************************************************************************
                case vxEnumSceneDebugMode.SSAO:
                    Engine.GraphicsDevice.Clear(Color.Black);
                    Engine.GraphicsDevice.BlendState = BlendState.Opaque;
                    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    Engine.SpriteBatch.Begin("Debug - SSAO");
                    Engine.SpriteBatch.Draw(Renderer.SSAOPostProcess.SSAOStippleMap, Engine.GraphicsDevice.Viewport.Bounds, Color.White);
                    Engine.SpriteBatch.End();

                    // Draw Debug Text
                    AddDebugString("Screen Space Ambient Occulusions");
                    DrawDebugStrings();
                    break;

                // Draw a Wire Frame Mesh of the Current View
                // *****************************************************************************
                case vxEnumSceneDebugMode.SSRUVs:
                    Engine.GraphicsDevice.Clear(Color.Black);
                    Engine.GraphicsDevice.BlendState = BlendState.Opaque;
                    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    Engine.SpriteBatch.Begin("Debug - SSR UVs");
                    Engine.SpriteBatch.Draw(Renderer.SSRPostProcess.SSRUVCoordMap, Engine.GraphicsDevice.Viewport.Bounds, Color.White);
                    Engine.SpriteBatch.End();

                    // Draw Debug Text
                    AddDebugString("Screen Space Ambient Occulusions");
                    DrawDebugStrings();
                    break;

                // Draw a Wire Frame Mesh of the Current View
                // *****************************************************************************
                case vxEnumSceneDebugMode.PhysicsDebug:

					Engine.GraphicsDevice.Clear(Color.DimGray);
					Engine.GraphicsDevice.BlendState = BlendState.Opaque;
					Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

					PhysicsDebugViewer.Update();
					PhysicsDebugViewer.Draw(Camera.View, Camera.Projection);

					Engine.GraphicsDevice.RasterizerState = rs_wire;
					Color color = new Color(0.21f, 0.21f, 0.21f, 1);

					// First Draw it without the Wireframe
					foreach (vxEntity3D entity in Entities)
						entity.RenderDebugWire(Camera, true, color);

					Engine.GraphicsDevice.RasterizerState = rs_solid;

					// Draw Debug Text
					AddDebugString("Physics Bodies");
					AddDebugString("# of Physics Entities: " + PhyicsSimulation.Entities.Count);
					DrawDebugStrings();


					break;
			}

		}
        */

        public override void DrawPhysicsDebug(vxCamera camera)
        {
            PhysicsDebugViewer.Update();
            PhysicsDebugViewer.Draw(camera.View, camera.Projection);
        }

        void UpdateDebug()
        {

            if (vxDebug.IsDebugMeshVisible)
                PhysicsDebugViewer.Update();
            /*
			// check for other keys pressed on keyboard
			if (vxInput.IsNewKeyPress(Keys.OemCloseBrackets))
			{

				var previousSceneShadowMode = SceneDebugDisplayMode;
				SceneDebugDisplayMode = vxUtil.NextEnumValue(SceneDebugDisplayMode);

				if (previousSceneShadowMode < vxEnumSceneDebugMode.BlockPattern && SceneDebugDisplayMode >= vxEnumSceneDebugMode.BlockPattern ||

					previousSceneShadowMode >= vxEnumSceneDebugMode.BlockPattern && SceneDebugDisplayMode < vxEnumSceneDebugMode.BlockPattern)
				{
					Renderer.swapShadowMapWithBlockTexture();
				}

				foreach (vxEntity3D entity in Entities)
					entity.renderShadowSplitIndex = SceneDebugDisplayMode >= vxEnumSceneDebugMode.SplitColors;
			}

			if (vxInput.IsNewKeyPress(Keys.OemOpenBrackets))
			{

				var previousSceneShadowMode = SceneDebugDisplayMode;
				SceneDebugDisplayMode = vxUtil.PreviousEnumValue(SceneDebugDisplayMode);

				if (previousSceneShadowMode < vxEnumSceneDebugMode.BlockPattern && SceneDebugDisplayMode >= vxEnumSceneDebugMode.BlockPattern ||

					previousSceneShadowMode >= vxEnumSceneDebugMode.BlockPattern && SceneDebugDisplayMode < vxEnumSceneDebugMode.BlockPattern)
				{
					Renderer.swapShadowMapWithBlockTexture();
				}

				foreach (vxEntity3D entity in Entities)
					entity.renderShadowSplitIndex = SceneDebugDisplayMode >= vxEnumSceneDebugMode.SplitColors;
			}

			Renderer.mSnapShadowMaps = true;
			if (vxInput.IsNewKeyPress(Keys.F))
			{
				Renderer.mSnapShadowMaps = !Renderer.mSnapShadowMaps;
			}
			//vxConsole.WriteToInGameDebug("f:" + Engine.Renderer.mSnapShadowMaps); 

			if (vxInput.IsNewKeyPress(Keys.T))
			{
				foreach (vxEntity3D entity in Entities)
					entity.IsTextureEnabled = !entity.IsTextureEnabled;
			}
            */
        }





        public override void DrawDebug(GameTime gameTime)
        {
            base.DrawDebug(gameTime);

            foreach (vxCamera3D camera in Cameras)
            {
                //DrawDebug(camera);
                vxDebug.DrawShapes(gameTime, camera.View, camera.Projection);

            }

        }
    }
}

