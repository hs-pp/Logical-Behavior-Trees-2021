using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.BuiltInNodes
{
    [Serializable]
    public class EntryNode : ANode
    {

#if UNITY_EDITOR
        public override string Name => "Entry";
        public override bool HasInport => false;
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