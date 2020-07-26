using System.Collections;
using System.Collections.Generic;
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

            PortView outPort = edgeView.output as PortView;
            PortView inPort = edgeView.input as PortView;

            // Outports can only have one edge connected to them.
            if (outPort.Owner.OutportHasEdge(outPort.PortIndex))
            {
                Debug.LogError("Outport already has edge.");
                return;
            }

            (graphView as NodeGraphView).AddEdge(edgeView);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            Debug.Log("OnDropOutsidePort " + (edge.input != null) + " " + (edge.output != null));

        }
    }
}