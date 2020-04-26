
//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using VerticesEngine;
//using VerticesEngine.UI.Controls;


//#if __ANDROID__

//using Android.App;
//using Android.Content;
//using Android.Views;
////using MGTemplate.Droid;


//#elif __IOS__
//using Foundation;
//using GameKit;
//using UIKit;
//#endif

//namespace VerticesEngine.Profile
//{
//    /// <summary>
//    /// This class holds all information for a user profile. The sign in method depends on
//    /// release (i.e. Android would be google signin, PC/Desktop would be Steam etc...).
//    /// </summary>
//    public class vxUserProfile
//    {
//        /// <summary>
//        /// Gets a value indicating whether this <see cref="T:VerticesEngine.Profile.vxUserProfile"/> is signed in.
//        /// </summary>
//        /// <value><c>true</c> if is signed in; otherwise, <c>false</c>.</value>
//		public bool IsSignedIn
//        {
//#if __IOS__
//            get { return GKLocalPlayer.LocalPlayer.Authenticated; }
//#else
//            get { return _isSignedIn; }
//#endif
//        }
//        bool _isSignedIn = false;


//        /// <summary>
//        /// Gets the user identifier. This identifier is handed out by the virtex server.
//        /// </summary>
//        /// <value>The user identifier.</value>
//        public string UserID
//        {
//#if __IOS__
//            get { return GKLocalPlayer.LocalPlayer.PlayerID; }
//#else
//            get { return _userID; }
//#endif
//        }
//        string _userID = "xxxxxxxxxxxxxxxxxxxxxxxxx";


//        /// <summary>
//        /// Gets the name of the user.
//        /// </summary>
//        /// <value>The name of the user.</value>
//        public string UserName
//        {
//            get { return _userName; }
//        }
//        string _userName = "user_name";


//        /// <summary>
//        /// Gets the profile picture.
//        /// </summary>
//        /// <value>The profile picture.</value>
//        public Texture2D ProfilePicture
//        {
//            get { return _profilePicture; }
//        }
//        Texture2D _profilePicture;


//        public string ProfilePicturePath
//        {
//            get { return _profilePicturePath; }
//        }
//        string _profilePicturePath = "";


//        public Dictionary<object, vxAchievement> Achievements = new Dictionary<object, vxAchievement>();


//        //public vxProfileHelper ProfileHelper;

//        vxEngine Engine;

//#if __IOS__

//        /// <summary>
//        /// Gets the view controller.
//        /// </summary>
//        /// <value>The view controller.</value>
//        UIViewController ViewController
//        {
//            get
//            {
//                return Engine.Game.Services.GetService(typeof(UIViewController)) as UIViewController;
//            }
//        }
//#endif

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:VerticesEngine.Profile.vxUserProfile"/> class.
//        /// </summary>
//        public vxUserProfile(vxEngine Engine)
//        {
//            this.Engine = Engine;
//            // Setup Google Play Services Helpe
//            //ProfileHelper = new vxProfileHelper(Engine);

//#if __ANDROID__
//            // Set Gravity and View for Popups
//            //ProfileHelper.GravityForPopups = (GravityFlags.Top | GravityFlags.Center);
//            //ProfileHelper.ViewForPopups = Engine.Game.Services.GetService<View>();

//            //// Hook up events
//            //ProfileHelper.OnSignedIn += (object sender, EventArgs e) =>
//            //{
//            //    Console.WriteLine("SignedIn!");
//            //    _userName = ProfileHelper.SignedInUserName;
//            //    vxNotificationManager.Add(new vxNotification(Engine, "Signed In as: " + _userName, Color.DeepSkyBlue));
//            //    _isSignedIn = true;

//            //};
//            //ProfileHelper.OnSignInFailed += (object sender, EventArgs e) =>
//            //{
//            //    Console.WriteLine("Error Signing In!");
//            //    vxNotificationManager.Add(new vxNotification(Engine, "Error Signing In...", Color.Red));

//            //};
//            //ProfileHelper.OnSignedOut += (object sender, EventArgs e) =>
//            //{
//            //    Console.WriteLine("Sign Out");
//            //    vxNotificationManager.Add(new vxNotification(Engine, "Signed Out", Color.DarkViolet));
//            //    _userName = "signed out";
//            //    _isSignedIn = false;
//            //};


