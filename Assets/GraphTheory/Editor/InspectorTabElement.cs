using GraphTheory.Editor.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    public class InspectorTabElement : TabContentElement
    {
        private NodeGraph m_nodeGraph = null;
        private NodeInspector m_nodeInspector = null;
        private GraphInspector m_graphInspector = null;

        public InspectorTabElement(NodeGraphView nodeGraphView)
        {
            Add(m_graphInspector = new GraphInspector(nodeGraphView));
            Add(m_nodeInspector = new NodeInspector());
            m_nodeInspector.SetVisible(false);
        }

        public void SetOpenNodeGraph(NodeGraph nodeGraph)
        {
            if (nodeGraph == null)
            {
                Reset();
                return;
            }

            m_nodeGraph = nodeGraph;
            m_graphInspector.SetNodeGraph(nodeGraph);
        }

        private void Reset()
        {
            m_nodeGraph = null;
            m_nodeInspector.Reset();
            m_graphInspector.Reset();
        }

        public void SetNode(ANode node, SerializedProperty serializedNode)
        {
            m_graphInspector.SetVisible(node == null);
            m_nodeInspector.SetVisible(node != null);
            if (node != null)
            {
                m_nodeInspector.SetNode(node, serializedNode);
            }
        }



        public override void DeserializeData(string data)
        {
        }

        public override string GetSerializedData()
        {
            return ""; 
        }
        
        public class NodeInspector : VisualElement
        {
            private PropertyField m_propertyField = null;
            private IMGUIContainer m_imguiContainer = null;
            private SerializedProperty m_selectedNodeProperty = null;

            private VisualElement m_nodeTitleContainer = null;
            private Label m_nodeNameLabel = null;
            private Label m_nodeIdLabel = null;
            private TextField m_nodeCommentField = null;

            public NodeInspector()
            {
                // This can probably be broken out into its own uxml

                m_nodeTitleContainer = new VisualElement();
                m_nodeNameLabel = new Label("name");
                m_nodeNameLabel.style.fontSize = 40;
                m_nodeTitleContainer.Add(m_nodeNameLabel);
                Add(m_nodeTitleContainer);
                m_nodeIdLabel = new Label("id");
                m_nodeTitleContainer.Add(m_nodeIdLabel);

                m_nodeCommentField = new TextField();
                m_nodeCommentField.style.minHeight = 100;
                m_nodeCommentField.multiline = true;
                VisualElement textInput = m_nodeCommentField.Q<VisualElement>("unity-text-input");
                textInput.style.unityTextAlign = TextAnchor.UpperLeft;
                textInput.style.overflow = Overflow.Visible;
                textInput.style.whiteSpace = WhiteSpace.Normal;
                m_nodeTitleContainer.Add(m_nodeCommentField);

                m_imguiContainer = new IMGUIContainer();
                m_imguiContainer.onGUIHandler += OnIMGUIDraw;
                m_imguiContainer.style.display = DisplayStyle.None;
                Add(m_imguiContainer);

                m_propertyField = new PropertyField();
                m_propertyField.style.display = DisplayStyle.None;
                Add(m_propertyField);
            }

            public void SetVisible(bool visible)
            {
                style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }

            public void SetNode(ANode node, SerializedProperty serializedNode)
            {
                UnselectNode();
                if (node == null || serializedNode == null)
                {
                    return;
                }

                m_selectedNodeProperty = serializedNode;

                m_nodeTitleContainer.style.display = DisplayStyle.Flex;
                m_nodeNameLabel.text = node.GetType().Name;
                m_nodeIdLabel.text = node.Id;
                m_nodeCommentField.bindingPath = serializedNode.FindPropertyRelative("m_comment").propertyPath;
                m_nodeCommentField.Bind(serializedNode.serializedObject);
                
                if (node.UseIMGUIPropertyDrawer)
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
                    m_propertyField.Bind(m_selectedNodeProperty.serializedObject);
                    m_propertyField.style.display = DisplayStyle.Flex;
                    Add(m_propertyField);
                }
            }

            private void OnIMGUIDraw()
            {
                if (m_selectedNodeProperty == null)
                    return;

                m_selectedNodeProperty.serializedObject.Update();

                GUILayout.BeginVertical();
                EditorGUILayout.PropertyField(m_selectedNodeProperty, true);
                GUILayout.EndVertical();
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
                m_nodeTitleContainer.style.display = DisplayStyle.None;
            }

            public void Reset()
            {
                UnselectNode();
            }
        }
    }
}