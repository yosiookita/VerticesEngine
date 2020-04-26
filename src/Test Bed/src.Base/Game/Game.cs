#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using VerticesEngine.Graphics;
using VerticesEngine.Input;
using VerticesEngine.Screens;
using VerticesEngine.Screens.Async;
using VerticesEngine.UI.Themes;
using VerticesEngine.Utilities;
#endregion

namespace Virtex.App.VerticesTechDemo
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class VerticesTechDemoGame : vxGame
    {
        #region Initialization


        //public static vxModel Model_Items_WoodenCrate { get; set; }
        public static vxModel Model_Items_Rock { get; set; }
        public static vxModel Model_Items_WaterCrate { get; set; }
        public static vxModel Model_Items_Concrete { get; set; }
        public static vxModel Model_Items_Teapot { get; set; }


        /// <summary>
        /// The main game constructor.
        /// </summary>
		public VerticesTechDemoGame() : base()
        {

        }

        protected override vxGameConfig GetGameConfig()
        {
            return new vxGameConfig("Vertices Engine Tech Demo",
                                    vxEnumGameType.ThreeDimensional,
                                   vxGameConfigFlags.IsCursorVisible |
                                    vxGameConfigFlags.GraphicsSettings |
                                    vxGameConfigFlags.AudioSettings);
        }

        public override void OnGameStart()
        {
            base.OnGameStart(new IntroBackground2D(), new MainvxMenuBaseScreen());
        }

        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            vxInput.IsCursorVisible = true;
            //Engine.MultiThreadPhysics = false;
            IsMouseVisible = false;
        }

        public override void LoadGUITheme()
        {
            base.LoadGUITheme();

            vxGUITheme.Fonts.FontPack = new VerticesEngine.UI.vxFontPack(Engine, "Fonts");
            vxGUITheme.ArtProviderForMenuScreenItems = new MenuItemArtProvider(Engine);
        }

        public override void LoadGlobalContent()
        {
            base.LoadGlobalContent();

            //Model model = Engine.Game.Content.Load<Model>("Models/items/gridcubex2/model");
            //Console.WriteLine(model.Tag);
            //foreach(var mesh in model.Meshes)
            //{   
            //    Console.WriteLine("\t" +mesh.Tag);
            //    foreach(var part in mesh.MeshParts)
            //    {
            //        var elmnts = part.VertexBuffer.VertexDeclaration.GetVertexElements();
            //        foreach(var elmnt in elmnts)
            //            Console.WriteLine("\t\t"+elmnt);

            //        foreach(var p in part.Effect.Parameters)
            //        {
            //            Console.Write(p.Name);
            //            Console.Write("\t"+p.ParameterClass);
            //            Console.Write("\t" +p.ParameterType);
            //            Console.WriteLine("");
            //        }
            //    }
            //}



            //Model_Items_ModelObjs = Engine.ContentManager.LoadModel("Models/modelobjs/modelobjs");
            //Model_Items_WoodenCrate = Engine.ContentManager.LoadModel("Models/items/wooden crate/wooden crate");

            vxModel ConcreteBlockModel = Engine.ContentManager.LoadModel("Models/items/concrete_cube/concrete_cube");
            Model_Items_Concrete = Engine.ContentManager.LoadModel("Models/items/concrete_cube/concrete_cube");
            Model_Items_Teapot = Engine.ContentManager.LoadModel("Models/items/teapot/teapot");
            //Model_Items_Rock = Engine.ContentManager.LoadModel(Rock.EntityDescription.FilePath);

            //vxModel model = Engine.ContentManager.LoadModel("Models/homestead/displaymodel", "textures");
            vxInput.CursorSprite = Content.Load<Texture2D>("Textures/cursor/cursor");
            vxInput.CursorSpriteClicked = Content.Load<Texture2D>("Textures/cursor/cursor_clicked");
        }


        #endregion

    }
}
