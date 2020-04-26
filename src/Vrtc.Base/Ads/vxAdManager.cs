using System;
using Microsoft.Xna.Framework;
using VerticesEngine;
using Microsoft.Xna.Framework.GamerServices;
using VerticesEngine.UI.Controls;

#if __IOS__
using CoreGraphics;
using UIKit;
using Google.MobileAds;
#endif


#if __ANDROID__
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;
#endif

namespace VerticesEngine.Mobile.Ads
{
    /// <summary>
    /// The engine ad manager for mobile releases.
    /// </summary>
    public static class vxAdManager
    {
        /// <summary>
        /// The ad unit identifier.
        /// </summary>
        static string AdUnitID;

#if __ANDROID__

        /// <summary>
        /// The Android Game view.
        /// </summary>
        static View GameView;


        /// <summary>
        /// The ad container.
        /// </summary>
        static LinearLayout AdContainer;

        /// <summary>
        /// The rewarded video ad.
        /// </summary>
        static public IRewardedVideoAd RewardedVideoAd;

        /// <summary>
        /// The reward ad listener.
        /// </summary>
        public static vxRewardAdListener RewardAdListener;

        static InterstitialAd mInterstitialAd;

#elif __IOS__

        /// <summary>
        /// The view controller.
        /// </summary>
        UIViewController ViewController;


        /// <summary>
        /// The location.
        /// </summary>
        CGPoint Location;

        /// <summary>
        /// The ad view banner.
        /// </summary>
        BannerView AdViewBanner;

        /// <summary>
        /// The ad view interstitial.
        /// </summary>
        Interstitial AdViewInterstitial;

        /// <summary>
        /// The is add on screen.
        /// </summary>
        bool isAddOnScreen = false;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:VerticesEngine.Admob.vxAdManager"/> ads are being received.
        /// </summary>
        /// <value><c>true</c> if ads are being received; otherwise, <c>false</c>.</value>
        public bool AdsAreBeingReceived { get; private set; }

#endif

        static vxEngine Engine;

        /// <summary>
        /// Initialise a new AdMob Ad Overlay. This is a Cross platform method which will run on both Android and iOS.
        /// </summary>
        /// <param name="game">The host game to et the service container from</param>
        /// <param name="location">The location to place the add on the screen</param>
        public static void Init(Game Game, Vector2 location)
        {

#if __ANDROID__

            //Interstitial Ads
            mInterstitialAd = new InterstitialAd(Game.Activity);

            // Reward Ads
            MobileAds.Initialize(Game.Activity);
            RewardedVideoAd = MobileAds.GetRewardedVideoAdInstance(Game.Activity);
            RewardAdListener = new vxRewardAdListener();
            RewardedVideoAd.RewardedVideoAdListener = RewardAdListener;


            GameView = (View)Game.Services.GetService(typeof(View));

            // Create the Ad Container
            AdContainer = new LinearLayout(Game.Activity)
            {
                Orientation = Orientation.Horizontal
            };
            AdContainer.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Bottom);
            AdContainer.SetBackgroundColor(Android.Graphics.Color.Transparent); // Need on some devices, not sure why

            //AdContainer.AddView(RewardedVideoAd);

            // A layout to hold the ad container and game view
            var mainLayout = new FrameLayout(Game.Activity);

            mainLayout.AddView(GameView);
            mainLayout.AddView(AdContainer);
            Game.Activity.SetContentView(mainLayout);
            //GameView.Parent.

#elif __IOS__
            ViewController = Game.Services.GetService(typeof(UIViewController)) as UIViewController;
            Location = new CGPoint(location.X, location.Y);
#endif
        }

        public static void Initalise(vxEngine engine)
        {
            Engine = engine;
        }

