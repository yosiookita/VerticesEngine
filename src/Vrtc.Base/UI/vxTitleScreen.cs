#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using VerticesEngine;
using VerticesEngine.Graphics;
using VerticesEngine.Input;
using VerticesEngine.Mathematics;
using VerticesEngine.UI.Menus;
using VerticesEngine.Utilities;

#endregion

namespace VerticesEngine.UI
{
    /// <summary>
    /// This is the Vertices Engine Title Screen which is the first screen when the Engine is launched.
    /// </summary>
    public class vxTitleScreen : vxBaseScene
    {
        #region Fields


        public static bool IsDarkStart = false;

        public static Color LightCol = new Color(1);
        public static Color DarkCol = new Color(0.15f, 0.15f, 0.15f, 1);

        List<Tri> Tris = new List<Tri>();

        public Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 4), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        public Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1280f / 720f, 0.01f, 10000f);

        public Model SphereModel;

        public static float FogStart = 0;

        public static float FogEnd = 12;

        ContentManager content;

        SpriteFont TitleFont;

        Texture2D Logo;

        Texture2D EngineTitle;

        Texture2D Splitter;

        Texture2D BuiltWithTexture;

        float pauseAlpha;

        float UpdateCount = 0;

        KeyboardState CurrentKeyboardState = new KeyboardState();
        KeyboardState PreviousKeyboardState = new KeyboardState();

        #endregion

        #region Initialization

#if DEBUG
        float UpdateTime = 0.25f;
