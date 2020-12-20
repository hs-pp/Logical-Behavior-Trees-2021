using System;
using UnityEngine;

namespace Logical
{
    /// <summary>
    /// The class that actually runs a graph.
    /// This class has three essential methods that control the lifetime of a graph:
    /// 1. StartGraph(): Starts the graph, beginning at the graph's EntryNode.
    /// 2. UpdateGraph(): Calls OnUpdateNode() on the currently active node in the graph. This effectively does nothing if none of your nodes require OnUpdateNode().
    /// 3. StopGraph(): Stops the graph and exits out of the current node properly.
    /// 
    /// This class does not need to be attached to a monobehaviour, but it'll need someone to call it's UpdateGraph()
    /// method assuming the graph that is running needs updates called on its nodes.
    /// 
    /// NodeGraphController is a basic monobehaviour that uses a GraphRunner to execute graphs at runtime.
    /// </summary>
    public class GraphRunner
    {
        private NodeCollection m_nodeCollection = null;
        public AGraphProperties GraphProperties { get; private set; }
        public BlackboardData BlackboardData { get; private set; }

        [NonSerialized]
        public Action OnGraphStart = null;
        [NonSerialized]
        public Action OnGraphStop = null;
        [NonSerialized]
        public Action<ANode> OnNodeChange = null;
        private ANode m_currentNode = null;

        public GraphRunner(NodeGraph nodeGraph, AGraphProperties graphProperties, BlackboardData blackboardData)
        {
            m_nodeCollection = nodeGraph.NodeCollection;
            GraphProperties = graphProperties;
            BlackboardData = blackboardData;

            if (m_nodeCollection == null)
            {
                Debug.LogError("No Graph attached to GraphRunner!");
            }
        }

        public GraphRunner(NodeCollection nodeCollection, AGraphProperties graphProperties, BlackboardData blackboardData)
        {
            m_nodeCollection = nodeCollection;
            GraphProperties = graphProperties;
            BlackboardData = blackboardData;
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