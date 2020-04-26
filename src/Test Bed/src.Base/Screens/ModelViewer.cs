using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VerticesEngine;
using VerticesEngine.Entities;
using VerticesEngine.Screens;
using VerticesEngine.Graphics;
using VerticesEngine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Graphics;

namespace Virtex.App.VerticesTechDemo
{
    public class ModelViewer : vxModelViewer
    {
        public ModelViewer(vxEngine Engine):base(Engine)
        {

        }
		float height = 0;

		public override vxRenderer3D InitialiseRenderingEngine()
		{
            return new vxRenderer3D(this)
            {
                ShadowMapSize = 1024,
                ShadowBoundingBoxSize = 128
            };
                
		}

        public override vxEntity3D LoadModel()
        {
            //vxModel model = Engine.InternalAssets.Models.UnitCylinder;

			vxModel model = Engine.ContentManager.LoadModel("Models/items/rock/model");
			vxEntity3D entity = new vxEntity3D(this, model, Vector3.Zero);
			height = entity.BoundingShape.Radius/4;
			return entity;
        }

        public override void UpdateModel(GameTime gameTime)
        {
            // Calculate the camera matrices.
            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            //Matrix rotation = Matrix.CreateRotationY(time * 0.5f);
            Matrix rotation = Matrix.Identity;

			Vector3 pos = new Vector3(0, height, 0);
			Camera.OrbitTarget = pos;
            
            Model.WorldTransform =
                Matrix.CreateScale(0.35f) *
                Matrix.CreateRotationX(-MathHelper.PiOver2) * rotation *
				     Matrix.CreateTranslation(0, height, 0);
			
			//foreach (vxModelMesh mesh in this.Model.Model.ModelMeshes)
			//{
			//		foreach (EffectParameter para in mesh.UtilityEffect.Parameters)
			//		{
			//			vxConsole.WriteToInGameDebug(para.Name + " : " + para.ParameterClass + " : " + para.ParameterType + " : " + vxEffect.GetParameterValue(para));
			//			//vxConsole.WriteToInGameDebug(para.Name + " : " + para.ParameterClass + " : " + para.ParameterType + " : " + para.Elements.Count);
			//		}
			//	}
			}
        }
    }

