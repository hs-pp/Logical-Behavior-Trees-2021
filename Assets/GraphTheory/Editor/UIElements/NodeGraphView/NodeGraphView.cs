using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using UnityEditor;

namespace GraphTheory.Editor.UIElements
{
    public class NodeGraphView : GraphView
    {
        private GridBackground m_gridBackground = null;
        private MiniMap m_miniMap = null;
        private NodeCreationWindow m_nodeCreationWindow = null;
        private IEdgeConnectorListener m_edgeConectorListener = null;

        private NodeGraph m_nodeGraph = null;
        private GraphTypeMetadata m_graphTypeMetadata = null;
        private NodeCollection m_nodeCollection = null;
        private SerializedProperty m_nodeListProp = null;
        private Dictionary<string, NodeView> m_nodeViews = new Dictionary<string, NodeView>();
        private Vector2 m_mousePosition = Vector2.zero;

        public Action<NodeView> OnRemoveNode = null;
        public Action<ISelectable> OnAddToSelection = null;
        public Action<ISelectable> OnRemoveFromSelection = null;
        public Action OnClearSelection = null;

        public NodeGraphView() 
        {
            styleSheets.Add(Resources.Load<StyleSheet>("GraphTheory/UIElements/NodeGraph/NodeGraphView"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new CustomContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // Grid lines
            m_gridBackground = new GridBackground();
            Insert(0, m_gridBackground);
            m_gridBackground.StretchToParentSize();

            // Minimap
            m_miniMap = new MiniMap { anchored = true };
            m_miniMap.SetPosition(new Rect(0, 0, 200, 200));
            this.RegisterCallback<GeometryChangedEvent>((GeometryChangedEvent evt) => { m_miniMap.SetPosition(new Rect(evt.newRect.xMax - 210, evt.newRect.yMax - 210, 200, 200)); });
            Add(m_miniMap);

            m_graphTypeMetadata = new GraphTypeMetadata();
            m_nodeCreationWindow = ScriptableObject.CreateInstance<NodeCreationWindow>();
            m_nodeCreationWindow.Setup(this, m_graphTypeMetadata);

            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition, 0, 0), m_nodeCreationWindow);
            };
            graphViewChanged += OnGraphViewChanged;

            m_edgeConectorListener = new EdgeConnectorListener(this);
            serializeGraphElements += CopyAndSerializeGraphElements;
            unserializeAndPaste += UnserializeAndPasteGraphElements;
            canPasteSerializedData += CanUnserializeAndPaste;
            
            RegisterCallback<MouseMoveEvent>(x => { m_mousePosition = x.localMousePosition;});
            Undo.undoRedoPerformed += () => { SetNodeCollection(m_nodeGraph); };
        }

        public void SetNodeCollection(NodeGraph nodeGraph)
        {
            Reset();
            if(nodeGraph == null)
            {
                return;
            }

            NodeCollection nodeCollection = GetNodeCollectionByBreadcrumb(nodeGraph);
            m_nodeListProp = new SerializedObject(nodeGraph).FindProperty("m_nodeCollection").FindPropertyRelative("m_nodes");

            if (nodeGraph == null || nodeCollection == null)
                return;

            m_nodeGraph = nodeGraph;
            m_nodeCollection = nodeCollection;
            m_graphTypeMetadata.SetNewGraphType(m_nodeGraph.GetType());

            m_nodeGraph.OnNodeOutportAdded += OnNodeOutportAdded;
            m_nodeGraph.OnNodeOutportRemoved += OnNodeOutportRemoved;

            List<ANode> nodeData = m_nodeCollection.GetAllNodes();
            for(int i = 0; i < nodeData.Count; i++)
            {
                CreateNodeView(nodeData[i], m_nodeListProp.GetArrayElementAtIndex(i));
            }
            for(int j = 0; j < nodeData.Count; j++)
            {
                m_nodeViews[nodeData[j].Id].OnLoadView();
            }
            RefreshSerializedNodeReferences();
        }

        private void Reset()
        {
            if(m_nodeGraph != null)
            {
                m_nodeGraph.OnNodeOutportAdded -= OnNodeOutportAdded;
                m_nodeGraph.OnNodeOutportRemoved -= OnNodeOutportRemoved;
            }

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
            return m_nodeViews.ContainsKey(id) ? m_nodeViews[id] : null;
        }

