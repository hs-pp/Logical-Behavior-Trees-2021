using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphTheory.Editor
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private NodeGraphView m_graphView = null;

        public EdgeConnectorListener(NodeGraphView graphView)
        {
            m_graphView = graphView;
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            m_graphView.CreateEdgeView(edge as EdgeView);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            Debug.Log("OnDropOutsidePort");
        }
    }
}