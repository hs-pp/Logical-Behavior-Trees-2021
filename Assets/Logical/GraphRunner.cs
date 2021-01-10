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
        public BlackboardProperties BlackboardProperties { get; private set; }

        private GraphControls graphControls = null;

        [NonSerialized]
        public Action OnGraphStart = null;
        [NonSerialized]
        public Action OnGraphStop = null;
        [NonSerialized]
        public Action<ANode> OnNodeChange = null;
        private ANode m_currentNode = null;

        public GraphRunner(NodeGraph nodeGraph, AGraphProperties graphProperties, BlackboardProperties blackboardProperties)
        {
            m_nodeCollection = nodeGraph.NodeCollection;
            GraphProperties = graphProperties;
            BlackboardProperties = blackboardProperties;

            graphControls = new GraphControls(GraphProperties, BlackboardProperties, TraverseEdge);

            if (m_nodeCollection == null)
            {
                Debug.LogError("No Graph attached to GraphRunner!");
            }
        }

        public GraphRunner(NodeCollection nodeCollection, AGraphProperties graphProperties, BlackboardProperties blackboardProperties)
        {
            m_nodeCollection = nodeCollection;
            GraphProperties = graphProperties;
            BlackboardProperties = blackboardProperties;
        }

        public void StartGraph()
        {
            OnGraphStart?.Invoke();
            m_currentNode = m_nodeCollection.GetEntryNode();
            m_currentNode?.OnNodeEnter(graphControls);
        }

        public void StopGraph()
        {
            m_currentNode?.OnNodeExit(graphControls);
            OnGraphStop?.Invoke();
        }

        public void UpdateGraph()
        {
            m_currentNode?.OnNodeUpdate(graphControls);
        }

        private void TraverseEdge(OutportEdge edge)
        {
            if(!m_currentNode.ContainsOutport(edge))
            {
                Debug.LogError("Ahhhh! Trying to traverse edge from non-current node.");
                return;
            }

            m_currentNode?.OnNodeExit(graphControls);

            if (edge == null)
            {
                StopGraph();
                return;
            }

            m_currentNode = m_nodeCollection.GetNodeById(edge.ConnectedNodeId);
            m_currentNode?.OnNodeEnter(graphControls);
        }
    }

    /// <summary>
    /// Nodes do not have a reference to its parent graph by design and instead is provided this class
    /// when the node is being run. This data class contains all the functionality needed for a node 
    /// when being run within a graph.
    /// </summary>
    public class GraphControls
    {
        public AGraphProperties GraphProperties { get; private set; }
        public BlackboardProperties BlackboardProperties { get; private set; }

        private Action<OutportEdge> m_onTraverseEdge = null;

        public GraphControls(AGraphProperties graphProperties, 
            BlackboardProperties blackboardProperties, 
            Action<OutportEdge> onTraverseEdge)
        {
            GraphProperties = graphProperties;
            BlackboardProperties = blackboardProperties;
            m_onTraverseEdge = onTraverseEdge;
        }

        /// <summary>
        /// Call this method from any of a node's methods to traverse a particular edge and enter another node.
        /// </summary>
        /// <param name="index"> The index of the edge (aka outport) we want to traverse. </param>
        /// <param name="node"> The current node. </param>
        public void TraverseEdge(int index, ANode node)
        {
            m_onTraverseEdge?.Invoke(node.GetOutportEdge(index));
        }

    }
}