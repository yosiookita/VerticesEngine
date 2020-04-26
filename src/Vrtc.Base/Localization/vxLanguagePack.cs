using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using VerticesEngine.Utilities;
using VerticesEngine;




public enum vxLanguage
{
    English,
    French,
    Spanish,
    German,
    Russian,
    Japanese,
    Korean,
    Mandarin,
    Cantonese,
}

/// <summary>
/// A base set of keys for general text localization.
/// </summary>
public enum vxLocalisationKey
{
    GameName,

    SetLanguage,
    SetLanguageConfirmation,
    SetLanguageConfirmationVerbose,

    //Main Menu
    Play,
    Restart,
    LevelSelect,
    Multiplayer,
    Workshop,
    DLC,
    Mods,
    InstalledMods,
    Sandbox,
    Settings,
    Exit,

    // File IO
    Save,
    Saving,
    Load,
    Loading,

    //Multiplayer
    Splitscreen,
    Networked,

    // Networked
    SignIn,
    SignOut,
    UserName,
    LAN_Title,
    LAN,
    Online_Title,
    Online,
    Join,
    CreateNew,

    //Pause Screen
    Pause,
    Paused,
    Resume,
    Quit,
    QuitConfirm,

    // Tutorials,
    Tutorials,


    //Settings Page
    Controls,
    Graphics,
    Localisation,
    Audio,


    //Graphics Settings
    GraphicsSettings,
    Resolution,
    FullScreen,
    Windowed,

    //Keyboard Settings
    KeyboardSettings,
    GamepadSettings,
    MouseSettings,
    Keyboard_PresetTitle,

    //Keyboard Key Bindings Name
    KeyName_Forward,
    KeyName_Back,
    KeyName_Left,
    KeyName_Right,
    KeyName_Jump,
    KeyName_Croutch,
    KeyName_Interact1,
    KeyName_Interact2,

    //Audio
    Music,
    SoundEffects,

    // Profile
    Profile,
    UpgradeText,
    ViewLeaderboards,
    LeaderboardScoreSubmitted,
    ViewAchievements,
    LeaveAReview,
    ReviewAsk,
    ReviewAskTitle,


    //Misc
    Tutorial,
    HotKeys,
    Challenges,
    Next,
    Previous,
    Debug,
    Yes,
    No,
    OK,
    Cancel,
    New,
    Open,
    Back,
    PleaseWait,
    Level,
    Credits,
    File,
    Title,
    By,
    License,
    Link,
    Sound,
    Physics,
    Textures,
    Texture,
    Libraries,
    Library,
    From,
    Best,
    Top,
    GameOver,
    Score,
    Time,
    Speed,
    Velocity,
    Direction,
    Up,
    Down,
    Left,
    Right,
    Share,
    Sharing,

    //misc gameplay
    Enemy,
    Enemies,
    PowerUp,
    PowerUps,
    Boost

}

namespace VerticesEngine.Localization
{
    public class vxLanguagePackInfo
    {
        /// <summary>
        /// Get's the Language Name of this Pack
        /// </summary>
        [XmlAttribute("Name")]
        public vxLanguage Name = vxLanguage.English;


        [XmlAttribute("Remarks")]
        public string Remarks = "Add Remarks Here";


        [XmlAttribute("SpecificFontPath")]
        public string SpecificFontPath = "";

        public vxLanguagePackInfo() { }
    }

    public class vxLocalisedString
    {
        [XmlAttribute("key")]
        public string Key = "";

        [XmlElement("Text")]
        public string Value = "";

        [XmlAttribute("description")]
        public string Description;


        public vxLocalisedString() { }

        public vxLocalisedString(string Key, string Value, string Description)
        {
            this.Key = Key;
            this.Value = Value;

            if (Description != "")
                this.Description = Description;
        }
    }

    /// <summary>
    /// A base class which can be inherited too hold all text for a specific language pack.
    /// </summary>
    public partial class vxLanguagePack
    {
        /// <summary>
        /// Get's the Language Name of this Pack
        /// </summary>
        public string LanguageName
        {
            get { return Info.Name.ToString(); }
        }

        [XmlElement("LanguagePackInfo")]
        public vxLanguagePackInfo Info = new vxLanguagePackInfo();

        [XmlElement("LocalisedText")]
        public List<vxLocalisedString> Texts = new List<vxLocalisedString>();

        /// <summary>
        /// The collection holding all of the language keys and text.
        /// </summary>
        private Dictionary<object, string> Collection = new Dictionary<object, string>();



        /// <summary>
        /// Gets the <see cref="T:VerticesEngine.Localization.vxLanguagePackBase"/> with the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        public string this[object key]
        {
            get
            {
                key = key.ToString();
                if (Collection.ContainsKey(key))
                    return Collection[key];
                else
                {
                    string outputWarning = string.Format("KEY '{0}' IN LANGUAGE DICTIONARY NOT FOUND!", key);
                    vxConsole.WriteWarning(this.ToString(), outputWarning);

                    // if we can't find the string, then output the key split by capital characters
                    return key.ToString().SplitIntoSentance();
                }
            }
            set
            {
                key = key.ToString();
                if (Collection.ContainsKey(key))
                    Collection[key] = value;
                else
                {
                    string outputWarning = string.Format("KEY '{0}' IN LANGUAGE DICTIONARY NOT FOUND!", key);
                    vxConsole.WriteWarning(this.ToString(), outputWarning);
                }
            }
        }

