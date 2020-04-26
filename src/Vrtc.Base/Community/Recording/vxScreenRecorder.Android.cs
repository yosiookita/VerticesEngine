#if __ANDROID0__
using System;
using Android.Gms.Games;
using Android.Gms.Games.Video;

using VerticesEngine;
using Microsoft.Xna.Framework;
using VerticesEngine.Profile;

namespace VerticesEngine.Media
{
    public class vxScreenRecorder : vxBase, vxIScreenRecorder
    {
        public static int RC_VIDEO_OVERLAY = 9011;

        vxVideoListener videoListener;



        public bool IsSupported
        {
            get
            {
                if (vxPlayerProfile.client != null && Engine.PlayerProfile.IsSignedIn)
                    return GamesClass.Videos.IsCaptureSupported(vxPlayerProfile.client);
                else
                    return false;
            }
        }

        public vxScreenRecorderState RecordingState
        {
            get { return videoListener.RecorderState; }
        }

        //vxEngine Engine;

        public vxScreenRecorder(vxEngine engine) : base(engine)
        {
            //this.Engine = engine;
            videoListener = new vxVideoListener();
        }



        public void Start()
        {
            if (vxPlayerProfile.client != null && Engine.PlayerProfile.IsSignedIn)
            {
                videoListener.RecorderState = vxScreenRecorderState.Initialised;

                var intent = GamesClass.Videos.GetCaptureOverlayIntent(vxPlayerProfile.client);

                //GamesClass.Videos.
                GamesClass.Videos.RegisterCaptureOverlayStateChangedListener(vxPlayerProfile.client, videoListener);
                Microsoft.Xna.Framework.Game.Activity.StartActivityForResult(intent, RC_VIDEO_OVERLAY);

                //intent.SetFlags(Android.Content.ActivityFlags.NewTask);
                //Android.App.Application.Context.StartActivity(intent);


                //var result = GamesClass.Videos.GetCaptureCapabilities(vxPlayerProfile.client);
                //var avail = GamesClass.Videos.IsCaptureAvailable(vxAndroidPlayerProfile.client, 0);
            }
        }

        public void Stop()
        {
            //throw new NotImplementedException();

        }
    }


    public class vxVideoListener : Java.Lang.Object, IVideosCaptureOverlayStateListener
    {
        public vxScreenRecorderState RecorderState;

        public vxVideoListener()
        {
            RecorderState = vxScreenRecorderState.Dismissed;
        }

        public void OnCaptureOverlayStateChanged(int overlayState)
        {
            Console.WriteLine("STATE: "+overlayState);
            switch(overlayState)
            {
                case 1:
                    RecorderState = vxScreenRecorderState.Captureing;
                    break;
                case 3:
                    RecorderState = vxScreenRecorderState.Stopped;
                    break;
            }
        }
    }

}

#endif