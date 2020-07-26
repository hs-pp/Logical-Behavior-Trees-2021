using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

namespace GraphTheory.Editor.UIElements
{
    public class NodeGraphView : GraphView
    {
        private GridBackground m_gridBackground = null;
        private MiniMap m_miniMap = null;
        private NodeCreationWindow m_nodeCreationWindow = null;
        private IEdgeConnectorListener m_edgeConectorListener = null;

        private NodeGraph m_nodeGraph = null;
        private NodeGraphData m_nodeGraphData = null;
        private Dictionary<string, NodeView> m_nodeViews = new Dictionary<string, NodeView>();
        private List<EdgeView> m_edgeViews = new List<EdgeView>();

        public Type GraphType { get { return m_nodeGraph.GetType(); } }

        public NodeGraphView() 
        {
            styleSheets.Add(Resources.Load<StyleSheet>("GraphTheory/NodeGraph/NodeGraphView"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            CustomContentDragger contentDragger = new CustomContentDragger();
            this.AddManipulator(contentDragger);
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            //Add(new GraphAxisLabels(this, contentDragger));

            // Grid lines
            m_gridBackground = new GridBackground();
            Insert(0, m_gridBackground);
            m_gridBackground.StretchToParentSize();

            // Minimap
            m_miniMap = new MiniMap { anchored = true };
            m_miniMap.SetPosition(new Rect(0, 0, 200, 200));
            this.RegisterCallback<GeometryChangedEvent>(SetMiniMapPosition);
            Add(m_miniMap);

            m_nodeCreationWindow = ScriptableObject.CreateInstance<NodeCreationWindow>();

            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition, 0, 0), m_nodeCreationWindow);
            };
            graphViewChanged += OnGraphViewChanged;

            m_edgeConectorListener = new EdgeConnectorListener(this);
        }


        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            compatiblePorts.AddRange(ports.ToList().Where(p => {
                var portView = p as PortView;

                if (p.direction == startPort.direction)
                    return false;

                //TODO: Check if the edge already exists

                return true;
            }));