        public vxLanguage Name;

        public vxLanguagePack(vxLanguage name) : this()
        {
            Info.Name = name;
        }

        /// <summary>
        /// Constructor for the vxLanguagePack base class. Add all of your required language keys here.
        /// </summary>
        public vxLanguagePack()
        {
            //Info.Name = Name;

            foreach (var val in Enum.GetValues(typeof(vxLocalisationKey)))
                Add(val);

            this[vxLocalisationKey.SetLanguage] = "Set the Language";
            this[vxLocalisationKey.SetLanguageConfirmation] = "Set '{0}' as the Current Language.";
            this[vxLocalisationKey.SetLanguageConfirmationVerbose] = "Set '{0}' as the Current Language.\n\nYou can change this later on in Settings > Localization.";


            this[vxLocalisationKey.ReviewAskTitle] = "Enjoying it?";
            this[vxLocalisationKey.ReviewAsk] = "Enjoying the Game?\nLeave a review!";

            this[vxLocalisationKey.OK] = "OK";

            this[vxLocalisationKey.QuitConfirm] = "Are you sure you want to quit?";
        }

        public void Add(object key)
        {
            Add(key, key.ToString().SplitIntoSentance());
        }

        /// <summary>
        /// Add's a line of text to the language pack based off of a supplied key.
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="key">Key.</param>
        /// <param name="Text">This GUI Items Text.</param>
        /// <param name="Description">Description.</param>
        public void Add(object key, string Text, string Description = "")
        {
            key = key.ToString();

            //First Check if the Key exists, if so, then overwrite the old values
            if (Collection.ContainsKey(key))
            {
                Collection[key] = Text;
            }
            //If not, Add a new Key
            else
            {
                Collection.Add(key, Text);

                Texts.Add(new vxLocalisedString(key.ToString(), Text, Description));
            }
        }


        void Clear()
        {
            // Now Clear out the collections
            Collection.Clear();
            Texts.Clear();
        }

        public void Initialise(vxLanguagePack language)
        {
            // Set up the Info
            Info = language.Info;

            //Clear();



            foreach (vxLocalisedString text in language.Texts)
            {
                //Add(text.Key, text.Value, text.Description);
                if (Collection.ContainsKey(text.Key))
                {
                    Collection[text.Key] = text.Value;
                }
            }
        }

        /// <summary>
        /// Dumps the unicode.
        /// </summary>
        public void DumpUnicode()
        {

            // update Text List
            foreach (var text in Texts)
                if (Collection.ContainsKey(text.Key))
                    text.Value = Collection[text.Key];

            var writer = new StreamWriter("unicode_" + LanguageName.ToLower() + ".xml");

            Dictionary<string, string> unicodes = new Dictionary<string, string>();
            foreach (var entry in Collection)
            {
                //writer.WriteLine("<!-- " + entry.Value + " -->");
                foreach (char c in entry.Value)
                {
                    string uC = ((int)c).ToString("D");

                    if (unicodes.ContainsKey(uC) == false)
                    {
                        unicodes.Add(uC, c.ToString());
                        string str = "<CharacterRegion><Start>&#" +
                            uC + ";</Start><End>&#" +
                            uC + ";</End></CharacterRegion>";
                        writer.WriteLine(str);
                    }
                    //Console.WriteLine(((int)'갟').ToString("X4"));
                    //Console.WriteLine(string.Format("{0} : {1}", c, ((int)c).ToString("")));
                }
            }
            writer.Close();
        }



