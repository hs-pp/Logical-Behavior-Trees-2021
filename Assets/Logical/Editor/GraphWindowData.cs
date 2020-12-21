using System.Collections.Generic;
using UnityEngine;

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
        public Vector2 GraphViewPosition;
        public bool ShowMinimap = true;
        public List<string> SelectedGraphElements = new List<string>();
    }
}