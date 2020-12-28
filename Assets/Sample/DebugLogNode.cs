using Logical;
using System;
using UnityEngine;

[Serializable]
[SupportedGraphTypes(typeof(SampleNodeGraph))]
public class DebugLogNode : ANode
{
    [SerializeField]
    public string Message;
    public override void OnNodeEnter(GraphControls graphControls)
    {
        Debug.Log(Message);
        graphControls.TraverseEdge(this, 0);
    }
}
