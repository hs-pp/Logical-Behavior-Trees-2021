using Logical;
using System;
using UnityEngine;

[Serializable]
[SupportedGraphTypes(typeof(SampleNodeGraph))]
public class SampleNode : ANode
{
    int framecount = 0;
    public override void OnNodeEnter(GraphRunner graphRunner)
    {
        Debug.Log("Entered sample node dog");
    }

    public override void OnNodeUpdate(GraphRunner graphRunner)
    {
        framecount++;
        if(framecount >= 100)
        {
            graphRunner.TraverseEdge(GetOutportEdge(0));
        }
    }

    public override void OnNodeExit(GraphRunner graphRunner)
    {
        Debug.Log("Exited sample node cat");
    }
}
