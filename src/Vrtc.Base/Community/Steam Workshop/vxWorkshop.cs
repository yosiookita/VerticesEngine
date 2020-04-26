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
    /// Steam Workshop.
    /// </summary>
    public static class vxWorkshop 
    {
        public static vxWorkshopSearchCriteria PreviousSearch
        {
            get { return _previousSearch; }   
        }
        static vxWorkshopSearchCriteria _previousSearch;

        internal static void OnSearch(vxWorkshopSearchCriteria search)
        {
            _previousSearch = search;
        }

        internal static void OnSearchResultsReceived(List<vxIWorkshopItem> results)
        {
            _previousSearch.setResults(results);
        }
    }
}
