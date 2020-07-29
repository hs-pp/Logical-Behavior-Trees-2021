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
        public override void OnNodeEnter(NodeGraphData nodeGraphData)
        {
            Debug.Log("EntryNode Enter");
            base.OnNodeEnter(nodeGraphData);
            nodeGraphData.ChangeNode(this, GetOutportEdge(0));
        }

        public override void OnNodeUpdate()
        {
            Debug.Log("EntryNode Update");
        }

        public override void OnNodeExit()
        {
            Debug.Log("EntryNode Exit");
        }

#if UNITY_EDITOR
        public override string Name => "Entry";
        public override List<Type> CompatibleGraphs { get { return null; } }

        public EntryNode() : base()
        {
            CreateOutport();
        }

        public override void DrawNodeView(Node nodeView)
        {
            nodeView.mainContainer.Add(new Label(Position.ToString()));
        }
#endif
    }
}