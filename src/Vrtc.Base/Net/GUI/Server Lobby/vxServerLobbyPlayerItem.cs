
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Events;
using VerticesEngine.Net;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// Sets up a Serve List Dialog Item which holds information pertaining too a Discovered Server.
    /// </summary>
    public class vxServerLobbyPlayerItem : vxScrollPanelItem
    {
        /// <summary>
        /// Get's the Player's Info
        /// </summary>
        public vxNetPlayerInfo Player
        {
            get
            {
                if (vxNetworkManager.Client.PlayerManager.Players.ContainsKey(id))
                    return vxNetworkManager.Client.PlayerManager.Players[id];
                else
                    return null;
            }
        }

        string id = "";

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxServerLobbyPlayerItem"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		/// <param name="player">Player.</param>
		/// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
		/// <param name="buttonImage">Button image.</param>
		/// <param name="ElementIndex">Element index.</param>
        public vxServerLobbyPlayerItem(vxEngine Engine, vxNetPlayerInfo player, Vector2 Position, Texture2D buttonImage)
            : base(Engine, player.UserName, Position, buttonImage)
        {
            id = player.ID;
        }

		public override void Draw()
        {
            base.Draw();
            string status = "null";

                    if(Player != null)
                        status = (Player.Status == vxEnumNetPlayerStatus.InServerLobbyReady) ? "Ready" : "Not Ready";
            

                    Engine.SpriteBatch.DrawString(vxInternalAssets.Fonts.DebugFont, "Status: " + status,
                new Vector2((int)(Position.X + Height + Padding.X * 2), (int)(Position.Y + vxGUITheme.Fonts.Font.MeasureString(Text).Y + 10)),
    Theme.Text.Color);

        }
    }
}