//            //ProfileHelper.Initialize();
//#endif
//        }

//        /// <summary>
//        /// Signs in the user returning the sign in result as a boolean. 
//        /// </summary>
//        /// <returns>Whether the sign in attempt was successful or not.</returns>
//        public void SignIn()
//        {
//            if (Engine.Game.Config.HasProfileSupport)
//            {
//#if __ANDROID__
//                //ProfileHelper.SignIn();
//#elif __IOS__
//            GKLocalPlayer.LocalPlayer.AuthenticateHandler = (ui, error) => {

//                // If ui is null, that means the user is already authenticated,
//                // for example, if the user used Game Center directly to log in

//                if (ui != null)
//                    Console.WriteLine(ui);
//                else
//                {
//                    // Check if you are authenticated:
//                    var authenticated = GKLocalPlayer.LocalPlayer.Authenticated;

//                    if (authenticated == true)
//                    {

//                        //AppDelegate.Shared.ViewController.PresentViewController(controller, true, null);
//                    }
//                }
//                Console.WriteLine("Authentication result: {0}", error);
//            };

//#endif
//            }
//        }

//        public void SignOut()
//        {
//#if __ANDROID__
//            //ProfileHelper.SignOut();
//#elif __IOS__
//            // Not Available in iOS. Must be done in system.
//#endif
//        }



//        /// <summary>
//        /// Unlocks the achievement by using the object which corresponds to the specified achivement
//        /// in the GameAchievements Dictionary.
//        /// </summary>
//        /// <param name="key">Identifier key for the GameAchievments Dictionary.</param>
//        public void UnlockAchievement(object key)
//        {
//            Achievements[key].Achieved = true;
//            if (IsSignedIn)
//            {
//#if __ANDROID__

//                //ProfileHelper.UnlockAchievement(Achievements[key].ID);
//                Console.WriteLine("ID: " + Achievements[key].ID);
//#elif __IOS__
//                var achievement = new GKAchievement(Achievements[key].ID, GKLocalPlayer.LocalPlayer.PlayerID)
//                {
//                    //PercentComplete = 100
//                };
//                GKAchievement.ReportAchievements(new[] { achievement },
//                                                 delegate (NSError error)
//                                                {
//                                                    if(error != null)
//                        Console.WriteLine(error.ToString());
//                                                    else
//                                                        Console.WriteLine("ERROR IS NULL");
//                                                });
//                //GKAchievement.ReportAchievementsAsync(new[] { achievement }, null);
//                //vxNotificationManager.Add(new vxNotification(Engine, "Achievement Unlocked! : " + key, Color.DeepPink));

//#else
//            vxNotificationManager.Add(new vxNotification(Engine, "Achievement Unlocked! : " + key, Color.DeepPink));
//#endif
//            }
//        }


//        /// <summary>
//        /// Unlocks the achievement by using the object which corresponds to the specified achivement
//        /// in the GameAchievements Dictionary.
//        /// </summary>
//        /// <param name="key">Identifier key for the GameAchievments Dictionary.</param>
//        public void IncrementAchievement(object key, int increment)
//        {
//#if __ANDROID__
//            //Achievements[key].Achieved = true;
//            //if (IsSignedIn)
//                //ProfileHelper.IncrementAchievement(Achievements[key].ID, increment);
//            //Console.WriteLine("ID: " + Achievements[key].ID);
//#else
//            vxNotificationManager.Add(new vxNotification(Engine, "Achievement Unlocked! : " + key, Color.DeepPink));
//#endif
//        }

//        public void ViewAchievments()
//        {
//#if __ANDROID__
//            //if (IsSignedIn)
//                //ProfileHelper.ShowAchievements();
//#elif __IOS__
//            //UIViewController ViewController = Engine.Game.Services.GetService(typeof(UIViewController)) as UIViewController;

//            GKGameCenterViewController controller = new GKGameCenterViewController();
//            controller.Finished += (object sender, EventArgs e) => {
//                controller.DismissViewController(true, null);
//            };
//            controller.ViewState = GKGameCenterViewControllerState.Achievements;
//            ViewController.PresentViewController(controller, true, null);
//#endif
//        }

