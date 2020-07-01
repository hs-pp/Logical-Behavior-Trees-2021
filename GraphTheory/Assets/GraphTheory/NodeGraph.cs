using System;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory.Internal.OdinSerializer;
using System.Linq;

namespace GraphTheory
{
    public abstract class NodeGraph : SerializedScriptableObject
    {
        [SerializeField]
        private NodeGraphData m_nodeGraphData;

        public NodeGraphData NodeGraphData { get { return m_nodeGraphData; } }
    }
}