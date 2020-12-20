using System.Collections.Generic;

namespace Logical.Editor
{
    /// <summary>
    /// Basic editor window data that need to be serialized and saved.
    /// </summary>
    [System.Serializable]
    public class GraphWindowData
    {
        public float MainSplitViewPosition = 200;

        public TabGroupData MainTabGroup = new TabGroupData();

        public string OpenGraphGUID = "";
        public List<string> SelectedGraphElements = new List<string>();
    }
}