using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphTheory.Editor.UIElements
{
    public class NodeView : Node
    {
        private NodeGraphView m_nodeGraphView = null;
        public ANode Node { get; private set; } = null;
        public SerializedProperty SerializedNode { get; set; } = null;
        public string NodeId { get { return Node != null ? Node.Id : string.Empty; } }

        private NodeDisplayContainers m_nodeDisplayContainers = null;
        private PortView m_inport = null;
        private List<PortView> m_outports = new List<PortView>();
        private Dictionary<string, EdgeView> m_edgeViews = new Dictionary<string, EdgeView>();
        private IEdgeConnectorListener m_edgeConnectorListener = null;
        private NodeDrawer m_nodeDrawer = null;

        public NodeView(ANode node, 
            SerializedProperty serializedNode, 
            NodeGraphView nodeGraphView, 
            IEdgeConnectorListener edgeConnectorListener,
            NodeDrawer nodeDrawer) : base()
        {
            if (node == null)
                return;

            Node = node;
            SerializedNode = serializedNode;
            m_nodeGraphView = nodeGraphView;
            m_edgeConnectorListener = edgeConnectorListener;
            m_nodeDrawer = nodeDrawer;
            m_nodeDrawer.SetNodeView(this, SerializedNode);

            title = m_nodeDrawer.DisplayName;
            m_nodeDisplayContainers = new NodeDisplayContainers(this);

            bool isEntryNode = Node is BuiltInNodes.EntryNode;
            if (isEntryNode)
            {
                this.capabilities = this.capabilities & (~Capabilities.Deletable);
            }

            if (!isEntryNode)
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
                m_nodeDisplayContainers.SetInport(m_inport);
            }

            for (int j = 0; j < serializedNode.FindPropertyRelative("m_outports").arraySize; j++)
            {
                PortView newPort = new PortView(this,
                    Orientation.Horizontal,
                    Direction.Output,
                    Port.Capacity.Single,
                    typeof(bool),
                    j,
                    m_edgeConnectorListener);
                newPort.portName = "";
                m_outports.Add(newPort);
                m_nodeDisplayContainers.AddNewOutport(newPort);
            }

            // Draw node
            m_nodeDrawer.OnDrawHeader(m_nodeDisplayContainers.HeaderContainer);
            m_nodeDrawer.OnDrawTitle(m_nodeDisplayContainers.PreTitleContainer, m_nodeDisplayContainers.PostTitleContainer);
            m_nodeDrawer.OnDrawInport(m_nodeDisplayContainers.InportContainer);
            for(int i = 0; i < m_outports.Count; i++)
            {
                m_nodeDrawer.OnDrawOutport(i, m_nodeDisplayContainers.OutportContainers[i]);
            }
            m_nodeDrawer.OnDrawBody(m_nodeDisplayContainers.BodyContainer);
            m_nodeDrawer.OnDrawFooter(m_nodeDisplayContainers.FooterContainer);

            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(Node.Position, Vector2.zero));

            //this.RegisterCallback<GeometryChangedEvent>((GeometryChangedEvent gce) => { Debug.Log(gce.newRect.position); });
        }

        public void OnLoadView()
        {
            List<OutportEdge> edges = Node.GetAllEdges();
            for (int k = 0; k < edges.Count; k++)
            {
                if (!edges[k].IsValid)
                {
                    continue;
                }
                EdgeView edgeView = new EdgeView()
                {
                    OutportEdge = edges[k],
                    input = m_nodeGraphView.GetNodeViewById(edges[k].ConnectedNodeId).m_inport,
                    output = m_outports[k],
                };
                edgeView.Setup();
                AddEdgeView(edgeView);
            }
        }

        public void OnUnloadView()
        {
            foreach (EdgeView edgeView in m_edgeViews.Values)
            {
                if (m_nodeGraphView.Contains(edgeView))
                {
                    m_nodeGraphView.RemoveElement(edgeView);
                }
            }
            m_edgeViews.Clear();
        }

        public void OnDeleteNode()
        {
            List<EdgeView> edgeViewsCopy = new List<EdgeView>(m_edgeViews.Values);
            for (int i = 0; i < edgeViewsCopy.Count; i++)
            {
                RemoveEdge(edgeViewsCopy[i]);
            }
        }

        public bool OutportHasEdge(int outportIndex)
        {
            return Node.OutportEdgeIsValid(outportIndex);
        }

        public PortView GetOutport(int outportIndex)
        {
            return m_outports[outportIndex];
        }

        public void AddEdge(EdgeView edgeView)
        {
            if (edgeView.FirstPort.Node == this)
            {
                OutportEdge outportEdge = new OutportEdge()
                {
                    SourceNodeId = edgeView.FirstPort.Node.NodeId,
                    ConnectedNodeId = edgeView.SecondPort.Node.NodeId
                };
                Node.AddOutportEdge(edgeView.FirstPort.PortIndex, outportEdge);
                edgeView.OutportEdge = outportEdge;
            }
            AddEdgeView(edgeView);
        }

        public void AddEdgeView(EdgeView edgeView)
        {
            if (edgeView.FirstPort.Node == this)
            {
                m_edgeViews.Add(edgeView.EdgeId, edgeView);
                edgeView.FirstPort.Connect(edgeView);
                edgeView.SecondPort.Node.AddEdgeView(edgeView);
                m_nodeGraphView.Add(edgeView);
            }
            else if (edgeView.SecondPort.Node == this)
            {
                edgeView.SecondPort.Connect(edgeView);
            }
        }

        public void RemoveEdge(EdgeView edgeView)
        {
            if (edgeView.FirstPort.Node == this)
            {
                edgeView.FirstPort.Disconnect(edgeView);
                edgeView.SecondPort.Node.RemoveEdge(edgeView);
                if (edgeView.parent != null && edgeView.parent is GraphView)
                {
                    (edgeView.parent as GraphView).RemoveElement(edgeView);
                }
                m_edgeViews.Remove(edgeView.EdgeId);
                Node.RemoveOutportEdge(edgeView.FirstPort.PortIndex);
            }
            else if (edgeView.SecondPort.Node == this)
            {
                edgeView.SecondPort.Disconnect(edgeView);
            }
        }

        public EdgeView GetEdgeViewById(string id)
        {
            return m_edgeViews[id];
        }

        public void UpdateNodeDataPosition()
        {
            Node.Position = this.GetPosition().position;
        }
    }
}