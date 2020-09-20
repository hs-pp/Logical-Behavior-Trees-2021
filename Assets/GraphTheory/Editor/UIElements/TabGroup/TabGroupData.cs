using System.Collections.Generic;

namespace GraphTheory.Editor
{
    /// <summary>
    /// Serialized data for the tab group.
    /// </summary>
    [System.Serializable]
    public class TabGroupData
    {
        public int SelectedTab = 0;
        public List<string> TabData = new List<string>();
    }
}