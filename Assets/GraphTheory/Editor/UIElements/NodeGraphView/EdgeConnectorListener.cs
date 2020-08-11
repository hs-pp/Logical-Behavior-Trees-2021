using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphTheory.Editor.UIElements
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
            var edgeView = edge as EdgeView;
            
            if (edgeView?.input == null || edgeView?.output == null)
                return;

            edgeView.Setup();

            // Outports can only have one edge connected to them.
            if (edgeView.FirstPort.Node.OutportHasEdge(edgeView.FirstPort.PortIndex))
            {

                Debug.LogError("Outport already has edge.");
                return;
            }

            edgeView.FirstPort.Node.AddEdge(edgeView);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            Debug.Log("OnDropOutsidePort");
        }
    }
}