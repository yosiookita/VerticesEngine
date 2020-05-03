#if __STEAM__
using Facepunch.Steamworks;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using VerticesEngine.Community;
using VerticesEngine.Community.Events;
using VerticesEngine.Plugins;
using VerticesEngine.Utilities;

namespace VerticesEngine.Profile
{
    /// <summary>
    /// Basic wrapper for interfacing with the Platform Specfiic Play Services Game API's Such as Google Play and Steam.
    /// </summary>
    public class vxPlayerProfileSteam : vxIPlayerProfile
    {
        public bool IsSignedIn
        {
            get { return _isSighnedIn; }
        }

        private bool _isSighnedIn = false;


        public string DisplayName
        {
            get
            {
                return _steamUserName;
            }
        }
        private string _steamUserName = "";

        public string UserID
        {
            get
            {
                return _steamUserID;
            }
        }
        string _steamUserID = "";

        private string CurrentLanguage { get; set; } = "";
        private string AvailableLanguages { get; set; } = "";
        private string InstallDir { get; set; } = "";

        public PlayerProfileBackend ProfileBackend
        {
            get
            {
                return PlayerProfileBackend.Steam;
            }
        }


        public Texture2D Avatar
        {
            get
            {
                return _steamUserAvatar;
            }
        }
        private Texture2D _steamUserAvatar;

        public string PreferredLanguage
        {
            get { return _preferredLanguage; }
        }

        public Dictionary<object, vxAchievement> Achievements 
        {
            get { return achievements; }
        }

        string _preferredLanguage = "english";


        vxEngine Engine;



        /// <summary>
        /// Initialises a New Player Profile using Steam as the backend.
        /// </summary>
        /// <param name="Engine"></param>
        internal vxPlayerProfileSteam(vxEngine Engine)
        {
            this.Engine = Engine;
        }

        public void Initialise()
        {
            int p = (int)Environment.OSVersion.Platform;
            //Console.WriteLine(Environment.OSVersion.Platform);
            //Console.WriteLine(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

            if (vxEngine.PlatformOS == vxPlatformOS.OSX)
            {
                Facepunch.Steamworks.Config.ForcePlatform(Facepunch.Steamworks.OperatingSystem.Osx, 
                                                          Facepunch.Steamworks.Architecture.x64);
            }

            SteamClient = new Facepunch.Steamworks.Client(uint.Parse(Engine.Game.AppID));

            // Make sure we started up okay
            _isSighnedIn = SteamClient.IsValid;

            if (_isSighnedIn)
            {
                _steamUserName = SteamClient.Username;
                _steamUserID = SteamClient.SteamId.ToString();

                if (vxEngine.BuildType == vxBuildType.Debug)
                {
                    //vxConsole.WriteLine("Steam Account Logged In");
                    //vxConsole.WriteLine("     Username: " + SteamClient.Username);
                    //vxConsole.WriteLine("     SteamID:  " + SteamClient.SteamId);
                    //vxConsole.WriteLine("     AppID:    " + SteamClient.AppId);
                    //vxConsole.WriteLine("     BuildID:  " + SteamClient.BuildId);
                }
            }
        }

        public string[] GetInstalledMods()
        {
            // for steam, there are two directories to search, the mod directory under 'My Games/...' and under the steam location

            List<string> mods = new List<string>();

            // the first local list under 'My Games/...'
            var regularList = vxPluginManager.GetAvailableModsInPath(vxIO.PathToMods);
            mods.AddRange(regularList);

            // now get the 
            if (IsSignedIn && SteamClient.InstallFolder != null)
            {
                DirectoryInfo dir = new DirectoryInfo(System.IO.Path.Combine(SteamClient.InstallFolder.FullName, "../../workshop/content", SteamClient.AppId.ToString()));

                var steamList = vxPluginManager.GetAvailableModsInPath(dir.FullName);
                mods.AddRange(steamList);
            }
            return mods.ToArray();
        }


