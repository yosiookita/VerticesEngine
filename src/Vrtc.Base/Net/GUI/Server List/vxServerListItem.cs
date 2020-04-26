
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;
using VerticesEngine.Net.Messages;

namespace VerticesEngine.UI.Dialogs
{
    /// <summary>
    /// File Chooser Dialor Item.
    /// </summary>
    public class vxServerListItem : vxScrollPanelItem
    {
        /// <summary>
        /// The name of the Server.
        /// </summary>
        public string ServerName
        {
            get { return serverInfo.ServerName; }
        }

        /// <summary>
        /// The Server Addess
        /// </summary>
        public string ServerAddress
        {
            get { return serverInfo.ServerIP; }
        }

        /// <summary>
        /// The Server Port
        /// </summary>
        public string ServerPort
        {
            get { return serverInfo.ServerPort; }
        }


        //public float Ping
        //{
        //    get { return serverInfo.Ping; }
        //}

        vxNetMsgServerInfo serverInfo;

        /// <summary>
        /// Sets up a Serve List Dialog Item which holds information pertaining too a Discovered Server.
        /// </summary>
        /// <param name="vxEngine"></param>
        /// <param name="ServerName"></param>
        /// <param name="ServerAddress"></param>
        /// <param name="ServerPort"></param>
        /// <param name="Position"></param>
        /// <param name="buttonImage"></param>
        /// <param name="ElementIndex"></param>
        public vxServerListItem(vxEngine Engine, vxNetMsgServerInfo serverInfo, Vector2 Position, Texture2D buttonImage,
            int ElementIndex):base(Engine, serverInfo.ServerName, Position, buttonImage, ElementIndex)
        {
            Padding = new Vector2(4);

            this.serverInfo = serverInfo;

            // Set Text
            Text = ServerName;

            // Set Position
            this.Position = Position;
            OriginalPosition = Position;

            Index = ElementIndex;
            ButtonImage = buttonImage;
            Bounds = new Rectangle(0, 0, 64, 64);
            this.Engine = Engine;

            Width = 3000;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(new Color(0.15f, 0.15f, 0.15f, 0.5f), Color.DarkOrange, Color.DeepPink),
                new vxColourTheme(Color.LightGray, Color.Black, Color.Black));
            
            IsTogglable = true;
            Height = 64;
        }

        public override void Draw()
        {
            //base.Draw();

            Theme.SetState(this);

            //Draw Button Background
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds, Color.Black);
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds.GetBorder(-1), GetStateColour(Theme.Background));

            //Draw Icon
            int iconOffset = 0;
            if (ButtonImage != null)
            {
                Engine.SpriteBatch.Draw(ButtonImage, new Rectangle(
                    (int)(Position.X + Padding.X),
                    (int)(Position.Y + Padding.Y),
                    (int)(Height - Padding.X * 2),
                    (int)(Height - Padding.Y * 2)),
                                        Color.LightGray);

                iconOffset = Height;
            }

            //Draw Text String
            Engine.SpriteBatch.DrawString(vxGUITheme.Fonts.Font, Text,
                                          new Vector2(
                                              (int)(Position.X + iconOffset + Padding.X * 2),
                                              (int)(Position.Y + 8)),
                                          GetStateColour(Theme.Text));



            Font = vxInternalAssets.Fonts.ViewerFont;
            Console.WriteLine(ServerAddress +":" + ServerPort+":"+ ToggleState);
#if DEBUG
            Engine.SpriteBatch.DrawString(Font, "Address: " + ServerAddress + ":"+ServerPort,
                                          new Vector2(
                                              (int)(Position.X + Padding.X * 8),
                                              (int)(Bounds.Bottom - Font.LineSpacing - Padding.Y)),
                                          Theme.Text.Color * 0.5f);
#endif

            string players = string.Format("Players: {0} / {1}", serverInfo.NumberOfPlayers, serverInfo.MaxNumberOfPlayers);
            Engine.SpriteBatch.DrawString(Font, players,
                                          new Vector2(
                                              (int)(Bounds.Right - Font.MeasureString(players).X - Padding.X * 2),
                                              (int)(Bounds.Top + Padding.Y)),
                                          Theme.Text.Color);

        }
    }
}
