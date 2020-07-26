using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class PortView : Port
    {
        public NodeView Owner { get; private set; } = null;
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
            Owner = owner;
            PortIndex = index;
            this.m_EdgeConnector = new EdgeConnector<EdgeView>(edgeConnectorListener);
            this.AddManipulator(m_EdgeConnector);
        }
    }
}