using Microsoft.Xna.Framework;
using System.Collections.Generic;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;

namespace VerticesEngine
{
    /// <summary>
    /// The falloff rate for area of effects. Often time this is used in Terrain Editing
    /// and painting.
    /// </summary>
    public enum vxEnumFalloffRate
    {
        /// <summary>
        /// A Linear Rate is used for porpoatial fall off.
        /// </summary>
        Linear,

        /// <summary>
        /// The same value is used across the entire area of effect, useful for creating clifs
        /// or sharp drops.
        /// </summary>
        Flat,

        /// <summary>
        /// A Smooth transition between the center and outsides of the mesh.
        /// </summary>
        Smooth
    }

    /// <summary>
    /// The mode for scultping, whether it's averaging, or creates a delta (addative/subtractive).
    /// </summary>
    public enum vxEnumAreaOfEffectMode
    {
        /// <summary>
        /// Creates a Delta 
        /// </summary>
        Delta,

        /// <summary>
        /// Averages the values within the area of effect
        /// </summary>
        Averaged,
    }

    public partial class vxGameplayScene3D
    {

        public int TexturePaintType = 1;

        /// <summary>
        /// The Terrain Sculpt Toggel Button
        /// </summary>
        vxRibbonButtonControl SculptButton;

        /// <summary>
        /// The Terrain Texture Paint Toggel Button.
        /// </summary>
        vxRibbonButtonControl TexturePaintButton;


        vxRibbonButtonControl DeltaSculptButton;
        vxRibbonButtonControl AverageSculptButton;

        vxRibbonContextualTabPage terrainTabPage;
        List<vxToolbarButton> FallOffButtons = new List<vxToolbarButton>();

        /// <summary>
        /// A List of the Terrain Texture Paint Buttons.
        /// </summary>
        List<vxTxtrPaintToolbarButton> TxtrPaintButtons = new List<vxTxtrPaintToolbarButton>();

        public vxEnumFalloffRate FalloffRate;

        public vxEnumAreaOfEffectMode AreaOfEffectMode;

