using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.Utilities;
using VerticesEngine;
using VerticesEngine.UI.Dialogs;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
    /// <summary>
    /// Toolbar control that holds <see cref="VerticesEngine.UI.Controls.vxScrollPanelComboxBoxItem"/> 
    /// </summary>
	public class vxSettingsGUIItem : vxScrollPanelItem
    {
        
		/// <summary>
		/// Gets the selected index of the Combox Box.
		/// </summary>
		/// <value>The index of the selected.</value>
		public int SelectedIndex
		{
			get { return _selectedIndex; }
            set {
                _selectedIndex = value;
                SetOption();
            }
		}
        int _selectedIndex = 0;


        vxButtonImageControl Btn_Add;

        vxButtonImageControl Btn_Remove;

        /// <summary>
        /// Returns True if the current SelectedIndex is equal to the first Option
        /// </summary>
        /// <returns></returns>
        public bool ToBool()
        {
            return (SelectedIndex == 0);
        }


        public int ToEnum()
        {
            return (SelectedIndex);
        }



        /// <summary>
        /// Gets the current Value of this setting
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        string _value = "";
        
        Vector2 valuePos;

        Vector2 TitlePos;

        /// <summary>
        /// The arrow spacing which is set externally so that all items have the same width
        /// </summary>
        public static int ArrowSpacing = 100;



        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxSettingsComboBoxGUIItem"/> class.
        /// </summary>
        /// <param name="Engine">Engine.</param>
        /// <param name="UIManager">GUIM anager.</param>
        /// <param name="Title">Title.</param>
        /// <param name="Value">Value.</param>
        public vxSettingsGUIItem(vxEngine Engine, vxGUIManager UIManager, string Name, string InitValue, bool wrapInc = true) :
            base(Engine, Name, Vector2.Zero, null, 0)
        {
            _value = InitValue;
            Height = vxLayout.GetScaledSize(64);

            //Set the Game Engine.AddNewItem
            this.Engine = Engine;


            //Set GUI Stuff.
            int ofst = 8;
            Btn_Add = new vxButtonImageControl(Engine,
                Vector2.Zero, vxGUITheme.SpriteSheetLoc.ArrowBtnFwd)
            {
                Width = Height - ofst * 2,
                Height = Height - ofst * 2
            };

            Btn_Add.Clicked += delegate
            {
                if (wrapInc)
                    _selectedIndex = (_selectedIndex + 1) % Options.Count;
                else
                    _selectedIndex = MathHelper.Clamp(_selectedIndex + 1, 0, Options.Count-1);

                SetOption();

                if (ValueChangedEvent != null)
                    ValueChangedEvent(this, new EventArgs());
            };
            //GUIItemList.Add (Btn_Add);

            Btn_Remove = new vxButtonImageControl(Engine,
                Vector2.Zero,
                vxGUITheme.SpriteSheetLoc.ArrowBtnBack)
            {
                Width = Height - ofst * 2,
                Height = Height - ofst * 2
            };

            Btn_Remove.Clicked += delegate
            {
                if (wrapInc)
                {
                    _selectedIndex = (_selectedIndex - 1) % Options.Count;

                    if (_selectedIndex < 0)
                        _selectedIndex = Options.Count - 1;
                }
                else
                {
                    _selectedIndex = MathHelper.Clamp(_selectedIndex - 1, 0, Options.Count-1);
                }

                SetOption();

                if (ValueChangedEvent != null)
                    ValueChangedEvent(this, new EventArgs());
            };

            // Set the Padding and Sizes
            this.Font = vxGUITheme.Fonts.Font;
            Bounds = new Rectangle(0, 0, (int)(vxLayout.Scale.X * 64), Height);
            Padding = new Vector2((int)(vxLayout.Scale.X * 16), (Bounds.Height / 2 - Btn_Add.Bounds.Height / 2));


            Theme.Background.HoverColour = Color.DarkOrange;
        }

        void SetOption()
        {
            if (_selectedIndex >= 0 && _selectedIndex < Options.Count)
                _value = Options[_selectedIndex].ToString();
            else
                _value = "Invalid Option";
        }

        public EventHandler<EventArgs> ValueChangedEvent;

        List<object> Options = new List<object>();


        public void AddOption(object item)
        {
            Options.Add(item);

            // now check if the arrow width needs to be increased
            ArrowSpacing = Math.Max(ArrowSpacing, (int)(Font.MeasureString(item.ToString()).X * vxLayout.Scale.X + Padding.X * 2));

            if(item.ToString() == Value)
            {
                _selectedIndex = Options.Count - 1;
            }
        }


		public override void Update()
        {
            base.Update();

            // Left Justified
            TitlePos = new Vector2(Bounds.Left + Padding.X, Position.Y + Height / 2);

            // cenetered
            valuePos = new Vector2(Bounds.Right - Padding.X - Btn_Add.Width - ArrowSpacing / 2, Position.Y + Height / 2);


            // Set the Position walking left from the right side
            Btn_Add.Position = new Vector2(Bounds.Right - Padding.X - Btn_Add.Width, Position.Y + Padding.Y);
            Btn_Add.Update();


            Btn_Remove.Position = new Vector2(Bounds.Right - Padding.X - Btn_Add.Width - ArrowSpacing - Btn_Remove.Width, Position.Y + Padding.Y);
            Btn_Remove.Update();
        }


        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public override void Draw()
        {
            //base.Draw();
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds.GetBorder(1), Color.Black);
            Engine.SpriteBatch.Draw(DefaultTexture, Bounds, Theme.Background.Color);

            //Label.Draw();


            var titleFont = vxGUITheme.Fonts.Font;

            SpriteBatch.DrawString(titleFont, Text, TitlePos + Vector2.One * 2, (HasFocus ? Color.Black * 0.75f : Color.Gray * 0.5f), vxLayout.ScaleAvg, vxHorizontalJustification.Left, vxVerticalJustification.Middle);
            SpriteBatch.DrawString(titleFont, Text, TitlePos, Color.White, vxLayout.ScaleAvg, vxHorizontalJustification.Left, vxVerticalJustification.Middle);

            SpriteBatch.DrawString(titleFont, Value, valuePos + Vector2.One * 2, (HasFocus ? Color.Black * 0.75f : Color.Gray * 0.5f), vxLayout.ScaleAvg, vxHorizontalJustification.Center, vxVerticalJustification.Middle);
            SpriteBatch.DrawString(titleFont, Value, valuePos, Color.White, vxLayout.ScaleAvg, vxHorizontalJustification.Center, vxVerticalJustification.Middle);

            if (true)
            {
                Btn_Add.Draw();
                Btn_Remove.Draw();
            }
        }
    }
}
