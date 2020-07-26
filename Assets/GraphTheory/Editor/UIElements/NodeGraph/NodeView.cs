using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class NodeView : Node
    {
        private ANode m_node = null;
        public string NodeId { get { return m_node != null ? m_node.Id : string.Empty; } }
        private PortView m_inport = null;
        private List<PortView> m_outports = new List<PortView>();
        private IEdgeConnectorListener m_edgeConnectorListener = null;
        
        public NodeView(ANode node, IEdgeConnectorListener edgeConnectorListener) : base()
        {
            m_node = node;
            m_edgeConnectorListener = edgeConnectorListener;
            if(m_node != null)
            {
                title = m_node.Name;

                m_node.DrawNodeView(this);

                if (m_node.HasInport)
                {
                    //Add ports
                    m_inport = new PortView(this,
                        Orientation.Horizontal,
                        Direction.Input,
                        Port.Capacity.Single,
                        typeof(bool),
                        0,
                        m_edgeConnectorListener);
                    m_inport.portName = "";
                    inputContainer.Add(m_inport);
                }

                for (int j = 0; j < m_node.NumOutports; j++)
                {
                    PortView newPort = new PortView(this,
                        Orientation.Horizontal,
                        Direction.Output,
                        Port.Capacity.Single,
                        typeof(bool),
                        j,
                        m_edgeConnectorListener);
                    newPort.portName = $"Outport {m_outports.Count}";
                    m_outports.Add(newPort);
                    outputContainer.Add(newPort);
                }
                RefreshExpandedState();
                RefreshPorts();
                
                SetPosition(new Rect(m_node.Position, m_node.Size));

                //this.RegisterCallback<GeometryChangedEvent>((GeometryChangedEvent gce) => { Debug.Log(gce.newRect.position); });
            }
        }

        public void OnRemove()
        {

        }

        public Port GetInport()
        {
            return m_inport;
        }

        public Port GetOutport(int outportIndex)
        {
            return m_outports[outportIndex];
        }

        public void ConnectPort(int outportIndex, NodeView otherNode)
        {
            m_node.AddOutportEdge(outportIndex, new OutportEdge() { ConnectedNodeId = otherNode.NodeId });
        }

        public void RemovePort(int outportIndex)
        {
            m_node.RemoveOutportEdge(outportIndex);
        }

        public bool OutportHasEdge(int outportIndex)
        {
            return m_node.GetOutportEdge(outportIndex) != null;
        }

        public void UpdateNodeDataPosition()
        {
            m_node.Position = this.GetPosition().position;
        }
    }
}