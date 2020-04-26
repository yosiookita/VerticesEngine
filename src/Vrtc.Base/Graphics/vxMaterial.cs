using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VerticesEngine.Graphics
{
    public enum ReflectionType
    {
        None,
        CubeMaps,
        SSR
    }

    /// <summary>
    /// A material is what is used to draw a mesh. It can expose common shader parameters along with allowing for expansion
    /// </summary>
    public class vxMaterial : vxGameObject, IDisposable
    {

        /// <summary>
        /// The Current Render Technique for thie Model
        /// </summary>
        public string RenderTechnique = "Technique_Main";

        #region Shaders

        public vxShader Shader
        {
            get { return _shader; }
        }
        vxShader _shader;

        /// <summary>
        /// Which pass should this material be drawn in?
        /// </summary>
        public string MaterialRenderPass;

        /// <summary>
        /// The utility effect.
        /// </summary>
        public vxUtilityEffect UtilityEffect;

        /// <summary>
        /// The debug effect.
        /// </summary>
        public vxDebugEffect DebugEffect;


        public vxOutlineEffect OutlineEffect;

        #endregion

        /// <summary>
        /// Sets the world.
        /// </summary>
        /// <value>The world.</value>
        public Matrix World
        {
            set { SetEffectParameter("World", value); }
        }

        /// <summary>
        /// The World * View * Projection Cumulative Matrix Value.
        /// </summary>
        public Matrix WVP
        {
            set { SetEffectParameter("wvp", value); }
        }

        public Matrix WorldInverseTranspose
        {
            set { SetEffectParameter("WorldInverseTranspose", value); }
        }

        public Matrix View
        {
            set { SetEffectParameter("View", value); }
        }

        public Matrix Projection
        {
            set { SetEffectParameter("Projection", value); }
        }

        /// <summary>
        /// Model Alpha Value for Transparency
        /// </summary>
        [vxShowInInspector("Alpha Value", vxInspectorCategory.GraphicalProperies, "The Alpha value to fade the Entity by (if it's supported).")]
        public float Transparency
        {
            set
            {
                _mAlphaValue = value;
                SetEffectParameter("AlphaValue", value);
                SetEffectParameter("Alpha", value);
            }
            get { return _mAlphaValue; }
        }
        private float _mAlphaValue = 1;

        #region -- Texture Properties --

        /// <summary>
        /// Gets the Main Texture for this material. This will return null if no texture is set.
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                if(_shader.Parameters[shaderkey_mainTexture] != null)
                _shader.Parameters[shaderkey_mainTexture].SetValue(_texture);
                UtilityEffect.DiffuseTexture = _texture;
                DebugEffect.DiffuseTexture = _texture;
            }
        }
        private Texture2D _texture;

        protected string shaderkey_mainTexture = "Texture";
        //protected string shaderkey_mainTexture = "_MainTex";


        /// <summary>
        /// Off Set for Textures Used On the Model
        /// </summary>
        public Vector2 TextureUVOffset
        {
            get { return _textureUVOffset; }
            set
            {
                _textureUVOffset = value;
                SetEffectParameter("UVOffset", value);
                SetEffectParameter("TextureUVOffset ", value);
                UtilityEffect.UVOffset = _textureUVOffset;
            }
        }
        Vector2 _textureUVOffset = Vector2.Zero;


        /// <summary>
        /// A UV Factor to keep repeating UV Coordinates the same during plane scaling.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public Vector2 UVFactor
        {
            get { return _uvFactor; }
            set
            {
                _uvFactor = value;
                SetEffectParameter("UVFactor", value);
                UtilityEffect.UVFactor = _uvFactor;
            }
        }
        Vector2 _uvFactor = Vector2.One;

        /// <summary>
        /// Toggles whether or not the main diffuse texture is shown.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public bool IsTextureEnabled
        {
            set
            {
                _textureEnabled = value;
                SetEffectParameter("IsTextureEnabled", value);
            }
            get { return _textureEnabled; }
        }
        bool _textureEnabled;

        #region - Normal Mapping -


        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public bool IsNormalMapEnabled
        {
            get { return _isNormalMapEnabled; }
            set
            {
                _isNormalMapEnabled = value;

                SetEffectParameter("IsNormalMapEnabled", value);

                UtilityEffect.IsNormalMapEnabled = value;
            }
        }
        private bool _isNormalMapEnabled = true;

        /// <summary>
        /// Gets the Main Texture for this material. This will return null if no texture is set.
        /// </summary>
        public Texture2D NormalMap
        {
            get { return _normalMap; }
            set
            {
                _normalMap = value;
                if (_shader.Parameters[shaderkey_normalMap] != null)
                    _shader.Parameters[shaderkey_normalMap].SetValue(_normalMap);
                UtilityEffect.NormalMap = _normalMap;
            }
        }
        private Texture2D _normalMap;

        protected string shaderkey_normalMap = "NormalMap";

        #endregion


        #endregion

        #region -- Lighting --


        /// <summary>
        /// Should this material be drawn to the derffered back buffers
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public bool IsDefferedRenderingEnabled
        {
            get { return _doDepthMapping; }
            set
            {
                _doDepthMapping = value;

                SetEffectParameter("DoDepthMapping", value);
            }
        }
        private bool _doDepthMapping = true;



        #region  - Specular -


        /// <summary>
        /// SpecularIntensity of the Shader
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float SpecularIntensity
        {
            set
            {
                _specularIntensity = value;
                SetEffectParameter("SpecularIntensity", value);
            }
            get { return _specularIntensity; }
        }
        float _specularIntensity = 8;

        /// <summary>
        /// SpecularIntensity of the Shader
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float SpecularPower
        {
            set
            {
                _specularPower = value;
                SetEffectParameter("SpecularPower", value);
            }
            get { return _specularPower; }
        }
        float _specularPower = 1;
        #endregion


        /// <summary>
        /// The Light Direction which is Shining on this Object
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public Vector3 LightDirection
        {
            get { return _lightDirection; }
            set
            {
                _lightDirection = value;
                SetEffectParameter("LightDirection", value);
                UtilityEffect.LightDirection = _lightDirection;
            }
        }
        Vector3 _lightDirection;


        /// <summary>
        /// The Light Colour to be used in the Model Shader.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public Color LightColor
        {
            get { return _lightColor; }
            set
            {
                _lightColor = value;
                SetEffectParameter("LightColor", value.ToVector4());
            }
        }
        Color _lightColor;

        /// <summary>
        /// The Ambient Light Colour for this Models Shader.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public Color AmbientLightColor
        {
            get { return _ambientLightColor; }
            set
            {
                _ambientLightColor = value;
                SetEffectParameter("AmbientLight", value.ToVector4());
                UtilityEffect.AmbientLightColor = _ambientLightColor;
            }
        }
        Color _ambientLightColor;


        /// <summary>
        /// Gets or sets the ambient light intensity.
        /// </summary>
        /// <value>The ambient light intensity.</value>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float AmbientLightIntensity
        {
            get { return _ambientLightIntensity; }
            set
            {
                _ambientLightIntensity = value;
                SetEffectParameter("AmbientIntensity", value);
                UtilityEffect.AmbientLightColor = _ambientLightColor;
            }
        }
        float _ambientLightIntensity;

        public float DiffuseIntensity
        {
            set
            {
                _diffuseIntensity = value;

                SetEffectParameter("DiffuseIntensity", _diffuseIntensity);

            }
            get { return _diffuseIntensity; }
        }
        float _diffuseIntensity = 1;

        #region - Emission -

        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public bool IsEmissionEnabled
        {
            get { return _isEmissionEnabled; }
            set
            {
                _isEmissionEnabled = value;

                SetEffectParameter("DoEmissiveMapping", value);
            }
        }
        private bool _isEmissionEnabled = true;


        [vxShowInInspector(vxInspectorCategory.GraphicalProperies, "The amount of emissive bloom (glow) from the Emissivity map.")]
        public float EmissiveMapIntensity
        {
            get { return _glowIntensity; }
            set
            {
                _glowIntensity = value;
                UtilityEffect.GlowIntensity = _glowIntensity;
                SetEffectParameter("GlowIntensity", _glowIntensity);
            }
        }
        private float _glowIntensity = 1;

        /// <summary>
        /// Emissive Colour for use in Highlighting a Model.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public Color EmissiveColour
        {
            set
            {
                _emissiveColour = value;
                SetEffectParameter("EmissiveColour", value.ToVector4());
                UtilityEffect.EmissiveColour = value;
            }
            get { return _emissiveColour; }
        }
        Color _emissiveColour;

        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float EmissiveIntensity
        {
            set
            {
                _emissiveIntensity = value;
                SetEffectParameter("EmissiveIntensity", _emissiveIntensity);
            }
            get { return _emissiveIntensity; }
        }
        float _emissiveIntensity = 1;
        #endregion

        #region - Reflections -

        /// <summary>
        /// Texture which is applied as the Reflection Texture.
        /// NOTE: This must be added to the Main Model Shader.
        /// </summary>
        public TextureCube ReflectionTextureCube
        {
            set
            {
                _reflectionTexture = value;
                SetEffectParameter("ReflectionTexture", value);
            }
            get { return _reflectionTexture; }
        }
        TextureCube _reflectionTexture;



        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public ReflectionType ReflectionType
        {
            get { return _reflectionType; }
            set {
                _reflectionType = value;

                switch(_reflectionType)
                {
                    case ReflectionType.None:
                        DoSSR = false;
                        DoReflections = false;
                        break;
                    case ReflectionType.CubeMaps:
                        DoSSR = false;
                        DoReflections = true;
                        break;
                    case ReflectionType.SSR:
                        DoSSR = true;
                        DoReflections = true;
                        break;
                }
            }
        }
        ReflectionType _reflectionType = ReflectionType.CubeMaps;


        public bool DoReflections
        {
            get { return _doReflections; }
            set
            {
                _doReflections = value;
                //UtilityEffect.Parameters["DoReflections"].SetValue(_doReflections);
            }
        }
        bool _doReflections = true;



        public bool DoSSR
        {
            get { return _doSSR; }
            set
            {
                _doSSR = value;
                UtilityEffect.DoSSR = value;
            }
        }
        bool _doSSR = true;


        /// <summary>
        /// The overall Reflection Amount to be applied in the Shader.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float ReflectionIntensity
        {
            set
            {
                _reflectionAmount = value;
                SetEffectParameter("ReflectionIntensity", _reflectionAmount);
            }
            get { return _reflectionAmount; }
        }
        float _reflectionAmount = 1;

        #endregion

        #region - Fresnel -

        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float FresnelBias
        {
            set
            {
                _fresnelBias = value;
                SetEffectParameter("fFresnelBias", _fresnelBias);
            }
            get { return _fresnelBias; }
        }
        float _fresnelBias = 0.025f;


        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float FresnelPower
        {
            set
            {
                _fresnelPower = value;
                SetEffectParameter("fFresnelPower", _fresnelPower);
            }
            get { return _fresnelPower; }
        }
        float _fresnelPower = 6.0f;

        #endregion

        #endregion

        #region -- Shadows --

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:VerticesEngine.Entities.vxEntity3D"/> should do shadow map.
        /// </summary>
        /// <value><c>true</c> if do shadow map; otherwise, <c>false</c>.</value>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public bool IsShadowCaster
        {
            get { return _isShadowCaster; }
            set
            {
                _isShadowCaster = value;
                UtilityEffect.IsShadowsEnabled = _isShadowCaster;
            }
        }
        private bool _isShadowCaster = false;



        /// <summary>
        /// Gets or sets the shadow brightness.
        /// </summary>
        /// <value>The shadow brightness.</value>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float ShadowBrightness
        {
            get { return _shadowBrightness; }
            set
            {
                _shadowBrightness = value;
                UtilityEffect.ShadowBrightness = _shadowBrightness;
            }
        }
        float _shadowBrightness = 0.25f;


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:VerticesEngine.Entities.vxEntity3D"/> render
        /// shadow split index.
        /// </summary>
        /// <value><c>true</c> if render shadow split index; otherwise, <c>false</c>.</value>
        //public bool renderShadowSplitIndex
        //{
        //    set
        //    {
        //        _renderShadowSplitIndex = value; //UpdateRenderTechnique();
        //        if (Model != null)
        //        {
        //            foreach (var mesh in Model.Meshes)
        //                mesh.DebugEffect.ShadowDebug = _renderShadowSplitIndex;
        //        }
        //    }
        //    get { return _renderShadowSplitIndex; }
        //}
        //bool _renderShadowSplitIndex;



        /*
        [vxGraphicalPropertyAttribute(Title = "Shadow MaxShadowLoops")]
        public int MaxShadowLoops
        {
            get { return _maxShadowLoops; }
            set
            {
                _maxShadowLoops = value;

                if (Model != null)
                {
                    foreach (var mesh in Model.Meshes)
                        mesh.UtilityEffect.Parameters["MaxShadowLoops"].SetValue(_maxShadowLoops);
                }

            }
        }
        int _maxShadowLoops = 4;
        */




        /// <summary>
        /// The Poisson Kernel to be used for Shadow Mapping Edge Blending
        /// </summary>
        //public Vector2[] PoissonKernel
        //{
        //    get { return _poissonKernel; }
        //    set
        //    {
        //        _poissonKernel = value;
        //        if (Model != null)
        //        {
        //            foreach (var mesh in Model.Meshes)
        //                mesh.UtilityEffect.PoissonKernel = _poissonKernel;
        //        }
        //    }
        //}
        //Vector2[] _poissonKernel;



        /// <summary>
        /// Gets the random texture2 d.
        /// </summary>
        /// <value>The random texture2 d.</value>
        //public Texture2D RandomTexture2D
        //{
        //    get { return _randomTexture2D; }
        //    set
        //    {
        //        _randomTexture2D = value;
        //        if (Model != null)
        //        {
        //            foreach (var mesh in Model.Meshes)
        //                mesh.UtilityEffect.RandomTexture2D = _randomTexture2D;
        //        }
        //    }
        //}
        //Texture2D _randomTexture2D;

        #endregion

        #region -- Distortion --


        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public bool IsDistortionEnabled
        {
            get { return _doDistortionMapping; }
            set
            {
                _doDistortionMapping = value;
                SetEffectParameter("DoDistortionMapping", _doDistortionMapping);
            }
        }
        bool _doDistortionMapping = true;

        /// <summary>
        /// A global amount controlling the amount of distortion this entity causes.
        /// </summary>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float DistortionScale
        {
            get { return _distortionScale; }
            set
            {
                _distortionScale = value;
                UtilityEffect.DistortionScale = _distortionScale;
            }
        }
        float _distortionScale = 0.25f;

        /// <summary>
        /// The Distortion Map. 
        /// </summary>
        public Texture2D DistortionMap
        {
            get { return _distortionMap; }
            set
            {
                _distortionMap = value;
                UtilityEffect.DistortionMap = _distortionMap;
            }
        }
        Texture2D _distortionMap;


        /// <summary>
        /// Off Set for Textures Used On the Model
        /// </summary>
        public Vector2 DistortionUVOffset
        {
            get { return _distortionUVOffset; }
            set
            {
                _distortionUVOffset = value;
                UtilityEffect.DistortionUVOffset = _distortionUVOffset;
            }
        }
        Vector2 _distortionUVOffset = Vector2.Zero;

        #endregion

        #region -- Utility Properties --


        /// <summary>
        /// Gets or sets the outline thickness.
        /// </summary>
        /// <value>The outline thickness.</value>
        [vxShowInInspector(vxInspectorCategory.GraphicalProperies)]
        public float OutlineThickness
        {
            set
            {
                _outlineThickness = MathHelper.Clamp(value, MinOutlineThickness, MaxOutlineThickness);

                SetEffectParameter("LineThickness", _outlineThickness);
            }
            get { return _outlineThickness; }
        }
        float _outlineThickness = 1;

        /// <summary>
        /// The max outline thickness.
        /// </summary>
        public float MaxOutlineThickness = 1;

        /// <summary>
        /// The minimum outline thickness.
        /// </summary>
        public float MinOutlineThickness = 0.07f;


        public bool DoAuxDepth
        {
            get { return _doAuxDepth; }
            set
            {
                _doAuxDepth = value;
                SetEffectParameter("DoAuxDepth", _doAuxDepth);
            }
        }
        private bool _doAuxDepth = true;

        #endregion

        /// <summary>
        /// Creates a new material using the specified shader
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="shader"></param>
        public vxMaterial(vxShader shader) : base()
        {
            _shader = shader;
            UtilityEffect = new vxUtilityEffect(vxInternalAssets.Shaders.PrepPassShader);
            DebugEffect = new vxDebugEffect(vxInternalAssets.Shaders.DebugShader);
            OutlineEffect = new vxOutlineEffect(vxInternalAssets.Shaders.OutlineShader.Clone());

            Initalise();
        }

        public virtual void Initalise()
        {
            // set initial render pass
            MaterialRenderPass = vxRenderer.Passes.OpaquePass;

            IsTextureEnabled = true;
            IsNormalMapEnabled = true;
            IsDefferedRenderingEnabled = true;
            Transparency = 1;

            // Set Lighting Values
            AmbientLightColor = Color.White;
            AmbientLightIntensity = 0.51f;
            SpecularIntensity = 0;
            SpecularPower = 1;
            LightDirection = Vector3.Normalize(new Vector3(100, 130, 0));
            LightColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            DiffuseIntensity = 1;

            // Emission
            IsEmissionEnabled = true;
            EmissiveMapIntensity = 1;
            EmissiveColour = Color.White;
            EmissiveIntensity = 0.1f;

            // Reflections
            ReflectionType = ReflectionType.CubeMaps;
            ReflectionIntensity = 1;
            FresnelPower = 2;
            FresnelBias = 0.025f;
            DistortionScale = 0.0125f;
            // Some Items, which don't need Reflections, are added before the scene is actually
            // fully instantiated, therefore, a check to see if the Sky Box exists yet is needed.
            //if (Scene.SkyBox != null)
            //ReflectionTextureCube = Scene.SkyBox.SkyboxTextureCube;

            // Utility Shader
            UtilityEffect.UVFactor = new Vector2(1, 1);
            UtilityEffect.DiffuseLight = new Color(0.5f, 0.5f, 0.5f, 1);
            IsShadowCaster = true;
            ShadowBrightness = 0.355f;
            UtilityEffect.PoissonKernel = vxShadowEffect.PoissonKernel;
            UtilityEffect.ShadowBlurStart = 4;
            UtilityEffect.NumberOfShadowBlendSamples = 4;
            UtilityEffect.BlendSampleCount = 8.0f;
            UtilityEffect.PoissonKernelScale = vxShadowEffect.PoissonKernelScale;
            //UtilityEffect.TileBounds = vxShadowEffect.ShadowSplitTileBounds;
            UtilityEffect.RandomTexture2D = vxInternalAssets.Textures.RandomValues;
            UtilityEffect.DebugShadowSplitColors = vxShadowEffect.ShadowSplitColors;
            DoAuxDepth = true;

            // Debug Shader
            DebugEffect.WireColour = new Color(0.1f, 0.1f, 0.1f, 1);
        }

        public void SetPass()
        {
            _shader.CurrentTechnique = _shader.Techniques[RenderTechnique];
        }


        #region -- Utility Methods --


        public void SetEffectParameter(string param, float value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value);

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, bool value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value);

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, Vector2 value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value);

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, Vector3 value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value);

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, Vector4 value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value);

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, Matrix value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value);

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, Color value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value.ToVector4());

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value.ToVector4());
        }

        public void SetEffectParameter(string param, Texture2D value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value);

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value);
        }

        public void SetEffectParameter(string param, TextureCube value)
        {
            if (_shader.Parameters[param] != null)
                _shader.Parameters[param].SetValue(value);

            if (UtilityEffect.Parameters[param] != null)
                UtilityEffect.Parameters[param].SetValue(value);
        }

        public vxMaterial Clone()
        {
            vxMaterial other = (vxMaterial)this.MemberwiseClone();
            other._shader = new vxShader(this.Shader);
            other.Initalise();
            return other;
        }

        #endregion
    }
}
