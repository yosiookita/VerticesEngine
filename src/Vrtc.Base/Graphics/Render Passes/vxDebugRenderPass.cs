
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using VerticesEngine.Input;
using VerticesEngine.Utilities;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// Grab a scene that has already been rendered, 
    /// and add a distortion effect over the top of it.
    /// </summary>
    public class vxDebugRenderPass : vxRenderPass, vxIRenderPass
    {
       
        vxMainScene3DRenderPass mainPass;

        vxScenePrepRenderingPass prepPass;

        vxCascadeShadowRenderPass shadowPass;

        vxEnumSceneDebugMode SceneDebugDisplayMode = vxEnumSceneDebugMode.Default;

        public RenderTarget2D DebugMap;

        public vxDebugRenderPass(vxRenderer Renderer) :
        base(Renderer, "Debug Render Pass", vxInternalAssets.PostProcessShaders.DistortSceneEffect)
        {
            mainPass = Renderer.GetRenderingPass<vxMainScene3DRenderPass>();
            prepPass = Renderer.GetRenderingPass<vxScenePrepRenderingPass>();
            shadowPass = Renderer.GetRenderingPass<vxCascadeShadowRenderPass>();
        }


        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();
            //vxInternalAssets.PostProcessShaders.DistortSceneEffect.Parameters["MatrixTransform"].SetValue(MatrixTransform);

            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            DebugMap = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);

        }

        public void Update()
        {

            // check for other keys pressed on keyboard
            if (vxInput.IsNewKeyPress(Keys.OemCloseBrackets))
            {
                var previousSceneShadowMode = SceneDebugDisplayMode;
                SceneDebugDisplayMode = vxUtil.NextEnumValue(SceneDebugDisplayMode);

                //if (previousSceneShadowMode < vxEnumSceneDebugMode.BlockPattern && SceneDebugDisplayMode >= vxEnumSceneDebugMode.BlockPattern ||

                //    previousSceneShadowMode >= vxEnumSceneDebugMode.BlockPattern && SceneDebugDisplayMode < vxEnumSceneDebugMode.BlockPattern)
                //{
                //    //Renderer.swapShadowMapWithBlockTexture();
                //}

                //foreach (vxEntity3D entity in Entities)
                //    entity.renderShadowSplitIndex = SceneDebugDisplayMode >= vxEnumSceneDebugMode.SplitColors;
            }

            if (vxInput.IsNewKeyPress(Keys.OemOpenBrackets))
            {
                var previousSceneShadowMode = SceneDebugDisplayMode;
                SceneDebugDisplayMode = vxUtil.PreviousEnumValue(SceneDebugDisplayMode);

                //if (previousSceneShadowMode < vxEnumSceneDebugMode.BlockPattern && SceneDebugDisplayMode >= vxEnumSceneDebugMode.BlockPattern ||

                //    previousSceneShadowMode >= vxEnumSceneDebugMode.BlockPattern && SceneDebugDisplayMode < vxEnumSceneDebugMode.BlockPattern)
                //{
                //    Renderer.swapShadowMapWithBlockTexture();
                //}

                //foreach (vxEntity3D entity in Entities)
                //    entity.renderShadowSplitIndex = SceneDebugDisplayMode >= vxEnumSceneDebugMode.SplitColors;
            }

        }

        public void Prepare()
        {
            switch (SceneDebugDisplayMode)
            {
                case vxEnumSceneDebugMode.PhysicsDebug:

                    Engine.GraphicsDevice.SetRenderTarget(DebugMap);
                    Engine.GraphicsDevice.Clear(Color.Black);
                    Engine.GraphicsDevice.BlendState = BlendState.Opaque;
                    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    Engine.CurrentScene.DrawPhysicsDebug(Renderer.Camera);

                    break;

                case vxEnumSceneDebugMode.SplitColors:

                    Engine.GraphicsDevice.SetRenderTarget(DebugMap);
                    Engine.GraphicsDevice.Clear(Color.Black);
                    Engine.GraphicsDevice.BlendState = BlendState.Opaque;
                    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    shadowPass.DrawDebug();

                    break;
            }
        }

        public RasterizerState rs_wire = new RasterizerState() { FillMode = FillMode.WireFrame, };
        public RasterizerState rs_solid = new RasterizerState() { FillMode = FillMode.Solid, };

        public void Apply()
        {
            switch (SceneDebugDisplayMode)
            {
                case vxEnumSceneDebugMode.EncodedIndex:

                    Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    Engine.SpriteBatch.Begin("Physics Debug Call");
                    Engine.SpriteBatch.Draw(Renderer.EncodedIndexResult, Viewport.Bounds, Color.White);
                    Engine.SpriteBatch.End();

                    DrawSceneWireFrame(Color.WhiteSmoke*0.025f);

                    // Draw Debug Text
                    AddDebugString("Encoded Index");
                    //AddDebugString("# of Physics Entities: " + PhyicsSimulation.Entities.Count);
                    DrawDebugStrings();
                    break;

                case vxEnumSceneDebugMode.SplitColors:

                    Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    Engine.SpriteBatch.Begin("Split Colours");
                    Engine.SpriteBatch.Draw(DebugMap, DebugMap.Bounds, Color.White * 0.75f);
                    Engine.SpriteBatch.End();

                    DrawSceneWireFrame(Color.WhiteSmoke * 0.025f);

                    // Draw Debug Text
                    AddDebugString("Split Colours");
                    //AddDebugString("# of Physics Entities: " + PhyicsSimulation.Entities.Count);
                    DrawDebugStrings();
                    break;


                case vxEnumSceneDebugMode.PhysicsDebug:

                    Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    Engine.SpriteBatch.Begin("Physics Debug Call");
                    Engine.SpriteBatch.Draw(DebugMap, DebugMap.Bounds, Color.White * 0.5f);
                    Engine.SpriteBatch.End();

                    DrawSceneWireFrame(Color.WhiteSmoke*0.125f);

                    // Draw Debug Text
                    AddDebugString("Physics Bodies");
                    //AddDebugString("# of Physics Entities: " + PhyicsSimulation.Entities.Count);
                    DrawDebugStrings();
                    break;
            }

        }

        List<string> DebugText = new List<string>();

        void DrawSceneWireFrame(Color wireColour)
        {
            Engine.GraphicsDevice.RasterizerState = rs_wire;

            for (int i = 0; i < Renderer.totalItemsToDraw; i++)
            {
                vxEntity3D entity = (vxEntity3D)Engine.CurrentScene.Entities[Renderer.drawList[i]];
                if (entity.IsEntityDrawable)
                {
                    Matrix TempWorld = entity.WorldTransform;// * Renderer.Camera.ViewProjection;
                    Matrix TempWVP = entity.WorldTransform * Renderer.Camera.ViewProjection;

                    for (int mi = 0; mi < entity.Model.Meshes.Count; mi++)
                    {
                        var mesh = entity.Model.Meshes[mi];
                        mesh.Material.DebugEffect.DoDebugWireFrame = true;
                        mesh.Material.DebugEffect.WireColour = wireColour;
                        //mesh.Material.DebugEffect.CurrentTechnique = entity.Model.Meshes[mi].Material.DebugEffect.Techniques["Technique_PrepPass"];
                        mesh.Material.DebugEffect.World = TempWorld;
                        mesh.Material.DebugEffect.WVP = TempWVP;

                        mesh.Draw(mesh.Material.DebugEffect);
                    }
                }
            }
            Engine.GraphicsDevice.RasterizerState = rs_solid;
        }

        public void AddDebugString(string text)
        {
            DebugText.Add(text);
        }

        public void DrawDebugStrings()
        {
            Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Vector2 Padding = new Vector2(1);
            Vector2 Position = new Vector2(5);

            Engine.SpriteBatch.Begin("Debug - Gameplay3D Text");

            // Now loop through all text and draw it
            foreach (string text in DebugText)
            {
                Engine.SpriteBatch.DrawString(vxInternalAssets.Fonts.DebugFont, text, Position + Padding, Color.Black);
                Engine.SpriteBatch.DrawString(vxInternalAssets.Fonts.DebugFont, text, Position, Color.White);
                Position += new Vector2(0, vxInternalAssets.Fonts.DebugFont.LineSpacing);
            }
            DebugText.Clear();

            Engine.SpriteBatch.End();
        }
    }
}
