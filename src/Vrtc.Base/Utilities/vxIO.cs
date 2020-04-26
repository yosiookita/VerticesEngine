using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using VerticesEngine;
#if __ANDROID__
using Android.Views;
using Android.Content;
#endif

namespace VerticesEngine.Utilities
{
	/// <summary>
	/// Collection of static utility methods.
	/// </summary>
    public static class vxIO
    {
        static vxEngine Engine;

        /// <summary>
        /// Gets the log directory.
        /// </summary>
        /// <value>The log directory.</value>
        public static string LogDirectory
        {
            get { return Path.Combine(vxIO.PathToSettings, "logs"); }
        }

        static string virtexrootfolder = "My Games";
        static string GameName = "Virtex Game";


        internal static void Init(vxEngine engine)
        {
            Engine = engine;

            vxGameConfig config = engine.Game.Config;
            GameName = config.GameName;

            //if (vxEngine.PlatformType != vxPlatformType.Mobile)
                CheckDirExist();
        }

        /// <summary>
        /// This checks if the appropriate directories exist, but only fires after the permissions have been requested on the proper platforms.
        /// </summary>
        /// <param name="engine"></param>
        static void CheckDirExist()
        {
            EnsureDirExists(PathToSettings);
            EnsureDirExists(PathToSandbox);
            EnsureDirExists(PathToTempFolder);
            EnsureDirExists(PathToMods);
            EnsureDirExists(LogDirectory);
        }

        public static void EnsureDirExists(string path)
        {
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }

        public static string PathToRoot
        {
            get
            {
#if __ANDROID__
                //return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, virtexrootfolder , GameName);
                return Game.Activity.GetExternalFilesDir(string.Empty).AbsolutePath;
#elif __IOS__
                //return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + virtexrootfolder + "/" + GameName + "/" + sndpath;
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), virtexrootfolder, GameName);
#else
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), virtexrootfolder, GameName);
#endif
            }
        }
        /// <summary>
        /// The path to settings. If you'd like to modify this, then overide the GetSettingsPath() method in
        /// your main vxGame class.
        /// </summary>
        public static string PathToSettings
        {
            get
            {
                string modPath = "Profiles";

                modPath = Path.Combine(PathToRoot, modPath);
                return modPath;
            }
        }


        /// <summary>
        /// The path to sandbox. If you'd like to modify this, then overide the GetSandboxPath() method in
        /// your main vxGame class.
        /// </summary>
        public static string PathToSandbox
        {
            get
            {
                return Path.Combine(PathToRoot, "Sandbox");
            }
        }

        /// <summary>
        /// The path to temp folder.
        /// </summary>
        public static string PathToTempFolder
        {
            get
            {
                return Path.Combine(PathToRoot, "Temp");
            }
        }

        /// <summary>
        /// The path to localization files.
        /// </summary>
        public static string PathToLocalizationFiles
        {
            get
            {
                return Path.Combine(ContentRootPath, "local");
            }
        }



        public static string PathToMods
        {
            get
            {
                return Path.Combine(PathToRoot, "Mods");
            }
        }


        public static string ContentRootPath
        {
            get
            {
                var filePath = "Content";

                // OSX needs to move up a directory
                if (vxEngine.PlatformOS == vxPlatformOS.OSX && Directory.Exists("../Resources"))
                {
                    filePath = Path.Combine("../Resources", filePath);
                    vxConsole.WriteLine("USING OSX RESOURCE FOLDER");
                }
                return filePath;
            }
        }



        /// <summary>
        /// Clears the temp directory.
        /// </summary>
        public static void ClearTempDirectory()
        {
            //Clear Out the Temp Directory
            DirectoryInfo tempDirectory = new DirectoryInfo(PathToTempFolder);

            try
            {
                foreach (FileInfo file in tempDirectory.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in tempDirectory.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch
                    {
                    }
                }
            }
            catch { }
        }

        public static StreamReader LoadTextFile(string path)
        {
#if __ANDROID__
            return new StreamReader(TitleContainer.OpenStream(path));
#else
           return new StreamReader(path);
#endif
        }



        /// <summary>
        /// Load an Image from it's path. this is not for *.xnb files. Note this loads from the content directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture2D LoadImage(string path)
        {
            Texture2D texture = vxInternalAssets.Textures.DefaultDiffuse;
            try
            {
#if __ANDROID__
                Stream fileStream = Game.Activity.Assets.Open("Content/" + path + ".png");
                texture = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
#else
                var filePath = "Content/" + path + ".png";
                if (vxEngine.PlatformOS == vxPlatformOS.OSX && Directory.Exists("../Resources"))
                    filePath = "../Resources/" + filePath;

                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                {
                    texture = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
                }
#endif
            }
            catch(Exception ex)
            {
                vxConsole.WriteException("vxIO.LoadImage(...)", ex);
            }
            return texture;
            /*
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                Texture2D texture = Texture2D.FromStream(engine.GraphicsDevice, fileStream);
                fileStream.Close();
                return texture;
            }*/

            //using (var fileStream = TitleContainer.OpenStream(path))
            //{
            //    Texture2D texture = Texture2D.FromStream(engine.GraphicsDevice, fileStream);
            //    fileStream.Close();
            //    return texture;
            //}
        }

        #region File Compression

        public delegate void ProgressDelegate(string sMessage);

        public static void CompressFile(string sDir, string sRelativePath, GZipStream zipStream)
        {
            //Compress file name
            char[] chars = sRelativePath.ToCharArray();
            zipStream.Write(BitConverter.GetBytes(chars.Length), 0, sizeof(int));
            foreach (char c in chars)
                zipStream.Write(BitConverter.GetBytes(c), 0, sizeof(char));

            //Compress file content
            byte[] bytes = File.ReadAllBytes(Path.Combine(sDir, sRelativePath));
            zipStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
            zipStream.Write(bytes, 0, bytes.Length);
        }

        public static bool DecompressFile(string sDir, GZipStream zipStream, ProgressDelegate progress)
        {
            //Decompress file name
            byte[] bytes = new byte[sizeof(int)];
            int Readed = zipStream.Read(bytes, 0, sizeof(int));
            if (Readed < sizeof(int))
                return false;

            int iNameLen = BitConverter.ToInt32(bytes, 0);
            bytes = new byte[sizeof(char)];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < iNameLen; i++)
            {
                zipStream.Read(bytes, 0, sizeof(char));
                char c = BitConverter.ToChar(bytes, 0);
                sb.Append(c);
            }
            string sFileName = sb.ToString();
            if (progress != null)
                progress(sFileName);

            //Decompress file content
            bytes = new byte[sizeof(int)];
            zipStream.Read(bytes, 0, sizeof(int));
            int iFileLen = BitConverter.ToInt32(bytes, 0);

            bytes = new byte[iFileLen];
            zipStream.Read(bytes, 0, bytes.Length);

            string sFilePath = Path.Combine(sDir, sFileName);
            string sFinalDir = Path.GetDirectoryName(sFilePath);
            if (!Directory.Exists(sFinalDir))
                Directory.CreateDirectory(sFinalDir);
            TryAgain:

            try
            {
                using (FileStream outFile = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    outFile.Write(bytes, 0, iFileLen);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                goto TryAgain;
            }
            //Console.WriteLine("Decompressed too: "+sFinalDir);
            return true;
        }

        public static void CompressDirectory(string sInDir, string sOutFile, ProgressDelegate progress)
        {
            string[] sFiles = Directory.GetFiles(sInDir, "*.*", SearchOption.AllDirectories);
            int iDirLen = sInDir[sInDir.Length - 1] == Path.DirectorySeparatorChar ? sInDir.Length : sInDir.Length + 1;

            using (FileStream outFile = new FileStream(sOutFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (GZipStream str = new GZipStream(outFile, CompressionMode.Compress))
                foreach (string sFilePath in sFiles)
                {
                    string sRelativePath = sFilePath.Substring(iDirLen);
                    if (progress != null)
                        progress(sRelativePath);
                    CompressFile(sInDir, sRelativePath, str);
                }
        }

        public static void DecompressToDirectory(string sCompressedFile, string sDir, ProgressDelegate progress, bool IsContentFile = true)
		{
#if __ANDROID__
            //if (GamePlayType == GamePlayType.GamePlay)
            //{
            //    Stream reader = Game.Activity.Assets.Open(this.FilePath);
            //    LevelFile = (CartoonPhysicsLevelFile)deserializer.Deserialize(reader);
            //    reader.Close();
            //}
            //else
            //{
            //StreamReader reader = new StreamReader(this.FilePath);
            //LevelFile = (CartoonPhysicsLevelFile)deserializer.Deserialize(reader);
            //reader.Close();
            //}
            if (IsContentFile)
            {
                using (Stream inFile = Game.Activity.Assets.Open(sCompressedFile))
                using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                    while (DecompressFile(sDir, zipStream, progress));
            }
            else
			{
				using (FileStream inFile = new FileStream(sCompressedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
					while (DecompressFile(sDir, zipStream, progress)) ;
            }
#else
            using (FileStream inFile = new FileStream(sCompressedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                while (DecompressFile(sDir, zipStream, progress)) ;
#endif
		}




        #endregion
    }
}