        Workshop.Query query;
        public void SearchWorkshop(vxWorkshopSearchCriteria searchCrteria)
        {
            if (IsSignedIn)
            {
                // capture the search criteria here
                vxWorkshop.OnSearch(searchCrteria);

                 query = SteamClient.Workshop.CreateQuery();
                
                
                query.QueryType = Workshop.QueryType.SubscriptionItems;

                    switch(searchCrteria.ItemCriteria)
                {
                    case vxWorkshopItemSearchCriteria.AllPublished:
                        query.UserQueryType = Workshop.UserQueryType.Published;
                        break;
                    case vxWorkshopItemSearchCriteria.MyPublished:
                        query.UserQueryType = Workshop.UserQueryType.Published;
                        query.UserId = SteamClient.SteamId;
                        break;
                        case vxWorkshopItemSearchCriteria.Subscribed:
                        query.UserQueryType = Workshop.UserQueryType.Subscribed;
                        query.UserId = SteamClient.SteamId;
                            break;
                        case vxWorkshopItemSearchCriteria.Favourited:
                        query.UserQueryType = Workshop.UserQueryType.Favorited;
                        query.UserId = SteamClient.SteamId;
                            break;
                        case vxWorkshopItemSearchCriteria.Followed:
                        query.UserQueryType = Workshop.UserQueryType.Followed;
                        query.UserId = SteamClient.SteamId;
                            break;
                    }

                //SteamClient.Workshop.GetItem(0).
                foreach (var tag in searchCrteria.TagsToInclude)
                    query.RequireTags.Add(tag);

                foreach (var tag in searchCrteria.TagsToExclude)
                    query.ExcludeTags.Add(tag);


                //query.RequireTags.Add("Mod");


                if (searchCrteria.SearchText != "")
                    query.SearchText = searchCrteria.SearchText;
                //Workshop.UserQueryType.Subscribed
                //query.RequireTags.Add("Mod");
                query.OnResult = OnSearchResultReceived;
                query.Run();
            }
            else
            {
                //vxSceneManager.AddScreen(new vxMes)
            }
        }

        void OnSearchResultReceived(Workshop.Query obj)
        {
            List<vxIWorkshopItem> workshopItems = new List<vxIWorkshopItem>();

            if (obj.Items != null)
            {
                foreach (var item in obj.Items)
                {
                    if(item.Title != null && item.Title != "")
                        workshopItems.Add(new vxWorkshopItem(item));
                }

                SteamClient.Workshop.OnItemInstalled += Workshop_OnItemInstalled;  
                SearchResultReceived?.Invoke(this, new vxWorkshopSeachReceievedEventArgs(workshopItems));
            }
        }

        void Workshop_OnItemInstalled(ulong obj)
        {
            vxConsole.WriteNetworkLine("Installed: "+obj);
        }



        public event EventHandler<vxWorkshopSeachReceievedEventArgs> SearchResultReceived; 

        public event EventHandler<vxWorkshopItemPublishedEventArgs> ItemPublished; 
        // Do something here before the event…  


        /// <summary>
        /// The steam client.
        /// </summary>
        Facepunch.Steamworks.Client SteamClient;


        Workshop.Editor _item;


        public bool IsPublishing { get { return _isPublishing; } }
        bool _isPublishing = false;

        public float PublishProgress { get { return _publishProgress; } }
        float _publishProgress = 0;




        public void Publish(string title, string description, string imgPath, string folderPath, string[] tags,
                           ulong idToUpdate = 0, string changelog = "")
        {
            if (IsSignedIn)
            {
                Console.WriteLine("Uploading Folder: " + folderPath);

                if (idToUpdate == 0)
                {
                    vxConsole.WriteNetworkLine("Creating New Workshop Item...");
                    _item = SteamClient.Workshop.CreateItem(uint.Parse(Engine.Game.AppID), Workshop.ItemType.Community);
                    //_item.Tags.Add("Mods");
                    //_item.Tags.Add("Mod");
                }
                else
                {
                    vxConsole.WriteNetworkLine("Updating Workshop Item '"+idToUpdate+"'...");
                    _item = SteamClient.Workshop.EditItem(idToUpdate);
                    _item.WorkshopUploadAppId = uint.Parse(Engine.Game.AppID);

                }



                //Facepunch.Steamworks
                _item.Title = title;
                _item.Description = description;
                _item.Visibility = Workshop.Editor.VisibilityType.Public;

                _item.PreviewImage = imgPath; 

                _item.Folder = folderPath;

                _item.Tags.AddRange(tags);

                    _item.ChangeNote = changelog;

                _item.Publish();

                vxConsole.WriteNetworkLine("Done");
            }
        }


