#if !__ANDROID__ && !__IOS__ && !__STEAM__
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerticesEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.UI.Controls;
using VerticesEngine.Community;
using VerticesEngine.Community.Events;

namespace VerticesEngine.Profile
{
    /// <summary>
    /// Basic wrapper for interfacing with the GooglePlayServices Game API's
    /// </summary>
    public class vxPlayerProfile : vxIPlayerProfile
    {
        vxEngine Engine;

        public vxPlayerProfile(vxEngine Engine)
        {
            this.Engine = Engine;
        }


        // Sign In - Out
        // **********************************************************

        public bool IsSignedIn
        {
            get { return _isSteamRunning; }
        }

        private bool _isSteamRunning = false;

        //bool vxIPlayerProfile.IsSignedIn => throw new NotImplementedException();


        public void SignIn()
        {
            //vxNotificationManager.Add(new vxNotification(Engine, "Signing In ...", Color.DeepPink));
        }

        public void SignOut()
        {
            //vxNotificationManager.Add(new vxNotification(Engine, "Signing Out ...", Color.DeepPink));
        }



        // User Info
        // **********************************************************


        public void ShareImage(string path, string extratxt = "")
        {
            //throw new NotImplementedException();
        }

        public string DisplayName
        {
            get { return ""; }
        }

        public string UserID
        {
            get { return ""; }
        }


        public PlayerProfileBackend ProfileBackend
        {
            get { return PlayerProfileBackend.None; }
        }


        // Achievements
        // **********************************************************





        public Texture2D Avatar
        {
            get
            {
                return null;
            }
        }

        public string PreferredLanguage
        {
            get { return "english"; }
        }

        public Dictionary<object, vxAchievement> Achievements 
        { 
            get 
            {
                return _achievements;
            } 
        }

        public bool IsPublishing
        {
            get; set;
        }

        public float PublishProgress
        {
            get; set;
        }

        Dictionary<object, vxAchievement> _achievements = new Dictionary<object, vxAchievement>();

        public event EventHandler<vxWorkshopSeachReceievedEventArgs> SearchResultReceived;

        public event EventHandler<vxWorkshopItemPublishedEventArgs> ItemPublished;

        public void AddAchievement(object key, vxAchievement achievement)
        {
            _achievements.Add(key, achievement);
        }

        public vxAchievement GetAchievement(object key)
        {
            return _achievements[key];
        }

        public Dictionary<object, vxAchievement> GetAchievements()
        {
            return _achievements;
        }

        public void IncrementAchievement(object key, int increment)
        {
            //Achievements[key].
        }

        public void UnlockAchievement(object key)
        {
            _achievements[key].Achieved = true;
            vxNotificationManager.Add(new vxNotification(Engine, "Achievement Unlocked! : " + key, Color.DeepPink));
        }

        public void ViewAchievments()
        {

        }




        // Leaderboards
        // **********************************************************
        public void SubmitLeaderboardScore(string id, long score)
        {
            vxNotificationManager.Add(new vxNotification(Engine, "Score Submitted: " + score, Color.DeepPink));
        }

        public void ViewLeaderboard(string id)
        {

        }

        public void Initialise()
        {
            //throw new NotImplementedException();
        }

        public void InitialisePlayerInfo()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void ViewAllLeaderboards()
        {
            //throw new NotImplementedException();
        }

        public void SearchWorkshop(vxWorkshopSearchCriteria searchCrteria)
        {
            //throw new NotImplementedException();
        }

        public void Publish(string title, string description, string imgPath, string folderPath, ulong idToUpdate = 0, string changelog = "")
        {
            //throw new NotImplementedException();
        }

        public void Update()
        {
            //throw new NotImplementedException();
        }

        public void OpenURL(string url)
        {
            //throw new NotImplementedException();
        }

        public void OpenStorePage(string url)
        {
            //throw new NotImplementedException();
        }


        public string[] GetInstalledMods()
        {
            return new string[] { };
        }


        public void Publish(string title, string description, string imgPath, string folderPath, string[] tags,
                            ulong idToUpdate = 0, string changelog = "")
        { }
    }
}
#endif