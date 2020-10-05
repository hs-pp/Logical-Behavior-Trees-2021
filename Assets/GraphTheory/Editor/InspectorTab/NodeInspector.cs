using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    public class NodeInspector : VisualElement
    {
        private static readonly string NODE_NAME_LABEL = "node-name-label";
        private static readonly string NODE_ID_LABEL = "node-id-label";
        private static readonly string COMMENT_FIELD = "comment-field";
        private static readonly string COMMENT_PLACEHOLDER = "comment-placeholder";
        private static readonly string INSPECTOR_AREA = "inspector-area";

        private NodeGraphView m_nodeGraphView = null;
        private PropertyField m_propertyField = null;
        private IMGUIContainer m_imguiContainer = null;
        private SerializedProperty m_selectedNodeProperty = null;

        private Label m_nodeNameLabel = null;
        private Label m_nodeIdLabel = null;
        private TextField m_nodeCommentField = null;
        private Label m_commentPlaceholder = null;
        private VisualElement m_inspectorArea = null;

        public NodeInspector(NodeGraphView nodeGraphView)
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/NodeInspector");
            xmlAsset.CloneTree(this);

            // This can probably be broken out into its own uxml
            m_nodeGraphView = nodeGraphView;

            m_nodeNameLabel = this.Q<Label>(NODE_NAME_LABEL);
            m_nodeIdLabel = this.Q<Label>(NODE_ID_LABEL);

            m_nodeCommentField = this.Q<TextField>(COMMENT_FIELD);
            VisualElement textInput = m_nodeCommentField.Q<VisualElement>("unity-text-input");
            textInput.style.unityTextAlign = TextAnchor.UpperLeft;
            textInput.style.overflow = Overflow.Visible;
            textInput.style.whiteSpace = WhiteSpace.Normal;
            m_nodeCommentField.isDelayed = true;

            m_nodeCommentField.RegisterCallback<ChangeEvent<string>>((str) => { OnCommentBoxFocusOut(); });
            m_nodeCommentField.RegisterCallback<FocusInEvent>((target) => { OnCommentBoxFocusIn(); });

            m_commentPlaceholder = this.Q<Label>(COMMENT_PLACEHOLDER);
            OnCommentBoxFocusOut();

            m_inspectorArea = this.Q<VisualElement>(INSPECTOR_AREA);

            m_imguiContainer = new IMGUIContainer();
            m_imguiContainer.onGUIHandler += OnIMGUIDraw;
            m_imguiContainer.style.display = DisplayStyle.None;
            m_inspectorArea.Add(m_imguiContainer);

            m_propertyField = new PropertyField();
            m_propertyField.style.display = DisplayStyle.None;
            m_inspectorArea.Add(m_propertyField);
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
            
            m_nodeNameLabel.text = AddSpacesToSentence(node.GetType().Name);
            m_nodeIdLabel.text = node.Id;
            m_nodeCommentField.bindingPath = serializedNode.FindPropertyRelative("m_comment").propertyPath;
            m_nodeCommentField.Bind(serializedNode.serializedObject);

            m_commentPlaceholder.style.display = (string.IsNullOrEmpty(serializedNode.FindPropertyRelative("m_comment").stringValue))
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            if (node.UseIMGUIPropertyDrawer)
            {
                m_imguiContainer.style.display = DisplayStyle.Flex;

            }
            else
            {
                if (m_propertyField != null)
                {
                    m_inspectorArea.Remove(m_propertyField);
                }
                m_propertyField = new PropertyField(m_selectedNodeProperty);
                m_propertyField.Bind(m_selectedNodeProperty.serializedObject);
                m_propertyField.style.display = DisplayStyle.Flex;

                // This requires Unity 2020.2 to work correctly https://forum.unity.com/threads/uielements-developer-guide.648043/#post-6073137
                m_propertyField.RegisterCallback<SerializedPropertyChangeEvent>(x =>
                {
                    m_nodeGraphView.GetNodeViewById(node.Id).HandleOnSerializedPropertyChanged();
                });

                m_inspectorArea.Add(m_propertyField);
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
        }

        public void Reset()
        {
            UnselectNode();
        }

        private string AddSpacesToSentence(string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        private void OnCommentBoxFocusIn()
        {
            m_commentPlaceholder.style.display = DisplayStyle.None;
        }

        private void OnCommentBoxFocusOut()
        {
            if(m_selectedNodeProperty == null)
            {
                return;
            }

            if(string.IsNullOrEmpty(m_nodeCommentField.value))
            {
                m_commentPlaceholder.style.display = DisplayStyle.Flex;
            }
        }
    }
}