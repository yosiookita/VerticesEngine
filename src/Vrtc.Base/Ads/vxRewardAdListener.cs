#if __ANDROID__
using System;
using Android.Gms.Ads.Reward;
using VerticesEngine;
using VerticesEngine.UI.Controls;

namespace VerticesEngine.Mobile.Ads
{

    public class vxRewardAdListenerEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public vxRewardAdListenerEventArgs()
        {
            
        }
    }

    public class vxRewardAdListener : Java.Lang.Object, IRewardedVideoAdListener
    {
        public event EventHandler<vxRewardAdListenerEventArgs> Rewarded;
        public event EventHandler<vxRewardAdListenerEventArgs> AdClosed;

        //vxEngine Engine;
        public vxRewardAdListener()
        {
            //this.Engine = Engine;
        }


        public void OnRewarded(IRewardItem reward)
        {
            // Raise the Clicked event.
            if (Rewarded != null)
                Rewarded(this, new vxRewardAdListenerEventArgs());
        }

        public void OnRewardedVideoAdClosed()
        {
            Console.WriteLine("OnRewardedVideoAdClosed");

            // Raise the Clicked event.
            if (AdClosed != null)
                AdClosed(this, new vxRewardAdListenerEventArgs());
        }

        public void OnRewardedVideoAdFailedToLoad(int errorCode)
        {
            Console.WriteLine("OnRewardedVideoAdFailedToLoad");
            //vxNotificationManager.Add(new vxNotification(Engine, "Error Loading Ad: "+errorCode, Microsoft.Xna.Framework.Color.Red));
        }

        public void OnRewardedVideoAdLeftApplication()
        {
            Console.WriteLine("OnRewardedVideoAdLeftApplication");
        }

        public void OnRewardedVideoAdLoaded()
        {
            Console.WriteLine("OnRewardedVideoAdLoaded");
        }

        public void OnRewardedVideoAdOpened()
        {
            Console.WriteLine("OnRewardedVideoAdOpened");
        }

        public void OnRewardedVideoStarted()
        {
            Console.WriteLine("OnRewardedVideoStarted");
        }
    }
}
#endif