using System;
using UnityEngine;

namespace Logical.BuiltInNodes
{
    /// <summary>
    /// The EntryNode is a required node in any NodeGraph.
    /// It is automatically created when a new graph instance is created and
    /// is not deletable. New instances of the EntryNode are also not
    /// manually creatable.
    /// </summary>
    [Serializable]
    public class EntryNode : ANode
    {
        public override void OnNodeEnter(GraphControls graphControls)
        {
            Debug.Log("EntryNode Enter");
            graphControls.TraverseEdge(this, 0);
        }

        public override void OnNodeUpdate(GraphControls graphControls)
        {
            Debug.Log("EntryNode Update");
        }

        public override void OnNodeExit(GraphControls graphControls)
        {
            Debug.Log("EntryNode Exit");
        }
    }
}