using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;

namespace VerticesEngine.Graphics
{
	public class vxUtilityEffect : vxShader
	{
		/// <summary>
		/// Sets the world.
		/// </summary>
		/// <value>The world.</value>
		public Matrix World
		{
			set { Parameters["World"].SetValue(value); }
		}

		/// <summary>
		/// The World * View * Projection Cumulative Matrix Value.
		/// </summary>
		public Matrix WVP
		{
			set { Parameters["wvp"].SetValue(value); }
		}


		//      public Vector3 CameraPosition
		//{
		//	set { Parameters["CameraPos"].SetValue(value); }
		//}

		/// <summary>
		/// An offset for Texture Coordinates.
		/// </summary>
		public Vector2 UVOffset
		{
			set
			{
				if (Parameters["UVOffset"] != null) Parameters["UVOffset"].SetValue(value);
			}
		}

		/// <summary>
		/// An offset for Distortion Coordinates.
		/// </summary>
		public Vector2 DistortionUVOffset
		{
			set
			{
				if (Parameters["DistUVOffset"] != null) Parameters["DistUVOffset"].SetValue(value);
			}
		}



		/// <summary>
		/// A UV Factor to keep repeating UV Coordinates the same during plane scaling.
		/// </summary>
		public Vector2 UVFactor
		{
			set
			{
				if (Parameters["UVFactor"] != null) Parameters["UVFactor"].SetValue(value);
			}
		}

		#region Lighting 

		public Color DiffuseLight
		{
			set
			{
				if (Parameters["DiffuseLight"] != null)
					Parameters["DiffuseLight"].SetValue(value.ToVector3());
			}
		}


		public float SpecularIntensity
		{
			set
			{
				if (Parameters["SpecularIntensity"] != null) Parameters["SpecularIntensity"].SetValue(value);
			}
		}

		public float SpecularPower
		{
			set
			{
				if (Parameters["SpecularPower"] != null) Parameters["SpecularPower"].SetValue(value);
			}
		}



		/// <summary>
		/// Sets the light direction.
		/// </summary>
		/// <value>The light direction.</value>
		public Vector3 LightDirection
		{
			set
			{
				if (Parameters["LightDirection"] != null) Parameters["LightDirection"].SetValue(value);
			}
		}

		#endregion

		#region Colours

		public Color AmbientLightColor
		{
			set
			{
				if (Parameters["AmbientLight"] != null)
					Parameters["AmbientLight"].SetValue(value.ToVector4());
			}
		}

		public Color EmissiveColour
		{
			set
			{
				if (Parameters["EmissiveColour"] != null)
					Parameters["EmissiveColour"].SetValue(value.ToVector4());
			}
		}

		public Color SelectionColour
		{
			set
			{
				if (Parameters["SelectionColour"] != null)
					Parameters["SelectionColour"].SetValue(value.ToVector4());
			}
		}

		#endregion



		#region Textures and Maps

		/// <summary>
		/// Sets a value indicating whether this <see cref="T:VerticesEngine.Graphics.vxUtilityEffect"/> do texture.
		/// </summary>
		/// <value><c>true</c> if do texture; otherwise, <c>false</c>.</value>
		public bool DoTexture
		{
			set { Parameters["DoTexture"].SetValue(value); }
		}


		/// <summary>
		/// Gets or sets the diffuse texture. Note this is not needed by the Utility effect but
        /// kept for information purposes.
		/// </summary>
		/// <value>The diffuse texture.</value>
		public Texture2D DiffuseTexture
		{
			get { return _diffusetexture; }
			set
			{
				_diffusetexture = value;
				//Parameters["Texture"].SetValue(value);
			}
		}
		Texture2D _diffusetexture;



		/// <summary>
		/// Normal Map for this mesh.
		/// </summary>
		public Texture2D NormalMap
		{
			get { return _normalMap; }
			set
			{
				_normalMap = value;
				if (Parameters["NormalMap"] != null)
					Parameters["NormalMap"].SetValue(value);
			}
		}
		Texture2D _normalMap;


        /// <summary>
        /// Gets or sets the surface map for this mesh. The Surface Map uses the following RGBA channels as:
        /// R: Specular Power,
        /// G: Specular Intensity,
        /// B: Reflection Map Value,
        /// A: Emissivity.
        /// </summary>
        /// <value>The surface map.</value>
		public Texture2D SurfaceMap
		{
            get { return _surfaceMap; }
			set
			{
				_surfaceMap = value;
                if (Parameters["SurfaceMap"] != null)
                    Parameters["SurfaceMap"].SetValue(value);
			}
		}
		Texture2D _surfaceMap;

        public float GlowIntensity
        {
            get { return _glowIntensity; }
            set
            {
                _glowIntensity = value;
                //if (Parameters["GlowIntensity"] != null)
                    Parameters["GlowIntensity"].SetValue(value);
            }
        }
        private float _glowIntensity = 1;



        public bool IsNormalMapEnabled
        {
            set { Parameters["DoNormalMapping"].SetValue(value); }
        }

        public bool DoSSR
        {
            get;
            set;
            //set { Parameters["DoSSRefection"].SetValue(value); }
        }

        
        public bool ReflectionIntensity
        {
            set { Parameters["ReflectionIntensity"].SetValue(value); }
        }



