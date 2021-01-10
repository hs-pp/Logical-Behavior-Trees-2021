using Logical;
using System;
using UnityEngine;

[Serializable]
[Node(typeof(SampleNodeGraph))]
public class DebugLogNode : ANode
{
    [SerializeField]
    public string Message;
    public override void OnNodeEnter(GraphControls graphControls)
    {
        Debug.Log(Message);
        graphControls.TraverseEdge(0, this);
    }
}
