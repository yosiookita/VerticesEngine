using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VerticesEngine;


namespace VerticesEngine.Net
{
    public class vxNetPlayerManager
    {
		vxEngine Engine;

        /// <summary>
        /// An entity collection containing all Network Players
        /// </summary>
        public Dictionary<string, vxNetPlayerInfo> Players = new Dictionary<string, vxNetPlayerInfo>();

        public vxNetPlayerManager(vxEngine engine)
        {
            this.Engine = engine;
        }

        public void Add(vxNetPlayerInfo entity)
        {
            Players.Add(entity.ID, entity);
        }

        public bool Contains(vxNetPlayerInfo entity)
        {
            return Players.ContainsKey(entity.ID);
        }
    }
}
