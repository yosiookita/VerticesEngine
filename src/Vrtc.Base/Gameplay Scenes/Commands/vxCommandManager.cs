using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VerticesEngine;

namespace VerticesEngine.Commands
{
	/// <summary>
	/// Manages Do-Undo control in a sandbox enviroment.
	/// </summary>
	public class vxCommandManager
	{
		public readonly vxEngine Engine;

        public bool IsActive = true;

		List<vxCommand> Commands = new List<vxCommand>();

		/// <summary>
		/// Gets the number of Commands currently in the list.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get { return Commands.Count; }
		}


		/// <summary>
		/// The index of the current cmd. 
		/// </summary>
		public int CurrentCmdIndex = -1;

        /// <summary>
        /// Should the Command List show the current list.
        /// </summary>
        public bool ShowDebugOutput = false;

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:VerticesEngine.Base.vxCommandManager"/> can do.
		/// </summary>
		/// <value><c>true</c> if can do; otherwise, <c>false</c>.</value>
		public bool CanRedo
		{
			get { return (CurrentCmdIndex < Count-1); }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:VerticesEngine.Base.vxCommandManager"/> can undo.
		/// </summary>
		/// <value><c>true</c> if can undo; otherwise, <c>false</c>.</value>
		public bool CanUndo
		{
			get { return (CurrentCmdIndex >= 0); }
		}





		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.Commands.vxCommandManager"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		public vxCommandManager()
		{

		}

		public event EventHandler<EventArgs> OnChange;

		/// <summary>
		/// Add the specified command and clears all commands after the current Command Index.
		/// </summary>
		/// <returns>The add.</returns>
		/// <param name="command">Command.</param>
		public void Add(vxCommand command)
		{
            if (IsActive)
            {
                // Remore any commands which are ahead of the current cmd index
                if (Count > 0)
                {
                    while (CurrentCmdIndex < Count - 1)
                        Commands.RemoveAt(Count - 1);
                }

                // Add it to the Commands Collection
                Commands.Add(command);
                CurrentCmdIndex = Count - 1;

                command.Do();
            }

			if (OnChange != null)
				OnChange(this, new EventArgs());
		}


		/// <summary>
		/// Calls the Redo for the current command.
		/// </summary>
		public void ReDo()
		{
			if (CanRedo && IsActive)
			{
				CurrentCmdIndex++;
				CurrentCmdIndex = MathHelper.Clamp(CurrentCmdIndex, -1, Count - 1);
				Commands[CurrentCmdIndex].Do();
			}


			if (OnChange != null)
				OnChange(this, new EventArgs());
		}

		/// <summary>
		/// Calls the Undo for the current command.
		/// </summary>
		public void Undo()
		{
			if (CanUndo && IsActive)
			{
				Commands[CurrentCmdIndex].Undo();
				CurrentCmdIndex--;
				CurrentCmdIndex = MathHelper.Clamp(CurrentCmdIndex, -1, Count - 1);
			}

			if (OnChange != null)
				OnChange(this, new EventArgs());
		}


		public void Draw()
		{
            if (ShowDebugOutput)
            {
                Vector2 pos = new Vector2(0, 64);
                Engine.SpriteBatch.Begin("Debug - Command Manager");
                int i = 0;
                foreach (vxCommand cmd in Commands)
                {
                    if (i == CurrentCmdIndex)
                        DrawString(">", pos);
                    i++;

                    DrawString(cmd.Tag, pos + Vector2.UnitX * 10);
                    pos += Vector2.UnitY * vxInternalAssets.Fonts.ViewerFont.LineSpacing;
                }
                Engine.SpriteBatch.End();
            }
		}

		void DrawString(string text, Vector2 Pos)
		{
				Engine.SpriteBatch.DrawString(
					vxInternalAssets.Fonts.ViewerFont,
					text,
					Pos+Vector2.One,
					Color.Black);
			
							Engine.SpriteBatch.DrawString(
					vxInternalAssets.Fonts.ViewerFont,
					text,
					Pos,
					Color.White);
		}
	}
}
