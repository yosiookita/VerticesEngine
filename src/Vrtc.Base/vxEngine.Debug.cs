using Microsoft.Xna.Framework;

using VerticesEngine.Utilities;
using VerticesEngine.Net;

using System;
using VerticesEngine.Mathematics;
using System.Collections.Generic;
using VerticesEngine.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Graphics;

namespace VerticesEngine
{
    public sealed partial class vxEngine// : DrawableGameComponent
    {

        internal void RegisterCommand(string command, string description, DebugCommandExecute callback)
        {
            vxDebug.CommandUI.RegisterCommand(command, description, callback);
        }

        /// <summary>
        /// Applies any command line arguments in the .
        /// </summary>
        private void ApplyCommandLineArgs()
        {
            //Get Command Line Arguments that have been passed to the main game
            string[] args = System.Environment.GetCommandLineArgs();

            string argoutput = "Applying Command Line Arguments: ";

            for (int argIndex = 1; argIndex < args.Length; argIndex++)
            {
                argoutput += args[argIndex] + " ";
            }

            //Parse Command Line Arguments Here
            for (int argIndex = 0; argIndex < args.Length; argIndex++)
            {

                try
                {
                    TryToApplyCMDArg(args, argIndex);
                }
                catch (Exception ex)
                {
                    vxConsole.WriteLine(string.Format(" >> cmd line error: {0} <<", ex.Message));
                }

            }
        }

        /// <summary>
        /// Tries to apply CMD argument.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <param name="argIndex">Argument index.</param>
        private void TryToApplyCMDArg(string[] args, int argIndex)
        {
            //Get Argument
            string arg = args[argIndex];

            switch (arg)
            {

                //Sets the Build Conifg too Debug. This for debugging release builds in the wild
                case "-dev":
                case "-debug":
                case "-console":
                    SetBuildType(vxBuildType.Debug);
                    break;

                //Set Resolution Width
                case "-w":
                case "-width":
                    vxDebug.CommandUI.ExecuteCommand(string.Format("width {0}", args[argIndex + 1]));
                    break;
                //Set Resolution Height
                case "-h":
                case "-height":
                    vxDebug.CommandUI.ExecuteCommand(string.Format("height {0}", args[argIndex + 1]));
                    break;

                case "-sw":
                case "-startwindowed":
                case "-window":
                case "-windowed":
                    vxDebug.CommandUI.ExecuteCommand(string.Format("window"));
                    break;

                //Set Fullsreen
                case "-full":
                case "-fullscreen":
                    vxDebug.CommandUI.ExecuteCommand(string.Format("fullscreen"));
                    break;

                case "-fps":
                    vxDebug.CommandUI.ExecuteCommand(string.Format("fps"));
                    break;
                case "-tr":
                    vxDebug.CommandUI.ExecuteCommand(string.Format("tr"));
                    break;

                case "-v":
                    vxDebug.IsVerbose = true;
                    break;

                // only load mods if it's in debug/dev mode
                case "-mod":
                    if (BuildType == vxBuildType.Debug)
                    {
                        try
                        {
                            DevModPaths.Add(args[argIndex + 1]);
                            //PluginManager.LoadAssembly(args[argIndex + 1]);
                        }
                        catch (Exception ex)
                        {
                            vxConsole.WriteException(this, ex);
                        }
                    }
                    break;
            }
        }

        List<string> DevModPaths = new List<string>();
        internal string[] LoadDevMods()
        {
            //DevModPaths.Add(@"C:\Users\rtroe\Documents\GitHub\chaoticworkshop-modkit\ChaoticWorkshopModKit\bin\Debug\ChaoticWorkshop.Mods.Template.dll");
            return DevModPaths.ToArray();
        }

        public void DrawDebugFlag()
        {
            //enginevtext += "\n"+GameName + " v." + GameVersion;
            //enginevtext += "\nVertices Engine v." + EngineVersion + " - (" + PlatformType + ")";
            if (DevModPaths.Count > 0)
            {
                SpriteFont engineFont = vxInternalAssets.Fonts.ViewerFont;

                // Engine Version Text
                string enginevtext = GameName + " - Dev Mode\n========================\n";

                enginevtext += "Loaded Development DLL's:\n";

                foreach (var dll in DevModPaths)
                    enginevtext += "  " + dll + "\n";

                // Text Size + Padding and offset from screen edges
                Vector2 EngineTextSize = engineFont.MeasureString(enginevtext);
                int padding = 5;

                SpriteBatch.DrawString(vxInternalAssets.Fonts.ViewerFont,
                                       enginevtext,
                                       (padding + 1) * Vector2.One,
                                       Color.Black);

                SpriteBatch.DrawString(vxInternalAssets.Fonts.ViewerFont,
                                       enginevtext,
                                       (padding) * Vector2.One,
                                       Color.Lime);
            }

            //int offset = 5;

            // Set the text position
            //Vector2 EngineTextPosition = new Vector2(padding);

            //Draw Version Information
            //spriteBatch.Draw(InternalAssets.Textures.Blank,
            //				 new Rectangle(
            //					 EngineTextPosition.ToPoint().X - padding,
            //					 EngineTextPosition.ToPoint().Y - padding/2,
            //					(int)EngineTextSize.X + 2 * padding,
            //					(int)EngineTextSize.Y + padding),
            //	Color.Black * 0.75f * Alpha);


        }

