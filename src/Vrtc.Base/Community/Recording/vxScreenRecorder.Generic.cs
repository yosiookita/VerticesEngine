#if !__ANDROID0__ 
using System;
using Microsoft.Xna.Framework;
using VerticesEngine;
using VerticesEngine.Profile;

namespace VerticesEngine.Media
{
    public class vxScreenRecorder : vxGameObject, vxIScreenRecorder
    {

        public bool IsSupported
        {
            get
            {

                return false;
            }
        }

        public vxScreenRecorderState RecordingState
        {
            get { return vxScreenRecorderState.Dismissed; }
        }

        public vxScreenRecorder() : base()
        {

        }



        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}

#endif