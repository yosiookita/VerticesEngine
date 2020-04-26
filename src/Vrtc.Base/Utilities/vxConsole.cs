using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine;
using System.Diagnostics;
using VerticesEngine.Diagnostics;
using VerticesEngine.Utilities;

namespace VerticesEngine
{
    /// <summary>
    /// Console Utility which provides output too both the in-game console window as well as 
    /// the system console if available.
    /// </summary>
    public class vxConsole
    {
        /// <summary>
        /// The current instance of the running Engine.
        /// </summary>
        static vxEngine Engine;

        /// <summary>
        /// The collection of Debug Strings.
        /// </summary>
        static List<string> InGameDebugLines = new List<string>();

        /// <summary>
        /// The debug string location.
        /// </summary>
        public static Vector2 DebugStringLocation = new Vector2(5, 5);

        static Rectangle InGameBackground = new Rectangle(0, 0, 1, 1);

        /// <summary>
        /// Initialises the vxConsole Static Object.
        /// </summary>
        /// <param name="engine">Engine.</param>
        internal static void Initialize(vxEngine engine)
        {
            Engine = engine;

            font = vxInternalAssets.Fonts.DebugFont;
            //This is just temporary, this is re-loaded for global uses when the vxEngine is Initialised.
            string gameVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


#if !__IOS__ && !__ANDROID__
            try
            {
                Console.Title = "VIRTICES ENGINE DEBUG CONSOLE v." + gameVersion;
            }
            catch
            {
            }
#endif

            string backend = "COMPILER FLAG NOT FOUND";
            //There's only two choices for a backend, XNA or MonoGame. The entire code base will be eventually
            //be moved over ONLY too MonoGame as XNA is no longer supported.
#if VRTC_PLTFRM_XNA
			backend = "XNA";
#elif VRTC_PLTFRM_DRTCX
			backend = "MonoGame [DirectX]";
#elif VRTC_PLTFRM_GL
            backend = "MonoGame [OpenGL]";
#elif __IOS__ || __ANDROID__
			backend = "MonoGame [Android]";
#elif __IOS__
			backend = "MonoGame [iOS]";
#endif

            // Set Build Tag info
            string EngineBuildType = "Engine Build Flags: ";

#if DEBUG
            EngineBuildType += "-Debug ";
#else
			EngineBuildType += "-Release ";
#endif



            WriteLine("____   ____             __  .__                     ");
            WriteLine("\\   \\ /   /____________/  |_|__| ____  ____   ______");
            WriteLine(" \\   Y   // __ \\_  __ \\   __\\  |/ ___\\/ __ \\ /  ___/");
            WriteLine("  \\     /\\  ___/|  | \\/|  | |  \\  \\__\\  ___/ \\___ \\ ");
            WriteLine("   \\___/  \\___  >__|   |__| |__|\\___  >___  >____  >");
            WriteLine("              \\/                    \\/    \\/     \\/ ");
            WriteLine("VERTICES ENGINE - (C) VIRTEX EDGE DESIGN");
            WriteLine("///////////////////////////////////////////////////////////////////////");
            WriteLine(string.Format("Game Name:          {0}", engine.GameName));
            WriteLine(string.Format("Game Version:       {0}", engine.GameVersion));
            WriteLine(string.Format("Graphical Backend:  {0}", backend));
            WriteLine(string.Format("Engine Version      v.{0}", gameVersion));
            WriteLine(EngineBuildType);



            WriteLine(string.Format("Cmd Line Args:      {0}", Engine.CMDLineArgsToString()));

            WriteLine("///////////////////////////////////////////////////////////////////////");

        }


        /// <summary>
        /// Writes a debug line which is outputed to both the engine debug window and the system console.
        /// </summary>
        /// <remarks>If debug information is needed, this method is useful for outputing any object.ToString() value 
        /// too both the in-engine debug window as well as the system console if it is available.</remarks>
        /// <param name="output">The object too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteLine"/> method.
        /// <code>
        /// vxConsole.WriteLine("Output of Foo is: " + foo.Output.ToString());
        /// </code>
        /// </example>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteLine"/> method with different variable types as inputs.
        /// <code>
        /// vxConsole.WriteLine(string.Format("X,Y,Z Max: {0}, {1}, {2}", Level_X_Max, Level_Y_Max, Level_Z_Max));
        /// </code>
        /// </example>
        public static void WriteLine(object output)
        {
            WriteLine(output, ConsoleColor.Green);
        }

        public static void WriteLine(object output, ConsoleColor consoleColor)
        {
            WriteLine(DebugCommandMessage.Standard, output, consoleColor);
        }

        public static void WriteDLCLine(object output)
        {
            WriteLine(DebugCommandMessage.Internal, "[PLUGIN MANAGER] : " + output, ConsoleColor.Cyan);
        }

