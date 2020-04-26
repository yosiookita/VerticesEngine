using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using VerticesEngine.ContentManagement;

namespace VerticesEngine
{
    public class vxTerrainManager : vxGameObject
    {
        /*
        Texture2D Texture01;// { get { return Textures[0]; } }
        Texture2D Texture02;// { get { return Textures[1]; } }
        Texture2D Texture03;// { get { return Textures[2]; } }
        Texture2D Texture04;// { get { return Textures[3]; } }
        */
        public List<Texture2D> Textures = new List<Texture2D>();

        /// <summary>
        /// A list of all the Terrains in the current scene
        /// </summary>
        public List<vxTerrainEntity> Terrains = new List<vxTerrainEntity>();

        public vxTerrainManager()
        {
            /*
            Textures.Add(Texture01);
            Textures.Add(Texture02);
            Textures.Add(Texture03);
            Textures.Add(Texture04);
            */
            LoadTexturePack("Textures/terrain/txtrs", true);
        }

        /// <summary>
        /// Loads a Texture Pack using the speciefied path. Each texture must have the notation of
        /// 'texture_i' where 'i' is the texture index.
        /// </summary>
        /// <param name="path">path to the four textures.</param>
        public void LoadTexturePack(string path, bool UseEngineContentManager = false)
        {
            ContentManager Content = Engine.Game.Content;

            if (UseEngineContentManager)
                Content = vxContentManager.Instance;


            for(int i = 0; i < 4; i++)
                Textures.Add(Content.Load<Texture2D>(Path.Combine(path, "texture_"+i.ToString())));

            UpdateTextures();
        }

        /// <summary>
        /// Adds a Terrain to the terrain manager.
        /// </summary>
        /// <param name="terrain"></param>
        public void Add(vxTerrainEntity terrain)
        {
            // Set the Textures
            SetTextures(terrain);

            Terrains.Add(terrain);
        }

        void UpdateTextures()
        {
            foreach (vxTerrainEntity terrain in Terrains)
                SetTextures(terrain);
        }

        void SetTextures(vxTerrainEntity terrain)
        {
            terrain.Texture01 = Textures[0];
            terrain.Texture02 = Textures[1];
            terrain.Texture03 = Textures[2];
            terrain.Texture04 = Textures[3];
        }

        internal void Update()
        {

        }
    }
}