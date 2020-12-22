using Logical;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[SupportedGraphTypes(typeof(SampleNodeGraph))]
public class DebugLogNode : ANode
{
    [SerializeField]
    public string Message;
    public override void OnNodeEnter(GraphRunner graphRunner)
    {
        Debug.Log(Message);
        TraverseEdge(graphRunner, 0);
    }
}
