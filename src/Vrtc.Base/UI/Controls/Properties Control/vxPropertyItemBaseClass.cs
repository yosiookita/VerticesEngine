using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using VerticesEngine.Input;
using VerticesEngine.UI.Controls.Commands;
using VerticesEngine.UI.Themes;
using VerticesEngine.Utilities;

namespace VerticesEngine.UI.Controls
{
    /// <summary>
    /// Property Item Base Control Class
    /// </summary>
    public class vxPropertyItemBaseClass : vxGUIControlBaseClass
    {
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnValueChange();
            }
        }
        string _value = ""; 

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                IsEnabled = !_isReadOnly;
            }
        }
        private bool _isReadOnly = false;

        vxPropertyGroup propertyGroup;

        vxToggleImageButton ToggleButton;

        //vxPropertyControlTextbox PropertyBox;
		public vxGUIControlBaseClass PropertyBox;

        public List<vxGUIControlBaseClass> Items = new List<vxGUIControlBaseClass>();

        public bool IsExpanded
        {
            get { return ToggleButton.ToggleState; }
            set { ToggleButton.ToggleState = value; }
        }

        Rectangle Container = new Rectangle();

        Rectangle Splitter = new Rectangle();

        public PropertyInfo PropertyInfo;

        //public object TargetObject;

        /// <summary>
        /// The target objects.
        /// </summary>
        public List<object> TargetObjects = new List<object>();

        // The string to show if different objects have different values.
        public string VariesString = "* VARIES *";

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <returns>The property value.</returns>
        public virtual string GetPropertyValueAsString()
        {
			try
			{
                // Get the Initial Value.
                string PropValueDisplay = PropertyInfo.GetValue(TargetObjects[0]).ToString();

                foreach(var objt in TargetObjects)
                {
                    // Check if the Property Info exists
                    if (PropertyInfo.GetValue(objt) == null)
                        PropValueDisplay = "";
                    else
                    {
                        // Get the Object Property Value
                        string objectValue = PropertyInfo.GetValue(objt).ToString();

                        // If the Values differe, than return the 'varies' string.
                        if (objectValue != PropValueDisplay)
                            return VariesString;
                    }
                }
                return PropValueDisplay;
			}
			catch
			{
				return "";
			}
        }

        public enum PropertyResponse
        {
            NULL,
            Varies,
            Error
        }

        public virtual object GetPropertyValue()
        {
            try
            {
                // Get the Initial Value.
                object PropValueDisplay = PropertyInfo.GetValue(TargetObjects[0]);

                foreach (var objt in TargetObjects)
                {
                    // Check if the Property Info exists
                    if (PropertyInfo.GetValue(objt) == null)
                        return PropertyResponse.NULL;
                    else
                    {
                        // Get the Object Property Value
                        object objectValue = PropertyInfo.GetValue(objt);

                        // If the Values differe, than return the 'varies' string.
                        //if (objectValue != PropValueDisplay)
                        if(OnSelectionSetPropertyCompairson(objectValue, PropValueDisplay)==false)
                            return PropertyResponse.Varies;
                    }
                }
                return PropValueDisplay;
            }
            catch
            {
                return PropertyResponse.Error;
            }
        }

        string Title = "";
        string Description = "";

        protected virtual PropertyInfo GetPropertyInfoLate()
        {
            return null;
        }

        protected virtual List<object> GetTargetObjects()
        {
            return new List<object>();
        }


        public vxPropertyItemBaseClass(vxPropertyGroup propertyGroup, PropertyInfo PropertyInfo) :
        this(propertyGroup, PropertyInfo, null)
        { }

        public vxPropertyItemBaseClass(vxPropertyGroup propertyGroup, PropertyInfo PropertyInfo, List<object> TargetObjects) :
        base(propertyGroup.Engine)
        {
            if (PropertyInfo == null)
                PropertyInfo = GetPropertyInfoLate();

            if (TargetObjects == null)
                TargetObjects = GetTargetObjects();

            Padding = new Vector2(2);

            this.IsReadOnly = PropertyInfo.CanWrite;

            this.IsEnabled = PropertyInfo.CanWrite;

            this.propertyGroup = propertyGroup;

            this.PropertyInfo = PropertyInfo;
            this.TargetObjects = TargetObjects;

            // Set the Title and Description
            foreach (var attr in PropertyInfo.GetCustomAttributes(typeof(vxShowInInspectorAttribute)))
            {
                vxShowInInspectorAttribute attribute = (vxShowInInspectorAttribute)attr;
                Title = attribute.Label;
                Description = string.IsNullOrEmpty(attribute.Description) ? "" : attribute.Description;

                // this forces readonly
                if (attribute.IsReadOnly)
                    IsReadOnly = attribute.IsReadOnly;
            }
        

            //Set Text
            this.Text = Title != "" ? Title : PropertyInfo.Name;
            this.Value = GetPropertyValueAsString();

            if(Title != PropertyInfo.Name)
                Title = Title != "" ? Title + " - [" + PropertyInfo.Name + "]" : PropertyInfo.Name;
            
            Bounds = new Rectangle(0, 0, 64, 64);


            //Font = vxInternalAssets.Fonts.ViewerFont;
            Font = propertyGroup.Font;
            TextSize = Font.MeasureString(Text);

            Height = GetHeight();
            Width = 3000;

            Theme = new vxGUIControlTheme(
                new vxColourTheme(new Color(0.15f, 0.15f, 0.15f, 0.5f), Color.DarkOrange, Color.DeepSkyBlue),
                new vxColourTheme(Color.LightGray));

            ToggleButton = new vxToggleImageButton(Engine,
                                            Vector2.Zero,
                                                   vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/misc/bullet_arrow_right"),
                                                   vxInternalAssets.LoadInternalTexture2D("Textures/sandbox/misc/bullet_arrow_down"));
            
            ToggleButton.Clicked += delegate{
                propertyGroup.PropertyControl.ResetLayout();
            };
            ToggleButton.ToggleState = false;


            this.PositionChanged += delegate{
                ResetLayout();
            };

            Theme.Background = new vxColourTheme(
                Color.Transparent,
                Color.Gray * 0.25f,
                Color.DarkOrange);


            Theme.Text = new vxColourTheme(
                Color.White * 0.75f,
                Color.White,
                Color.Black);
			
            IsVisible = false;

			PropertyBox = CreatePropertyInputControl();
			PropertyBox.IsEnabled = IsEnabled;
        }




		public virtual vxGUIControlBaseClass CreatePropertyInputControl()
		{
			Value = Value == null ? "" : Value;
			return new vxPropertyControlTextbox(this, Value.ToString());
		}

        public virtual void OnValueChange()
        {
            if(PropertyBox != null)
                PropertyBox.Text = Value;
        }

        public virtual void RefreshValue()
        {
            this.Value = GetPropertyValueAsString();
        }

        /// <summary>
        /// Performs the selection set property compairson. Override this to apply property specific casts.
        /// </summary>
        /// <returns><c>true</c>, if selection set property compairson was oned, <c>false</c> otherwise.</returns>
        /// <param name="currentObject">Current object.</param>
        /// <param name="previousObject">Previous object.</param>
        public virtual bool OnSelectionSetPropertyCompairson(object currentObject, object previousObject)
        {
            return (currentObject == previousObject);
        }

        public virtual int GetHeight()
        {
            return vxLayout.GetScaledHeight(Math.Max(Font.LineSpacing * 16/13, 16));
        }

		public virtual void SetValue(object newValue)
		{
			try
            {
                // Apply the new value to all objects in the selection set.
                Engine.CurrentScene.CommandManager.Add(
                    new vxCMDPropertyValueChange(this, TargetObjects, newValue));
			}
			catch (Exception ex)
			{
				vxConsole.WriteLine("Error Setting Propterty");
				vxConsole.WriteLine("Propterty: "+PropertyInfo.Name);
				vxConsole.WriteLine("Namespace: " + PropertyInfo.PropertyType);
				vxConsole.WriteLine(ex.Message);
			}
		}

        Rectangle TopBounds = new Rectangle();

        Point Borders = new Point(16);
        public override void ResetLayout()
        {
            if (Items.Count > 0)
            {
                //GetHeight();
                Vector2 RunningLength = Position + Borders.ToVector2() + Vector2.UnitY * (GetHeight() - 16 + Padding.Y);
                Container.Location = Position.ToPoint() + Borders;

                    
                if (IsExpanded)
                {
                    foreach (var property in Items)
                    {
                        // Set position and width
                        property.Width = this.Width - Borders.X - 3;
                        property.Position = RunningLength;
                        RunningLength += Vector2.UnitY * (property.Height + 1);
                    }
                    Height = (int)(RunningLength.Y - Position.Y);//Math.Max(GetHeight(),(int)(RunningLength.Y - Position.Y));
                }
                else
                {
                    Height = GetHeight();
                }

                Splitter = new Rectangle(
                    (int)Position.X + Container.Width / 2,
                    (int)Position.Y + Height,
                    1, Container.Height);

                ToggleButton.Position = Position - new Vector2(Borders.X, 0);

                //ToggleButton.Update();
            }
            Container.Width = this.Width - Borders.X - 1;
            Container.Height = this.Height - Borders.X;

            //Vector2 PropertyPos = new Vector2((int)Position.X + Container.Width / 2, (int)Position.Y);
			PropertyBox.Position = Position + Padding + new Vector2(Width / 2 - 16, 0);
			PropertyBox.Width = (int)(Bounds.Right - PropertyBox.Position.X);

            TopBounds.Location = Bounds.Location;
            TopBounds.Width = this.Width;
            TopBounds.Height = GetHeight();
        }

        public bool IsHighlighted = false;

        public override void Update()
		{
			IsUpdateable = false;
            base.Update();


            foreach (var property in Items)
            {
                property.Update();

                if (property.HasFocus)
                    this.HasFocus = false;
            }

            if (Items.Count > 0)
                ToggleButton.Update();

            PropertyBox.Update();

            if(vxInput.IsNewMainInputDown())
            {
                IsHighlighted = HasFocus;

                if (HasFocus && IsVisible)
                {
                    propertyGroup.PropertyControl.TitleLabel.Text = this.Title;
                    propertyGroup.PropertyControl.DescriptionLabel.Text = this.Description;
                }
            }
            IsVisible = false;
        }



        public override void Draw()
        {
            //Now get the Art Provider to draw the scene
            IsVisible = true;
            base.Draw();


            //Rectangle splitter = new Rectangle(0, 0, 1, Container.Height);
            SpriteBatch.Draw(DefaultTexture, TopBounds, IsHighlighted ? Theme.Background.SelectedColour : GetStateColour(Theme.Background));
            
            // Draw this properties control. This can be a drop down, text box, image, etc...
            DrawPropertyControl();


            if (Items.Count > 0)
            {
                if (IsExpanded)
                {
                    foreach (var property in Items)
                    {
                        property.Draw();
                    }
                }

                ToggleButton.Draw();
            }
        }

		public override void DrawText()
		{
			base.DrawText();

            Vector2 textPos = Position + new Vector2(Padding.X, GetHeight() / 2 - vxLayout.GetScaledHeight(Font.MeasureString(Text).Y / 2+2));
            SpriteBatch.DrawString(Font, Text, textPos.ToIntValue(), IsHighlighted ? Theme.Text.SelectedColour : GetStateColour(Theme.Text), vxLayout.Scale);
			DrawPropertyControlText();

			if (Items.Count > 0)
				if (IsExpanded)
					foreach (var property in Items)
						property.DrawText();
		}

        public virtual void DrawPropertyControl()
        {
            PropertyBox.Draw();
        }

        public virtual void DrawPropertyControlText()
		{
			PropertyBox.DrawText();
		}


        public override void DrawBorder()
        {
            // Draw Left Border
            propertyGroup.PropertyControl.LineBatch.DrawLine(
                Position + new Vector2(0,-1), Position + new Vector2(Width, -1), Color.Black * 0.35f);

        }
    }
}