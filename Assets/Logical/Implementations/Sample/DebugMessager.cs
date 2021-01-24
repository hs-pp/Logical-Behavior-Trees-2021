using Logical;
using System;
using UnityEngine;

[Serializable]
[Node(typeof(TemplateNodeGraph))]
public class DebugMessager : ANode
{
    public string message;
    public override void OnNodeEnter(GraphControls graphControls)
    {
        Debug.Log(message);
        graphControls.TraverseEdge(0, this);
    }
    public override void OnNodeUpdate(GraphControls graphControls)
    {
    }
    public override void OnNodeExit(GraphControls graphControls)
    {
    }

#if UNITY_EDITOR
    public override int DefaultNumOutports { get { return 1; } }
    public override bool UseIMGUIPropertyDrawer { get { return false; } }
#endif

}
