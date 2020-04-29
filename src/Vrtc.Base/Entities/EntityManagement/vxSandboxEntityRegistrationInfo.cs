
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using VerticesEngine.Graphics;
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
    }
}