
//XNA/MONOGAME
using Microsoft.Xna.Framework;

//Virtex vxEngine Declaration
using VerticesEngine;
using VerticesEngine.Entities;
using VerticesEngine.Graphics;
using System.Linq;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    public class Plant : vxEntity3D
    {
        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public Plant(vxGameplayScene3D scene, vxModel entityModel, Vector3 StartPosition)
            : base(scene, entityModel, StartPosition)
        {
            //World = Matrix.CreateRotationY(-MathHelper.PiOver2);
        }

		public override void InitShaders ()
		{
			base.InitShaders ();

			if (this.Model != null) {
				if (this.Model.ModelMain != null) {
					foreach (var part in this.Model.ModelMain.Meshes.SelectMany(m => m.MeshParts)) {
						if (part.Effect.Parameters ["LightDirection"] != null)
							part.Effect.Parameters ["LightDirection"].SetValue (Vector3.Normalize (new Vector3 (100, 130, 0)));

						if (part.Effect.Parameters ["EvissiveColour"] != null)
							part.Effect.Parameters ["EvissiveColour"].SetValue (new Vector4 (0));

						if (part.Effect.Parameters ["AmbientLightColor"] != null)
							part.Effect.Parameters ["AmbientLightColor"].SetValue (new Vector4 (0.2f, 0.2f, 0.2f, 1.0f));

						if (part.Effect.Parameters ["PoissonKernel"] != null)
                            part.Effect.Parameters ["PoissonKernel"].SetValue (Scene.Renderer.PoissonKernel);

						/*
						float[] thres = new float[2];
						thres[0] = 0.8f;
						thres[1] = 0.4f;
						if (part.Effect.Parameters ["ToonThresholds"] != null)
							part.Effect.Parameters ["ToonThresholds"].SetValue (thres);

						float[] ToonBrightnessLevels = new float[3];
						ToonBrightnessLevels[0] = 0.9f;
						ToonBrightnessLevels[1] = 0.5f;
						ToonBrightnessLevels[2] = 0.2f;
						if (part.Effect.Parameters ["ToonBrightnessLevels"] != null)
							part.Effect.Parameters ["ToonBrightnessLevels"].SetValue (ToonBrightnessLevels);
*/

						
						//if (part.Effect.Parameters ["FogColor"] != null)
						//	part.Effect.Parameters ["FogColor"].SetValue (new Vector4(1,1,1,1));

						//if (part.Effect.Parameters ["RandomTexture3D"] != null)
						//	part.Effect.Parameters ["RandomTexture3D"].SetValue (Engine.Renderer.RandomTexture3D);
						if (part.Effect.Parameters ["RandomTexture2D"] != null)
                            part.Effect.Parameters ["RandomTexture2D"].SetValue (Scene.Renderer.RandomTexture2D);

						//By Default, Don't Show Fog
						//DoFog = Engine.Current3DSceneBase.DoFog;

						SpecularIntensity = 0;
						SpecularPower = 1;
					}
				}
			}
		}
    }
}
