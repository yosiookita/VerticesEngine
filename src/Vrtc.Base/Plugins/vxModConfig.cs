using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VerticesEngine.Utilities;

namespace VerticesEngine.Plugins
{
    public class vxModConfig
    {
        public long ID = 0;

        public string File = "mymod.dll";

        public string Name = "my mod's main dll name";

        public string Description = "my mod description";

        public string Author = "";

        [JsonIgnore]
        public string Path;

        private vxModConfig()
        {

        }

        public const string ConfigFileName = "modconfig.json";

        /// <summary>
        /// Loads a modconfig.json file from the path specified. Note you can only have
        /// one modconfig per folder and mod.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static vxModConfig Load(string path)
        {
            string filePath = System.IO.Path.Combine(path, ConfigFileName);

            try
            {
                StreamReader reader = new StreamReader(filePath);

                string json = reader.ReadToEnd();
                reader.Close();

                var config = JsonConvert.DeserializeObject<vxModConfig>(json);

                config.Path = path;

                return config;
            }
            catch (Exception ex)
            {
                vxConsole.WriteException("vxModConfig.Load(...)", ex);
                return new vxModConfig();
            }
        }

        public void Save()
        {

            // get the save path
            string pathToSave = System.IO.Path.Combine(Path, ConfigFileName);

            // now wipe out the path data bc we don't want to save that
            //this.Path = "";

            // now convert it to JSON
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            // now save the file
            StreamWriter writer = new StreamWriter(pathToSave);
            writer.Write(json);
            writer.Close();
        }
    }

    public class vxModItemStatus
    {
        /// <summary>
        /// The name of this mod
        /// </summary>
        public string Name;

        /// <summary>
        /// The path to this mod
        /// </summary>
        public string Path;

        /// <summary>
        /// Is this mod enabled?
        /// </summary>
        public bool IsEnabled = false;
    }
}
