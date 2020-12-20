using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Logical.Editor
{
    /// <summary>
    /// Visual representation of a port from where an edge can be made to connect
    /// two nodes together.
    /// </summary>
    public class PortView : Port
    {
        public NodeView Node { get; private set; } = null;
        public int PortIndex { get; private set; } = -1;

        public PortView(NodeView owner, 
            Orientation portOrientation, 
            Direction portDirection, 
            Capacity capacity, 
            Type type, 
            int index,
            IEdgeConnectorListener edgeConnectorListener) 
            : base(portOrientation, portDirection, capacity, type)
        {
            Node = owner;
            PortIndex = index;
            this.m_EdgeConnector = new EdgeConnector<EdgeView>(edgeConnectorListener);
            this.AddManipulator(m_EdgeConnector);
        }
    }
}