		/// <summary>
		/// Specular Map. Note, setting this will set the same specular map for all meshes.
		/// </summary>
		public RenderTarget2D ShadowMap
		{
			get { return _shadowMap; }
			set
			{
				_shadowMap = value;
				if (Parameters["ShadowMap"] != null)
					Parameters["ShadowMap"].SetValue(value);
			}
		}
		RenderTarget2D _shadowMap;




		#endregion

		#region Distortion Code

		public float DistortionScale
		{
			set
			{
				if (Parameters["DistortionScale"] != null) Parameters["DistortionScale"].SetValue(value);
			}
		}

		public Texture2D DistortionMap
		{
			get { return _distortionMap; }
			set
			{
				_distortionMap = value;
                if (Parameters["DistortionMap"] != null)
                    Parameters["DistortionMap"].SetValue(_distortionMap);
			}
		}
		Texture2D _distortionMap;

		#endregion

		#region Shadows

		/// <summary>
		/// Sets the shadow transform.
		/// </summary>
		/// <value>The shadow transform.</value>
		public Matrix[] ShadowTransform
		{
			set
			{
				if (Parameters["ShadowTransform"] != null) Parameters["ShadowTransform"].SetValue(value);
			}
		}

		/// <summary>
		/// Sets the tile bounds.
		/// </summary>
		/// <value>The tile bounds.</value>
		public Vector4[] TileBounds
		{
			set
			{
				if (value != null)
					if (Parameters["TileBounds"] != null)
						Parameters["TileBounds"].SetValue(value);
			}
		}


		/// <summary>
		/// Sets a value indicating whether this <see cref="T:VerticesEngine.Graphics.vxUtilityEffect"/> do shadow.
		/// </summary>
		/// <value><c>true</c> if do shadow; otherwise, <c>false</c>.</value>
		public bool IsShadowsEnabled
		{
			set { Parameters["DoShadow"].SetValue(value); }
		}



		/// <summary>
		/// Sets the shadow brightness. 0 being the Darkest and 1 being no shadow.
		/// </summary>
		/// <value>The shadow brightness.</value>
		public float ShadowBrightness
		{
            set { 
                if (Parameters["ShadowBrightness"] != null)
                Parameters["ShadowBrightness"].SetValue(value); 
            }
		}

		/// <summary>
		/// Sets the number samples for shadow edge blending.
		/// </summary>
		/// <value>The number samples.</value>
		public int NumberOfShadowBlendSamples
		{
			set { 
				if (Parameters["numSamples"] != null)
					Parameters["numSamples"].SetValue(value); }
		}


        /// <summary>
        /// Gets or sets the random texture2 d.
        /// </summary>
        /// <value>The random texture2 d.</value>
		public Texture2D RandomTexture2D
		{
			get { return _randomTexture2D; }
			set
			{
				_randomTexture2D = value;
				if (Parameters["RandomTexture2D"] != null)
					Parameters["RandomTexture2D"].SetValue(_randomTexture2D);
			}
		}
		Texture2D _randomTexture2D;


        /// <summary>
        /// Gets or sets the poisson kernel.
        /// </summary>
        /// <value>The poisson kernel.</value>
		public Vector2[] PoissonKernel
		{
			get { return _poissonKernel; }
			set
			{
				_poissonKernel = value;
				if (Parameters["pk"] != null)
					Parameters["pk"].SetValue(_poissonKernel);
            }
		}
		Vector2[] _poissonKernel;

        /// <summary>
        /// Gets or sets the poisson kernel scale.
        /// </summary>
        /// <value>The poisson kernel scale.</value>
		public float[] PoissonKernelScale
        {
            get { return _poissonKernelScale; }
            set
            {
                _poissonKernelScale = value;
                if (Parameters["PoissonKernelScale"] != null)
                    Parameters["PoissonKernelScale"].SetValue(_poissonKernelScale);
            }
        }
        float[] _poissonKernelScale;

        /// <summary>
        /// The number of blends to be done when blurring the shadow edge
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float BlendSampleCount
        {
            get { return _blendSampleCount; }
            set
            {
                _blendSampleCount = value;

                if (Parameters["sampleCount"] != null)
                    Parameters["sampleCount"].SetValue(_blendSampleCount);
            }
        }
        float _blendSampleCount;




        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public int ShadowBlurStart
        {
            get { return _shadowBlurStart; }
            set
            {
                _shadowBlurStart = value;
                if (Parameters["ShadowBlurStart"] != null)
                    Parameters["ShadowBlurStart"].SetValue(_shadowBlurStart);
            }
        }
        int _shadowBlurStart = 4;



        public Color[] DebugShadowSplitColors
        {
            get { return _debugShadowSplitColors; }
            set
            {
                _debugShadowSplitColors = value;
                List<Vector4> colours = new List<Vector4>();
                foreach(var color in value)
                {
                    colours.Add(color.ToVector4());
                }
                Parameters["SplitColors"].SetValue(colours.ToArray());
            }
        }
        Color[] _debugShadowSplitColors;


        #endregion


        public vxUtilityEffect(Effect effect) : base(effect)
		{


		}
	}
}