            return compatiblePorts;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                for (int i = 0; i < graphViewChange.elementsToRemove.Count; i++)
                {
                    if (graphViewChange.elementsToRemove[i] is NodeView)
                    {
                        RemoveNode(graphViewChange.elementsToRemove[i] as NodeView);
                    }
                    else if (graphViewChange.elementsToRemove[i] is EdgeView)
                    {
                        RemoveEdge(graphViewChange.elementsToRemove[i] as EdgeView);
                    }
                }
            }
            if (graphViewChange.movedElements != null)
            {
                for (int k = 0; k < graphViewChange.movedElements.Count; k++)
                {
                    if(graphViewChange.movedElements[k] is NodeView)
                    {
                        (graphViewChange.movedElements[k] as NodeView).UpdateNodeDataPosition();
                    }
                }
            }
            return graphViewChange;
        }

        public void SetNodeGraphData(NodeGraph nodeGraph, string path)
        {
            Reset();
            m_nodeGraphData = GetNodeGraphDataByBreadcrumb(nodeGraph, path);
            if (m_nodeGraph != nodeGraph)
            {
                m_nodeCreationWindow.Setup(this);
            }
            m_nodeGraph = nodeGraph;

            if (m_nodeGraphData == null)
                return;

            List<ANode> nodeData = m_nodeGraphData.GetAllNodes();
            for(int i = 0; i < nodeData.Count; i++)
            {
                m_nodeViews.Add(nodeData[i].Id, CreateNodeView(nodeData[i]));
            }
            for(int j = 0; j < nodeData.Count; j++)
            {
                List<OutportEdge> edges = nodeData[j].GetAllEdges();
                for(int k = 0; k < edges.Count; k++)
                {
                    if(edges[k] == null)
                    {
                        continue;
                    }
                    EdgeView edgeView = new EdgeView()
                    {
                        input = m_nodeViews[edges[k].ConnectedNodeId].GetInport(),
                        output = m_nodeViews[nodeData[j].Id].GetOutport(k),
                    };
                    AddElement(edgeView);
                    m_edgeViews.Add(edgeView);
                }
            }
        }

        private NodeGraphData GetNodeGraphDataByBreadcrumb(NodeGraph graph, string path)
        {
            return graph.NodeGraphData;
        }

        /// <summary>
        /// Create both a data instance as well as a visual instance of a fresh node.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="pos"></param>
        public void CreateNode(Type nodeType, Vector2 pos)
        {
            ANode node = m_nodeGraphData.CreateNode(nodeType, pos);
            CreateNodeView(node);
        }

        private NodeView CreateNodeView(ANode node)
        {
            NodeView nodeView = new NodeView(node, m_edgeConectorListener);
            AddElement(nodeView);
            return nodeView;
        }

        private void RemoveNode(NodeView nodeView)
        {
            for(int i = m_edgeViews.Count - 1; i >= 0; i--)
            {
                if((m_edgeViews[i].output as PortView).Owner.NodeId == nodeView.NodeId
                    || (m_edgeViews[i].input as PortView).Owner.NodeId == nodeView.NodeId)
                {
                    RemoveEdge(m_edgeViews[i]);
                }
            }
            nodeView.OnRemove();
            m_nodeGraphData.RemoveNode(nodeView.NodeId);
            Debug.Log("Removed node " + nodeView.NodeId);
        }

        private void SetMiniMapPosition(GeometryChangedEvent evt)
        {
            m_miniMap.SetPosition(new Rect(evt.newRect.xMax - 210, evt.newRect.yMax - 210, 200, 200));
        }

        private void Reset()
        {
            foreach(string id in m_nodeViews.Keys)
            {
                RemoveElement(m_nodeViews[id]);
            }
            foreach(EdgeView edgeView in m_edgeViews)
            {
                RemoveElement(edgeView);
            }
            m_nodeViews.Clear();
            m_edgeViews.Clear();
        }

        public void AddEdge(EdgeView edgeView)
        {
            PortView outPort = edgeView.output as PortView;
            PortView inPort = edgeView.input as PortView;
            outPort.Owner.ConnectPort(outPort.PortIndex, inPort.Owner);
            m_edgeViews.Add(edgeView);
            AddElement(edgeView);
        }

        private void RemoveEdge(EdgeView edgeView)
        {
            // By the time this method gets called, the edgeview is already removed from the graphview
            // We just need to remove the data representation of it.
            PortView outPort = edgeView.output as PortView;
            outPort.Owner.RemovePort(outPort.PortIndex);
            Debug.Log("Removing edgeeeeeee");
            m_edgeViews.Remove(edgeView);
            if(Contains(edgeView))
            {
                RemoveElement(edgeView);
            }
        }
    }
}

// Trying to figure out how to undo forced dark theme. No success yes...
// MAYBE I should always force dark theme by calling this method myself. That would future proof this scenario.
//https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/GraphViewEditor/Views/GraphView.cs
//https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/UIElementsEditor/UIElementsEditorUtility.cs
//https://github.com/Unity-Technologies/UnityCsReference/blob/master/External/MirroredPackageSources/com.unity.ui/Core/VisualElementStyleSheetSet.cs
//UIElementsEditorUtility.ForceDarkStyleSheet(false);
//Type type = Assembly.GetAssembly(typeof(UnityEditor.UIElements.ColorField)).GetTypes().FirstOrDefault(x => x.Name == "UIElementsEditorUtility");
//MemberInfo[] infos = type.GetMembers(BindingFlags.Static | BindingFlags.NonPublic);
//StyleSheet darkStyle = type.GetField("s_DefaultCommonDarkStyleSheet", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as StyleSheet;
//StyleSheet lightStyle = type.GetField("s_DefaultCommonLightStyleSheet", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as StyleSheet;

//MethodInfo swap = typeof(VisualElementStyleSheetSet).GetMethod("Swap", BindingFlags.NonPublic | BindingFlags.Instance);
//VisualElement e = this;
//while (e != null)
//{
//    Debug.Log("loop");
//    if (e.styleSheets.Contains(lightStyle))
//    {
//        Debug.Log("swapping");
//        swap.Invoke(e.styleSheets, new object[] { lightStyle, darkStyle });
//        break;
//    }
//    e = e.parent;
//}