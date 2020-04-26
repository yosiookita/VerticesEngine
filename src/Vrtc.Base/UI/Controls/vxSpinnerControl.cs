﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using VerticesEngine.UI.Events;
using VerticesEngine.Mathematics;
using VerticesEngine.Utilities;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{
    /// <summary>
    /// Spinner Control which allows value incrementing
    /// </summary>
    public class vxSpinnerControl : vxGUIControlBaseClass
    {
        /// <summary>
        /// The value.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { _value = value;

                if (ValueChanged != null)
                    ValueChanged(this, new vxValueChangedEventArgs(this, Value, PreviousValue));
                PreviousValue = Value;
            }
        }
        private int _value;


        /// <summary>
        /// Occurs when the value changes.
        /// </summary>
        public event EventHandler<vxValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// The Format String
        /// </summary>
        public string formatString = "#,###";

        /// <summary>
        /// The previous value.
        /// </summary>
        private int PreviousValue;

        private int size;

        private int buttonGap;

        private int tick;

        private vxButtonImageControl IncrementButton;

        private vxButtonImageControl DecrementButton;

        private List<vxGUIControlBaseClass> GUIItemList = new List<vxGUIControlBaseClass>();

        vxSpinnerControlArtProvider ArtProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.UI.Controls.vxSpinner"/> class.
        /// </summary>
        /// <param name="Engine">The Vertices Engine Reference.</param>
        /// <param name="Value">Value.</param>
        /// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        /// <param name="ButtonSize">Size.</param>
        /// <param name="BackArrow">Back arrow.</param>
        /// <param name="ForwardArrow">Forward arrow.</param>
        public vxSpinnerControl(vxEngine Engine, int Value, Vector2 position, int ButtonSize, int ButtonGap, int Tick,
                        Rectangle BackArrow = new Rectangle(), Rectangle ForwardArrow = new Rectangle()) : base(Engine, position)
        {
            this.Engine = Engine;

            this.Value = Value;

            this.size = ButtonSize;

            this.buttonGap = ButtonGap;

            this.tick = Tick;


            IncrementButton = new vxButtonImageControl(Engine,
                                        new Vector2(ButtonSize + ButtonGap, 0),
                ForwardArrow,
                ForwardArrow)
            {
                //DrawHoverBackground = false,
                Width = ButtonSize,
                Height = ButtonSize
            };

            IncrementButton.Clicked += ValueIncrease;

            GUIItemList.Add(IncrementButton);

            DecrementButton = new vxButtonImageControl(Engine, new Vector2(0, 0), BackArrow, BackArrow)
            {
                //DrawHoverBackground = false,
                Width = ButtonSize,
                Height = ButtonSize
            };

            DecrementButton.Clicked += ValueDecrease;

            GUIItemList.Add(DecrementButton);

            this.Font = vxGUITheme.Fonts.Font;

            foreach (vxGUIControlBaseClass item in GUIItemList)
            {
                item.Font = this.Font;
            }

            //Have this button get a clone of the current Art Provider
            ArtProvider = (vxSpinnerControlArtProvider)vxGUITheme.ArtProviderForSpinners.Clone();
        }

        public override void OnShadowStatusChange()
        {
            IncrementButton.IsShadowVisible = IsShadowVisible;
            DecrementButton.IsShadowVisible = IsShadowVisible;

            foreach (vxGUIControlBaseClass item in GUIItemList)
                item.IsShadowVisible = this.IsShadowVisible;
        }

        protected override void OnEnableStateChanged()
        {
            base.OnEnableStateChanged();

            foreach (vxGUIControlBaseClass item in GUIItemList)
                item.IsEnabled = IsEnabled;
        }

        protected virtual void ValueIncrease(object sender, vxGuiItemClickEventArgs e)
        {
            Value += tick;
        }

        protected virtual void ValueDecrease(object sender, vxGuiItemClickEventArgs e)
        {
            Value -= tick;
        }

        public override void Update()
        {
            base.Update();
            foreach (vxGUIControlBaseClass item in GUIItemList)
            {
                item.Position = this.Position + item.OriginalPosition;
                item.Update();
            }
        }


        /// <summary>
        /// Draws the GUI Item
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            Vector2 FontSize = Font.MeasureString(Value.ToString(formatString));

            // Get the center point
            Bounds = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (IncrementButton.Bounds.Right - DecrementButton.Bounds.Left),
                IncrementButton.Bounds.Height);

            Vector2 TextPos = Bounds.Center.ToVector2();


            if (IsShadowVisible)
                Engine.SpriteBatch.DrawString(Font,
                                              Value.ToString(formatString),
                                              TextPos + ShadowOffset,
                                              ShadowColour * ShadowTransparency,
                                              0, 
                                              FontSize / 2, 
                                              vxLayout.Scale, SpriteEffects.None, 1);

            Engine.SpriteBatch.DrawString(Font,
                                          Value.ToString(formatString),
                                          TextPos,
                                          Theme.Text.Color, 
                                          0, 
                                          FontSize / 2, 
                                          vxLayout.Scale, SpriteEffects.None, 1);


            foreach (vxGUIControlBaseClass item in GUIItemList)
                item.Draw();
        }
    }
}
