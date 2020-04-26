#if __STEAM__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.UI.Dialogs;

using System.IO;
using System.ComponentModel;

namespace VerticesEngine.Community
{
    public class vxWorkshopItem : vxIWorkshopItem
    {
        Facepunch.Steamworks.Workshop.Item Item;

        public string ID
        {
            get { return Item.Id.ToString(); }
        }

        public string PreviewImageURL
        {
            get { return Item.PreviewImageUrl; }
        }

        public string Author{
            get { return Item.OwnerName; }
        }

        public string Title
        {
            get { return Item.Title; }
        }


        public string Description
        {
            get { return Item.Description; }
        }


        public ulong Size
        {
            get { return Item.Size; }
        }

        public bool IsInstalled
        {
            get { return Item.Installed; }
        }


        public bool IsSubscribed
        {
            get { return Item.Subscribed; }
        }

        public string InstallPath
        {
            get { return Item.Directory.FullName; }
        }

        public Texture2D PreviewImage
        {
            get { return _previewImage; }
            set { _previewImage = value; }
        }
        Texture2D _previewImage;

        public vxWorkshopItemType ItemType
        {
            get { 
            foreach(var tag in Item.Tags)
                {
                    if(tag == "mod")
                    {
                        return vxWorkshopItemType.Mod;
                    }
                }
                return vxWorkshopItemType.SandboxFile; }
        }

        public vxWorkshopItemStatus Status
        {
            get
            {
                if (Item.DownloadPending)
                    return vxWorkshopItemStatus.DownloadPending;
                else if (Item.Downloading)
                    return vxWorkshopItemStatus.Downloading;
                else if (Item.Subscribed)
                    return vxWorkshopItemStatus.Subscribed;

                return vxWorkshopItemStatus.None;
            }
        }

        public vxWorkshopItem(Facepunch.Steamworks.Workshop.Item item)
        {
            this.Item = item;


            
        }

        public void Download()
        {
            if (Item.Subscribed)
                Item.UnSubscribe();
            else
                Item.Subscribe();
        }

        public bool Downloading
        {
            get { return Item.Downloading; }
        }

        public double DownloadProgress
        {
            get { return Item.DownloadProgress; }
        }
    }
}
#endif