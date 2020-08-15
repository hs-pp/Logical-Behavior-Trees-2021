using GraphTheory.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    /// <summary>
    /// TODO:
    /// 1. IMGUI/UIElements switch bool
    /// 2. Bind() should be renamed
    /// 3.
    /// </summary>
    public class InspectorTabElement : TabContentElement
    {
        private NodeGraph m_nodeGraph = null;
        private SerializedObject m_nodeGraphSO = null;
        private PropertyField m_propertyField = null;
        private IMGUIContainer m_imguiContainer = null;
        private SerializedProperty m_selectedNodeProperty = null;

        private VisualElement m_nodeTitleContainer = null;
        private Label m_nodeNameLabel = null;
        private Label m_nodeIdLabel = null;
        private TextField m_nodeCommentField = null;

        public InspectorTabElement()
        {
            m_imguiContainer = new IMGUIContainer();
            m_imguiContainer.onGUIHandler += OnIMGUIDraw;
            m_imguiContainer.style.display = DisplayStyle.None;
            m_imguiContainer.StretchToParentSize();
            Add(m_imguiContainer);
            m_propertyField = new PropertyField();
            m_propertyField.style.display = DisplayStyle.None;
            Add(m_propertyField);

            m_nodeTitleContainer = new VisualElement();
            m_nodeNameLabel = new Label("INdpectedsr");
            m_nodeNameLabel.style.fontSize = 40;
            m_nodeTitleContainer.Add(m_nodeNameLabel);
            Add(m_nodeTitleContainer);
            m_nodeIdLabel = new Label("983687938434");
            m_nodeTitleContainer.Add(m_nodeIdLabel);

            m_nodeCommentField = new TextField();
            m_nodeCommentField.style.minHeight = 100;
            m_nodeCommentField.multiline = true;
            VisualElement textInput = m_nodeCommentField.Q<VisualElement>("unity-text-input");
            textInput.style.unityTextAlign = TextAnchor.UpperLeft;
            textInput.style.overflow = Overflow.Visible;
            textInput.style.whiteSpace = WhiteSpace.Normal;
            m_nodeTitleContainer.Add(m_nodeCommentField);
        }

        public void SetOpenNodeGraph(NodeGraph nodeGraph)
        {
            if(nodeGraph == null)
            {
                Reset();
                return;
            }

            m_nodeGraph = nodeGraph;
            m_nodeGraphSO = new SerializedObject(m_nodeGraph);
        }

        private void Reset()
        {
            m_nodeGraph = null;
            m_nodeGraphSO = null;
            UnselectNode();
        }

        private void UnselectNode()
        {
            m_selectedNodeProperty = null;
            if (m_imguiContainer != null)
            {
                m_imguiContainer.style.display = DisplayStyle.None;
            }
            if (m_propertyField != null)
            {
                m_propertyField.style.display = DisplayStyle.None;
            }
        }

        public void SetNode(string nodeId)
        {
            if(m_nodeGraphSO == null)
            {
                Debug.LogError("NodeGraph not set!");
                return;
            }

            UnselectNode();
            if (string.IsNullOrEmpty(nodeId))
            {
                return;
            }
            
            SerializedProperty nodeCollection = m_nodeGraphSO.FindProperty("m_nodeCollection");
            SerializedProperty nodes = nodeCollection.FindPropertyRelative("m_nodes");
            for(int i = 0; i < nodes.arraySize; i++) //TODO: Make this more efficient!!
            {
                if(nodeId == nodes.GetArrayElementAtIndex(i).FindPropertyRelative("m_id").stringValue)
                {
                    m_selectedNodeProperty = nodes.GetArrayElementAtIndex(i);
                    break;
                }
            }

            if(m_selectedNodeProperty == null)
            {
                return;
            }

            bool useIMGUI = false;
            if(useIMGUI)
            {
                m_imguiContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (m_propertyField != null)
                {
                    Remove(m_propertyField);
                }
                m_propertyField = new PropertyField(m_selectedNodeProperty);
                m_propertyField.Bind(m_nodeGraphSO);
                m_propertyField.style.display = DisplayStyle.Flex;
                Add(m_propertyField);
            }
        }

        private void OnIMGUIDraw()
        {
            if (m_selectedNodeProperty == null)
                return;

            m_nodeGraphSO.Update();

            GUILayout.BeginVertical();
            EditorGUILayout.PropertyField(m_selectedNodeProperty, true);
            GUILayout.EndVertical();
        }

        public override void DeserializeData(string data)
        {
        }

        public override string GetSerializedData()
        {
            return ""; 
        }
    }
}