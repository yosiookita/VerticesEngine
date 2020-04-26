﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace VerticesEngine.Graphics
{
    /// <summary>
    /// Intermediate buffer. 
    /// Optionally displays one of the intermediate buffers used 
    /// by the bloom postprocess, so you can see exactly what is
    /// being drawn into each rendertarget.
    /// </summary>
    public enum vxEnumBlurDirection
    {
        /// <summary>
        /// The pre bloom.
        /// </summary>
        PreBloom,

        /// <summary>
        /// The blurred horizontally.
        /// </summary>
        BlurredHorizontally,

        /// <summary>
        /// The blurred both ways.
        /// </summary>
        BlurredBothWays,

        /// <summary>
        /// The final result.
        /// </summary>
        FinalResult,
    }

    public class vxSceneBlurPostProcess : vxRenderPass, vxIRenderPass
    {
        /// <summary>
        /// Controls how much blurring is applied to the bloom image.
        /// The typical range is from 1 up to 10 or so.
        /// </summary>
        public float UtilityBlurAmount = 3;

        public float BlurSizeFactor = 0.5f;

        float[] sampleWeightsX, sampleWeightsY;// = new float[sampleCount];
        Vector2[] sampleOffsetsX, sampleOffsetsY;// = new Vector2[sampleCount];

        int SampleCount = 15;

        public RenderTarget2D BloomTempRenderTarget;

        public vxSceneBlurPostProcess(vxRenderer Renderer) :
        base(Renderer, "Scene Blur", vxInternalAssets.PostProcessShaders.SceneBlurEffect)
        {

            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            BloomTempRenderTarget = new RenderTarget2D(GraphicsDevice,
                (int)(pp.BackBufferWidth * BlurSizeFactor), (int)(pp.BackBufferHeight * BlurSizeFactor),
                false, pp.BackBufferFormat, pp.DepthStencilFormat);

            OnGraphicsRefresh();
            RegisterRenderTargetsForDebug();
        }


        public override void RegisterRenderTargetsForDebug()
        {
            base.RegisterRenderTargetsForDebug();

            Renderer.RegisterDebugRenderTarget("Blur", Renderer.BlurredScene);
            //Renderer.RegisterDebugRenderTarget("Light Map", _deferredLitScene);
            //Renderer.RegisterDebugRenderTarget("Light Map", LightMap);

        }

        public override void OnGraphicsRefresh()
        {
            base.OnGraphicsRefresh();

            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            BloomTempRenderTarget = new RenderTarget2D(GraphicsDevice,
                (int)(pp.BackBufferWidth * BlurSizeFactor), (int)(pp.BackBufferHeight* BlurSizeFactor), 
                false, pp.BackBufferFormat, pp.DepthStencilFormat);


            Renderer.BlurredScene = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat);
            
            //First Setup the Sample Count
            SampleCount = Effect.Parameters["SampleWeights"].Elements.Count;

            sampleWeightsX = new float[SampleCount];
            sampleOffsetsX = new Vector2[SampleCount];

            sampleWeightsY = new float[SampleCount];
            sampleOffsetsY = new Vector2[SampleCount];

            //Now Generate the Sample Weights and Sample Offsets for this given Resolution
            SetBloomEffectParameters(1.0f / (float)BloomTempRenderTarget.Width, 0, UtilityBlurAmount,
                vxEnumBlurDirection.BlurredHorizontally, SampleCount);


            SetBloomEffectParameters(0, 1.0f / (float)BloomTempRenderTarget.Height, UtilityBlurAmount,
                vxEnumBlurDirection.BlurredBothWays, SampleCount);
            
        }

        public void Prepare()
        {

        }

        public void Apply()
        {
            RenderTarget2D scene = Renderer.GetCurrentTempTarget();
            // pass in the RMA map and light map and apply it to the scene
            GraphicsDevice.SetRenderTarget(BloomTempRenderTarget);

            Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //Engine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Parameters["SceneTexture"].SetValue(scene);
            //Set First Temp Blur Rendertarget 
            Effect.Parameters["SampleWeights"].SetValue(sampleWeightsX);
            Effect.Parameters["SampleOffsets"].SetValue(sampleOffsetsX);
            //Parameters["DistortionMap"].SetValue(_distortionMap);

            Effect.CurrentTechnique = Effect.Techniques["SceneBlur"];
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Renderer.RenderQuad(Vector2.One * -1, Vector2.One);
            }





            // pass in the RMA map and light map and apply it to the scene
            GraphicsDevice.SetRenderTarget(Renderer.BlurredScene);
            
            Engine.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //Engine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Parameters["SceneTexture"].SetValue(BloomTempRenderTarget);
            //Set First Temp Blur Rendertarget 
            Effect.Parameters["SampleWeights"].SetValue(sampleWeightsY);
            Effect.Parameters["SampleOffsets"].SetValue(sampleOffsetsY);
            //Parameters["DistortionMap"].SetValue(_distortionMap);

            Effect.CurrentTechnique = Effect.Techniques["SceneBlur"];
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Renderer.RenderQuad(Vector2.One * -1, Vector2.One);
            }

            if (Engine.CurrentScene.IsPausable && Engine.CurrentScene.IsActive == false)
            {
                GraphicsDevice.SetRenderTarget(Renderer.GetNewTempTarget("Blur Pass"));
                SpriteBatch.Begin();
                SpriteBatch.Draw(scene, GraphicsDevice.Viewport.Bounds, Color.White);
                SpriteBatch.Draw(Renderer.BlurredScene, GraphicsDevice.Viewport.Bounds, Color.White * Engine.CurrentScene.PauseAlpha);
                SpriteBatch.End();
            }
            /*
            // grab the current render target
            SceneToBlur = Renderer.GetCurrentTempTarget();

            Renderer.GraphicsDevice.SetRenderTarget(BlurredScene);

            DrawFullscreenQuad("SceneToBlur", SceneToBlur, SceneToBlur.Width, SceneToBlur.Height,  null);

            //Set First Temp Blur Rendertarget 
            Effect.Parameters["SampleWeights"].SetValue(sampleWeightsX);
            Effect.Parameters["SampleOffsets"].SetValue(sampleOffsetsX);

            Effect.Parameters["TextureSampler"].SetValue(SceneToBlur);

            DrawRenderTargetIntoOther("Blur.HorzPass", SceneToBlur, BloomTempRenderTarget, null);

            Effect.Parameters["TextureSampler"].SetValue(BloomTempRenderTarget);
            Effect.Parameters["SampleWeights"].SetValue(sampleWeightsY);
            Effect.Parameters["SampleOffsets"].SetValue(sampleOffsetsY);

            DrawRenderTargetIntoOther("Blur.VertPass", BloomTempRenderTarget, BlurredScene, null);
            */
        }

        public void SetBloomEffectParameters(float dx, float dy, float theta, vxEnumBlurDirection BlurDirection, int sampleCount)
        {
            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeBloomGaussian(0, theta);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeBloomGaussian(i + 1, theta);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            if (BlurDirection == vxEnumBlurDirection.BlurredHorizontally)
            {
                sampleWeightsX = sampleWeights;
                sampleOffsetsX = sampleOffsets;
            }
            else
            {
                sampleWeightsY = sampleWeights;
                sampleOffsetsY = sampleOffsets;
            }
        }


        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        public float ComputeBloomGaussian(float n, float theta)
        {
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}