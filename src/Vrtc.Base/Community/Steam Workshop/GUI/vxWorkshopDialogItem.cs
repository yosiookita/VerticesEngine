using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;
using VerticesEngine.Serilization;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.UI.Controls;
using System.Net;

namespace VerticesEngine.Community.Dialogs
{
    /// <summary>
    /// Workshop dialog item.
    /// </summary>
    public class vxWorkshopDialogItem : vxScrollPanelItem
	{
        //public string Title = "";

        public string Author
        {
            get { return Item.Author; }
        }


        public bool IsSubscribed
        {
            get { return Item.IsSubscribed; }
        }


        public string Description
        {
            get { return Item.Description; }
        }

        ///// <summary>
        ///// The file path.
        ///// </summary>
        public string FileSize
        {
            get
            {
                float size = Item.Size / 1024.0f / 1024.0f;

                return Math.Round((float)size, 3).ToString()+ " MB";
            }
        }

        public bool IsProcessed = false;

		/// <summary>
		/// The art provider.
		/// </summary>
		public new vxWorkshopDialogItemArtProvider ArtProvider;

        public vxIWorkshopItem Item
        {
            get { return vxWorkshop.PreviousSearch.Results[ItemIndex]; }
        }

        int ItemIndex;


        // 925 674 32 32

        vxButtonImageControl ButtonImageControl;

        Rectangle downloadLoc = new Rectangle(925, 674, 32, 32);
        Rectangle downloadedLoc = new Rectangle(893, 674, 32, 32);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxFileDialogItem"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="FileInfo">File info.</param>
        public vxWorkshopDialogItem(vxEngine Engine, int itemIndex) :
        base(Engine, vxWorkshop.PreviousSearch.Results[itemIndex].Title, Vector2.Zero, null, 0)
		{
			Padding = new Vector2(4);

            ItemIndex = itemIndex;

            this.Position = Position;
			OriginalPosition = Position;

            //vxWorkshop.PreviousSearch.Results[itemIndex].

			Height = 128;
			Width = 3000;

			IsTogglable = true;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(new Color(0.15f, 0.15f, 0.15f, 0.5f), Color.DarkOrange, Color.DeepSkyBlue, new Color(0.15f, 0.15f, 0.15f, 0.15f)),
                new vxColourTheme(Color.LightGray, Color.LightGray, Color.LightGray, Color.DimGray));
            
            ArtProvider = (vxWorkshopDialogItemArtProvider)vxGUITheme.ArtProviderForWorkshopDialogItem.Clone();

            ButtonImageControl = new vxButtonImageControl(Engine, Position, downloadLoc);

            ButtonImageControl.IsTogglable = true;
                ButtonImageControl.Theme.Background = new vxColourTheme(Color.Gray, Color.White,Color.White);
            
            ButtonImageControl.Clicked += delegate {

                vxConsole.WriteLine("Downloading "+Item.Title+"...");

                Item.Download();

            };

		}

        public override void Update()
        {
            base.Update();


            ButtonImageControl.Position = new Vector2(Bounds.Right - 64, Bounds.Center.Y - 32);
            ButtonImageControl.Update();
           // Console.WriteLine(Item.Downloading);

            if(Item.Status == vxWorkshopItemStatus.Downloading)
            {
                //Console.WriteLine(Item.DownloadProgress);

                //Rectangle progress = new Rectangle(Bounds.Right - 200, Bounds.Bottom - 10, (int)(175.0f * Item.DownloadProgress), 4);
                //SpriteBatch.Draw(DefaultTexture, progress, Color.DeepSkyBlue);
            }
        }



        public override void ThisSelect()
		{
			ToggleState = true;
		}
		public override void UnSelect()
		{
			ToggleState = false;
		}

		public override void Draw()
		{
			// Now draw this GUI item using it's ArtProvider.
            this.ArtProvider.Draw(this);

            this.IsEnabled = Item.IsSubscribed && Item.IsInstalled;
            ButtonImageControl.MainSpriteSheetLocation = this.IsEnabled ? downloadedLoc : downloadLoc;
            ButtonImageControl.HoverSpriteSheetLocation = ButtonImageControl.MainSpriteSheetLocation;
            ButtonImageControl.ToggleState = true;// Item.IsInstalled;
            ButtonImageControl.Draw();


            if (Item.Status != vxWorkshopItemStatus.None)
            {

            Rectangle progress = new Rectangle(Bounds.Right - 200, Bounds.Bottom - 10, (int)(175.0f * Item.DownloadProgress), 4);
                Rectangle progressBack = progress.GetBorder(1);
                progressBack.Width = 176;

                SpriteFont SubFont = vxGUITheme.Fonts.FontSmall;
                SpriteBatch.DrawString(SubFont, Item.Status.ToString(),
                                       new Vector2(progressBack.X, progressBack.Y - SubFont.LineSpacing), Color.White);


                if (Item.Status == vxWorkshopItemStatus.Downloading || Item.Status == vxWorkshopItemStatus.DownloadPending)
                {
                    SpriteBatch.Draw(DefaultTexture, progressBack, Color.Black);
                    SpriteBatch.Draw(DefaultTexture, progress, Color.DarkOrange);
                }
            }
		}