        void InitialiseTerrainToolbar()
        {
            terrainTabPage = new vxRibbonContextualTabPage(RibbonControl, "Terrain", "Edit Terrain", Color.Green);


            var terraincameraGroup = new vxRibbonControlGroup(terrainTabPage, "Edit Terrain");
            SculptButton = new vxRibbonButtonControl(terraincameraGroup, "Scult",
                                                             vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/mode_sulpt"), vxEnumButtonSize.Big);
            SculptButton.IsTogglable = true;
            SculptButton.ToggleState = true;
            SculptButton.Clicked += SculptButton_Clicked;
            SculptButton.SetToolTip("Toggle Terrain Sculpt Mode");

            TexturePaintButton = new vxRibbonButtonControl(terraincameraGroup, "Texture Paint",
                                                                 vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/mode_txtrpaint"), vxEnumButtonSize.Big);
            TexturePaintButton.IsTogglable = true;
            TexturePaintButton.Clicked += TexturePaintButton_Clicked;
            TexturePaintButton.SetToolTip("Toggle Terrain Texture Painting Mode");


            var terrainEditFalloffGroup = new vxRibbonControlGroup(terrainTabPage, "Falloff Type");

            DeltaSculptButton = new vxRibbonButtonControl(terrainEditFalloffGroup, "Delta",
                                                                 vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/sculpt_delta"), vxEnumButtonSize.Big);

            DeltaSculptButton.IsTogglable = true;
            DeltaSculptButton.ToggleState = true;
            DeltaSculptButton.Clicked += DeltaSculptButton_Clicked; ;
            DeltaSculptButton.SetToolTip("Area effect creates a Delta");


            AverageSculptButton = new vxRibbonButtonControl(terrainEditFalloffGroup, "Average",
                                                             vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/sculpt_avg"), vxEnumButtonSize.Big);
            AverageSculptButton.IsTogglable = true;
            AverageSculptButton.Clicked += AverageSculptButton_Clicked; ;
            AverageSculptButton.SetToolTip("Area effect creates an Average");




            var terrainEditTypeGroup = new vxRibbonControlGroup(terrainTabPage, "Terrain Edit Type");

            var terrainEditSmooth = new vxRibbonButtonControl(terrainEditTypeGroup, "Smooth",
                                                                 vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/edit_smooth"));

            var terrainEditLinear = new vxRibbonButtonControl(terrainEditTypeGroup, "Linear",
                                                                vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/edit_linear"));
            var terrainEditFlat = new vxRibbonButtonControl(terrainEditTypeGroup, "Flat",
                                                                vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/edit_flat"));

           


            var terrainEditEnd = new vxRibbonButtonControl(new vxRibbonControlGroup(terrainTabPage, "Finish"), "Exit",
                                                           vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/rbn/terrain/exit"), vxEnumButtonSize.Big);
            
            terrainEditEnd.Clicked += delegate
            {
                //EntitiesTabPage.SelectTab();
                RibbonControl.Pages[2].SelectTab();
                RibbonControl.RemoveContextTab(terrainTabPage);

                SandboxEditMode = vxEnumSanboxEditMode.SelectItem;
            };



            //// Handle Scuplting Porporational Types

            //foreach (vxEnumFalloffRate type in Enum.GetValues(typeof(vxEnumFalloffRate)))
            //{
            //    string texturePath = "Textures/sandbox/tlbr/terrain/sculpt_" + type.ToString().ToLower();
            //    vxToolbarButton fallOffButton = new vxToolbarButton(Engine, Engine.InternalAssets, texturePath);
            //    fallOffButton.IsTogglable = true;
            //    fallOffButton.Clicked += FallOffButton_Clicked;
            //    fallOffButton.Text = ((int)type).ToString();
            //    fallOffButton.SetToolTip("Set Active Sculpting Falloff too: " + type);

            //    FallOffButtons.Add(fallOffButton);
            //}
            //FallOffButtons[0].ToggleState = true;


            //// Terrain Painint
            //for (int i = 0; i < TerrainManager.Textures.Count; i++)
            //{
            //    vxTxtrPaintToolbarButton terrainPaintBtn = new vxTxtrPaintToolbarButton(Engine, TerrainManager, i);
            //    //terrainPaintBtn.IsTogglable = true;
            //    terrainPaintBtn.Enabled = false;
            //    terrainPaintBtn.Clicked += TerrainPaintBtn_Clicked;
            //    TxtrPaintButtons.Add(terrainPaintBtn);
            //    terrainPaintBtn.SetToolTip("Set Active Texture for Painting To Texture " + (i+1).ToString());
            //}
            //TxtrPaintButtons[0].ToggleState = true;


            //vxToolbarButton ExitTerrainEdtiorButton = new vxToolbarButton(Engine, Engine.InternalAssets, "Textures/sandbox/tlbr/terrain/exit");
            //ExitTerrainEdtiorButton.Clicked += ExitTerrainEdtiorButton_Clicked;
            //ExitTerrainEdtiorButton.SetToolTip("Exit from Terrain Editing");


            //TerrainEditorToolbar.AddItem(SculptButton);
            //TerrainEditorToolbar.AddItem(TexturePaintButton);
            //TerrainEditorToolbar.AddItem(new vxToolbarSpliter(Engine, 5));
            
            //TerrainEditorToolbar.AddItem(DeltaSculptButton);
            //TerrainEditorToolbar.AddItem(AverageSculptButton);
            //TerrainEditorToolbar.AddItem(new vxToolbarSpliter(Engine, 5));
            
            //foreach (vxToolbarButton fllOffbtn in FallOffButtons)
            //    TerrainEditorToolbar.AddItem(fllOffbtn);

            //TerrainEditorToolbar.AddItem(new vxToolbarSpliter(Engine, 5));

            //foreach (vxTxtrPaintToolbarButton button in TxtrPaintButtons)
            //    TerrainEditorToolbar.AddItem(button);

            //TerrainEditorToolbar.AddItem(new vxToolbarSpliter(Engine, 5));
            //TerrainEditorToolbar.AddItem(ExitTerrainEdtiorButton);


        }





        // Scult or Texture Paint Mode
        // ***************************************************************************************************
        private void SculptButton_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SculptButton.ToggleState = true;
            TexturePaintButton.ToggleState = false;

            TerrainEditState = vxEnumTerrainEditMode.Sculpt;

            // Disable all Texture Painting Buttons
            foreach (vxTxtrPaintToolbarButton button in TxtrPaintButtons)
                button.IsEnabled = false;
        }

        private void TexturePaintButton_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            SculptButton.ToggleState = false;
            TexturePaintButton.ToggleState = true;

            TerrainEditState = vxEnumTerrainEditMode.TexturePaint;

            // Enable all Texture Painting Buttons
            foreach (vxTxtrPaintToolbarButton button in TxtrPaintButtons)
                button.IsEnabled = true;
        }



        private void DeltaSculptButton_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            AverageSculptButton.ToggleState = false;
            DeltaSculptButton.ToggleState = true;

            AreaOfEffectMode = vxEnumAreaOfEffectMode.Delta;
        }

        private void AverageSculptButton_Clicked(object sender, vxGuiItemClickEventArgs e)
        {

            AverageSculptButton.ToggleState = true;
            DeltaSculptButton.ToggleState = false;

            AreaOfEffectMode = vxEnumAreaOfEffectMode.Averaged;
        }


        // Scult Mode
        // ***************************************************************************************************

        private void FallOffButton_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            FalloffRate = (vxEnumFalloffRate)int.Parse(e.GUIitem.Text);

            foreach (vxToolbarButton btn in FallOffButtons)
                btn.ToggleState = false;

            e.GUIitem.ToggleState = true;
        }



        // Texture Paint Mode
        // ***************************************************************************************************

        private void TerrainPaintBtn_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            foreach (vxTxtrPaintToolbarButton button in TxtrPaintButtons)
                button.ToggleState = false;

            vxTxtrPaintToolbarButton item = (vxTxtrPaintToolbarButton)e.GUIitem;

            TexturePaintType = item.TexturePaintIndex;
            item.ToggleState = true;
        }



        // Exit
        // ***************************************************************************************************
        private void ExitTerrainEdtiorButton_Clicked(object sender, vxGuiItemClickEventArgs e)
        {
            TerrainEditorToolbar.Position = new Vector2(0, -50);
            MainToolbar.Position = new Vector2(0, 0);
        }
    }
}