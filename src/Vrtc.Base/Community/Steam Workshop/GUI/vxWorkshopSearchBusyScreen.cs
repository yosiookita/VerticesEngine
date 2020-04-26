using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VerticesEngine.Util;
//using VerticesEngine;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.MessageBoxs;
using VerticesEngine.Serilization;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.Community.Events;

namespace VerticesEngine.Community.Dialogs
{
    /// <summary>
    /// The Workshop Busy Screen
    /// </summary>
    public class vxWorkshopSearchBusyScreen : vxMessageBox
    {


        vxWorkshopSearchCriteria searchCriteria;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.vxSaveBusyScreen"/> class.
        /// </summary>
        public vxWorkshopSearchBusyScreen(vxWorkshopSearchCriteria searchCriteria)
            : base("Searching", "Searching", UI.vxEnumButtonTypes.Ok)
        {
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            this.searchCriteria = searchCriteria;

        }

        public override void LoadContent()
        {
            base.LoadContent();

            OKButton.Text = "Cancel";

            OKButton.Clicked += delegate {

                ExitScreen();

                Engine.PlayerProfile.SearchResultReceived -= PlayerProfile_SearchResultReceived;
            };
        }


        int Inc = 0;


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (Inc == 40)
            {
                StartSearch();
            }
            string msgText = "Searching " + searchCriteria.ItemCriteria + " Items";
            int origLength = msgText.Length + 5;
            Inc++;

            msgText += new string('.', (int)(Inc / 10) % 3);

            Message = msgText + "\n" + new string(' ', origLength * 3 /2 );;
            //Bounds.Width = 300;
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }

        public void StartSearch()
        {
            Engine.PlayerProfile.SearchWorkshop(searchCriteria);
            Engine.PlayerProfile.SearchResultReceived += PlayerProfile_SearchResultReceived;
        }



        void PlayerProfile_SearchResultReceived(object sender, vxWorkshopSeachReceievedEventArgs e)
        {
            vxWorkshop.OnSearchResultsReceived(e.Items);

            ExitScreen();
            
            vxSceneManager.AddScene(new vxWorkshopSearchResultDialog( 0));

            Engine.PlayerProfile.SearchResultReceived -= PlayerProfile_SearchResultReceived;
        }
    }
}
