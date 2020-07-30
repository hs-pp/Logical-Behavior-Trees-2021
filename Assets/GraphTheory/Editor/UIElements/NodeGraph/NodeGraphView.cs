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
        private NodeCollection m_nodeCollection = null;
        private Dictionary<string, NodeView> m_nodeViews = new Dictionary<string, NodeView>();

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
            this.RegisterCallback<GeometryChangedEvent>((GeometryChangedEvent evt) => { m_miniMap.SetPosition(new Rect(evt.newRect.xMax - 210, evt.newRect.yMax - 210, 200, 200)); });
            Add(m_miniMap);

            m_nodeCreationWindow = ScriptableObject.CreateInstance<NodeCreationWindow>();

            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition, 0, 0), m_nodeCreationWindow);
            };
            graphViewChanged += OnGraphViewChanged;

            m_edgeConectorListener = new EdgeConnectorListener(this);
        }

        public void SetNodeCollection(NodeGraph nodeGraph, string path)
        {
            Reset();

            NodeCollection nodeCollection = GetNodeCollectionByBreadcrumb(nodeGraph, path);

            if (nodeGraph == null || nodeCollection == null)
                return;

            m_nodeGraph = nodeGraph;
            m_nodeCollection = nodeCollection;
            m_nodeCreationWindow.Setup(this);

            List<ANode> nodeData = m_nodeCollection.GetAllNodes();
            for(int i = 0; i < nodeData.Count; i++)
            {
                CreateNodeView(nodeData[i]);
            }
            for(int j = 0; j < nodeData.Count; j++)
            {
                m_nodeViews[nodeData[j].Id].OnLoadView();
            }
        }

        private void Reset()
        {
            m_nodeGraph = null;
            m_nodeCollection = null;
            foreach (string id in m_nodeViews.Keys)
            {
                m_nodeViews[id].OnUnloadView();
                RemoveElement(m_nodeViews[id]);
            }
            m_nodeViews.Clear();
        }

        public NodeView GetNodeViewById(string id)
        {
            return m_nodeViews[id];
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            PortView portOne = startPort as PortView;
            List<Port> compatiblePorts = new List<Port>();

            compatiblePorts.AddRange(ports.ToList().Where(port => {
                PortView portTwo = port as PortView;

                if (portOne.direction == portTwo.direction)
                    return false;

                if (portOne.Node == portTwo.Node)
                    return false;
                
                return true;
            }));

            return compatiblePorts;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                for (int i = graphViewChange.elementsToRemove.Count - 1; i >= 0; i--)
                {
                    if (graphViewChange.elementsToRemove[i] is NodeView)
                    {
                        DeleteNode(graphViewChange.elementsToRemove[i] as NodeView);
                    }
                    else if (graphViewChange.elementsToRemove[i] is EdgeView)
                    {
                        EdgeView edgeView = graphViewChange.elementsToRemove[i] as EdgeView;
                        edgeView.FirstPort.Node.RemoveEdge(edgeView);
                    }
                }
            }
            if (graphViewChange.movedElements != null)
            {
                for (int k = 0; k < graphViewChange.movedElements.Count; k++)
                {
                    if (graphViewChange.movedElements[k] is NodeView)
                    {
                        (graphViewChange.movedElements[k] as NodeView).UpdateNodeDataPosition();
                    }
                }
            }
            return graphViewChange;
        }

        private NodeCollection GetNodeCollectionByBreadcrumb(NodeGraph graph, string path)
        {
            return graph.NodeCollection;
        }

        /// <summary>
        /// Create both a data instance as well as a visual instance of a fresh node.
        /// </summary>
        public NodeView CreateNode(Type nodeType, Vector2 pos)
        {
            ANode node = m_nodeCollection.CreateNode(nodeType, pos);
            return CreateNodeView(node);
        }

        private NodeView CreateNodeView(ANode node)
        {
            NodeView nodeView = new NodeView(node, this, m_edgeConectorListener);
            AddElement(nodeView);
            m_nodeViews.Add(node.Id, nodeView);
            return nodeView;
        }

        private void DeleteNode(NodeView nodeView)
        {
            nodeView.OnDeleteNode();
            m_nodeCollection.RemoveNode(nodeView.NodeId);
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            if(selection.Count == 1)
            {
                //Debug.Log("selected" + selectable.GetType());
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