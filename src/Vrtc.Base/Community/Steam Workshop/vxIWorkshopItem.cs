using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.UI.Dialogs;


namespace VerticesEngine.Community
{
    public enum vxWorkshopItemStatus
    {
        None,
        DownloadPending,
        Downloading,
        Subscribed
    }

    public enum vxWorkshopItemType
    {
        SandboxFile,
        Mod
    }

    public interface vxIWorkshopItem 
    {
        string ID { get; }

        string PreviewImageURL { get; }

        Texture2D PreviewImage { get; set; }

        string InstallPath { get; }

        string Author { get; }

        string Title { get; }

        string Description { get; }

        ulong Size { get; }

        bool IsSubscribed { get; }

        bool IsInstalled { get; }

        void Download();

        bool Downloading { get; }

        double DownloadProgress { get; }

        vxWorkshopItemStatus Status { get; }

        vxWorkshopItemType ItemType { get; }
    }
}
