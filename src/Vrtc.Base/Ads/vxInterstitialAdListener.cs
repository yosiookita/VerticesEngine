#if __ANDROID__
using System;
using Android.Gms.Ads.Reward;
using VerticesEngine;
using VerticesEngine.UI.Controls;

namespace VerticesEngine.Mobile.Ads
{
    public class vxInterstitialAdListener : Android.Gms.Ads.AdListener
    {
        vxEngine Engine;

        public vxInterstitialAdListener(vxEngine eng)
        {
            Engine = eng;
        }


        public override void OnAdClosed()
        {
            
        }
    }
}
#endif