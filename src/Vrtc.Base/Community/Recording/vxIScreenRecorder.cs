using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace VerticesEngine.Media
{
    public enum vxScreenRecorderState
    {
        Initialised,
        Captureing,
        Stopped,
        Dismissed,
    }

	public interface vxIScreenRecorder
    {
        bool IsSupported { get; }

        vxScreenRecorderState RecordingState { get; } 

        void Start();

        void Stop();
    }
}
