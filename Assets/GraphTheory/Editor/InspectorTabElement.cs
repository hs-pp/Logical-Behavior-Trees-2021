using GraphTheory.Editor.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

        public class GraphInspector : VisualElement
        {
            private SerializedObject m_nodeGraphSO = null;
            private SerializedProperty m_graphPropertiesProp = null;
            private PropertyField m_propertyField = null;
            private IMGUIContainer m_imguiContainer = null;
            private BlackboardContainer m_blackboardContainer = null;

            public GraphInspector(NodeGraphView nodeGraphView)
            {
                m_imguiContainer = new IMGUIContainer();
                m_imguiContainer.onGUIHandler += OnIMGUIDraw;
                m_propertyField = new PropertyField();
                m_blackboardContainer = new BlackboardContainer(nodeGraphView);
                Add(m_blackboardContainer);
            }

            public void SetNodeGraph(NodeGraph nodeGraph)
            {
                Reset();

                if (nodeGraph == null)
                    return;

                m_nodeGraphSO = new SerializedObject(nodeGraph);
                m_graphPropertiesProp = m_nodeGraphSO.FindProperty("GraphProperties");
                if (nodeGraph.UseIMGUIPropertyDrawer)
                {
                    m_imguiContainer.Bind(m_nodeGraphSO);
                    Insert(0, m_imguiContainer);
                }
                else
                {
                    m_propertyField = new PropertyField(m_graphPropertiesProp);
                    m_propertyField.Bind(m_nodeGraphSO);
                    Insert(0, m_propertyField);
                }
            }

            public void Reset()
            {
                m_nodeGraphSO = null;
                m_graphPropertiesProp = null;
                if (m_propertyField != null && m_propertyField.parent == this)
                {
                    Remove(m_propertyField);
                    m_propertyField.Bind(null);
                    m_propertyField = null;
                }
                if (m_imguiContainer.parent == this)
                {
                    Remove(m_imguiContainer);
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

        public class BlackboardContainer : VisualElement
        {
            private Blackboard m_blackboard = null;
            private BlackboardData m_blackboardData = new BlackboardData();
            private Dictionary<Type, Type> m_blackboardElementLookup = new Dictionary<Type, Type>();
            
            public BlackboardContainer(NodeGraphView nodeGraphView)
            {
                name = "blackboardcontainer";
                m_blackboard = new Blackboard();
                m_blackboard.windowed = true;
                m_blackboard.graphView = nodeGraphView;
                m_blackboard.addItemRequested += OnAddClicked;
                Add(m_blackboard);

                List<Type> blackboardElementImps = new List<Type>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    blackboardElementImps.AddRange(assemblies[i].GetTypes().Where(x 
                        => typeof(BlackboardElement).IsAssignableFrom(x)
                        && x != typeof(BlackboardElement)
                        && !x.IsAbstract));
                }

                for(int i = 0; i < blackboardElementImps.Count; i++)
                {
                    BlackboardElementTypeAttribute attr = blackboardElementImps[i].GetCustomAttribute<BlackboardElementTypeAttribute>();

                    if (attr == null)
                        continue;

                    Type elementType = attr.ElementType;
                    if(!m_blackboardElementLookup.ContainsKey(elementType))
                    {
                        m_blackboardElementLookup.Add(elementType, blackboardElementImps[i]);
                        Debug.Log(elementType.ToString() + " => " + blackboardElementImps[i].ToString());
                    }
                }
            }

            private void OnAddClicked(Blackboard blackboard)
            {
                GenericMenu menu = new GenericMenu();
                foreach(Type supportedType in m_blackboardElementLookup.Keys)
                {
                    menu.AddItem(new GUIContent(supportedType.Name), false, () =>
                    {
                        AddNewElement(supportedType);
                    });
                }
                menu.ShowAsContext();
            }

            private void AddNewElement(Type type)
            {
                BlackboardElement newElement = Activator.CreateInstance(m_blackboardElementLookup[type]) as BlackboardElement;
                BlackboardField bf = new BlackboardField { text = newElement.Name, typeText = newElement.Type.Name };
                VisualElement descRow = new VisualElement();
                TextField propertyVal = new TextField("Value:");
                propertyVal.value = "yo";
                descRow.Add(propertyVal);
                Button deleteButton = new Button { text = "Delete" };
                descRow.Add(deleteButton);
                BlackboardRow br = new BlackboardRow(bf, descRow);
                m_blackboard.Add(br);
            }
        }

    }
}