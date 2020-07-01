using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GraphTheory
{
    [System.Serializable]
    public class NodeGraphData
    {
        [SerializeField]
        private Dictionary<string, ANode> m_nodes = new Dictionary<string, ANode>();

        public ANode GetConnectedNode(InportEdge inportEdge)
        {
            return null;
        }
        public ANode GetConnectedNode(OutportEdge outportEdge)
        {
            return null;
        }

#if UNITY_EDITOR
        public List<ANode> GetAllNodes()
        {
            return m_nodes.Values.ToList();
        }
        public ANode CreateNode(Type type)
        {
            return null;
        }
        public void RemoveNode(string nodeId)
        {

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