
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using VerticesEngine.Plugins;

namespace VerticesEngine
{
    /// <summary>
    /// This class holds all information to register an entity with the vxSandbox enviroment.
    /// </summary>
    public class vxSandboxEntityRegistrationInfo
    {
        //vxEngine Engine;

        public Type Type;

        /// <summary>
        /// The Icon for thie Item
        /// </summary>
        public Texture2D Icon;

        /// <summary>
        /// The Key for this item
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The Name of the Current Item
        /// </summary>
        public string Description;

        /// <summary>
        /// File Path to the main asset
        /// </summary>
        public string FilePath;

        public EntityType EntityType
        {
            get { return itemAttribute.EntityType; }
        }

        public string Name
        {
            get { return itemAttribute.Name.ToString(); }
        }

        public string Category
        {
            get { return itemAttribute.Category.ToString(); }
        }


        public string SubCategory
        {
            get { return itemAttribute.SubCategory.ToString(); }
        }

        vxRegisterAsSandboxEntityAttribute itemAttribute;

        public vxEntitySpriteSheetDefinition SpriteSheetInfo;

        vxIPlugin contentPack;

        /// <summary>
        /// Gets the content pack key that this entity belongs to.
        /// </summary>
        /// <value>The content pack key.</value>
        public virtual vxPluginMetaInfo ContentPack
        {
            get { return _contentPackKey; }
        }
        vxPluginMetaInfo _contentPackKey = new vxPluginMetaInfo();


        /// <summary>
        /// Hide's this entity in the sandbox creation UIs
        /// </summary>
        public bool HideFromSandboxUI
        {
            get { return itemAttribute.HideFromSandboxUI; }
        }

        /// <summary>
        /// Creates a Sandbox Entity Registration using the type, item attributes and content pack
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemAttribute"></param>
        /// <param name="contentPack"></param>
        public vxSandboxEntityRegistrationInfo(Type type,  vxRegisterAsSandboxEntityAttribute itemAttribute, ref vxIPlugin contentPack)
        {
            // set item attribute
            this.itemAttribute = itemAttribute;

            // type
            this.Type = type;

            // key
            this.Key = type.Name;

            this.Description = "";

            // file path
            this.FilePath = itemAttribute.AssetPath;

            // content pack
            this.contentPack = contentPack;

            // sprite sheet info
            SpriteSheetInfo = new vxEntitySpriteSheetDefinition(type.FullName, contentPack, itemAttribute.SpritesheetLocation);

            
            vxSandboxEntityMetaAttribute meta = type.GetCustomAttribute<vxSandboxEntityMetaAttribute>();
            if (meta != null)
                SpriteSheetInfo.IconSource = meta.IconLocation;

            if(vxEntityRegister.EntitySpriteSheetRegister.ContainsKey(Key) == false)
                vxEntityRegister.EntitySpriteSheetRegister.Add(Key, SpriteSheetInfo);
            _contentPackKey = new vxPluginMetaInfo(contentPack);

        }

        public void GenerateIcon(vxGameplayScene3D scene3D)
        {
            this.Icon = vxInternalAssets.Textures.Blank;

            return;

            // TODO: Reinstate
            //if (File.Exists(Path.Combine(Engine.Game.Content.RootDirectory, FilePath + "_ICONf.xnb")))
            //    this.Icon = Engine.Game.Content.Load<Texture2D>(FilePath + "_ICON");
            //else
            //{
            //    this.Icon = vxInternalAssets.Textures.Blank;

            //    RenderTarget2D render = new RenderTarget2D(
            //    Engine.GraphicsDevice,
            //        vxGameplayScene3D.SandboxItemButtonSize, vxGameplayScene3D.SandboxItemButtonSize);

            //    // Create a new entity

            //    System.Reflection.ConstructorInfo ctor = Type.GetConstructor(new[] { typeof(vxGameplayScene3D), typeof(Vector3) });

            //    vxEntity3D entity;

            //    // if there isn't this constructor, then there should be one with just the scene
            //    if (ctor == null)
            //    {
            //        ctor = Type.GetConstructor(new[] { typeof(vxGameplayScene3D) });
            //        entity = (vxEntity3D)ctor.Invoke(new object[] { scene3D });
            //    }
            //    else
            //    {
            //        entity = (vxEntity3D)ctor.Invoke(new object[] { scene3D, Vector3.Zero });
            //    }
            //    //vxEntity3D entity = NewEntityDelegate(Scene);

            //    // Get the Bounds so that it'll fit to the screen.
            //    float zoom = entity.BoundingShape.Radius;
            //    float modelRadius = zoom * 2.45f;

            //    Engine.GraphicsDevice.SetRenderTarget(render);
            //    Engine.GraphicsDevice.Clear(Color.DimGray * 0.25f);

            //    //Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 0.001f, 10000);
            //    Matrix Projection = Matrix.CreateOrthographic(modelRadius, modelRadius, 0.001f, modelRadius * 2);

            //    var WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, modelRadius));

            //    WorldMatrix *=
            //        Matrix.CreateFromAxisAngle(Vector3.Right, -MathHelper.PiOver4 * 2 / 3) *
            //              Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4);

            //    var View = Matrix.Invert(WorldMatrix);

            //    Engine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            //    //entity.Draw(Matrix.CreateTranslation(-entity.ModelCenter), View, Projection, );

            //    //cnt++;
            //    //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + "ICON" + cnt + ".png";
            //    //Stream streampng = File.OpenWrite(path);
            //    //render.SaveAsPng(streampng, render.Width, render.Height);
            //    //streampng.Flush();
            //    //streampng.Close();
            //    //streampng.Dispose();
            //    this.Icon = render;
            //    Engine.GraphicsDevice.SetRenderTarget(null);
            //    entity.Dispose();
            //    //render.Dispose();
            //    //Thread.Sleep(10);
            //    //FileStream filestream = new FileStream(path, FileMode.Open);
            //    //this.Icon = Texture2D.FromStream(Engine.GraphicsDevice, filestream);


            //}
        }
    }
}