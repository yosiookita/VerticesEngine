using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using VerticesEngine.Graphics;
using VerticesEngine.UI.Themes;

namespace VerticesEngine.UI.Controls
{


    class vxAttributeCategory
    {
        public string name;
        public List<vxPropertyAttributePair> attributes = new List<vxPropertyAttributePair>();

        public vxAttributeCategory(object name)
        {
            this.name = name.ToString().SplitIntoSentance();
        }
    }


    class vxPropertyAttributePair
    {
        public PropertyInfo propertyInfo;
        public vxShowInInspectorAttribute attribute;
        public vxPropertyAttributePair(PropertyInfo propertyInfo, vxShowInInspectorAttribute attribute)
        {
            this.propertyInfo = propertyInfo;
            this.attribute = attribute;
        }
    }


    /// <summary>
    /// Vx properties control.
    /// </summary>
    public class vxPropertiesControl : vxPanel
    {
        vxPropertiesScrollPanelControl ScrollPanel;

        /// <summary>
        /// The property type to extract.
        /// </summary>
        public Type PropertyTypeToExtract;

        public vxLabel SelectionName;

        public vxLabel TitleLabel;

        public vxLabel DescriptionLabel;

        public vxLineBatch LineBatch
        {
            get { return ScrollPanel.LineBatch; }
        }

        //vxLabel DescriptionName;


        enum ExpandedStateUsage
        {
            Save,
            Load
        }

        private vxImage IconImage;

        private Dictionary<string, bool> ExpandedStateDictionary = new Dictionary<string, bool>();

        private SortedDictionary<string, vxAttributeCategory> categories = new SortedDictionary<string, vxAttributeCategory>();

        private float SavedScrollbarPos = 0;

        private int FooterSize = 75;

        private int iconSize = 72;

        private List<object> SelectionSet = new List<object>();

        public vxPropertiesControl(vxEngine Engine, Vector2 Position, int Width, int Height, SpriteFont font = null)
            : base(Engine, Position, Width, Height)
        {
            // First Setup Font
            if (font == null)
                font = vxInternalAssets.Fonts.ViewerFont;
            this.Font = font;

            // Then the Footer Size
            FooterSize = Font.LineSpacing * 4;

            int iconSize = vxLayout.GetScaledSize(72);

            // Now The Panel Height
            int panelHeight = Height - (FooterSize + iconSize + 16);// Height;// - FooterSize;


            IconImage = new vxImage(Engine, DefaultTexture, Vector2.Zero, iconSize, iconSize);
            Items.Add(IconImage);

            SelectionName = new vxLabel(Engine, "Title", new Vector2(IconImage.Width + font.LineSpacing * 0.25f, font.LineSpacing * 0.25f).ToIntValue());
            SelectionName.Font = font;
            Items.Add(SelectionName);


            ScrollPanel = new vxPropertiesScrollPanelControl(Engine, Vector2.UnitY * (iconSize + 16), Width, panelHeight);
            Items.Add(ScrollPanel);


            //TitleLabel = new vxLabel(Engine, "Title", new Vector2(5, panelHeight + font.LineSpacing * 0.25f).ToIntValue());
            TitleLabel = new vxLabel(Engine, "Title", new Vector2(5, ScrollPanel.Bounds.Bottom + font.LineSpacing * 0.75f).ToIntValue());
            TitleLabel.Font = font;
            Items.Add(TitleLabel);

            DescriptionLabel = new vxLabel(Engine, "Description", new Vector2(5, ScrollPanel.Bounds.Bottom + font.LineSpacing * 2).ToIntValue());
            DescriptionLabel.Font = font;
            DescriptionLabel.Theme.Text =new vxColourTheme(Color.White * 0.65f);
            Items.Add(DescriptionLabel);

            DrawBackground = false;
        }


        public void GetPropertiesFromSelectionSet(List<vxEntity> entitySelectionSet)
        {
            if (entitySelectionSet.Count() > 0)
            {
                PropertyTypeToExtract = entitySelectionSet[0].GetType();

                SelectionName.Text = entitySelectionSet[0].GetTitle();
                IconImage.Texture = entitySelectionSet[0].GetIcon(iconSize, iconSize);
            }

            // Clear Current Selection Set
            SelectionSet.Clear();

            // Add and Process Selected Data List
            foreach(var entity in entitySelectionSet)
                SelectionSet.Add(entity);
            
            // Now loop through all items
            if(entitySelectionSet.Count() > 0)
                entitySelectionSet[0].GetProperties(this);

            // Now refresh the expansion state
            LoopThroughCollection(ExpandedStateUsage.Load);
        }

        public void AddItem(vxGUIControlBaseClass item)
        {
            ScrollPanel.AddItem(item);
        }


        public override void ResetLayout()
        {
           ScrollPanel.ResetLayout();
        }

