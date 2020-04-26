using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using VerticesEngine.Utilities;
using VerticesEngine.Serilization;
using VerticesEngine.Graphics;
using System.Reflection;

namespace VerticesEngine
{
    public enum vxEnumQuality
    {
        None,
        Low,
        Medium,
        High,
        Ultra
    }

    /// <summary>
    /// The Serializable Settings class. This class creates Property get/set .
    /// </summary>
    public class vxSettings
    {
        [vxEngineSettingsAttribute("GameVersion")]
        public static vxSerializableVersion GameVersion;

        [vxEngineSettingsAttribute("Language")]
        public static string Language = "";


        [vxEngineSettingsAttribute("srp")]
        public static bool ShownReviewPage = false;


        [vxEngineSettingsAttribute("rc")]
        public static int ReviewCounter = 0;

        public vxSettings()
        {
            GameVersion = new vxSerializableVersion();
        }



        public void Load(vxEngine Engine)
        {
            try
            {
                string path = Path.Combine(vxIO.PathToSettings, "settings.set");
                vxConsole.WriteIODebug(string.Format("Loading Settings at: {0}", path));

                XmlSerializer deserializer = new XmlSerializer(typeof(vxSettings));
                TextReader reader = new StreamReader(path);
                object obj = deserializer.Deserialize(reader);
                vxSettings Settings = (vxSettings)obj;
                reader.Close();

                /*
                this.GameVersion = Settings.GameVersion;
                this.ReviewCounter = Settings.ReviewCounter;
                this.Language = Settings.Language;
                */

            }

            catch (Exception exception)
            {
                vxConsole.WriteLine("Error Loading Settings! " + exception.Message);
                /*
                // There was an issue loading the Settings, then default to the Screen Settings
                Graphics.Screen.Resolution = new Point(
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                    GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

                Graphics.Screen.IsFullScreen = true;
                */

                Save(Engine);
            }
        }

        public void Save(vxEngine Engine)
        {
            try
            {
                vxConsole.WriteIODebug("Saving New Settings File.");
                //Write The Sandbox File
                XmlSerializer serializer = new XmlSerializer(typeof(vxSettings));
                using (TextWriter writer = new StreamWriter(Path.Combine(vxIO.PathToSettings, "settings.set")))
                {
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception exception)
            {
                vxConsole.WriteLine("Error Saving Settings : " + exception.Message);
            }
        }


        #region -- Loading and Saving ini files

        public static List<Type> settingTypes = new List<Type>();

        internal static void Init()
        {
            // get all classes which inherit from the vxSettingsAttribute class
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(vxSettingsAttribute)))
                    settingTypes.Add(t);
            }


            LoadINI();
        }

        public static void LoadINI()
        {
            foreach (var tp in settingTypes)
            {
                LoadINIFile(tp);
            }
        }

        public static void LoadINIFile(Type tp)
        {
            var st = tp.Name;
            string settingName = st.Substring(2, st.IndexOf("Attribute") - 2);

            // dictionary of keys and values
            Dictionary<string, string> settings = new Dictionary<string, string>();

            string filePath = Path.Combine(vxIO.PathToSettings, settingName + ".ini");

            // if there's no file, then save a version
            if (!File.Exists(filePath))
            {
                SaveINIFile(tp);
                //return;
            }

            //settings
            string[] settingsText = File.ReadAllLines(filePath);

            foreach (var line in settingsText)
            {
                if (line.Contains("="))
                {
                    string key = line.Substring(0, line.IndexOf("="));
                    string value = line.Substring(line.IndexOf("=") + 1);
                    settings.Add(key, value);
                    Console.WriteLine(string.Format("'{0}':'{1}'", key, value));
                }
            }


            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // get member
                var fields = type.GetFields().Where(field => Attribute.IsDefined(field, tp));
                foreach (FieldInfo setting in fields)
                {
                    var set = setting.GetCustomAttribute<vxSettingsAttribute>();
                    if (settings.ContainsKey(set.DisplayName))
                    {
                        if (setting.FieldType == typeof(bool))
                        {
                            setting.SetValue(setting, bool.Parse(settings[set.DisplayName]));
                        }
                        else if (setting.FieldType == typeof(int))
                        {
                            setting.SetValue(setting, int.Parse(settings[set.DisplayName]));
                        }
                        else if (setting.FieldType == typeof(float))
                        {
                            setting.SetValue(setting, float.Parse(settings[set.DisplayName]));
                        }
                        else if (setting.FieldType.IsEnum)
                        {
                            setting.SetValue(setting, Enum.Parse(setting.FieldType, settings[set.DisplayName]));
                        }
                        else
                        {
                            Console.WriteLine("----- No Setting Found for '" + setting.FieldType.ToString() + "' -----");
                        }
                        //Console.WriteLine("Property " + field.Name + " - " + field);
                    }

                }


                // get member
                var properties = type.GetProperties().Where(field => Attribute.IsDefined(field, tp));
                foreach (PropertyInfo setting in properties)
                {
                    var set = setting.GetCustomAttribute<vxSettingsAttribute>();
                    if (settings.ContainsKey(set.DisplayName))
                    {
                        if (setting.PropertyType == typeof(bool))
                        {
                            setting.SetValue(setting, bool.Parse(settings[set.DisplayName]));
                        }
                        else if (setting.PropertyType == typeof(int))
                        {
                            setting.SetValue(setting, int.Parse(settings[set.DisplayName]));
                        }
                        else if (setting.PropertyType == typeof(float))
                        {
                            setting.SetValue(setting, float.Parse(settings[set.DisplayName]));
                        }
                        else if (setting.PropertyType.IsEnum)
                        {
                            setting.SetValue(setting, Enum.Parse(setting.PropertyType, settings[set.DisplayName]));
                        }
                        else
                        {
                            Console.WriteLine("----- No Setting Found for '" + setting.PropertyType.ToString() + "' -----");
                        }
                        //Console.WriteLine("Property " + field.Name + " - " + field);
                    }

                }
            }
        }
        public static void SaveINI()
        {
            foreach (var tp in settingTypes)
            {
                SaveINIFile(tp);
            }
        }
        public static void SaveINIFile(Type tp)
        {
            var st = tp.Name;
            string settingName = st.Substring(2, st.IndexOf("Attribute") - 2);


            StreamWriter writer = new StreamWriter(Path.Combine(vxIO.PathToSettings, settingName + ".ini"));

            bool hasAttribute = false;
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                // get fields
                var fields = type.GetFields().Where(field => Attribute.IsDefined(field, tp));
                foreach (FieldInfo field in fields)
                {
                    if (hasAttribute == false)
                    {
                        hasAttribute = true;

                        writer.WriteLine("");
                        writer.WriteLine("[" + type.Name + "]");
                    }
                    var attr = field.GetCustomAttribute<vxSettingsAttribute>();

                    if(attr.IsSavedToINIFile)
                        writer.WriteLine(string.Format("{0}={1}", attr.DisplayName, field.GetValue(field)));

                }

                // get properties
                var properties = type.GetProperties().Where(prop => Attribute.IsDefined(prop, tp));
                foreach (var prop in properties)
                {
                    if (hasAttribute == false)
                    {
                        hasAttribute = true;

                        writer.WriteLine("");
                        writer.WriteLine("[" + type.Name + "]");
                    }
                    var attr = prop.GetCustomAttribute<vxSettingsAttribute>();

                    if (attr.IsSavedToINIFile)
                        writer.WriteLine(string.Format("{0}={1}", attr.DisplayName, prop.GetValue(prop)));
                }


                hasAttribute = false;
            }
            writer.Close();
        }


        #endregion
    }
}
