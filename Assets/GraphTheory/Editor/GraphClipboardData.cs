using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphTheory.Editor
{
    [Serializable]
    public class GraphClipboardData
    {
        public string GraphTypeName = "";
        public string SerializedGraphElements = "";

        public GraphClipboardData(NodeGraph nodeGraph, IEnumerable<GraphElement> elements)
        {
            GraphTypeName = nodeGraph.GetType().ToString();
        }
    }
}