using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Core.Entities;
using Virtex.Lib.Vrtc.Screens;
using Virtex.Lib.Vrtc.Core.Graphics;
using Virtex.Lib.Vrtc.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Virtex.Lib.Vrtc.Graphics;

namespace Virtex.App.VerticesTechDemo
{
    public class ModelViewer : vxModelViewer
    {
        public ModelViewer(vxEngine Engine):base(Engine)
        {

        }

        public override vxEntity3D LoadModel()
        {
            return new vxEntity3D(Engine, Engine.ContentManager.LoadModel("Models/items/rock/model"), Vector3.Zero);
        }

        public override void UpdateModel(GameTime gameTime)
        {
            // Calculate the camera matrices.
            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            //Matrix rotation = Matrix.CreateRotationY(time * 0.5f);
            Matrix rotation = Matrix.Identity;
            
            Model.World =
                Matrix.CreateScale(0.35f) *
                Matrix.CreateRotationX(-MathHelper.PiOver2) * rotation;
			
			foreach (vxModelMesh mesh in this.Model.Model.ModelMeshes)
			{
					foreach (EffectParameter para in mesh.UtilityEffect.Parameters)
					{
						vxConsole.WriteToInGameDebug(para.Name + " : " + para.ParameterClass + " : " + para.ParameterType + " : " + vxEffect.GetParameterValue(para));
						//vxConsole.WriteToInGameDebug(para.Name + " : " + para.ParameterClass + " : " + para.ParameterType + " : " + para.Elements.Count);
					}
				}
			}
        }
    }

