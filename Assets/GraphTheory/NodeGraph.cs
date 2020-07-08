using System;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory.Internal.OdinSerializer;
using System.Linq;
using System.Reflection;

namespace GraphTheory
{
    public abstract partial class NodeGraph : SerializedScriptableObject
    {
        [SerializeField]
        private NodeGraphData m_nodeGraphData;

        public NodeGraphData NodeGraphData { get { return m_nodeGraphData; } }
    }
}