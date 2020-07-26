using UnityEngine;
using GraphTheory.Internal.OdinSerializer;
using GraphTheory.BuiltInNodes;
using System;
using System.Collections.Generic;

namespace GraphTheory
{
    public abstract class NodeGraph : SerializedScriptableObject
    {
        [SerializeField]
        private NodeGraphData m_nodeGraphData;
        public NodeGraphData NodeGraphData { get { return m_nodeGraphData; } }

        public NodeGraph()
        {
            m_nodeGraphData = new NodeGraphData();
            m_nodeGraphData.CreateNode(typeof(EntryNode), Vector2.zero);
        }
    }
}