#else
        float UpdateTime = 5;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticesEngine.Screens.TitleScreen"/> class.
        /// </summary>
		public vxTitleScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticesEngine.Screens.TitleScreen"/> class.
        /// </summary>
        /// <param name="UpdateTime">Update time.</param>
        public vxTitleScreen(int UpdateTime) : this()
        {
            this.UpdateTime = UpdateTime;
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Engine.Game.Services, "Content");

            TitleFont = vxInternalAssets.LoadInternalSpriteFont("Fonts/font_splash_24");
            Logo = vxInternalAssets.LoadInternalTexture2D("TitleScreen/logo/vrtx/vrtx_title");
            EngineTitle = vxInternalAssets.LoadInternalTexture2D("TitleScreen/logo/vrtc/vrtc_title");
            SphereModel = Engine.ContentManager.Load<Model>("TitleScreen/sphere/sphere");
            Splitter = vxInternalAssets.LoadInternalTexture2D("TitleScreen/logo/spliiter");
            BuiltWithTexture = vxInternalAssets.LoadInternalTexture2D("TitleScreen/logo/vrtc/built_with");

            try
            {
                Engine.SplashScreen = Engine.Game.Content.Load<Texture2D>("SplashScreen");
                vxConsole.WriteLine("Loading Custom Splash Screen...");
            }
            catch
            {
                //vxConsole.WriteLine("Custom Splash Screen Not Fount...");
            }

            Vector3[] Positions ={
                new Vector3(0,0,0),
                new Vector3(-3,0,-3),
                new Vector3(2,1,-2),
                new Vector3(1,-2,-4),
                new Vector3(-1.5f,1,2),
                new Vector3(2,2,-5),
                new Vector3(-2,1,-5),
                new Vector3(-1,-2,-6),
                new Vector3(-4,-2,-3)
            };

            for (int i = 0; i < Positions.Length; i++)
                Tris.Add(new Tri(this, Positions[i]));
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        protected override void OnFirstUpdate()
        {
            base.OnFirstUpdate();

#if __ANDROID__
            IsWaitingOnPermissions = false;
            Engine.Game.RequestPermissions();
#else
            IsWaitingOnPermissions = false;
            Engine.Game.RequestPermissions();
#endif

        }

        public static bool IsWaitingOnPermissions = true;

        bool MainEntryFired = false;
        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            #region Fade and Base Update

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            #endregion

            CurrentKeyboardState = Keyboard.GetState();

            UpdateCount += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (MainEntryFired == false && IsWaitingOnPermissions == false)
            {
                if (UpdateCount > UpdateTime || CurrentKeyboardState.IsKeyDown(Keys.Enter) ||
                    vxEngine.Instance.Game.IsGameContentLoaded || vxInput.IsNewTouchPressed() == true)
                {
                    MainEntryFired = true;
                    Engine.Game.OnGameStart();
                }
            }

            FogEnd = 12 * Math.Min(UpdateCount, 4) / 4;


            foreach (Tri tri in Tris)
                tri.Update(gameTime);

            PreviousKeyboardState = CurrentKeyboardState;

            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        /// 
        public override void Draw(GameTime gameTime)
        {
            vxSpriteBatch spriteBatch = Engine.SpriteBatch;
            Viewport viewport = Engine.GraphicsDevice.Viewport;

            Engine.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            //Color backCol = IsDarkStart ? DarkCol : Color.White;
            Engine.GraphicsDevice.Clear(ClearOptions.Target, (IsDarkStart ? DarkCol : Color.White), 0, 0);

            GraphicsDevice device = Engine.Game.GraphicsDevice;

            foreach (Tri tri in Tris)
                tri.Draw(gameTime);


            //Draw SpriteBatch
            Engine.Game.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;

            spriteBatch.Begin("Title Screen");

            //Draw Version Information
            Engine.DrawVersionInfo(spriteBatch, TransitionAlpha, Color.DimGray);

            float scale = 2;

            float scalefactor = Math.Min(UpdateCount, 2) / 2;


            Rectangle VrtxLogo = new Rectangle(
            viewport.Width / 2 - Logo.Width / 2,
            viewport.Height * 1 / 3 - Logo.Height / 2,
                Logo.Width, Logo.Height);


            // Draw the Engine Title
            spriteBatch.Draw(Logo, VrtxLogo, Color.White * TransitionAlpha * (Math.Min(UpdateCount, 1) / 1));
            //spriteBatch.Draw(Logo,
            // new Vector2(
            //	 viewport.Width / 2,
            //	 viewport.Height / 2 - 1.5f * EngineTitle.Bounds.Height / scale - 8), null,
            // Color.White * TransitionAlpha * (Math.Min(UpdateCount, 1) / 1) * 0.5f,
            //0,
            // new Vector2(BuiltWithTexture.Width / 2, BuiltWithTexture.Height / 2),
            //Vector2.One / scale, SpriteEffects.None, 0);

            // //Draw the Engine Title
            //spriteBatch.Draw(EngineTitle,
            //new Vector2(
            //             viewport.Width / 2,
            // viewport.Height / 2 - EngineTitle.Bounds.Height/scale-2), null,
            //Color.White * TransitionAlpha * (Math.Min(UpdateCount, 2) / 2),
            //           0,
            //new Vector2(EngineTitle.Width / 2, EngineTitle.Height / 2),
            //Vector2.One/scale, SpriteEffects.None,0);

            float BuiltWithHeight = viewport.Height -
                                            EngineTitle.Bounds.Height / scale -
                                            BuiltWithTexture.Bounds.Height / scale - 25;

            //Draw the Engine Title
            spriteBatch.Draw(BuiltWithTexture,
                             new Vector2(
                                 viewport.Width / 2,
                                 BuiltWithHeight), null,
                             Color.White * TransitionAlpha * (Math.Min(UpdateCount, 1) / 1) * 0.5f,
                            0,
                             new Vector2(BuiltWithTexture.Width / 2, BuiltWithTexture.Height / 2),
                            Vector2.One / scale, SpriteEffects.None, 0);


            // Draw the Splitter
            spriteBatch.Draw(Splitter,
                             new Vector2(
                                 viewport.Width / 2,
                                 BuiltWithHeight + BuiltWithTexture.Bounds.Height / scale - 4), null,
                             Color.White * TransitionAlpha * (Math.Min(UpdateCount, 4) / 2 - 1) * 0.5f,
                            0,
                             new Vector2(Splitter.Width / 2, Splitter.Height / 2),
                             new Vector2((Math.Min(UpdateCount, 4) / 2 - 1), 1) / scale, SpriteEffects.None, 0);


            scale = 2.75f;
            //Draw the Logo
            spriteBatch.Draw(EngineTitle,
                             new Vector2(
                                 viewport.Width / 2,
                                 BuiltWithHeight + BuiltWithTexture.Bounds.Height / scale), null,
                             Color.White * TransitionAlpha * (Math.Min(UpdateCount, 4) / 2 - 1),
                            0,
                             new Vector2(EngineTitle.Width / 2, 0),
                            Vector2.One / scale, SpriteEffects.None, 0);

            spriteBatch.End();

            #region Transition Code
            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                vxSceneManager.FadeBackBufferToBlack(alpha);
            }
            #endregion
        }

        #endregion
    }

    /// <summary>
    /// The opening Tri Elements in the Splashscreen
    /// </summary>
    public class Tri
    {
        Matrix World;

        BasicEffect basicEffect;

        VertexPositionColor[] vertices;

        Model model;

        VertexBuffer vertexBuffer;

        vxTitleScreen Game;

        Vector3 Position;

        Color TriCol;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Screens.Tri"/> class.
        /// </summary>
        /// <param name="game">Game.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        public Tri(vxTitleScreen game, Vector3 position)
        {
            Game = game;
            Position = position;
            World = Matrix.CreateTranslation(Position);


            basicEffect = new BasicEffect(Game.Engine.GraphicsDevice);


            List<VertexPositionColor> VPCs = new List<VertexPositionColor>();

            float Size = 1;


            //TriCol = vxTitleScreen.LightCol;
            //if (vxTitleScreen.IsDarkStart)
            TriCol = vxTitleScreen.DarkCol;

            VPCs.Add(new VertexPositionColor(new Vector3(0, Size, 0), TriCol));
            VPCs.Add(new VertexPositionColor(new Vector3(Size / 2, 0, 0), TriCol));
            VPCs.Add(new VertexPositionColor(new Vector3(-Size / 2, 0, 0), TriCol));
            VPCs.Add(new VertexPositionColor(new Vector3(0, Size, 0), TriCol));

            vertices = VPCs.ToArray();

            vertexBuffer = new VertexBuffer(Game.Engine.GraphicsDevice, typeof(VertexPositionColor), VPCs.Count, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            angle += position.Length();

            model = game.SphereModel;

        }
        float angle = 0.0f;

        public void Update(GameTime gameTime)
        {
            angle += 0.005f;

            float factor = (float)Math.Sin(angle);

            World =
                Matrix.CreateScale(new Vector3(1 + factor / 4)) *
                Matrix.CreateRotationZ(angle) *
                Matrix.CreateRotationY(angle) *
                Matrix.CreateTranslation(Position);
        }

        public void Draw(GameTime gameTime)
        {
            Position += new Vector3(0.001f, 0, 0);

            basicEffect.World = World;

            basicEffect.View = Game.view;
            basicEffect.Projection = Game.projection;
            basicEffect.VertexColorEnabled = true;
            basicEffect.FogEnabled = true;
            basicEffect.FogColor = Vector3.One;
            basicEffect.FogStart = vxTitleScreen.FogStart;
            basicEffect.FogEnd = vxTitleScreen.FogEnd;

            Game.Engine.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.Engine.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 3);
            }


            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            Game.Engine.GraphicsDevice.RasterizerState = rasterizerState;

            foreach (VertexPositionColor vpc in vertices)
            {
                DrawModel(model, Matrix.CreateScale(15) *
                          Matrix.CreateTranslation(vpc.Position) *
                          World, Game.view, Game.projection);
            }
        }


        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;

                    //effect.EnableDefaultLighting();
                    effect.DiffuseColor = TriCol.ToVector3();
                    effect.EmissiveColor = Vector3.Zero;

                    effect.FogEnabled = true;
                    effect.FogColor = Vector3.One;
                    effect.FogStart = vxTitleScreen.FogStart;
                    effect.FogEnd = vxTitleScreen.FogEnd;
                }

                mesh.Draw();
            }
        }
    }
}
