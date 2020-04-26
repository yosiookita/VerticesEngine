using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vrtc.Localization
{
    public class vxLanguagePackFrenchBase : vxLanguagePack
    {
        public vxLanguagePackFrenchBase() : base("Francais")
        {
            //Main Menu
            Add(vxLocalization.Main_Multiplayer, "Mode Multijoueur");
            Add(vxLocalization.Main_Sandbox, "Sandbox");
            Add(vxLocalization.Main_Settings, "Parametres");
            Add(vxLocalization.Main_Exit, "Sortie");

            //Multiplayer
            Add(vxLocalization.Net_LAN, "Locale");
            Add(vxLocalization.Net_Online, "En Ligne");

            //Pause
            Add(vxLocalization.Pause, "Pause");
            Add(vxLocalization.Pause_Resume, "Continuer");
            Add(vxLocalization.Pause_AreYouSureYouWantToQuit, "Etes-vous sur de vouloir quitter ce jeu?");


            //Settings Page
            Add(vxLocalization.Settings_Controls, "Controles");
            Add(vxLocalization.Settings_Graphics, "Graphique");
            Add(vxLocalization.Settings_Localization, "Localisation");
            Add(vxLocalization.Settings_Audio, "Audio");

            //Graphics Page
            Add(vxLocalization.Graphics_GraphicsSettings, "Parametres Graphiques");
            Add(vxLocalization.Graphics_Resolution, "Resolution");
            Add(vxLocalization.Graphics_FullScreen, "Plein Ecran");
            Add(vxLocalization.Graphics_Windowed, "Fenetre");

            //Misc
            Add(vxLocalization.Misc_Yes, "Oui");
            Add(vxLocalization.Misc_No, "Non");
            Add(vxLocalization.Misc_OK, "OK");
            Add(vxLocalization.Misc_Cancel, "Annuler");
            Add(vxLocalization.Misc_New, "Nouveau");
            Add(vxLocalization.Misc_Open, "Ouvrir");
            Add(vxLocalization.Misc_Back, "Arriere");
        }
    }
}
