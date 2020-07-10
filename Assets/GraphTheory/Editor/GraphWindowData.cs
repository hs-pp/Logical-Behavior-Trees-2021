using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory.Editor.UIElements;

namespace GraphTheory.Editor
{
    [System.Serializable]
    public class GraphWindowData
    {
        public Vector2 WindowDimensions = new Vector2(600, 400);
        public float MainSplitViewPosition = 200;
        public TabGroupData MainTabGroup = new TabGroupData();
    }
}