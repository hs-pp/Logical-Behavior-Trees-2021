using Logical.Editor.UIElements;
using System.Collections.Generic;

namespace Logical.Editor
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