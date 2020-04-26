using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerticesEngine.Graphics
{
    public interface vxIRenderPass
    {
        void OnGraphicsRefresh();

        void LoadContent();

        void RegisterRenderTargetsForDebug();

        void Prepare();

        void Apply();

        void Dispose();
    }
}
