#if __IOS__
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerticesEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Foundation;
using GameKit;
using UIKit;
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
        UIViewController ViewController
        {
            get
            {
                return Engine.Game.Services.GetService(typeof(UIViewController)) as UIViewController;
            }
        }

        vxEngine Engine;



        public bool IsSignedIn
        {
            get { return GKLocalPlayer.LocalPlayer.Authenticated; }
        }


        public string DisplayName
        {
            get { return ""; }
        }



        public void SignIn()
        {
            GKLocalPlayer.LocalPlayer.AuthenticateHandler = (ui, error) =>
            {

                // If ui is null, that means the user is already authenticated,
                // for example, if the user used Game Center directly to log in

                if (ui != null)
                    Console.WriteLine(ui);
                else
                {
                    // Check if you are authenticated:
                    var authenticated = GKLocalPlayer.LocalPlayer.Authenticated;

                    if (authenticated == true)
                    {

                        //AppDelegate.Shared.ViewController.PresentViewController(controller, true, null);
                    }
                }
                Console.WriteLine("Authentication result: {0}", error);
            };
        }

        public void SignOut()
        {

        }


        public vxPlayerProfile(vxEngine Engine)
        {
            this.Engine = Engine;
        }



        // User Info
        // **********************************************************



        public void ShareImage(string path, string extratxt = "")
        {
            try
            {
                //UIViewController ViewController = Engine.Game.Services.GetService(typeof(UIViewController)) as UIViewController;

                UIImage image = new UIImage(path);
                NSObject[] activityItems = { image };

                UIActivityViewController activityViewController = new UIActivityViewController(activityItems, null);
                activityViewController.ExcludedActivityTypes = new NSString[] { };

                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                {
                    activityViewController.PopoverPresentationController.SourceView = ViewController.View;
                    activityViewController.PopoverPresentationController.SourceRect = new CoreGraphics.CGRect((ViewController.View.Bounds.Width / 2), (ViewController.View.Bounds.Height / 4), 0, 0);
                }

                ViewController.PresentViewController(activityViewController, true, null);
            }
            catch
            {
                vxNotificationManager.Add(new vxNotification(Engine, "Error Sharing Results...", Color.Red));
            }
        }




        public string[] GetInstalledMods()
        {
            return new string[] { };
        }


        public void Publish(string title, string description, string imgPath, string folderPath, string[] tags,
                            ulong idToUpdate = 0, string changelog = "")
        { }


        public string UserID
        {
            get { return GKLocalPlayer.LocalPlayer.PlayerID; }
        }


        public PlayerProfileBackend ProfileBackend
        {
            get { return PlayerProfileBackend.iOSGameCenter; }
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
            _achievements[key].Achieved = true;
        }

        public void UnlockAchievement(object key)
        {
            var achievement = new GKAchievement(Achievements[key].ID, GKLocalPlayer.LocalPlayer.PlayerID)
            {
                //PercentComplete = 100
            };
            GKAchievement.ReportAchievements(new[] { achievement },
                                             delegate (NSError error)
                                            {
                                                if (error != null)
                                                    Console.WriteLine(error.ToString());
                                                else
                                                    Console.WriteLine("ERROR IS NULL");
                                            });
            //GKAchievement.ReportAchievementsAsync(new[] { achievement }, null);
            //vxNotificationManager.Add(new vxNotification(Engine, "Achievement Unlocked! : " + key, Color.DeepPink));


            _achievements[key].Achieved = true;
            vxNotificationManager.Add(new vxNotification(Engine, "Achievement Unlocked! : " + key, Color.DeepPink));
        }

        public void ViewAchievments()
        {
            GKGameCenterViewController controller = new GKGameCenterViewController();
            controller.Finished += (object sender, EventArgs e) =>
            {
                controller.DismissViewController(true, null);
            };
            controller.ViewState = GKGameCenterViewControllerState.Achievements;
            ViewController.PresentViewController(controller, true, null);
        }




        // Leaderboards
        // **********************************************************
        public void SubmitLeaderboardScore(string id, long score)
        {
            GKScore newScore = new GKScore(id);
            newScore.Value = score;
            GKScore.ReportScores(new[] { newScore },
                                                 delegate (NSError error)
                                                 {
                                                     if (error != null)
                                                         Console.WriteLine(error.ToString());
                                                     else
                                                         Console.WriteLine("ERROR IS NULL");
                                                 });
        }

        public void ViewLeaderboard(string id)
        {

            GKGameCenterViewController controller = new GKGameCenterViewController();
            controller.Finished += (object sender, EventArgs e) =>
            {
                controller.DismissViewController(true, null);
            };
        }


        public void Initialise()
        {

        }

        public void InitialisePlayerInfo()
        {

        }

        public void Dispose()
        {

        }

        public void ViewAllLeaderboards()
        {

        }


        public bool IsPublishing { get { return false; } }

        public float PublishProgress { get { return 0; } }


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
    }
}
#endif