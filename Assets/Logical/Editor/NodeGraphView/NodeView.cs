using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Logical.Editor
{
    /// <summary>
    /// The visual representation of a single node withing a node graph in the editor window.
    /// </summary>
    public class NodeView : Node
    {
        public ANode Node { get; private set; } = null;
        public Type NodeType { get; private set; } = null;
        public NodeGraphView m_nodeGraphView = null;
        public SerializedProperty SerializedNode { get; set; } = null;
        public string NodeId { get; private set; } = null;

        private NodeDisplayContainers m_nodeDisplayContainers = null;
        private PortView m_inport = null;
        private List<PortView> m_outports = new List<PortView>();
        private Dictionary<string, EdgeView> m_edgeViews = new Dictionary<string, EdgeView>();
        private IEdgeConnectorListener m_edgeConnectorListener = null;
        private NodeViewDrawer m_nodeViewDrawer = null;

        public NodeView(ANode node,
            SerializedProperty serializedNode, 
            NodeGraphView nodeGraphView, 
            IEdgeConnectorListener edgeConnectorListener,
            NodeViewDrawer nodeViewDrawer) : base()
        {
            if(serializedNode == null)
            {
                return;
            }

            Node = node;
            NodeType = Node.GetType();
            SerializedNode = serializedNode;
            m_nodeGraphView = nodeGraphView;
            m_edgeConnectorListener = edgeConnectorListener;

            NodeId = SerializedNode.FindPropertyRelative(ANode.IdVarname).stringValue;

            m_nodeViewDrawer = nodeViewDrawer;
            m_nodeDisplayContainers = new NodeDisplayContainers(this, m_nodeViewDrawer);
            m_nodeViewDrawer.SetNodeView(this, SerializedNode, nodeGraphView.NodeGraph, m_nodeDisplayContainers);
            m_nodeViewDrawer.OnSetup();

            title = m_nodeViewDrawer.DisplayName;

            bool isEntryNode = (typeof(BuiltInNodes.EntryNode)).IsAssignableFrom(NodeType);
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
                    m_nodeViewDrawer.NodeColor,
                    m_edgeConnectorListener);
                m_inport.portName = "";
                m_nodeDisplayContainers.SetInport(m_inport);
            }

            CreateOutports();

            // Draw node
            m_nodeViewDrawer.Repaint(m_outports);

            RefreshExpandedState();
            RefreshPorts();

            Vector2 pos = SerializedNode.FindPropertyRelative(ANode.PositionVarName).vector2Value;
            SetPosition(new Rect(pos, Vector2.zero));

            //this.RegisterCallback<GeometryChangedEvent>((GeometryChangedEvent gce) => { Debug.Log(gce.newRect.position); });
        }

        public void RepaintNodeView()
        {
            SerializedNode.serializedObject.Update();
            
            CreateOutports();
            m_nodeViewDrawer.Repaint(m_outports);
            OnUnloadView();
            OnLoadView();
        }

        protected override void ToggleCollapse()
        {
            base.ToggleCollapse();
            m_nodeDisplayContainers.ResolveCollapsedPorts();
        }

        public void OnLoadView()
        {
            m_nodeGraphView.OnBlackboardElementChanged += HandleOnBlackboardElementChanged;

            SerializedProperty outports = SerializedNode.FindPropertyRelative(ANode.OutportsVarName);

            for (int k = 0; k < outports.arraySize; k++)
            {
                SerializedProperty outportProp = outports.GetArrayElementAtIndex(k);
                if (!OutportEdge.IsValid(outportProp))
                {
                    continue;
                }
                EdgeView edgeView = new EdgeView()
                {
                    OutportEdgeProp = outportProp,
                    input = m_nodeGraphView.GetNodeViewById(outportProp.FindPropertyRelative(OutportEdge.ConnectedNodeIdVarName).stringValue).m_inport,
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
                    if (edgeView.SecondPort != null)
                    {
                        edgeView.SecondPort.Disconnect(edgeView);
                    }
                }
            }
            m_edgeViews.Clear();

            m_nodeGraphView.OnBlackboardElementChanged -= HandleOnBlackboardElementChanged;
        }

        public void OnDeleteNode()
        {
            List<EdgeView> edgeViewsCopy = new List<EdgeView>(m_edgeViews.Values);
            for (int i = 0; i < edgeViewsCopy.Count; i++)
            {
                RemoveEdge(edgeViewsCopy[i]);
            }
        }

        public void CreateOutports()
        {
            m_outports.Clear();
            for (int j = 0; j < SerializedNode.FindPropertyRelative(ANode.OutportsVarName).arraySize; j++)
            {
                PortView newPort = new PortView(this,
                    Orientation.Horizontal,
                    Direction.Output,
                    Port.Capacity.Single,
                    typeof(bool),
                    j,
                    m_nodeViewDrawer.NodeColor,
                    m_edgeConnectorListener);
                newPort.portName = "";
                m_outports.Add(newPort);
            }
        }

        public bool OutportHasEdge(int outportIndex)
        {
            return OutportEdge.IsValid(SerializedNode.FindPropertyRelative(ANode.OutportsVarName).GetArrayElementAtIndex(outportIndex));
        }

        public PortView GetOutport(int outportIndex)
        {
            return m_outports[outportIndex];
        }

        public void AddEdge(EdgeView edgeView)
        {
            if (edgeView.FirstPort.Node == this)
            {
                SerializedProperty outportsProp = SerializedNode.FindPropertyRelative(ANode.OutportsVarName);
                SerializedProperty newOutportProp = outportsProp.GetArrayElementAtIndex(edgeView.FirstPort.PortIndex);
                newOutportProp.FindPropertyRelative(OutportEdge.ConnectedNodeIdVarName).stringValue = edgeView.SecondPort.Node.NodeId;
                newOutportProp.serializedObject.ApplyModifiedProperties();
                edgeView.OutportEdgeProp = newOutportProp;
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
                m_nodeGraphView.RemoveElement(edgeView);
                m_edgeViews.Remove(edgeView.EdgeId);
                OutportEdge.SetInvalid(SerializedNode.FindPropertyRelative(ANode.OutportsVarName)
                    .GetArrayElementAtIndex(edgeView.FirstPort.PortIndex));
            }
            else if (edgeView.SecondPort.Node == this)
            {
                edgeView.SecondPort.Disconnect(edgeView);
            }
        }

        public EdgeView GetEdgeViewById(string id)
        {
            if(m_edgeViews.ContainsKey(id))
            {
                return m_edgeViews[id];
            }
            return null;
        }

        public EdgeView GetEdgeViewByPortIndex(int outportIndex)
        {
            string edgeViewId = SerializedNode
                .FindPropertyRelative(ANode.OutportsVarName)
                .GetArrayElementAtIndex(outportIndex)
                .FindPropertyRelative(OutportEdge.IdVarName)
                .stringValue;

            return m_edgeViews[edgeViewId];
        }

        public void UpdateNodeDataPosition()
        {
            SerializedNode.FindPropertyRelative(ANode.PositionVarName).vector2Value = this.GetPosition().position;
            SerializedNode.serializedObject.ApplyModifiedProperties();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            // Add ability to open the node's script file
            // This isn't very performant but is there a better way?
            evt.menu.AppendAction("Open Node Script", (menuAction) => 
            {
                string nodeTypeName = NodeType.Name;
                IEnumerable<string> scriptPaths = AssetDatabase.FindAssets($"t:script {nodeTypeName}").Select(AssetDatabase.GUIDToAssetPath);
                foreach (string path in scriptPaths)
                {
                    if (Path.GetFileName(path) == $"{nodeTypeName}.cs")
                    {
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 0);
                        return;
                    }
                }
                Debug.LogError("Script not found. Is your node class in it's own script with its own name?");
            });
            evt.menu.AppendAction("Open NodeViewDrawer Script", (menuAction) =>
            {
                Type nodeViewDrawerType = m_nodeGraphView.GraphTypeMetadata.GetNodeViewDrawerType(NodeType);
                if(nodeViewDrawerType != typeof(NodeViewDrawer))
                {
                    string typeName = nodeViewDrawerType.Name;
                    IEnumerable<string> scriptPaths = AssetDatabase.FindAssets($"t:script {typeName}").Select(AssetDatabase.GUIDToAssetPath);
                    foreach (string path in scriptPaths)
                    {
                        if (Path.GetFileName(path) == $"{typeName}.cs")
                        {
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 0);
                            return;
                        }
                    }
                    Debug.LogError("Script not found. Is your nodeviewdrawer class in it's own script with its own name?");
                }
            });
        }

        /// <summary>
        /// This method is used to get all child graph elements that need to be deleted when the node is deleted.
        /// In our case, we must grab all the connected edges manually because the port elements are not where
        /// they are expected to be and the base method is unable to find them.
        /// This method does not exist in Unity 2019 which will make supporting older Unity versions very tough.
        /// </summary>
        public override void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
        {
            collectedElementSet.UnionWith(m_nodeDisplayContainers.GetAllPorts().SelectMany(c => c.connections)
                .Where(d => (d.capabilities & Capabilities.Deletable) != 0)
                .Cast<GraphElement>());

            // Base code:
            //collectedElementSet.UnionWith(inputContainer.Children().OfType<Port>().SelectMany(c => c.connections)
            //    .Where(d => (d.capabilities & Capabilities.Deletable) != 0)
            //    .Cast<GraphElement>());
            //collectedElementSet.UnionWith(outputContainer.Children().OfType<Port>().SelectMany(c => c.connections)
            //    .Where(d => (d.capabilities & Capabilities.Deletable) != 0)
            //    .Cast<GraphElement>());
        }

        public void HandleOnBlackboardElementChanged(int undoGroup)
        {
            m_nodeViewDrawer?.OnBlackboardElementChanged?.Invoke(undoGroup);
        }

        public void HandleOnSerializedPropertyChanged()
        {
            m_nodeViewDrawer.OnSerializedPropertyChanged?.Invoke();
        }
    }
}