        /// <summary>
        /// Adds the banner with ad at the default position.
        /// </summary>
        /// <param name="adUnitID">Ad unit identifier.</param>
        public static void InitBanner(string adUnitID)
        {
            Vector2 pos = new Vector2(
                Engine.GraphicsDevice.Viewport.Width / 2 - 320 / 2,
                Engine.GraphicsDevice.Viewport.Height - 50);
            
            InitBanner(adUnitID, pos);
        }
        /// <summary>
        /// Adds the banner.
        /// </summary>
        /// <param name="adUnitID">Ad unit identifier.</param>
        /// <param name="Location">Location.</param>
        public static void InitBanner(string adUnitID, Vector2 Location)
        {

#if __ANDROID__
            Location = Vector2.One;

            // The actual ad
            var bannerAd = new AdView(Game.Activity)
            {
                AdUnitId = adUnitID,
                AdSize = AdSize.Banner,
            };



            bannerAd.LoadAd(new AdRequest.Builder()
#if DEBUG            
                .AddTestDevice("DEADBEEF9A2078B6AC72133BB7E6E177") // Prevents generating real impressions while testing
#endif
                .Build());

            if (Location != Vector2.Zero)
            {
                AdContainer.SetX(Location.X);
                AdContainer.SetY(Location.Y);
            }

            // Give the game methods to show/hide the ad.
            AdContainer.AddView(bannerAd);

#elif __IOS__
            this.Location = new CGPoint(Location.X, Location.Y);
            // Setup your BannerView, review AdSizeCons class for more Ad sizes. 
            AdViewBanner = new BannerView(size: AdSizeCons.Banner, origin: this.Location)
            {
                AdUnitID = adUnitID,
                RootViewController = ViewController
            };


            // Wire AdReceived event to know when the Ad is ready to be displayed
            AdViewBanner.AdReceived += (object sender, EventArgs e) =>
            {
                if (!isAddOnScreen)
                    ViewController.View.AddSubview(AdViewBanner);
                isAddOnScreen = true;
                AdsAreBeingReceived = true;
            };

            AdViewBanner.ReceiveAdFailed += (object sender, BannerViewErrorEventArgs e) =>
            {
                Console.WriteLine(e.Error.DebugDescription);
                //throw new Exception(e.Error.DebugDescription);
                //throw new Exception(e.Error.Description); Might be more helpful??
            };

            Request request = Request.GetDefaultRequest();
            //request.TestDevices = new[] { "GAD_SIMULATOR_ID", "kGADSimulatorID" };

            AdViewBanner.LoadRequest(request);
#endif
        }


        /// <summary>
        /// Adds the initerstialel ad.
        /// </summary>
        /// <param name="adUnitID">Ad unit identifier.</param>
        public static void InitIniterstialelAd(string adUnitID)
        {
            AdUnitID = adUnitID;
#if __ANDROID__
            try
            {
                mInterstitialAd.AdUnitId = adUnitID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            mInterstitialAd.AdListener = new AdListener();
#elif __IOS__

            // Intersitials must be re-instantiated each time
            // the ad's are loaded.
#endif
            LoadIniterstialelAd();
        }

        public static void LoadIniterstialelAd()
        {

#if __ANDROID__
            if (mInterstitialAd.IsLoaded==false)
            {
            var adRequest = new AdRequest.Builder()
#if DEBUG
                                         .AddTestDevice("DEADBEEF9A2078B6AC72133BB7E6E177") // Prevents generating real impressions while testing
#endif
                                         .Build();
                mInterstitialAd.LoadAd(adRequest);
            }
#elif __IOS__

            Request request = Request.GetDefaultRequest();
#if DEBUG
            request.TestDevices = new[] { "GAD_SIMULATOR_ID", "kGADSimulatorID" };
#endif

            if (AdViewInterstitial == null || AdViewInterstitial.HasBeenUsed == true)
            {
                Console.WriteLine("Creating AdViewInterstitial");
                AdViewInterstitial = new Interstitial(this.AdUnitID);
                AdViewInterstitial.AdReceived += (object sender, EventArgs e) =>
                {
                    Console.WriteLine("Add Recived");
                };
                AdViewInterstitial.ScreenDismissed += delegate
                {
                    Console.WriteLine("Dissmised");
                Engine.Pause = false;
                };

                AdViewInterstitial.ReceiveAdFailed += (object sender, InterstitialDidFailToReceiveAdWithErrorEventArgs e) =>
                {
                    Console.WriteLine(e.Error.DebugDescription);
                //throw new Exception(e.Error.DebugDescription);
                //throw new Exception(e.Error.Description); Might be more helpful??
            };
                AdViewInterstitial.LoadRequest(request);
            }

#endif

        }
        

        /// <summary>
        /// Shows the initersial ad if there's one loaded.
        /// </summary>
        public static void ShowInitersialAd()
        {
#if __ANDROID__
            if (mInterstitialAd.IsLoaded)
            {
                mInterstitialAd.Show();
            }
#elif __IOS__
            Console.WriteLine("AdViewInterstitial.IsReady:" + AdViewInterstitial.IsReady);
            try
            {
                if (AdViewInterstitial != null)
                {
                    if (AdViewInterstitial.IsReady)
                    {
                        UIViewController viewController = Engine.Game.Services.GetService(typeof(UIViewController)) as UIViewController;
                        //viewController.PresentViewController(AdViewInterstitial, true, null);
                        AdViewInterstitial.PresentFromRootViewController(viewController);
                        Engine.Pause = true;
                    }
                }
            }
            catch
            {
                // do nothing for now
                vxNotificationManager.Add(new vxNotification(Engine, "Error Showing Ad", Color.Red));
            }
#endif
        }


        public static void LoadRewardVideo(string adUnitID)
        {
#if __ANDROID__
            var adRwdRequest = new AdRequest.Builder()
                                            .AddTestDevice("79A8192EE7BF475ED1B4F3A0B3908305")
                                            .Build();
            RewardedVideoAd.LoadAd(adUnitID, adRwdRequest);
#endif
        }

        public static void ShowRewardVideo()
        {
#if __ANDROID__
            if (RewardedVideoAd.IsLoaded)
            {
                RewardedVideoAd.Show();
            }
#endif
        }

	}
}