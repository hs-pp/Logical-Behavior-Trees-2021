using GraphTheory.Editor.UIElements;
using System.Collections.Generic;

namespace GraphTheory.Editor
{
    [System.Serializable]
    public class GraphWindowData
    {
        public float MainSplitViewPosition = 200;

        public TabGroupData MainTabGroup = new TabGroupData();

        public string OpenGraphGUID = "";
        public List<string> SelectedGraphElements = new List<string>();
    }
}