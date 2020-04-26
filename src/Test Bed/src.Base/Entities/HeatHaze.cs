
//XNA/MONOGAME
using Microsoft.Xna.Framework;

//Virtex vxEngine Declaration
using VerticesEngine;
using VerticesEngine.Entities;
using VerticesEngine.Graphics;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System;
using VerticesEngine.Scenes;

namespace Virtex.App.VerticesTechDemo
{
    public class HeatHaze : vxDistortionEntity
    {
        vxLightEntity coals;
        Random rand;
        float dir;
        /// <summary>
        /// Creates a New Instance of the Base Ship Class
        /// </summary>
        /// <param name="AssetPath"></param>
        public HeatHaze(vxGameplayScene3D scene, vxModel model, Texture2D distortionMap, Vector3 Position, float dir)
            : base(scene, model, distortionMap, Position)
        {
            rand = new Random();
            this.dir = dir;

            coals = new vxLightEntity(scene, Position, LightType.Point, Color.OrangeRed, 5, 1);

            DistortionTechnique = DistortionTechniques.HeatHaze;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            coals.lightRadius = 2.5f + (float)rand.NextDouble() * 0.25f;

            float i = (float)gameTime.TotalGameTime.TotalSeconds;
            Vector3 NewPosition = new Vector3(0, (float)Math.Sin(i* dir) /10+dir, 0) + Position;
            
            WorldTransform = Matrix.CreateRotationX(-MathHelper.PiOver2) *
                Matrix.CreateRotationY(i * dir) *
                 Matrix.CreateTranslation(NewPosition);
        }

    }
}
