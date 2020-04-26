using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VerticesEngine;
using VerticesEngine.UI.Themes;
using VerticesEngine.Mathematics;

namespace VerticesEngine.UI.Controls
{
    public enum vxCheckBoxState
    {
        Checked,
        UnChecked,
        Mixed
    }
	/// <summary>
	/// Label Class providing simple one line text as a vxGUI Item.
	/// </summary>
    public class vxPropertyControlCheckBox : vxLabel
    {
        public vxCheckBoxState CheckBoxState
        {
            get { return _checkBoxState; }
            set { 
                _checkBoxState = value;

                // Set the Toggle State based off of it's check status
                CheckBox.ToggleState = (_checkBoxState == vxCheckBoxState.Checked);

                switch(_checkBoxState)
                {
                    case vxCheckBoxState.Checked:
                        CheckBox.OnButtonImage = CheckedTexture;
                        break;
                    case vxCheckBoxState.Mixed:
                        CheckBox.OffButtonImage = MixedTexture;
                        break;
                    case vxCheckBoxState.UnChecked:
                        CheckBox.OffButtonImage = UncheckedTexture;
                        break;
                }
            }
        }
        vxCheckBoxState _checkBoxState = vxCheckBoxState.UnChecked;

        public vxToggleImageButton CheckBox;

        private Texture2D UncheckedTexture;
        private Texture2D CheckedTexture;
        private Texture2D MixedTexture;

        vxPropertyItemBaseClass PropertyControl;

        

		/// <summary>
		/// Initializes a new instance of the <see cref="VerticesEngine.UI.Controls.vxLabel"/> class.
		/// </summary>
		/// <param name="Engine">The Vertices Engine Reference.</param>
		/// <param name="text">This GUI Items Text.</param>
		/// <param name="position">This Items Start Position. Note that the 'OriginalPosition' variable will be set to this value as well.</param>
        public vxPropertyControlCheckBox(vxPropertyItemBaseClass Property, string text, vxCheckBoxState CheckBoxState) : 
        base(Property.Engine, text, Vector2.Zero)
        {
            this.PropertyControl = Property;

            this.Font = Property.Font;
            Width = (int)this.Font.MeasureString(Text).X;
            
            UncheckedTexture = vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/misc/check_box_uncheck");
            MixedTexture = vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/misc/check_box_mixed");
            CheckedTexture = vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/misc/check_box");

            CheckBox = new vxToggleImageButton(Engine, Vector2.Zero, UncheckedTexture, CheckedTexture);

            //CheckBox.ToggleState = (CheckBoxState == vxCheckBoxState.Checked);
            CheckBox.Theme = new vxGUIControlTheme(
                new vxColourTheme(Color.White, Color.White, Color.White));

            this.CheckBoxState = CheckBoxState;

        }

        protected override void OnEnableStateChanged()
        {
            base.OnEnableStateChanged();

            if(CheckBox != null)
                CheckBox.IsEnabled = this.IsEnabled;
        }

        public override void Update()
        {
            base.Update();

            CheckBox.Position = Position - new Vector2(0,2);
            CheckBox.Update();
        }

		/// <summary>
		/// Draws the GUI Item
		/// </summary>
		public override void Draw()
        {
            Width = (int)Font.MeasureString(Text).X;

            CheckBox.Draw();
		}

		public override void DrawText()
		{
            string text = "False";
            switch(CheckBoxState)
            {
                case vxCheckBoxState.Checked:
                    text = "True";
                    break;
                case vxCheckBoxState.Mixed:
                    text = PropertyControl.VariesString;
                    break;
            }
            SpriteBatch.DrawString(Font, text, Position + new Vector2(20, 0), GetStateColour(Theme.Text), vxLayout.Scale);
        }
    }

    public class vxPropertyItemBool : vxPropertyItemBaseClass
    {
        vxPropertyControlCheckBox CheckBoxControl;

        public vxPropertyItemBool(vxPropertyGroup propertyGroup, PropertyInfo PropertyInfo, List<object> TargetObjects) :
        base(propertyGroup, PropertyInfo, TargetObjects)
        {
            
        }

        public override bool OnSelectionSetPropertyCompairson(object currentObject, object previousObject)
        {
            return ((bool)currentObject == (bool)previousObject);
        }

        vxCheckBoxState GetCheckboxState()
        {
            object getValue = GetPropertyValue();
            //bool InitValue = false;
            vxCheckBoxState CheckBoxState = vxCheckBoxState.UnChecked;

            if (getValue is bool)
                CheckBoxState = (bool)getValue ? vxCheckBoxState.Checked : vxCheckBoxState.UnChecked;
            else if (getValue is PropertyResponse)
                CheckBoxState = vxCheckBoxState.Mixed;

            return CheckBoxState;
        }

		public override vxGUIControlBaseClass CreatePropertyInputControl()
		{
            CheckBoxControl = new vxPropertyControlCheckBox(this, Text, GetCheckboxState());
			CheckBoxControl.CheckBox.Clicked += delegate {
				
                if(CheckBoxControl.CheckBoxState != vxCheckBoxState.Checked)
                    CheckBoxControl.CheckBoxState = vxCheckBoxState.Checked;
                else
                    CheckBoxControl.CheckBoxState = vxCheckBoxState.UnChecked;

                SetValue(CheckBoxControl.CheckBoxState == vxCheckBoxState.Checked);
			};

			return CheckBoxControl;
        }

        public override void RefreshValue()
        {
            CheckBoxControl.CheckBoxState = GetCheckboxState();
            
            this.Value = GetPropertyValueAsString();

        }
    }
}
