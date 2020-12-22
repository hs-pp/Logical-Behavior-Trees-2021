using Logical.Editor.UIElements;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Logical.Editor
{
    /// <summary>
    /// This inspector is shown when the LogicalGraphWindow has the Inspector tab selected and no graph elements inside
    /// the graph grid selected. (Tap the grid, not the node elements)
    /// Simply, it shows the selected graph's graph properties and then the graph's blackboard data.
    /// </summary>
    public class GraphInspector : VisualElement
    {
        private static readonly string GRAPH_NAME_LABEL = "graph-name-label";
        private static readonly string GRAPH_OBJECT_FIELD = "opened-graph-field";
        private static readonly string GRAPH_PROPERTIES_AREA = "graph-properties-area";
        private static readonly string BLACKBOARD_AREA = "blackboard-area";

        private Label m_graphNameLabel = null;
        private ObjectDisplayField m_graphObjectField = null;
        private VisualElement m_graphPropertiesArea = null;
        private VisualElement m_blackboardArea = null;
        private SerializedObject m_nodeGraphSO = null;
        private SerializedProperty m_graphPropertiesProp = null;
        private PropertyField m_propertyField = null;
        private IMGUIContainer m_imguiContainer = null;
        private BlackboardView m_blackboardView = null;

        public Action<int> OnBlackboardElementChanged { get { return m_blackboardView.OnBlackboardElementChanged; } set { m_blackboardView.OnBlackboardElementChanged = value; } }

        public GraphInspector(NodeGraphView nodeGraphView)
        {
            var uxmlAsset = Resources.Load<VisualTreeAsset>(ResourceAssetPaths.GraphInspector_UXML);
            uxmlAsset.CloneTree(this);

            m_graphNameLabel = this.Q<Label>(GRAPH_NAME_LABEL);
            m_graphObjectField = this.Q<ObjectDisplayField>(GRAPH_OBJECT_FIELD);

            m_graphPropertiesArea = this.Q<VisualElement>(GRAPH_PROPERTIES_AREA);
            m_propertyField = new PropertyField();
            m_imguiContainer = new IMGUIContainer();
            m_imguiContainer.onGUIHandler += OnIMGUIDraw;

            m_blackboardArea = this.Q<VisualElement>(BLACKBOARD_AREA);
            m_blackboardView = new BlackboardView(nodeGraphView);
            m_blackboardArea.Add(m_blackboardView);
        }

        public void SetNodeGraph(NodeGraph nodeGraph)
        {
            Reset();

            if (nodeGraph == null)
                return;

            m_nodeGraphSO = new SerializedObject(nodeGraph);
            m_graphPropertiesProp = m_nodeGraphSO.FindProperty(NodeGraph.GraphProperties_VarName);

            m_graphNameLabel.text = nodeGraph.name;
            m_graphObjectField.SetObject(nodeGraph);

            if (nodeGraph.UseIMGUIPropertyDrawer)
            {
                m_imguiContainer.Bind(m_nodeGraphSO);
                m_graphPropertiesArea.Add(m_imguiContainer);
            }
            else
            {
                m_propertyField = new PropertyField(m_graphPropertiesProp);
                m_propertyField.Bind(m_nodeGraphSO);
                m_graphPropertiesArea.Add(m_propertyField);
            }

            m_blackboardView.SetNodeGraph(nodeGraph);
        }

        public void Reset()
        {
            m_nodeGraphSO = null;
            m_graphPropertiesProp = null;
            if (m_propertyField != null && m_propertyField.parent == m_graphPropertiesArea)
            {
                m_graphPropertiesArea.Remove(m_propertyField);
                m_propertyField.Bind(null);
                m_propertyField = null;
            }
            if (m_imguiContainer.parent == m_graphPropertiesArea)
            {
                m_graphPropertiesArea.Remove(m_imguiContainer);
                m_imguiContainer.Bind(null);
            }
        }

        private void OnIMGUIDraw()
        {
            if (m_nodeGraphSO == null)
                return;

            m_nodeGraphSO.Update();

            GUILayout.BeginVertical();
            EditorGUILayout.PropertyField(m_graphPropertiesProp);
            GUILayout.EndVertical();
        }

        public void SetVisible(bool visible)
        {
            style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}