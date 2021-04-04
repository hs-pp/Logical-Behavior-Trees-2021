using Logical;
using System;
using UnityEngine;

[Serializable]
[Node(typeof(SampleNodeGraph))]
public class SampleNode : ANode
{
    [SerializeField]
    private string SampleString = "HeyImASampleString";
    [SerializeField]
    private int SampleInt = 999;

    public override void OnNodeEnter(GraphControls graphControls)
    {
        Debug.Log($"Entered sample node. SampleString is {SampleString} and SampleInt is {SampleInt}");
    }

    public override void OnNodeUpdate(GraphControls graphControls)
    {
        graphControls.TraverseEdge(0, this);
    }

    public override void OnNodeExit(GraphControls graphControls)
    {
        Debug.Log("Exited sample node.");
    }

#if UNITY_EDITOR
    public override int DefaultNumOutports => base.DefaultNumOutports; // Override this in nodes that require more than one out port.
    public override bool UseIMGUIPropertyDrawer { get { return true; } } // Toggle on for LogicalGraphWindow to draw the property drawers using IMGUI. Otherwise, it uses UIToolkit property drawers
#endif
}