        public static void WriteIODebug(object output)
        {
            WriteLine(DebugCommandMessage.IO, output, ConsoleColor.White);
        }

        public static void WriteSettings(object output)
        {
            WriteLine(DebugCommandMessage.IO, output, ConsoleColor.DarkCyan);
        }

        public static void InternalWriteLine(object output)
        {
            WriteLine(DebugCommandMessage.Internal, output, ConsoleColor.DarkYellow);
        }

        static List<string> lines = new List<string>();

        public static void WriteLine(DebugCommandMessage messageType, object output, ConsoleColor consoleColor)
        {
            var line = messageType + ">>: " + output;
            lines.Add(line);

#if !__IOS__ && !__ANDROID__
            Console.ForegroundColor = consoleColor;
#endif
            Console.WriteLine(">>: " + output);

#if !__IOS__ && !__ANDROID__
            Console.ResetColor();
#endif
            if (Engine != null && vxDebug.CommandUI != null)
                vxDebug.CommandUI.Echo(messageType, ">>: " + output.ToString());
        }

        /// <summary>
        /// Writes out a Verbose Line. This line will be written to the console/output window if the Verbose Property is set too true
        /// </summary>
        /// <param name="output"></param>
        public static void WriteVerboseLine(object output)
        {
            if (Engine != null && vxDebug.IsDebugOutputVerbose)
                WriteLine(DebugCommandMessage.Verbose, output, ConsoleColor.Cyan);

        }

        /// <summary>
        /// Writes out a Verbose Line. This line will be written to the console/output window if the Verbose Property is set too true
        /// </summary>
        /// <param name="output"></param>
        /// <param name="consoleColor"></param>
        public static void WriteVerboseLine(object output, ConsoleColor consoleColor)
        {
            if (Engine != null && vxDebug.IsDebugOutputVerbose)
                WriteLine(DebugCommandMessage.Verbose, output, consoleColor);
        }

        /// <summary>
        /// Similar to the <see cref="WriteLine"/> method. This method writes out a line of text which is 
        /// is prefixed with a "Networking" tag to the console output line to help distuignish it against regular 
        /// console output.
        /// </summary>
        /// <param name="output">The object too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteNetworkLine"/> method.
        /// <code>
        /// vxConsole.WriteNetworkLine("Ping: " + foo.Ping.ToString());
        /// </code>
        /// </example>
        public static void WriteNetworkLine(object output)
        {
            var line = "NET >>:" + output;
            lines.Add(line);
#if !__IOS__ && !__ANDROID__
            Console.ForegroundColor = ConsoleColor.Cyan;
#endif
            Console.WriteLine("\t" + line);

#if !__IOS__ && !__ANDROID__
            Console.ResetColor();
#endif
            if (Engine != null)
                vxDebug.CommandUI.Echo(DebugCommandMessage.Net, line);
        }

        public static void WriteError(string text)
        {
            var line = "ER >>:" + text;
            lines.Add(line);
#if !__IOS__ && !__ANDROID__
            Console.ForegroundColor = ConsoleColor.Red;
#endif
            Console.WriteLine(text);
#if !__IOS__ && !__ANDROID__
            Console.ResetColor();
#endif
            if (Engine != null)
                vxDebug.CommandUI.EchoError(text);
        }

        /// <summary>
        /// Echos the Text to the Console
        /// </summary>
        /// <param name="text"></param>
        public static void Echo(string text)
        {
            Console.WriteLine(text);
            vxDebug.CommandUI.Echo(DebugCommandMessage.IO, "     " + text);
        }

        /// <summary>
        /// Writes out an error the error.
        /// </summary>
        /// <param name="SourceFile">Source file where the error is being sent from. Helpful for tracking where error's 
        /// are being generated. </param>
        /// <param name="output">The object holding the error data too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteError"/> method.
        /// <code>
        ///     try
        ///     {
        ///         foo.bar();
        ///     }
        ///     catch(Exception ex)
        ///     {
        ///         vxConsole.WriteError(this.ToString(), ex.Message);
        ///     }
        /// </code>
        /// </example>
        public static void WriteException(object sender, Exception ex)
        {
            WriteException(sender, ex, null);
        }

        public static bool VerboseErrorLogging = false;

