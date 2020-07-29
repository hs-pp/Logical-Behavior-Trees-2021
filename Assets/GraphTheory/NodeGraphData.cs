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

#if UNITY_EDITOR
        public ANode GetNodeById(string id)
        {
            return m_nodes[id];
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