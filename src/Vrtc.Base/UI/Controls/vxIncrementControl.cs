using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using VerticesEngine.UI;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.UI.Events;
using VerticesEngine.UI.Themes;
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Controls
{
	/// <summary>
	/// Scrollbar Item base class. This can be inherited too expand controls within one scrollbar item.
	/// </summary>
	public class vxIncrementControl : vxGUIControlBaseClass
    {
        vxSpinnerControl Spinner;
		List<vxGUIControlBaseClass> GUIItemList = new List<vxGUIControlBaseClass>();

		public float Value = 5;
		public float MinValue = 0;
		public float MaxValue = 10;

		public float Incremnet=1;

		/// <summary>
		/// The arrow start offset.
		/// </summary>
		public int ArrowStartOffset = 250;

        /// <summary>
        /// The size of the button sqaure.
        /// </summary>
		//public static int ButtonSqaureSize = 40;

		/// <summary>
		/// The space between arrows. This is measured as the maximum value between the text width of the 
		/// minimum and maximum values.
		/// </summary>
		public int ArrowSpace = 50;


		/// <summary>
		/// Occurs when the value changes.
		/// </summary>
		public event EventHandler<vxValueChangedEventArgs> ValueChanged;


		/// <summary>
		/// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxIncrementControl"/> class.
		/// </summary>
		/// <param name="Engine">Engine.</param>
		/// <param name="Value">Value.</param>
		/// <param name="MinValue">Minimum value.</param>
		/// <param name="MaxValue">Max value.</param>
		public vxIncrementControl (vxEngine Engine, string Text, Vector2 Position, float Value, float MinValue, float MaxValue) : 
		this(Engine, Text, Position, Value, MinValue, MaxValue, (MaxValue - MinValue) / 10)
        {

		}


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxIncrementControl"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="text">This GUI Items Text.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="Value">Value.</param>
        /// <param name="MinValue">Minimum value.</param>
        /// <param name="MaxValue">Max value.</param>
        /// <param name="inc">Inc.</param>
        public vxIncrementControl(vxEngine Engine, string Text,
                                  Vector2 Position, float Value, float MinValue, float MaxValue, float inc) : 
        this(Engine, Text, Position, Value, MinValue, MaxValue, inc, 250)
        {
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxIncrementControl"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="text">This GUI Items Text.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="Value">Value.</param>
        /// <param name="MinValue">Minimum value.</param>
        /// <param name="MaxValue">Max value.</param>
        /// <param name="inc">Inc.</param>
        /// <param name="ArrowStartOffset">Arrow start offset.</param>
		public vxIncrementControl(vxEngine Engine, string Text,
                          Vector2 Position, float Value, float MinValue, float MaxValue, float inc,
                                 int ArrowStartOffset) : base(Engine)
		{
            Height = (int)(64 * vxLayout.ScaleAvg * 3);
            int btnHeight = Height;
			this.Engine = Engine;
			this.Text = Text;
			this.Position = Position;
			this.Value = Value;
			this.MaxValue = MaxValue;
			this.MinValue = MinValue;
            this.ArrowStartOffset = (int)(ArrowStartOffset + (100) * vxLayout.ScaleAvg);

			Incremnet = inc;

			Font = vxInternalAssets.Fonts.MenuFont;
				
			btnHeight = (int)(64 * vxLayout.ScaleAvg);

            ArrowSpace = (int)(ArrowSpace * vxLayout.ScaleAvg);

            // The Spinner Control
            Spinner = new vxSpinnerControl(Engine, 
                                    (int)Value, 
                                    new Vector2(ArrowStartOffset, 0), 
                                    (int)(vxGUITheme.SpriteSheetLoc.ArrowBtnBack.Width * vxLayout.ScaleAvg), 
                                    ArrowSpace, 
                                    (int)inc,
                                   vxGUITheme.SpriteSheetLoc.ArrowBtnBack,
                                    vxGUITheme.SpriteSheetLoc.ArrowBtnFwd);

            GUIItemList.Add(Spinner);

            Spinner.ValueChanged += Spinner_ValueChanged;

			foreach (vxGUIControlBaseClass item in GUIItemList)
				item.Font = Font;

			Bounds = new Rectangle(0, 0, vxGUITheme.SpriteSheetLoc.ArrowBtnBack.Width, vxGUITheme.SpriteSheetLoc.ArrowBtnBack.Height);

			EnabledStateChanged += OnEnabledStateChanged;
        }

        void Spinner_ValueChanged(object sender, vxValueChangedEventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged(this, e);
        }

        void OnEnabledStateChanged(object sender, EventArgs e)
		{
			foreach (vxGUIControlBaseClass item in GUIItemList)
				item.IsEnabled = IsEnabled;
		}

        public override void Update()
        {
            base.Update();
			foreach (vxGUIControlBaseClass item in GUIItemList)
			{
				item.Position = Position + item.OriginalPosition;
				item.Update();
			}
        }

		/// <summary>
		/// Draw the specified Engine.
		/// </summary>
		public override void Draw()
        {
            base.Draw();


			Vector2 textSize = Font.MeasureString(Text);

			// Draw the Text
			Engine.SpriteBatch.DrawString (vxGUITheme.Fonts.Font, 
				Text, 
			                                 new Vector2 (
				                               Position.X,
											   Position.Y + Height / 2 - textSize.Y / 2),
				 Theme.Text.Color,
                                          0,
                                          Vector2.Zero,
                                          Math.Max(1, vxLayout.ScaleAvg),
                                          SpriteEffects.None,
                                          1);


            foreach (vxGUIControlBaseClass item in GUIItemList)
            {
                item.Draw();
            }
        }
    }
}