        public EdgeView GetEdgeViewById(string id)
        {
            EdgeView edgeView = null;
            foreach(NodeView nodeView in m_nodeViews.Values)
            {
                edgeView = nodeView.GetEdgeViewById(id);
                if(edgeView != null)
                {
                    return edgeView;
                }
            }
            return null;
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
                Undo.RegisterCompleteObjectUndo(m_nodeGraph, "Deleted Graph Elements");
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
            else if (graphViewChange.movedElements != null)
            {
                Undo.RegisterCompleteObjectUndo(m_nodeGraph, "Moved Graph Elements");
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

        private NodeCollection GetNodeCollectionByBreadcrumb(NodeGraph graph)
        {
            return graph.NodeCollection;
        }

        /// <summary>
        /// Create both a data instance as well as a visual instance of a fresh node.
        /// </summary>
        public NodeView CreateNode(Type nodeType, Vector2 pos)
        {
            Undo.RegisterCompleteObjectUndo(m_nodeGraph, "Created Node");
            ANode node = m_nodeCollection.CreateNode(nodeType, pos);
            m_nodeListProp.serializedObject.Update();
            NodeView nodeView = CreateNodeView(node);
            RefreshSerializedNodeReferences();
            return nodeView;
        }

        private NodeView CreateNodeView(ANode node, SerializedProperty serializedNode = null)
        {
            if(serializedNode == null)
            {
                int index = m_nodeGraph.NodeCollection.GetNodeIndex(node);
                if(index == -1)
                {
                    Debug.Log("wtf ");
                }
                serializedNode = m_nodeListProp.GetArrayElementAtIndex(index);
            }

            NodeView nodeView = new NodeView(node, serializedNode, this, m_edgeConectorListener,
                Activator.CreateInstance(m_graphTypeMetadata.GetNodeViewDrawerType(node.GetType())) as NodeViewDrawer);

            AddElement(nodeView);
            m_nodeViews.Add(node.Id, nodeView);
            return nodeView;
        }

        private void DeleteNode(NodeView nodeView)
        {
            OnRemoveNode?.Invoke(nodeView);
            nodeView.OnDeleteNode();
            m_nodeViews.Remove(nodeView.NodeId);
            m_nodeCollection.RemoveNode(nodeView.NodeId);
            RefreshSerializedNodeReferences();
        }

        private void OnNodeOutportAdded(string nodeId)
        {
            SetNodeCollection(m_nodeGraph); // Maybe make this more efficient one day.
        }

        private void OnNodeOutportRemoved(string nodeId, int index)
        {
            SetNodeCollection(m_nodeGraph); // Maybe make this more efficient one day.
        }

        public void CreateEdgeView(EdgeView edgeView)
        {
            if (edgeView?.input == null || edgeView?.output == null)
                return;

            Undo.RegisterCompleteObjectUndo(m_nodeGraph, "Created New Edge");
            edgeView.Setup();

            // Outports can only have one edge connected to them.
            if (edgeView.FirstPort.Node.OutportHasEdge(edgeView.FirstPort.PortIndex))
            {

                Debug.LogError("Outport already has edge.");
                return;
            }

            edgeView.FirstPort.Node.AddEdge(edgeView);
            m_nodeListProp.serializedObject.Update();
        }

        private void LoadSanitizedClipboardNodes(List<ANode> nodes)
        {
            List<NodeView> newNodeViews = new List<NodeView>();
            for(int i = 0; i < nodes.Count; i++)
            {
                m_nodeCollection.AddNode(nodes[i]);
                m_nodeListProp.serializedObject.Update();
                newNodeViews.Add(CreateNodeView(nodes[i]));
            }
            for(int i = 0; i < newNodeViews.Count; i++)
            {
                newNodeViews[i].OnLoadView();
            }
        }

        private void RefreshSerializedNodeReferences()
        {
            List<ANode> allNodes = m_nodeCollection.GetAllNodes();
            for(int i = 0; i < allNodes.Count; i++)
            {
                m_nodeViews[allNodes[i].Id].SerializedNode = m_nodeListProp.GetArrayElementAtIndex(i);
            }
        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            OnAddToSelection?.Invoke(selectable);
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            OnRemoveFromSelection?.Invoke(selectable);
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            OnClearSelection?.Invoke();
        }

        public void SetSelection(List<string> graphElementIds)
        {
            ClearSelection();
            for (int i = 0; i < graphElementIds.Count; i++)
            {
                NodeView nodeView = GetNodeViewById(graphElementIds[i]);
                if (nodeView != null)
                {
                    AddToSelection(nodeView);
                }
                else
                {
                    //its an edge
                    EdgeView edgeView = GetEdgeViewById(graphElementIds[i]);
                    if(edgeView != null)
                    {
                        AddToSelection(edgeView);
                        edgeView.RetainSelected = true; // Bandaid to Unity bug. See EdgeView.cs
                    }
                }
            }
        }

        private string CopyAndSerializeGraphElements(IEnumerable<GraphElement> elements)
        {
            GraphClipboardData data = new GraphClipboardData(m_nodeGraph, elements);
            return JsonUtility.ToJson(data, true);
        }
        
        private bool CanUnserializeAndPaste(string data)
        {
            GraphClipboardData clipboardData = null;
            try
            {
                clipboardData = JsonUtility.FromJson<GraphClipboardData>(data);
            }
            catch
            {
                return false;
            }
            return m_nodeGraph.GetType() == Type.GetType(clipboardData.GraphTypeName);
        }

        private void UnserializeAndPasteGraphElements(string operationName, string data)
        {
            GraphClipboardData copiedData = JsonUtility.FromJson<GraphClipboardData>(data);

            if(operationName == "Paste" || operationName == "Duplicate")
            {
                Undo.RegisterCompleteObjectUndo(m_nodeGraph, "Paste Graph Elements");

                Vector2 pos = m_mousePosition - new Vector2(contentViewContainer.transform.position.x, contentViewContainer.transform.position.y);
                List<ANode> clipboardNodes = SanitizeClipboardElements(copiedData.GetGraphElements(), pos);
                LoadSanitizedClipboardNodes(clipboardNodes);
            }
            else
            {
                Debug.Log("OTHER Operation " + operationName + "\n" + data);
            }
        }

        private List<ANode> SanitizeClipboardElements(List<ANode> clipboardElements, Vector2 newCenter)
        {
            // Reposition all elements around the new center
            Vector2 centerPos = Vector2.zero;
            for(int i = 0; i < clipboardElements.Count; i++)
            {
                centerPos += clipboardElements[i].Position;
            }
            centerPos /= clipboardElements.Count;

            // Generate new id's
            Dictionary<string, string> oldToNewIdList = new Dictionary<string, string>();
            for (int i = 0; i < clipboardElements.Count; i++)
            {
                oldToNewIdList.Add(clipboardElements[i].Id, Guid.NewGuid().ToString());
            }
            for (int i = 0; i < clipboardElements.Count; i++)
            {
                clipboardElements[i].SanitizeNodeCopy(oldToNewIdList[clipboardElements[i].Id],
                    clipboardElements[i].Position += newCenter - centerPos,
                    oldToNewIdList);
            }
            return clipboardElements;
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