        //      int local = -50;
        //      int Req = -50;
        ///// <summary>
        ///// Small Debug Utility that draws to the screen Connection Info. Debug Purposes only.
        ///// </summary>
        //void DrawNetworkGameConnectionInfo()
        //{
        //	if (BuildConfigType == vxBuildConfigType.Debug)
        //	{
        //		Req = -50;
        //		//SpriteBatch.Begin();
        //		if (ServerManager.Server != null && ServerManager.ServerStatus != NetPeerStatus.NotRunning)
        //		{
        //			Req = 5;
        //			local = vxMathHelper.Smooth(local, Req, 8);
        //			string output = string.Format(
        //				"NETWORK DEBUG INFO: | User Roll: {3} | Server Name: {0} | Port: {1} | Address: {2} | Status: {4}",
        //				ServerManager.Server.Configuration.AppIdentifier,
        //				ServerManager.Server.Configuration.Port.ToString(),
        //				ServerManager.Server.Configuration.BroadcastAddress,
        //				NetworkSessionManager.PlayerNetworkRole.ToString(),
        //				ServerManager.ServerStatus.ToString());

        //			int pad = 3;

        //			SpriteBatch.Draw(vxInternalAssets.Textures.Blank, new Rectangle(0, local + 0, 1000, (int)vxInternalAssets.Fonts.DebugFont.MeasureString(output).Y + 2 * pad), Color.Black * 0.75f);
        //			SpriteBatch.DrawString(vxInternalAssets.Fonts.DebugFont, output, new Vector2(pad, local + pad), Color.White);
        //		}
        //              else
        //                  NetworkSessionManager.Draw();
        //	}
        //}

        /// <summary>
        /// Draws the version info in the corner of the screen.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw the info with.</param>
        /// <param name="Alpha">Alpha to draw with, usually the TransitionAlpha value for the current screen.</param>
        /// <param name="color">Color to draw the text with.</param>
        public void DrawVersionInfo(vxSpriteBatch spriteBatch, float Alpha, Color color)
		{
			SpriteFont engineFont = vxInternalAssets.Fonts.ViewerFont;

			// Engine Version Text
			string enginevtext = "Engine v." + EngineVersion + " - (" + PlatformOS + ")";

			// Text Size + Padding and offset from screen edges
			Vector2 EngineTextSize = engineFont.MeasureString(enginevtext);
			int padding = 5;
			int offset = 5;

			// Set the text position
			Vector2 EngineTextPosition = new Vector2(offset + padding, GraphicsDevice.Viewport.Height - EngineTextSize.Y - (offset + padding));

			//Draw Version Information
			//spriteBatch.Draw(InternalAssets.Textures.Blank,
			//				 new Rectangle(
			//					 EngineTextPosition.ToPoint().X - padding,
			//					 EngineTextPosition.ToPoint().Y - padding/2,
			//					(int)EngineTextSize.X + 2 * padding,
			//					(int)EngineTextSize.Y + padding),
			//	Color.Black * 0.75f * Alpha);

			spriteBatch.DrawString(vxInternalAssets.Fonts.ViewerFont,
								   enginevtext,
								   EngineTextPosition,
								   color * 0.75f * Alpha);
		}


         void DrawIPContent(SpriteBatch spriteBatch, float Alpha, Color color)
        {
            SpriteFont engineFont = vxInternalAssets.Fonts.ViewerFont;

            // Engine Version Text
            string enginevtext = "Engine v." + EngineVersion + " - (" + PlatformOS + ")";

            // Text Size + Padding and offset from screen edges
            Vector2 EngineTextSize = engineFont.MeasureString(enginevtext);
            int padding = 5;
            int offset = 5;

            // Set the text position
            Vector2 EngineTextPosition = new Vector2(offset + padding, GraphicsDevice.Viewport.Height - EngineTextSize.Y - (offset + padding));

            //Draw Version Information
            //spriteBatch.Draw(InternalAssets.Textures.Blank,
            //               new Rectangle(
            //                   EngineTextPosition.ToPoint().X - padding,
            //                   EngineTextPosition.ToPoint().Y - padding/2,
            //                  (int)EngineTextSize.X + 2 * padding,
            //                  (int)EngineTextSize.Y + padding),
            //  Color.Black * 0.75f * Alpha);

            spriteBatch.DrawString(vxInternalAssets.Fonts.ViewerFont,
                                   enginevtext,
                                   EngineTextPosition,
                                   color * 0.75f * Alpha);
        }
	}
}
