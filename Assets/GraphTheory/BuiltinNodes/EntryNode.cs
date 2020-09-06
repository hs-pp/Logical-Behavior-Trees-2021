using System;
using UnityEngine;

namespace GraphTheory.BuiltInNodes
{
    [Serializable]
    public class EntryNode : ANode
    {
        public override void OnNodeEnter(GraphRunner graphRunner)
        {
            Debug.Log("EntryNode Enter");
            graphRunner.TraverseEdge(GetOutportEdge(0));
        }

        public override void OnNodeUpdate(GraphRunner graphRunner)
        {
            Debug.Log("EntryNode Update");
        }

        public override void OnNodeExit(GraphRunner graphRunner)
        {
            Debug.Log("EntryNode Exit");
        }
    }
}