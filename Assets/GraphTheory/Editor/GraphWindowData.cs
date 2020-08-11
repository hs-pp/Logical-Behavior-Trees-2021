using GraphTheory.Editor.UIElements;

namespace GraphTheory.Editor
{
    [System.Serializable]
    public class GraphWindowData
    {
        public float MainSplitViewPosition = 200;

        public TabGroupData MainTabGroup = new TabGroupData();

        public string OpenGraphGUID = "";
        public string GraphBreadcrumbPath = "";
    }
}