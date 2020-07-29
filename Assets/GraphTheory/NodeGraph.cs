using UnityEngine;
using GraphTheory.Internal.OdinSerializer;
using GraphTheory.BuiltInNodes;
using System;

namespace GraphTheory
{
    public abstract class NodeGraph : SerializedScriptableObject
    {
        [SerializeField]
        private NodeGraphData m_nodeGraphData;
        [NonSerialized]
        public Action OnGraphStart = null;
        [NonSerialized]
        public Action OnGraphStop = null;
        [NonSerialized]
        public Action<ANode> OnNodeChange = null;

        public void Awake()
        {
            //TODO: Register to runtime tracker here
        }

        public void StartGraph()
        {
            m_nodeGraphData.ParentNodeGraph = this;
            m_nodeGraphData.OnGraphStart += OnGraphStart;
            m_nodeGraphData.OnGraphStop += OnGraphStop;
            m_nodeGraphData.OnNodeChange += OnNodeChange;

            m_nodeGraphData.StartExecution();
        }
        public void UpdateGraph()
        {
            m_nodeGraphData.UpdateExecution();
        }
        public void StopGraph()
        {
            m_nodeGraphData.StopExecution();
        }

#if UNITY_EDITOR
        public NodeGraphData NodeGraphData { get { return m_nodeGraphData; } }

        public NodeGraph()
        {
            m_nodeGraphData = new NodeGraphData();
            ANode entryNode = m_nodeGraphData.CreateNode(typeof(EntryNode), Vector2.zero);
            m_nodeGraphData.SetEntryNode(entryNode.Id);
        }
#endif
    }
}