using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GraphTheory
{
    [System.Serializable]
    public class NodeCollection
    {
        [SerializeReference]
        private List<ANode> m_nodes = new List<ANode>();
        [SerializeField]
        private string m_entryNodeId = "";

        [NonSerialized]
        public Action OnGraphStart = null;
        [NonSerialized]
        public Action OnGraphStop = null;
        [NonSerialized]
        public Action<ANode> OnNodeChange = null;
        public ANode CurrentNode { get; private set; } = null;
        public NodeGraph ParentNodeGraph { get; set; } = null;

        public void StartExecution()
        {
            Debug.Log("Starting graphdata");
            OnGraphStart?.Invoke();

            CurrentNode = GetNodeById(m_entryNodeId);
            CurrentNode?.OnNodeEnter(this);
        }

        public void UpdateExecution()
        {
            CurrentNode?.OnNodeUpdate();
        }

        public void StopExecution()
        {
            CurrentNode = null;
            OnGraphStop?.Invoke();
            Debug.Log("Stopping graphdata");
        }

        public void TraverseEdge(OutportEdge edge)
        {
            CurrentNode?.OnNodeExit();

            if (edge == null)
            {
                StopExecution();
                return;
            }

            CurrentNode = GetNodeById(edge.ConnectedNodeId);
            CurrentNode?.OnNodeEnter(this);
        }

        public ANode GetNodeById(string id)
        {
            //TODO: DO A BINARY SEARCh
            return m_nodes.Find(x => x.Id == id);
        }

#if UNITY_EDITOR
        public void SetEntryNode(string nodeId)
        {
            m_entryNodeId = nodeId;
        }

        public List<ANode> GetAllNodes()
        {
            return m_nodes;
        }

        public int GetNodeIndex(ANode node)
        {
            return m_nodes.FindIndex(x => x == node);
        }

        public void AddNode(ANode node)
        {
            //TODO: SORT BY GUID
            m_nodes.Add(node);
        }

        public ANode CreateNode(Type type, Vector2 pos)
        {
            ANode node = Activator.CreateInstance(type) as ANode;
            node.Position = pos;
            AddNode(node);
            return node;
        }

        public void RemoveNode(string nodeId)
        {
            //TODO: MAKE THIS MORE EFFICIENT
            ANode node = m_nodes.Find(x => x.Id == nodeId);
            if (node != null)
            {
                m_nodes.Remove(node);
            }
        }
#endif
    }
}