        public void DumpLanguage(string type)
        {

            if (type == "xml")
            {
                // update Text List
                foreach (var text in Texts)
                    if (Collection.ContainsKey(text.Key))
                        text.Value = Collection[text.Key];

                string name = LanguageName.ToLower() + ".xml";
                XmlSerializer serializer = new XmlSerializer(typeof(vxLanguagePack));
                using (TextWriter writer = new StreamWriter(name))
                {
                    serializer.Serialize(writer, this);
                }
            }
            else if (type == "csv")
            {
                var writer = new StreamWriter(LanguageName.ToLower() + "." + type);
                writer.WriteLine("Language Translation:," + LanguageName);
                writer.WriteLine("Items:," + Texts.Count + "");

                writer.WriteLine("Key;Text;Translation;Notes (If Needed)");

                // update Text List
                foreach (var text in Texts)
                {
                    if (Collection.ContainsKey(text.Key))
                    {
                        text.Value = Collection[text.Key];

                        writer.WriteLine(text.Key + ";" + text.Value.Replace(System.Environment.NewLine, " ") + ";;" + text.Description);
                    }
                }

                System.Diagnostics.Process.Start(Environment.CurrentDirectory);
                writer.Close();
                vxConsole.WriteLine(string.Format("Language '{0}' saved to '{1}'", Info.Name, Environment.CurrentDirectory));
            }
            else if (type == "html")
            {
                var writer = new StreamWriter(LanguageName.ToLower() + "." + type);
                writer.WriteLine("<h1>Language Translation - " + LanguageName + "</h1>");
                writer.WriteLine("<p>Items: " + Texts.Count + "</p>");

                // update Text List
                foreach (var text in Texts)
                {
                    if (Collection.ContainsKey(text.Key))
                    {
                        text.Value = Collection[text.Key];

                        writer.WriteLine("<p>");
                        writer.WriteLine("<b>Text: " + text.Value + "</b>");
                        if (text.Description != "")
                            writer.WriteLine("<p>Description: " + text.Description + "</p>");
                        writer.WriteLine("<p>key: " + text.Key + "</p>");
                        writer.WriteLine("</p><hr>");

                    }
                }

                writer.Close();
                vxConsole.WriteLine(string.Format("Language '{0}' saved to '{1}'", Info.Name, Environment.CurrentDirectory));
            }
            else 
            {
                vxConsole.WriteLine(string.Format("File Type '{0}' Not Supported. Choose XML, HTML or CSV"));
            }
        }


        public void Save()
        {
            // update Text List
            foreach (var text in Texts)
                if (Collection.ContainsKey(text.Key))
                    text.Value = Collection[text.Key];

            string name = LanguageName.ToLower() + ".xml";
            XmlSerializer serializer = new XmlSerializer(typeof(vxLanguagePack));
            using (TextWriter writer = new StreamWriter(name))
            {
                serializer.Serialize(writer, this);
            }
            vxConsole.WriteLine(string.Format("Language '{0}' saved to '{1}'", Info.Name, Environment.CurrentDirectory));
        }

        public static void Save(vxLanguagePack LanguagePack)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(vxLanguagePack));
            using (TextWriter writer = new StreamWriter(LanguagePack.LanguageName.ToLower() + ".xml"))
            {
                serializer.Serialize(writer, LanguagePack);
            }
        }


        public static vxLanguagePack Load(vxEngine Engine, vxLanguage language)
        {
            //try
            //{
            XmlSerializer deserializer = new XmlSerializer(typeof(vxLanguagePack));

            string path = System.IO.Path.Combine(vxIO.PathToLocalizationFiles, language.ToString().ToLower() + ".xml");

#if __ANDROID__
            var reader = vxGame.Activity.Assets.Open(path);

#else
            TextReader reader = new StreamReader(path);
#endif

            object obj = deserializer.Deserialize(reader);
            //vxLanguagePack file = (vxLanguagePack)obj;

            vxLanguagePack pack = new vxLanguagePack();

            pack.Initialise((vxLanguagePack)obj);

            reader.Close();

            return pack;
            //}
            //catch (Exception ex)
            //{
            //    vxConsole.WriteError(ex);
            //}
            //return null;
        }


        #region -- Debug Methods --

        [Diagnostics.vxDebugMethod("local", "Provides Localization Commands for debugging.")]
        static void LocalisationDebug(vxEngine Engine, string[] args)
        {
            if (args.Length == 0)
            {

                vxConsole.Echo("Localization Settings");
                vxConsole.Echo("============================================================");
                vxConsole.Echo(string.Format("     Current Language: {0}", Engine.Language.LanguageName));
                vxConsole.Echo(string.Format("Available Languages"));
                int count = 0;
                foreach (vxLanguagePack pack in Engine.Languages)
                    vxConsole.Echo(string.Format("     {0}: {1}", count++, pack.LanguageName));


            }
            else
            {
                //foreach (string arg in args)
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "-set":
                            try
                            {
                                int index = int.Parse(args[1]);
                                if (index > 0 && index < Engine.Languages.Count)
                                    Engine.Language = Engine.Languages[index];
                            }
                            catch (Exception ex)
                            {
                                vxConsole.WriteError(ex.Message);
                            }
                            break;
                        case "-dump":
                            if (args.Length == 1)
                                Engine.Language.DumpLanguage("csv");
                            else
                            {
                                Engine.Language.DumpLanguage(args[1].Substring(1));
                            }

                            break;
                        case "-unidump":
                            vxConsole.Echo("Dumping Unicode Values...");
                            Engine.Language.DumpUnicode();
                            vxConsole.Echo("Done!");

                            //Console.WriteLine(((int)'갟').ToString("X4"));
                            break;

                        case "-help":
                            vxConsole.Echo("");
                            vxConsole.Echo("Help [Localization Settings]");
                            vxConsole.Echo("-------------------------------------------------");
                            vxConsole.Echo(" -set [index]     : Sets the current language to the index specified.");
                            vxConsole.Echo(" -dump [file type]   : Dumps the current language to an CSV, XML or HTML file.");
                            vxConsole.Echo("");
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
