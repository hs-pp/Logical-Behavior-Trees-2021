using UnityEngine;
using GraphTheory.Internal.OdinSerializer;
using GraphTheory.BuiltInNodes;
using System;

namespace GraphTheory
{
    public abstract class NodeGraph : SerializedScriptableObject
    {
        [SerializeField]
        private NodeCollection m_nodeCollection;
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
            m_nodeCollection.ParentNodeGraph = this;
            m_nodeCollection.OnGraphStart += OnGraphStart;
            m_nodeCollection.OnGraphStop += OnGraphStop;
            m_nodeCollection.OnNodeChange += OnNodeChange;

            m_nodeCollection.StartExecution();
        }
        public void UpdateGraph()
        {
            m_nodeCollection.UpdateExecution();
        }
        public void StopGraph()
        {
            m_nodeCollection.StopExecution();
        }

#if UNITY_EDITOR
        public NodeCollection NodeCollection { get { return m_nodeCollection; } }

        public NodeGraph()
        {
            m_nodeCollection = new NodeCollection();
            ANode entryNode = m_nodeCollection.CreateNode(typeof(EntryNode), Vector2.zero);
            m_nodeCollection.SetEntryNode(entryNode.Id);
        }
#endif
    }
}