using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;
using VerticesEngine.UI.Controls;
using System.Collections.Generic;

namespace VerticesEngine.UI.Dialogs
{
    public class vxFileDialogControlBarURLSplitter : vxFileDialogControlBarURLButton
    {
        public vxFileDialogControlBarURLSplitter(vxFileDialogControlBar ControlBar, Vector2 position) :
        base(ControlBar, "/", position, "")
        {

            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.Gray * 0.25f),
                new vxColourTheme(Color.White));
        }
    }
    public class vxFileDialogControlBarURLButton : vxButtonControl
    {
        public vxFileDialogControlBarURLButton(vxFileDialogControlBar ControlBar, string Text, Vector2 position, string urlPath):
        base(ControlBar.Engine, Text, position)
        {
            Font = vxInternalAssets.Fonts.ViewerFont;
            Width = (int)(Font.MeasureString(Text).X + Padding.X * 2);
            Height = 32;

            if(urlPath != "")
                Clicked += delegate {
                    ControlBar.FileExplorer.Path = urlPath;
                };

            Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.Gray * 0.25f, Color.DarkOrange, Color.DeepSkyBlue));
        }

        public override void Draw()
        {
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds, GetStateColour(Theme.Background));
            Vector2 TextSize = Font.MeasureString(Text).ToPoint().ToVector2();
            Vector2 pos = Position + new Vector2(Width / 2, Height / 2);
            pos = pos.ToPoint().ToVector2();
            Engine.SpriteBatch.DrawString(Font, Text,pos,
                GetStateColour(Theme.Text), 0, TextSize/2, 1, SpriteEffects.None, 1);
        }
    }
	/// <summary>
	/// File Chooser Dialor Item.
	/// </summary>
	public class vxFileDialogControlBar : vxPanel
	{

		/// <summary>
		/// The file path.
		/// </summary>
        public string FilePath {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnFilePathSet();
            }
        }
        string _filePath = "";
        List<vxFileDialogControlBarURLButton> Buttons = new List<vxFileDialogControlBarURLButton>();

        void OnFilePathSet()
        {
            Buttons.Clear();
            int runningWidth = 0;
            string urlPath = System.IO.Directory.GetDirectoryRoot(FilePath);

            foreach (var dir in FilePath.Split('/'))
            {
                Vector2 pos = Position + new Vector2(runningWidth + 32 * 3, 0);

                if (dir != "")
                {
                    var spltr = new vxFileDialogControlBarURLSplitter(this, pos);
                    runningWidth += spltr.Width;
                    Buttons.Add(spltr);
                }

                urlPath = System.IO.Path.Combine(urlPath, dir);

                pos = Position + new Vector2(runningWidth + 32 * 3, 0);
                var button = new vxFileDialogControlBarURLButton(this, dir, pos, urlPath);

                runningWidth += button.Width;
                Buttons.Add(button);
            }
        }

        public vxFileExplorerDialog FileExplorer;

        vxButtonImageControl BackButton;
        vxButtonImageControl ForwardButton;
        vxButtonImageControl DirUpButton;


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Dialogs.vxFileDialogControlBar"/> class.
        /// </summary>
        /// <param name="FileExplorer">File explorer.</param>
        public vxFileDialogControlBar(vxFileExplorerDialog FileExplorer, Vector2 Position):
        base(FileExplorer.Engine, Position, 400,32)
		{
			Padding = new Vector2(4);

            this.FileExplorer = FileExplorer;

            BackButton = new vxButtonImageControl(Engine, Vector2.Zero, 
                                          vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/fileexplorer/arrow_back"));
            ForwardButton = new vxButtonImageControl(Engine, new Vector2(32, 0), 
                                              vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/fileexplorer/arrow_forward"));
            DirUpButton = new vxButtonImageControl(Engine, new Vector2(64, 0), 
                                            vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/fileexplorer/arrow_up"));
            Items.Add(BackButton);
            Items.Add(ForwardButton);
            Items.Add(DirUpButton);
            DirUpButton.Clicked += delegate {
                
                string[] urlParts = FilePath.Split('/');
                string url = System.IO.Directory.GetDirectoryRoot(FilePath);

                // Build new path
                for (int i = 0; i < urlParts.Length-1; i++)
                    url = System.IO.Path.Combine(url, urlParts[i]);

                FileExplorer.Path = url;
            };


            DrawBackground = false;
		}

        public override void Update()
        {
            base.Update();
            // Now Draw the Controls for this panel.
            for (int i = 0; i < Buttons.Count; i++)
                Buttons[i].Update();
        }

        public override void Draw()
        {
            base.Draw();
            for (int i = 0; i < Buttons.Count; i++)
                Buttons[i].Draw();
        }
	}
}
