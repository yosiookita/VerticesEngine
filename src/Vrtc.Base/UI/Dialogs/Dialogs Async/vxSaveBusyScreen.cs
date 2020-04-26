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

namespace VerticesEngine
{
    /// <summary>
    /// The save busy screen.
    /// </summary>
    public class vxSaveBusyScreen : vxMessageBox
    {

        vxGameplaySceneBase Scene;

        public BackgroundWorker SaveFileAsyncWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.vxSaveBusyScreen"/> class.
        /// </summary>
        public vxSaveBusyScreen(vxGameplaySceneBase Scene)
            : base("Saving", "Saving File", UI.vxEnumButtonTypes.None)
        {
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            this.Scene = Scene;


            SaveFileAsyncWriter = new BackgroundWorker();
            SaveFileAsyncWriter.WorkerReportsProgress = true;
            SaveFileAsyncWriter.WorkerSupportsCancellation = true;
            SaveFileAsyncWriter.DoWork += OnAsyncWriteSaveFile;
            SaveFileAsyncWriter.ProgressChanged += OnAsyncSaveFileWriter_ProgressChanged;
            SaveFileAsyncWriter.RunWorkerCompleted += OnAsyncSaveFileWriter_RunWorkerCompleted;
        }

        int Inc = 0;

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (Inc == 25)
            {
                vxConsole.WriteIODebug("============================================");
                vxConsole.WriteIODebug("Saving File : '" + Scene.FileName + "'");
                StartSave();
            }
            string SavingText = "Saving File ";

            Inc++;

            SavingText += new string('.', (int)(Inc / 10) % 5);

            Message = SavingText;
            //Bounds.Width = 300;
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }

        public void StartSave()
        {
            vxIO.ClearTempDirectory();
            Scene.SaveSupportFiles();
            SaveFileAsyncWriter.RunWorkerAsync(Scene);
        }

        float percent = 0;
        public virtual void OnAsyncSaveFileWriter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            percent = e.ProgressPercentage / 100.0f;
            Console.Write(".");
        }



        /// <summary>
        /// The Async File Save Writer, override this to provide your own custom saving.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        public virtual void OnAsyncWriteSaveFile(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Engine.SpriteBatch.Begin("Save Screen");

            Engine.SpriteBatch.Draw(vxInternalAssets.Textures.Blank,
                                    new Rectangle(0, 0, (int)(Viewport.Width * percent), 2),
                                    Color.DeepSkyBlue);

            Engine.SpriteBatch.End();
        }


        void OnAsyncSaveFileWriter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool success = false;
            if (e.Error != null)
            {
                vxConsole.WriteLine("ERROR: " + e.Error.Message);
                vxConsole.WriteLine("StackTrace: " + e.Error.StackTrace);
                //ButtonImage = DefaultTexture;
            }
            else if (e.Result != null)
            {
                //Console.WriteLine();
                if (e.Result is bool)
                    success = (bool)e.Result;
            }
            vxConsole.WriteIODebug("Finished Save! Success: " + success);
            vxConsole.WriteIODebug("============================================");
            ExitScreen();
            Scene.IsDumping = false;
        }
    }
}