        void SavePropertyItemState(string preString, vxPropertyItemBaseClass item, ExpandedStateUsage usage)
        {
            string txt = preString + "." + item.Text.Replace(" ", string.Empty);
           // Console.WriteLine(txt  + "    Is Expanded: " + item.IsExpanded);

            if (usage == ExpandedStateUsage.Save)
            {
                SavedScrollbarPos = ScrollPanel.ScrollBar.TravelPosition;
                // Does the Dictionary Contain the Key, if so, save it's expanded state
                if (ExpandedStateDictionary.ContainsKey(txt))
                    ExpandedStateDictionary[txt] = item.IsExpanded;
                // If the key is not in the dictionary, add it, and save the state
                else
                    ExpandedStateDictionary.Add(txt, item.IsExpanded);
            }
            else if(usage == ExpandedStateUsage.Load)
            {
                ScrollPanel.ScrollBar.TravelPosition = SavedScrollbarPos;
                if (ExpandedStateDictionary.ContainsKey(txt))
                    item.IsExpanded = ExpandedStateDictionary[txt];
            }

            // Now recursively loop through
            foreach(var itm in item.Items)
            {
                if (itm is vxPropertyItemBaseClass)
                {
                    SavePropertyItemState(txt, (vxPropertyItemBaseClass)itm, usage);
                }
            }
        }

        void LoopThroughCollection(ExpandedStateUsage usage)
        {
            foreach (var grp in ScrollPanel.Items)
            {
                if (grp is vxPropertyGroup)
                {
                    vxPropertyGroup pg = grp as vxPropertyGroup;
                    foreach (var item in pg.Items)
                        if (item is vxPropertyItemBaseClass)
                            SavePropertyItemState(pg.Text.Replace(" ", string.Empty), item, usage);
                }
            }
        }

        public void Clear()
        {
            LoopThroughCollection(ExpandedStateUsage.Save);
            ScrollPanel.Clear();
        }



        /// <summary>
        /// Adds a properties group with all properties which have the specified attributes.
        /// </summary>
        /// <param name="pc">Pc.</param>
        /// <param name="GroupName">Group name.</param>
        /// <param name="AttributeType">Attribute type.</param>
        public void AddPropertiesFromType(Type EntityType)
        {
            List<PropertyInfo> propertiesInfo = EntityType.GetProperties().Where(
                p => p.GetCustomAttributes(typeof(vxShowInInspectorAttribute), true).Any()).ToList();

            categories.Clear();

            foreach (var property in propertiesInfo)
            {
                var attribute = property.GetCustomAttribute<vxShowInInspectorAttribute>();

                // first check if this category exists. if not, create it
                if (categories.ContainsKey(attribute.Category.ToString()) == false)
                    categories.Add(attribute.Category.ToString(), new vxAttributeCategory(attribute.Category));

                categories[attribute.Category.ToString()].attributes.Add(new vxPropertyAttributePair(property, attribute));
            }



            // now add the appropriate controls
            foreach (var category in categories.Values)
            {
                // create a new Category
                var propertiesGroup = new vxPropertyGroup(this, category.name);

                if (propertiesInfo.Count > 0)
                    this.AddItem(propertiesGroup);

                foreach (var property in category.attributes)
                {
                    propertiesGroup.Add(AddPropertyControl(propertiesGroup, property.propertyInfo, SelectionSet));
                }
            }
        }


        /// <summary>
        /// Adds the property to the property control.
        /// </summary>
        /// <returns>The property control.</returns>
        /// <param name="pg">Pg.</param>
        /// <param name="property">Property.</param>
        public vxPropertyItemBaseClass AddPropertyControl(vxPropertyGroup pg, PropertyInfo property, List<object> SelectionSet)
        {
            Type type = property.PropertyType;

            if (type == typeof(bool))
                return new vxPropertyItemBool(pg, property, SelectionSet);

			if (type == typeof(float))
                return new vxPropertyItemFloat(pg, property, SelectionSet);
            
			////if (type == typeof(float[]))
            //    //return new vxPropertyItemFloatArray(pg, property, TargetObject);
            
            if (type.IsEnum)
                return new vxPropertyItemChoices(pg, property, SelectionSet);

            if (type == typeof(Vector2))
                return new vxPropertyItemVector2(pg, property, SelectionSet);
            
            if (type == typeof(Vector3))
                return new vxPropertyItemVector3(pg, property, SelectionSet);

            //if (type == typeof(BoundingBox))
            //    return new vxPropertyItemBoundingBox(pg, property, TargetObject);

            //if (type == typeof(BoundingSphere))
            //    return new vxPropertyItemBoundingSphere(pg, property, TargetObject);

            if (type == typeof(Color))
                return new vxPropertyItemColour(pg, property, SelectionSet);


            if (type == typeof(vxModel))
                return new vxPropertyItemModel(pg, property, SelectionSet);
            

            if (type == typeof(Texture2D))
				return new vxPropertyItemTexture2D(pg, property, SelectionSet);

            return new vxPropertyItemBaseClass(pg, property, SelectionSet);
        }
    }
}
