using System;
using UnityEngine;

namespace GraphTheory
{
    public class GraphRunner
    {
        private NodeCollection m_nodeCollection = null;
        public IGraphProperties GraphProperties { get; private set; }

        [NonSerialized]
        public Action OnGraphStart = null;
        [NonSerialized]
        public Action OnGraphStop = null;
        [NonSerialized]
        public Action<ANode> OnNodeChange = null;
        private ANode m_currentNode = null;

        public GraphRunner(NodeGraph nodeGraph, IGraphProperties graphProperties)
        {
            m_nodeCollection = nodeGraph.NodeCollection;
            GraphProperties = graphProperties;
            if(m_nodeCollection == null)
            {
                Debug.LogError("No Graph attached to GraphRunner!");
            }
        }

        public GraphRunner(NodeCollection nodeCollection, IGraphProperties graphProperties)
        {
            m_nodeCollection = nodeCollection;
            GraphProperties = graphProperties;
            if (m_nodeCollection == null)
            {
                Debug.LogError("No Graph attached to GraphRunner!");
            }
        }

        public void StartGraph()
        {
            OnGraphStart?.Invoke();
            m_currentNode = m_nodeCollection.GetEntryNode();
            m_currentNode?.OnNodeEnter(this);
        }

        public void StopGraph()
        {
            m_currentNode?.OnNodeExit(this);
            OnGraphStop?.Invoke();
        }

        public void UpdateGraph()
        {
            m_currentNode?.OnNodeUpdate(this);
        }

        public void TraverseEdge(OutportEdge edge)
        {
            if(!m_currentNode.ContainsOutport(edge))
            {
                Debug.LogError("Ahhhh! Trying to traverse edge from non-current node.");
                return;
            }

            m_currentNode?.OnNodeExit(this);

            if (edge == null)
            {
                StopGraph();
                return;
            }

            m_currentNode = m_nodeCollection.GetNodeById(edge.ConnectedNodeId);
            m_currentNode?.OnNodeEnter(this);
        }
    }
}