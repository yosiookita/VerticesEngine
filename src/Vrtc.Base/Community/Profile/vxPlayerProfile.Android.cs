#if __ANDROID__
using System;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using Android.Gms.Games.LeaderBoard;
using Android.App;
using Android.Content;
using Android.Views;
using Java.Interop;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
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
    public class vxPlayerProfile : Java.Lang.Object, vxIPlayerProfile, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        vxEngine Engine;

        // The Google API Client
        public static GoogleApiClient client;

        // The Games Activity
        Activity activity;


        bool signedOut = true;


        bool signingin = false;


        bool resolving = false;

        Dictionary<string, List<ILeaderboardScore>> scores = new Dictionary<string, List<ILeaderboardScore>>();

        public string[] GetInstalledMods()
        {
            return new string[] { };
        }


        public void Publish(string title, string description, string imgPath, string folderPath, string[] tags,
                            ulong idToUpdate = 0, string changelog = ""){}

        // Activity Request Screens
        const int REQUEST_LEADERBOARD = 9002;
        const int REQUEST_ALL_LEADERBOARDS = 9003;
        const int REQUEST_ACHIEVEMENTS = 9004;
        const int RC_RESOLVE = 9001;

        /// <summary>
        /// Gets or sets a value indicating whether the user is signed out or not.
        /// </summary>
        /// <value><c>true</c> if signed out; otherwise, <c>false</c>.</value>
        public bool SignedOut
        {
            get { return signedOut; }
            set
            {
                if (signedOut != value)
                {
                    signedOut = value;
                    // Store if we Signed Out so we don't bug the player next time.
                    using (var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private))
                    {
                        using (var e = settings.Edit())
                        {
                            e.PutBoolean("SignedOut", signedOut);
                            e.Commit();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the gravity for the GooglePlay Popups. 
        /// Defaults to Bottom|Center
        /// </summary>
        /// <value>The gravity for popups.</value>
        public GravityFlags GravityForPopups { get; set; }

        /// <summary>
        /// The View on which the Popups should show
        /// </summary>
        /// <value>The view for popups.</value>
        public View ViewForPopups { get; set; }

        /// <summary>
        /// This event is fired when a user successfully signs in
        /// </summary>
        public event EventHandler OnSignedIn;

        /// <summary>
        /// This event is fired when the Sign in fails for any reason
        /// </summary>
        public event EventHandler OnSignInFailed;

        /// <summary>
        /// This event is fired when the user Signs out
        /// </summary>
        public event EventHandler OnSignedOut;
        public event EventHandler<vxWorkshopSeachReceievedEventArgs> SearchResultReceived;
        public event EventHandler<vxWorkshopItemPublishedEventArgs> ItemPublished;

        /// <summary>
        /// List of Achievements. Populated by LoadAchievements
        /// </summary>
        /// <value>The achievements.</value>
        //public List<IAchievement> Achievements
        //{
        //    get { return achievments; }
        //}

        //// Acvhievements
        //List<IAchievement> achievments = new List<IAchievement>();

        //public string SignedInUserName
        //{
        //    get { return GamesClass.GetCurrentAccountName(client); }
        //}



        // Sign In - Out
        // **********************************************************
        public bool IsSignedIn
        {
            get
            {

                if (client != null)
                    return (client.IsConnected);
                else
                    return false;

            }

        }




        // User Info
        // **********************************************************
        public void ShareImage(string path, string extratxt = "")
        {
            try
            {
                var shareIntent = new Intent(Intent.ActionSend);
                //intent.PutExtra(Intent.ExtraSubject, subject);
                shareIntent.PutExtra(Intent.ExtraText, extratxt);

                Java.IO.File photoFile = new Java.IO.File(path);

                var uri = Android.Net.Uri.FromFile(photoFile);
                shareIntent.PutExtra(Intent.ExtraStream, uri);
                shareIntent.SetType("image/*");
                shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission);

                Android.Content.PM.PackageManager pm = Game.Activity.PackageManager;
                var lract = pm.QueryIntentActivities(shareIntent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
                //bool resolved = false;
                //foreach (Android.Content.PM.ResolveInfo ri in lract)
                //{
                //    if (ri.ActivityInfo.Name.Contains("twitter"))
                //    {
                //        tweetIntent.SetClassName(ri.ActivityInfo.PackageName,
                //                ri.ActivityInfo.Name);
                //        resolved = true;
                //        break;
                //    }
                //}

                Game.Activity.StartActivity(Intent.CreateChooser(shareIntent, "Choose one"));
                //Android.App.Application.Context.StartActivity(Intent.CreateChooser(shareIntent, "Choose one"));
                //Android.App.Application.Context.StartActivity(resolved ?
                //tweetIntent :
                //Intent.CreateChooser(tweetIntent, "Choose one"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                vxNotificationManager.Add(new vxNotification(Engine, "Error Sharing Results...", Color.Red));
            }
        }


        /// <summary>
        /// Gets the display name which is the Current Account Name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                if (IsSignedIn)
                    return GamesClass.GetCurrentAccountName(client);
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets the user identifier. This doesn't return anything for Google Serverices.
        /// </summary>
        /// <value>The user identifier.</value>
        public string UserID
        {
            get { return ""; }
        }


        public PlayerProfileBackend ProfileBackend
        {
            get { return PlayerProfileBackend.GooglePlayServices; }
        }



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




        public vxPlayerProfile(vxEngine Engine)
        {
            this.Engine = Engine;
            this.activity = Game.Activity;
            this.GravityForPopups = GravityFlags.Top | GravityFlags.Center;

            if (Engine.Game.Config.HasProfileSupport)
            {
                // Set Gravity and View for Popups
                GravityForPopups = (GravityFlags.Top | GravityFlags.Center);
                ViewForPopups = Engine.Game.Services.GetService<View>();

                // Hook up events
                OnSignedIn += (object sender, EventArgs e) =>
                {
                    Console.WriteLine("SignedIn!");
                //_userName = ProfileHelper.SignedInUserName;
                vxNotificationManager.Add(new vxNotification(Engine, "Signed In as: " + DisplayName, Color.DeepSkyBlue));


                };
                OnSignInFailed += (object sender, EventArgs e) =>
                {
                    Console.WriteLine("Error Signing In!");
                    vxNotificationManager.Add(new vxNotification(Engine, "Error Signing In...", Color.Red));

                };
                OnSignedOut += (object sender, EventArgs e) =>
                {
                    Console.WriteLine("Sign Out");
                    vxNotificationManager.Add(new vxNotification(Engine, "Signed Out", Color.DarkViolet));
                //_userName = "";

            };
            }
        }

        public void Initialise()
        {

            var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private);
            signedOut = settings.GetBoolean("SignedOut", true);

            if (!signedOut)
                CreateClient();
        }


        public void InitialisePlayerInfo()
        {
            //throw new NotImplementedException();
        }

        private void CreateClient()
        {

            // did we log in with a player id already? If so we don't want to ask which account to use
            var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private);
            var id = settings.GetString("playerid", String.Empty);

            var builder = new GoogleApiClient.Builder(activity, this, this);
            builder.AddApi(GamesClass.API);
            builder.AddScope(GamesClass.ScopeGames);
            builder.SetGravityForPopups((int)GravityForPopups);
            if (ViewForPopups != null)
                builder.SetViewForPopups(ViewForPopups);
            if (!string.IsNullOrEmpty(id))
            {
                builder.SetAccountName(id);
            }
            client = builder.Build();
        }

        /// <summary>
        /// Start the GooglePlayClient. This should be called from your Activity Start
        /// </summary>
        public void Start()
        {

            if (SignedOut && !signingin)
                return;

            if (client != null && !client.IsConnected)
            {
                client.Connect();
            }
        }

        /// <summary>
        /// Disconnects from the GooglePlayClient. This should be called from your Activity Stop
        /// </summary>
        public void Stop()
        {

            if (client != null && client.IsConnected)
            {
                client.Disconnect();
            }
        }

        /// <summary>
        /// Reconnect to google play.
        /// </summary>
        public void Reconnect()
        {
            if (client != null)
                client.Reconnect();
        }

        /// <summary>
        /// Sign out of Google Play and make sure we don't try to auto sign in on the next startup
        /// </summary>
        public void SignOut()
        {
            Console.WriteLine("Attempting Signout");
            SignedOut = true;
            if (client != null)
            {
                if (client.IsConnected)
                {
                    GamesClass.SignOut(client);
                    Stop();
                    using (var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private))
                    {
                        using (var e = settings.Edit())
                        {
                            e.PutString("playerid", String.Empty);
                            e.Commit();
                        }
                    }
                    Console.WriteLine("Signed out");
                    client.Dispose();
                    client = null;
                    if (OnSignedOut != null)
                        OnSignedOut(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Attempt to Sign in to Google Play
        /// </summary>
        public void SignIn()
        {
            
            signingin = true;
            if (client == null)
                CreateClient();

            if (client.IsConnected)
                return;

            if (client.IsConnecting)
                return;

            //checkPlayServices();

            var result = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(activity);
            if (result != ConnectionResult.Success)
            {
                return;
            }

            Start();

        }

        private bool checkPlayServices()
        {
            GoogleApiAvailability googleAPI = GoogleApiAvailability.Instance;
            int result = googleAPI.IsGooglePlayServicesAvailable(activity);
            if (result != ConnectionResult.Success)
            {
                if (googleAPI.IsUserResolvableError(result))
                {
                    googleAPI.GetErrorDialog(activity, result, 1008).Show();
                }

                return false;
            }

            return true;
        }




        // Achievements
        // **********************************************************



        public Dictionary<object, vxAchievement> Achievements
        {
            get
            {
                return _achievements;
            }
        }

        Dictionary<object, vxAchievement> _achievements = new Dictionary<object, vxAchievement>();




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

            if (client != null)
                if (client.IsConnected)
            {
                
                    GamesClass.Achievements.Increment(client, _achievements[key].ID, increment);
                _achievements[key].Achieved = true;
            }
        }

        public void UnlockAchievement(object key)
        {

            if (client != null)
                if (client.IsConnected)
            {
                
                    GamesClass.Achievements.Unlock(client, _achievements[key].ID);
                _achievements[key].Achieved = true;
                vxNotificationManager.Add(new vxNotification(Engine, "Achievement Unlocked! : " + key, Color.DeepPink));
            }
        }

        public void ViewAchievments()
        {

            if (client != null)
            {
                if (client.IsConnected)
                {
                    var intent = GamesClass.Achievements.GetAchievementsIntent(client);
                    activity.StartActivityForResult(intent, REQUEST_ACHIEVEMENTS);
                }
            }
        }



        // Leaderboards
        // **********************************************************
        public void SubmitLeaderboardScore(string id, long score)
        {
            if (client != null)
                if (client.IsConnected)
                    GamesClass.Leaderboards.SubmitScore(client, id, score);
            
            vxNotificationManager.Add(new vxNotification(Engine, "Score Submitted: " + score, Color.DeepPink));
        }

        public void ViewLeaderboard(string id)
        {
            if (client != null)
                if (client.IsConnected)
            {
                var intent = GamesClass.Leaderboards.GetLeaderboardIntent(client, id);
                activity.StartActivityForResult(intent, REQUEST_LEADERBOARD);
            }
        }

        public void ViewAllLeaderboards()
        {
            if (client != null)
                if (client.IsConnected)
            {
                var intent = GamesClass.Leaderboards.GetAllLeaderboardsIntent(client);
                activity.StartActivityForResult(intent, REQUEST_ALL_LEADERBOARDS);
            }
        }



        /// <summary>
        /// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
        /// This is not immediate but will occur at the next sync of the google play client.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        /// <param name="value">The value of the score</param>
        //public void SubmitScore(string leaderboardCode, long value)
        //{
        //    if(client.IsConnected)
        //        GamesClass.Leaderboards.SubmitScore(client, leaderboardCode, value);
        //}

        ///// <summary>
        ///// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
        ///// This is not immediate but will occur at the next sync of the google play client.
        ///// </summary>
        ///// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        ///// <param name="value">The value of the score</param>
        ///// <param name="value">Additional MetaData to attach. Must be a URI safe string with a max length of 64 characters</param>
        //public void SubmitScore(string leaderboardCode, long value, string metadata)
        //{
        //    if (client.IsConnected)
        //        GamesClass.Leaderboards.SubmitScore(client, leaderboardCode, value, metadata);

        //}

        ///// <summary>
        ///// Show the built in leaderboard activity for the leaderboard code.
        ///// </summary>
        ///// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        //public void ShowLeaderBoardIntentForLeaderboard(string leaderboardCode)
        //{
        //    var intent = GamesClass.Leaderboards.GetLeaderboardIntent(client, leaderboardCode);
        //    activity.StartActivityForResult(intent, REQUEST_LEADERBOARD);
        //}

        ///// <summary>
        ///// Show the built in leaderboard activity for all the leaderboards setup for your application
        ///// </summary>
        //public void ShowAllLeaderBoardsIntent()
        //{
        //    var intent = GamesClass.Leaderboards.GetAllLeaderboardsIntent(client);
        //    activity.StartActivityForResult(intent, REQUEST_ALL_LEADERBOARDS);
        //}



        /// <summary>
        /// Load the Achievments. This populates the Achievements property
        /// </summary>
        public async Task LoadAchievements()
        {
            var ar = await GamesClass.Achievements.LoadAsync(client, false);
            if (ar != null)
            {
                //IAchievements achievements = ar.Achievements;
                //achievements.
                //achievments.Clear();
                //achievments.AddRange(ar.Achievements);
                //ar.Achievements.
                //ar.Achievements.
            }
        }


        public async Task LoadTopScores(string leaderboardCode)
        {
            var ar = await GamesClass.Leaderboards.LoadTopScoresAsync(client, leaderboardCode, 2, 0, 25);
            if (ar != null)
            {
                var id = ar.Leaderboard.LeaderboardId;
                if (!scores.ContainsKey(id))
                {
                    scores.Add(id, new List<ILeaderboardScore>());
                }
                scores[id].Clear();
                scores[id].AddRange(ar.Scores);
            }
        }

#region IGoogleApiClientConnectionCallbacks implementation

        public void OnConnected(Bundle connectionHint)
        {

            resolving = false;
            SignedOut = false;
            signingin = false;

            using (var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private))
            {
                using (var e = settings.Edit())
                {
                    e.PutString("playerid", GamesClass.GetCurrentAccountName(client));
                    e.Commit();
                }
            }

            if (OnSignedIn != null)
                OnSignedIn(this, EventArgs.Empty);
        }

        public void OnConnectionSuspended(int resultCode)
        {
            resolving = false;
            SignedOut = false;
            signingin = false;
            client.Disconnect();
            if (OnSignInFailed != null)
                OnSignInFailed(this, EventArgs.Empty);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            if (resolving)
                return;

            if (result.HasResolution)
            {
                resolving = true;
                result.StartResolutionForResult(activity, RC_RESOLVE);
                return;
            }

            resolving = false;
            SignedOut = false;
            signingin = false;
            if (OnSignInFailed != null)
                OnSignInFailed(this, EventArgs.Empty);
        }
#endregion

        /// <summary>
        /// Processes the Activity Results from the Signin process. MUST be called from your activity OnActivityResult override.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {

            if (requestCode == RC_RESOLVE)
            {
                if (resultCode == Result.Ok)
                {
                    Start();
                }
                else
                {
                    if (OnSignInFailed != null)
                        OnSignInFailed(this, EventArgs.Empty);
                }
            }
        }



        public bool IsPublishing { get { return false; }}

        public float PublishProgress { get { return 0; }}


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
                Intent webIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                webIntent.SetFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(webIntent);
        }

        public void OpenStorePage(string appid)
        {
            //        try {

            //            var uri = Android.Net.Uri.Parse("market://details?id=" + appid);
            //            var intent = new Android.Content.Intent(Android.Content.Intent.ActionView, uri);
            //            //Android.App.Application.Context.StartActivity(new Intent(Intent.ActionView, uri));
            //intent.SetFlags(Android.Content.ActivityFlags.NewTask);
            //Android.App.Application.Context.StartActivity(intent);

            //        } catch (Android.Content.ActivityNotFoundException anfe) {
            //            var uri = Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=" + appid);
            //var intent = new Android.Content.Intent(Android.Content.Intent.ActionView, uri);
            ////Android.App.Application.Context.StartActivity(new Intent(Intent.ActionView, uri));
            //intent.SetFlags(Android.Content.ActivityFlags.NewTask);
            //Android.App.Application.Context.StartActivity(intent);
            //}
            // you can also use BuildConfig.APPLICATION_ID

            Intent rateIntent = new Intent(Intent.ActionView,
                Android.Net.Uri.Parse("market://details?id=" + appid));
            bool marketFound = false;

            // find all applications able to handle our rateIntent
            var otherApps = Android.App.Application.Context.PackageManager.QueryIntentActivities(rateIntent, 0);
            foreach (Android.Content.PM.ResolveInfo otherApp in otherApps)
            {
                // look for Google Play application
                if (otherApp.ActivityInfo.ApplicationInfo.PackageName == "com.android.vending")
                {

                    Android.Content.PM.ActivityInfo otherAppActivity = otherApp.ActivityInfo;
                    ComponentName componentName = new ComponentName(
                        otherAppActivity.ApplicationInfo.PackageName,
                        otherAppActivity.Name
                            );
                    // make sure it does NOT open in the stack of your activity
                    rateIntent.AddFlags(ActivityFlags.NewTask);
                    // task reparenting if needed
                    rateIntent.AddFlags(ActivityFlags.ResetTaskIfNeeded);
                    // if the Google Play was already open in a search result
                    //  this make sure it still go to the app page you requested
                    rateIntent.AddFlags(ActivityFlags.ClearTop);
                    // this make sure only the Google Play app is allowed to
                    // intercept the intent
                    rateIntent.SetComponent(componentName);
                    Android.App.Application.Context.StartActivity(rateIntent);
                    marketFound = true;
                    break;

                }
            }

            // if GP not present on device, open web browser
            if (!marketFound)
            {
                Intent webIntent = new Intent(Intent.ActionView,
                Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=" + appid));
                webIntent.SetFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(webIntent);
            }
        }
    }
}
#endif