using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.BuiltInNodes
{
    [Serializable]
    public class EntryNode : ANode
    {
        public override void OnNodeEnter(NodeCollection nodeCollection)
        {
            Debug.Log("EntryNode Enter");
            base.OnNodeEnter(nodeCollection);
            TraverseEdge(0);
        }

        public override void OnNodeUpdate()
        {
            Debug.Log("EntryNode Update");
        }

        public override void OnNodeExit()
        {
            Debug.Log("EntryNode Exit");
        }
    }
}