        int index = 0;
        vxWorkshopSearchResultDialog openFileDialog;
        public virtual void Process(vxWorkshopSearchResultDialog openFileDialog, int index)
        {
            this.openFileDialog = openFileDialog;
            this.index = index;
            /*
            //openFileDialog.cri
            vxConsole.WriteNetworkLine("Processing '" + Text + "'");
            this.openFileDialog = openFileDialog;
            this.index = index;


                var BckgrndWrkr = new BackgroundWorker();

                BckgrndWrkr.DoWork += OnAsyncFileDetailsLoad;
                BckgrndWrkr.RunWorkerCompleted += HandleRunWorkerCompletedEventHandler;
                BckgrndWrkr.RunWorkerAsync(Item);
                */

            filePath = System.IO.Path.Combine(vxIO.PathToTempFolder, Item.ID + ".png");

            if (File.Exists(filePath) == false)
            {
                using (WebClient wc = new WebClient())
                {
                    //wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(Item.PreviewImageURL), filePath);
                }
            }
            else
            {
                OnProcessFinished();
            }
        }
        string filePath;

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("The download has been cancelled");
                return;
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {
                Console.WriteLine("An error ocurred while trying to download file");

                return;
            }
            
            //System.Threading.Thread.Sleep(100);
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                vxWorkshop.PreviousSearch.Results[ItemIndex].PreviewImage = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
            
            //vxWorkshop.PreviousSearch.Results[ItemIndex].PreviewImage = vxIO.LoadImage(filePath);
            OnProcessFinished();
            Console.WriteLine("File succesfully downloaded");

        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " + e.TotalBytesToReceive + " bytes retrieven.");
        }

        /*
        struct WorkshopInfoResult
        {
            public Texture2D Thumbnail;
        }
        
        /// <summary>
        /// An async method called to load file details.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        public virtual void OnAsyncFileDetailsLoad(object sender, DoWorkEventArgs e)
        {
            vxIWorkshopItem item = (vxIWorkshopItem)e.Argument;

            Texture2D thumbnail = new Texture2D(Engine.GraphicsDevice, 1, 1);

            WorkshopInfoResult FileInfoResult;
            try
            {
                string filePath = System.IO.Path.Combine(vxIO.PathToTempFolder, item.ID + ".png");
                if (File.Exists(filePath) == false)
                {
                    // TODO: Make On background Thread
                    using (var client = new System.Net.WebClient())
                    {
                        client.DownloadFile(item.PreviewImageURL, filePath);
                        client.Dispose();
                    }
                }
                //System.Threading.Thread.Sleep(100);
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    thumbnail = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
                    fileStream.Dispose();
                }
            }
            catch 
            {

            }



            FileInfoResult.Thumbnail = thumbnail;

            e.Result = FileInfoResult;
        }

        public Texture2D Screenshot;
        public virtual void HandleRunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            IsProcessed = true;
            if(e.Error != null)
            {
                vxConsole.WriteError("Exception With Workshop Item '" + Text + "'");
                vxConsole.WriteException(this,e.Error);

            }
            else if (e.Result != null)
            {
                vxConsole.WriteNetworkLine("Workshop Image Loaded for'" + Text + "'");
                if (e.Result is WorkshopInfoResult)
                {
                    var finfo = (WorkshopInfoResult)e.Result;
                    vxWorkshop.PreviousSearch.Results[ItemIndex].PreviewImage = finfo.Thumbnail;
                }
            }
            OnProcessFinished();
        }
        */
        void OnProcessFinished()
        {
            IsProcessed = true;
            if (vxWorkshop.PreviousSearch.Results[ItemIndex].PreviewImage != null)
                ButtonImage = vxWorkshop.PreviousSearch.Results[ItemIndex].PreviewImage;

            index++;
            openFileDialog.ProcessItemsAsync(index);

        }

	}
}
