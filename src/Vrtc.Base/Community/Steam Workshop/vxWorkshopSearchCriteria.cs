using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VerticesEngine.UI.Dialogs;
using VerticesEngine;

namespace VerticesEngine.Community
{
    /// <summary>
    /// Steam Workshop item search criteria.
    /// </summary>
    public enum vxWorkshopItemSearchCriteria
    {
        AllPublished,
        MyPublished,
        Followed,
        Favourited,
        Subscribed
    }

    /// <summary>
    /// Workshop search criteria.
    /// </summary>
    public class vxWorkshopSearchCriteria
    {
        /// <summary>
        /// The item criteria.
        /// </summary>
        public vxWorkshopItemSearchCriteria ItemCriteria;



        /// <summary>
        /// The search text.
        /// </summary>
        public string SearchText;


        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>The results.</value>
        public List<vxIWorkshopItem> Results
        {
            get { return _results; }
        }
        List<vxIWorkshopItem> _results = new List<vxIWorkshopItem>();


        /// <summary>
        /// The tags to include.
        /// </summary>
        public string[] TagsToInclude = { };

        /// <summary>
        /// The tags to exclude.
        /// </summary>
        public string[] TagsToExclude = { };


        internal void setResults(List<vxIWorkshopItem> results)
        {
            _results.Clear();
            _results.AddRange(results);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:VerticesEngine.Community.vxWorkshopSearchCriteria"/> class.
        /// </summary>
        /// <param name="SearchText">Search text.</param>
        /// <param name="ItemCriteria">Item criteria.</param>
        /// <param name="TagsToInclude">Tags to include.</param>
        /// <param name="TagsToExclude">Tags to exclude.</param>
        public vxWorkshopSearchCriteria(string SearchText, vxWorkshopItemSearchCriteria ItemCriteria,
            string[] TagsToInclude, string[] TagsToExclude)
        {
            Results.Clear();
            this.SearchText = SearchText;
            this.ItemCriteria = ItemCriteria;
            this.TagsToInclude = TagsToInclude;
            this.TagsToExclude = TagsToExclude;
        }
    }
}