//        public void SubmitLeaderboardScore(string id, long score)
//        {
//#if __ANDROID__
//            //if (IsSignedIn)
//                //ProfileHelper.SubmitScore(id, score);
//#elif __IOS__
//            GKScore newScore = new GKScore(id);
//            newScore.Value = score;
//            GKScore.ReportScores(new[] { newScore }, 
//                                                 delegate (NSError error)
//                                                 {
//                                                     if (error != null)
//                                                         Console.WriteLine(error.ToString());
//                                                     else
//                                                         Console.WriteLine("ERROR IS NULL");
//                                                 });
//#else
//            vxNotificationManager.Add(new vxNotification(Engine, "Score Submitted: " + score, Color.DeepPink));
//#endif
//        }

//        public void ViewLeaderboard(string id)
//        {
//#if __ANDROID__
//            if (IsSignedIn)
//            {
//                //if (id == "")
//                  //  ProfileHelper.ShowAllLeaderBoardsIntent();
//                //else
//                  //  ProfileHelper.ShowLeaderBoardIntentForLeaderboard(id);
//            }
//#elif __IOS__
//            //UIViewController ViewController = Engine.Game.Services.GetService(typeof(UIViewController)) as UIViewController;

//            GKGameCenterViewController controller = new GKGameCenterViewController();
//            controller.Finished += (object sender, EventArgs e) => {
//                controller.DismissViewController(true, null);
//            };
//            controller.ViewState = GKGameCenterViewControllerState.Leaderboards;
//            ViewController.PresentViewController(controller, true, null);
//#endif
//        }



//        /// <summary>
//        /// Shares an image using the preferred method in Android or iOS. 
//        /// </summary>
//        /// <param name="path">Path.</param>
//        public void ShareImage(string path, string extratxt = "")
//        {
//#if __ANDROID__
//            try
//            {
//                var shareIntent = new Intent(Intent.ActionSend);
//                //intent.PutExtra(Intent.ExtraSubject, subject);
//                shareIntent.PutExtra(Intent.ExtraText, extratxt);

//                Java.IO.File photoFile = new Java.IO.File(path);

//                var uri = Android.Net.Uri.FromFile(photoFile);
//                shareIntent.PutExtra(Intent.ExtraStream, uri);
//                shareIntent.SetType("image/*");
//                shareIntent.AddFlags(ActivityFlags.GrantReadUriPermission);

//                Android.Content.PM.PackageManager pm = Game.Activity.PackageManager;
//                var lract = pm.QueryIntentActivities(shareIntent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
//                //bool resolved = false;
//                //foreach (Android.Content.PM.ResolveInfo ri in lract)
//                //{
//                //    if (ri.ActivityInfo.Name.Contains("twitter"))
//                //    {
//                //        tweetIntent.SetClassName(ri.ActivityInfo.PackageName,
//                //                ri.ActivityInfo.Name);
//                //        resolved = true;
//                //        break;
//                //    }
//                //}

//                Android.App.Application.Context.StartActivity(Intent.CreateChooser(shareIntent, "Choose one"));
//                //Android.App.Application.Context.StartActivity(resolved ?
//                //tweetIntent :
//                //Intent.CreateChooser(tweetIntent, "Choose one"));
//            }
//            catch
//            {
//                vxNotificationManager.Add(new vxNotification(Engine, "Error Sharing Results...", Color.Red));
//            }
//#elif __IOS__
//                try
//                {
//                    //UIViewController ViewController = Engine.Game.Services.GetService(typeof(UIViewController)) as UIViewController;

//                    UIImage image = new UIImage(path);
//                    NSObject[] activityItems = { image };

//                    UIActivityViewController activityViewController = new UIActivityViewController(activityItems, null);
//                    activityViewController.ExcludedActivityTypes = new NSString[] { };

//                    if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
//                    {
//                        activityViewController.PopoverPresentationController.SourceView = ViewController.View;
//                        activityViewController.PopoverPresentationController.SourceRect = new CoreGraphics.CGRect((ViewController.View.Bounds.Width / 2), (ViewController.View.Bounds.Height / 4), 0, 0);
//                    }

//                    ViewController.PresentViewController(activityViewController, true, null);
//                }
//                catch
//                {
//                    vxNotificationManager.Add(new vxNotification(Engine, "Error Sharing Results...", Color.Red));
//                }
//#else
//            vxNotificationManager.Add(new vxNotification(Engine, "Sharing Not Available On This Platform.", Color.DeepPink));
//#endif
//        }
//    }
//}
