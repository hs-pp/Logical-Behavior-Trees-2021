using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logical.Editor
{
    /// <summary>
    /// Editor data saved related to the library tab.
    /// </summary>
    [System.Serializable]
    public class LibraryTabData
    {
        public List<string> FavoritesGUIDs = new List<string>();
        public List<string> RecentsGUIDs = new List<string>();
        public bool IsFavoritesFoldoutOpen = true;
        public bool IsRecentsFoldoutOpen = true;
        public string SearchQuery = "";
    }
}