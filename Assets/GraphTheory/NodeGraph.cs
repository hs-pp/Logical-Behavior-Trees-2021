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
        [SerializeField]
        private string m_entryNodeId = "";

        [NonSerialized]
        public Action OnGraphStart = null;
        [NonSerialized]
        public Action OnGraphStop = null;
        [NonSerialized]
        public Action<ANode> OnNodeChange = null;

        public NodeGraphData NodeGraphData { get { return m_nodeGraphData; } }
        public ANode CurrentNode { get; private set; } = null;

        public NodeGraph()
        {
            m_nodeGraphData = new NodeGraphData();
            ANode entryNode = m_nodeGraphData.CreateNode(typeof(EntryNode), Vector2.zero);
            m_entryNodeId = entryNode.Id;
        }

        public void Awake()
        {
            //TODO: Register to runtime tracker here
        }

        public void StartGraph()
        {
            Debug.Log("Starting graph");
            OnGraphStart?.Invoke();

            CurrentNode = GetNodeById(m_entryNodeId);
            CurrentNode?.OnNodeEnter(this);
        }

        public void UpdateGraph()
        {
            CurrentNode?.OnNodeUpdate();
        }

        public void StopGraph()
        {
            CurrentNode = null;
            OnGraphStop?.Invoke();
            Debug.Log("Stopping graph");
        }

        public void ChangeNode(ANode source, OutportEdge edge)
        {
            if (CurrentNode != source)
            {
                Debug.LogError("Source is not the currently running node!");
                return;
            }

            CurrentNode?.OnNodeExit();

            if (edge == null)
            {
                CurrentNode = null;
                StopGraph();
                return;
            }

            CurrentNode = GetNodeById(edge.ConnectedNodeId);
            CurrentNode?.OnNodeEnter(this);
        }

        private ANode GetNodeById(string id)
        {
            return m_nodeGraphData.GetNodeById(id);
        }
    }
}