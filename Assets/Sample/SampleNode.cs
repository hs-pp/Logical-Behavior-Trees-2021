using Logical;
using System;
using UnityEngine;

[Serializable]
[Node(typeof(SampleNodeGraph))]
public class SampleNode : ANode
{
    int framecount = 0;
    public override void OnNodeEnter(GraphControls graphControls)
    {
        Debug.Log("Entered sample node dog");
    }

    public override void OnNodeUpdate(GraphControls graphControls)
    {
        framecount++;
        if(framecount >= 100)
        {
            graphControls.TraverseEdge(this, 0);
        }
    }

    public override void OnNodeExit(GraphControls graphControls)
    {
        Debug.Log("Exited sample node cat");
    }
}
