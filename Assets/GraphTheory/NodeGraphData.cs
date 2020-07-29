using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GraphTheory
{
    public class NodeGraphData
    {
        [SerializeField]
        private Dictionary<string, ANode> m_nodes = new Dictionary<string, ANode>();
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
                StopExecution();
                return;
            }

            CurrentNode = GetNodeById(edge.ConnectedNodeId);
            CurrentNode?.OnNodeEnter(this);
        }

        public ANode GetNodeById(string id)
        {
            return m_nodes[id];
        }

#if UNITY_EDITOR
        public void SetEntryNode(string nodeId)
        {
            m_entryNodeId = nodeId;
        }
        public List<ANode> GetAllNodes()
        {
            return m_nodes.Values.ToList();
        }
        public ANode CreateNode(Type type, Vector2 pos)
        {
            ANode node = Activator.CreateInstance(type) as ANode;
            node.Position = pos;
            m_nodes.Add(node.Id, node);
            return node;
        }
        public void RemoveNode(string nodeId)
        {
            m_nodes.Remove(nodeId);
        }
        public void MakeConnection(ANode outportNode, int outportIndex, ANode inportNode, int inportIndex)
        {

        }
        public void BreakConnection(ANode outportNode, int outportIndex, ANode inportNode, int inportIndex)
        {

        }
#endif
    }
}