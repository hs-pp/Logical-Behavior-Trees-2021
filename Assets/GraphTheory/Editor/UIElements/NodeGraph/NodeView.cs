using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphTheory.Editor.UIElements
{
    public class NodeView : Node
    {
        private ANode m_node = null;
        public string NodeId { get { return m_node != null ? m_node.Id : string.Empty; } }
        private List<Port> m_inports = new List<Port>();
        private List<Port> m_outports = new List<Port>();

        public NodeView(ANode node) : base()
        {
            m_node = node;

            if(m_node != null)
            {
                title = m_node.Name;

                m_node.DrawNodeView(this);

                //Add ports
                for(int i = 0; i < m_node.NumInports; i++)
                {
                    Port newInport = InstantiatePort(Orientation.Horizontal, 
                        Direction.Input, 
                        Port.Capacity.Single, 
                        typeof(bool));
                    newInport.portName = $"Inport {i}";
                    m_inports.Add(newInport);
                    inputContainer.Add(newInport);
                }
                for (int j = 0; j < m_node.NumOutports; j++)
                {
                    Port newOutport = InstantiatePort(Orientation.Horizontal,
                        Direction.Output,
                        Port.Capacity.Single,
                        typeof(bool));
                    newOutport.portName = $"Outport {j}";
                    m_outports.Add(newOutport);
                    outputContainer.Add(newOutport);
                }
                RefreshExpandedState();
                RefreshPorts();

                Debug.Log(m_node.Size);
                SetPosition(new Rect(m_node.Position, m_node.Size));
            }
        }

        private Port GetPortInstance(Direction nodeDirection,
    Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }
    }
}