using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{

    /// <summary>
    /// Basic Button GUI Control.
    /// </summary>
    public static class vxNotificationManager
    {
        static List<vxNotification> Notifications = new List<vxNotification>();

        public static bool IsOnBottom = true;

        public static void Add(vxNotification notification)
        {
            Notifications.Add(notification);
        }

        public static void Update()
        {
            foreach (vxNotification notification in Notifications)
                notification.Update();
        }

        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public static void Draw()
        {
            foreach (vxNotification notification in Notifications)
                notification.Draw();
        }
    }
}
