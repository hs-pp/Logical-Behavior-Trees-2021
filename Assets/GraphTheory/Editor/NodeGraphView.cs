using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace GraphTheory.Editor
{
    public class NodeGraphView : GraphView
    {
        private GridBackground m_gridBackground = null;
        private MiniMap m_miniMap = null;
        private NodeGraph m_nodeGraph = null;
        private Dictionary<string, NodeView> m_nodeViews = new Dictionary<string, NodeView>();

        public NodeGraphView() 
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NodeGraph"));//TODO: Move this out of Resources.

            // Grid lines
            m_gridBackground = new GridBackground();
            Insert(0, m_gridBackground);
            m_gridBackground.StretchToParentSize();

            // Minimap
            m_miniMap = new MiniMap { anchored = true };
            m_miniMap.SetPosition(new Rect(10, 30, 200, 140));
            Add(m_miniMap);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public void SetNodeGraph(NodeGraph nodeGraph)
        {
            Reset();
            m_nodeGraph = nodeGraph;

            if (m_nodeGraph == null)
                return;

            List<ANode> nodeData = m_nodeGraph.NodeGraphData.GetAllNodes();
            for(int i = 0; i < nodeData.Count; i++)
            {
                m_nodeViews.Add(nodeData[i].Id, CreateNodeView(nodeData[i]));
            }
        }

        public void CreateNode()
        {
            ANode node = new TestNode();
            CreateNodeView(node);
        }

        private NodeView CreateNodeView(ANode node)
        {
            NodeView nodeView = new NodeView(node);
            AddElement(nodeView);
            return nodeView;
        }

        private void Reset()
        {
            foreach(string id in m_nodeViews.Keys)
            {
                RemoveElement(m_nodeViews[id]);
            }
            m_nodeViews.Clear();
        }
    }
}