using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTheory.Editor
{
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