        public void OpenURL(string url)
        {
            SteamClient.Overlay.OpenUrl(url);
        }

        public void OpenStorePage(string url)
        {
            OpenURL(url);
        }

        //bool dl = false;
        public void Update()
        {
            if(SteamClient != null)
                SteamClient.Update();

            if (_item != null)
            {
                _isPublishing = _item.Publishing;
                if (_item.Publishing)
                {
                    if (Math.Abs(_item.Progress) < 0.01f)
                    {
                        _isPublishing = _item.Publishing;
                        Console.WriteLine("Publishing started, please wait.");
                    }
                    else
                    {
                        Console.WriteLine("Publishing: " + _item.Progress);
                        _publishProgress = (float)_item.Progress;
                    }
                }
                else
                {
                    _publishProgress = 0;
                    _isPublishing = false;

                    string info = (_item.Error == null) ? "Upload Successful!" : _item.Error;

                    ItemPublished?.Invoke(this, new vxWorkshopItemPublishedEventArgs(_item.Id, (_item.Error == null), info));

                    vxConsole.WriteNetworkLine("Done publishing: ID: " + _item.Id + "\n" + _item.Error);

                    _item = null;
                }
            }
        }

        public Dictionary<object, vxAchievement> achievements = new Dictionary<object, vxAchievement>();

        public void AddAchievement(object key, vxAchievement achievement)
        {

        }

        public vxAchievement GetAchievement(object key)
        {
            return achievements[key];
        }

        public Dictionary<object, vxAchievement> GetAchievements()
        {
            return achievements;
        }

        public void IncrementAchievement(object key, int increment)
        {

        }

        public void ShareImage(string path, string extratxt = "")
        {

        }

        public void SignIn()
        {
            //throw new NotImplementedException();
        }

        public void SignOut()
        {
            //throw new NotImplementedException();
        }

        public void SubmitLeaderboardScore(string id, long score)
        {

        }

        public void UnlockAchievement(object key)
        {

        }

        public void ViewAchievments()
        {

        }

        public void ViewLeaderboard(string id)
        {

        }




        #region Player Helper Methods
        /// <summary>
        ///     Get your steam avatar.
        ///     Important:
        ///     The returned Texture2D object is NOT loaded using a ContentManager.
        ///     So it's your responsibility to dispose it at the end by calling <see cref="Texture2D.Dispose()" />.
        /// </summary>
        /// <param name="device">The GraphicsDevice</param>
        /// <returns>Your Steam Avatar Image as a Texture2D object</returns>
        private Texture2D GetSteamUserAvatar()
        {
            if(SteamClient != null && SteamClient.SteamId != 0)
                SteamClient.Friends.GetAvatar(Facepunch.Steamworks.Friends.AvatarSize.Medium, SteamClient.SteamId, OnImage);
            
            return null;
        }


        void OnImage(Facepunch.Steamworks.Image image)
        {

            if (image != null && image.IsLoaded)
            {
                _steamUserAvatar = new Texture2D(Engine.GraphicsDevice, image.Width, image.Height);
                Avatar.SetData<byte>(image.Data);
                vxConsole.WriteNetworkLine("Image Loaded");
                vxConsole.WriteNetworkLine("     Width:  "+image.Width);
                vxConsole.WriteNetworkLine("     Height: " + image.Height);
            }
            else
            {
                vxConsole.WriteException(this, new Exception("Error Loading Avatar"));
            }
        }


        /// <summary>
        ///     Replaces characters not supported by your spritefont.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="input">The input string.</param>
        /// <param name="replaceString">The string to replace illegal characters with.</param>
        /// <returns></returns>
        public static string ReplaceUnsupportedChars(SpriteFont font, string input, string replaceString = "")
        {
            string result = "";
            if (input == null)
            {
                return null;
            }

            foreach (char c in input)
            {
                if (font.Characters.Contains(c) || c == '\r' || c == '\n')
                {
                    result += c;
                }
                else
                {
                    result += replaceString;
                }
            }
            return result;
        }



        public void Dispose()
        {

        }

        public void ViewAllLeaderboards()
        {

        }

        public void InitialisePlayerInfo()
        {
            vxConsole.WriteLine("Profile.InitialisePlayerInfo()");

            GetSteamUserAvatar();
        }


        #endregion
    }
}
#endif