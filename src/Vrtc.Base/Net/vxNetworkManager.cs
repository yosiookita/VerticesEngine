using System;
using System.Collections.Generic;
using System.Text;

namespace VerticesEngine.Net
{
    public static class vxNetworkManager
    {
        /// <summary>
        /// The network client which handles all messaging between the server
        /// </summary>
        public static vxNetworkClient Client;


        /// <summary>
        /// The network server for if the player is hosting a game
        /// </summary>
        public static vxNetworkServer Server;


        /// <summary>
        /// What is this players Network Role
        /// </summary>
        public static vxEnumNetworkPlayerRole PlayerNetworkRole
        {
            get { return Client.PlayerNetworkRole; }
        }


        /// <summary>
        /// Initialises the Network Manager
        /// </summary>
        /// <param name="Engine"></param>
        public static void Init(vxEngine Engine)
        {
            Client = new vxNetworkClient(Engine);

            Server = new vxNetworkServer(Engine, Client.DefaultServerPort);
        }

        public static void Dispose()
        {
            Client.Dispose();

            Server.Dispose();
        }
    }
}
