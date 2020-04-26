using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Community;
using VerticesEngine.Community.Events;

namespace VerticesEngine.Profile
{
    public enum PlayerProfileBackend
    {
        None,

        Steam,

        Discord,

        GooglePlayServices,

        iOSGameCenter
    }

    public interface vxIPlayerProfile
    {

        /// <summary>
        /// Is the user signed in.
        /// </summary>
        /// <returns><c>true</c>, if signed in was ised, <c>false</c> otherwise.</returns>
        bool IsSignedIn { get; }


        /// <summary>
        /// Gets the user identifier. This identifier is handed out by the virtex server.
        /// </summary>
        /// <value>The user identifier.</value>
        string UserID { get; }


        /// <summary>
        /// This Initialises the main info
        /// </summary>
        void Initialise();

        /// <summary>
        /// This initialises the player info like icons. This is called later on as the graphics device must be active.
        /// </summary>
        void InitialisePlayerInfo();
        
        /// <summary>
        /// Shutdown and close any connections
        /// </summary>
        void Dispose();

        /// <summary>
        /// Gets the Player Profile Backend, whether it's Steam, Google Play Services, etc...
        /// </summary>
        PlayerProfileBackend ProfileBackend { get; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        string DisplayName { get; }


        /// <summary>
        /// Gets the profile picture.
        /// </summary>
        /// <value>The profile picture.</value>
        Texture2D Avatar { get; }

        /// <summary>
        /// Signs the user in.
        /// </summary>
        /// <returns><c>true</c>, if in was signed, <c>false</c> otherwise.</returns>
        void SignIn();


        /// <summary>
        /// Signs the user out.
        /// </summary>
        /// <returns><c>true</c>, if out was signed, <c>false</c> otherwise.</returns>
        void SignOut();


        Dictionary<object, vxAchievement> Achievements { get; }

        /// <summary>
        /// This method will scrub all names and profile text info for any characters which aren't
        /// available in the spritefonts which have been loaded.
        /// </summary>
        //void ScrubNames();
        string PreferredLanguage { get; }

        /// <summary>
        /// Adds the achievement.
        /// </summary>
        void AddAchievement(object key, vxAchievement achievement);

        /// <summary>
        /// Gets all of the currently installed mods. Note this does not mean
        /// they are enabled.
        /// </summary>
        /// <returns></returns>
        string[] GetInstalledMods();

        /// <summary>
        /// Gets the achievement.
        /// </summary>
        /// <returns>The achievement.</returns>
        /// <param name="key">Key.</param>
        vxAchievement GetAchievement(object key);


        /// <summary>
        /// Unlocks the achievement.
        /// </summary>
        /// <param name="key">Key.</param>
        void UnlockAchievement(object key);

        void SearchWorkshop(vxWorkshopSearchCriteria searchCrteria);


        event EventHandler<vxWorkshopSeachReceievedEventArgs> SearchResultReceived; 

        event EventHandler<vxWorkshopItemPublishedEventArgs> ItemPublished; 

        void Publish(string title, string description, string imgPath, string folderPath, string[] tags,
                           ulong idToUpdate = 0, string changelog = "");
        
        bool IsPublishing { get; }

        float PublishProgress { get; }

        void Update();


        void IncrementAchievement(object key, int increment);

        void ViewAchievments();


        void SubmitLeaderboardScore(string id, long score);

        void ViewLeaderboard(string id);

        void ViewAllLeaderboards();

        void ShareImage(string path, string extratxt = "");

        void OpenURL(string url);

        void OpenStorePage(string url);
    }
}
