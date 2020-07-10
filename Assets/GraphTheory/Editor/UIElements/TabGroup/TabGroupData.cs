using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTheory.Editor.UIElements
{
    /// <summary>
    /// Serialized data for the tab group.
    /// </summary>
    [System.Serializable]
    public class TabGroupData
    {
        public int SelectedTab = -1;
        public List<string> TabData = new List<string>();
    }
}