        public static void WriteException(object sender, Exception ex, string[] extrInfo)
        {
            if (VerboseErrorLogging == false)
            {
                WriteError("Exception Thrown in '" + sender.ToString() + "': " + ((ex != null) ? ex.Message : ""));

                if (extrInfo != null)
                {
                    WriteError("Extra Info");
                    foreach (string line in extrInfo)
                        WriteError("     " + line);
                }
            }
            else
            {
                WriteError("**************************************************");
                WriteError("Exception Thrown:");

                if (ex != null)
                {
                    WriteError("     Error Code: " + ex.HResult);
                    WriteError("     TargetSite " + ex.TargetSite);
                    WriteError("     Source " + ex.Source);

                    if (extrInfo != null)
                    {
                        WriteError("Extra Info");
                        foreach (string line in extrInfo)
                            WriteError("     " + line);
                    }

                    // Write 
                    WriteError("Message");
                    foreach (string line in ex.Message.Split(new char[] { '\n' }))
                        WriteError("     " + line);

                    // Write Inner Exception

                    WriteError("Inner Exception");
                    if (ex.InnerException != null)
                        foreach (string line in ex.InnerException.Message.Split(new char[] { '\n' }))
                            WriteError("     " + line);
                    else
                        WriteError("null");


                    // Write Stack Trace
                    if (ex.StackTrace != null)
                    {
                        WriteError("StackTrace");
                        foreach (string line in ex.StackTrace.Split(new char[] { '\n' }))
                            WriteError("     " + line);
                    }
                }
                WriteError("**************************************************");
            }
        }


        internal static void DumpLog()
        {
            using (var writer = new StreamWriter(Path.Combine(vxIO.LogDirectory, "log.txt")))
            {
                foreach (var line in lines)
                    writer.WriteLine(line);
            }
        }


        /// <summary>
        /// Writes out a warning to the debug and system console.
        /// </summary>
        /// <param name="SourceFile">Source file where the warning is being sent from. Helpful for tracking where warning's 
        /// are being generated. </param>
        /// <param name="output">The object holding the warning data too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteWarning"/> method.
        /// <code>
        ///     try
        ///     {
        ///         foo.bar();
        ///     }
        ///     catch(Exception ex)
        ///     {
        ///         vxConsole.WriteWarning(this.ToString(), ex.Message);
        ///     }
        /// </code>
        /// </example>
        public static void WriteWarning(string SourceFile, string output)
        {
            var line = "WARNING: '" + SourceFile + "' : >>: " + output;
            lines.Add(line);

#if !__IOS__ && !__ANDROID__
            Console.ForegroundColor = ConsoleColor.Yellow;
#endif
            Console.WriteLine("WARNING in '" + SourceFile + "' : >>: " + output);

#if !__IOS__ && !__ANDROID__
            Console.ResetColor();
#endif
            if (Engine != null)
            {
                vxDebug.CommandUI.EchoWarning("'" + SourceFile + "' : >>: " + output);
            }
        }


        /// <summary>
        /// Writes to in game debug. Activate the in-game debug window by running the 'cn' command.
        /// </summary>
        /// <remarks>NOTE: This is different than the Engine Debug console.</remarks>
        /// <param name="output">The object holding the error data too be outputed in the console.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WriteToInGameDebug"/> method.
        /// <code>
        /// vxConsole.WriteToInGameDebug("Player Position: " + foo.Position.ToString());
        /// </code>
        /// </example>
        public static void WriteInGameDebug(object sender, object output)
        {
            if (vxDebug.IsGameStatusConsoleVisible)
            {
                if (output != null)
                    InGameDebugLines.Add(sender + " : " + output);
                else
                    InGameDebugLines.Add("NULL");
            }
        }


        /// <summary>
        /// Clears the Ingame console ahead of the draw call, this is helpful if you have a lot of information being outputed,
        /// but only need a certain amount.
        /// </summary>
        public static void ClearInGameConsole()
        {
            InGameDebugLines.Clear();
        }


        static long CurrentUpdateTick = 0;

        static int padding = 5;
        static string outputText;

        static SpriteFont font;
        /// <summary>
        /// Draw the InGame debug window. To access this debug window, run the 'cn' command in the engine console.
        /// </summary>
        /// <remarks>NOTE: This is different than the Engine Debug console.</remarks>
        public static void Draw()
        {
            if (Engine != null && vxDebug.IsGameStatusConsoleVisible)
            {
                CurrentUpdateTick++;

                outputText = "In-Game Debug Console: " + vxEngine.PlatformOS;

                outputText += "\n===============================================";

                if (Engine.CurrentScene != null)
                    outputText += "\n" + ("Elapsed Time: " + vxTime.ElapsedTime);
                outputText += "\n" + ("# Models Loaded: " + Engine.ContentManager.LoadedModels.Count);

                foreach (string str in InGameDebugLines)
                {
                    outputText += "\n" + str;
                }


                InGameBackground.Width = Math.Max((int)font.MeasureString(outputText).X + 2 * padding, InGameBackground.Width);
                InGameBackground.Height = (int)font.MeasureString(outputText).Y + 2 * padding;


                Engine.SpriteBatch.DrawString(font, outputText, DebugStringLocation + Vector2.One, Color.Black);
                Engine.SpriteBatch.DrawString(font, outputText, DebugStringLocation, Color.White);

                //Clear it ever loop to prevent memory leaks
                InGameDebugLines.Clear();
            }
        }
    }
}
