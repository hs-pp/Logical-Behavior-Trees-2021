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
    public class GraphInspector : VisualElement
    {
        private static readonly string GRAPH_PROPERTIES_AREA = "graph-properties-area";
        private static readonly string BLACKBOARD_AREA = "blackboard-area";

        private VisualElement m_graphPropertiesArea = null;
        private VisualElement m_blackboardArea = null;
        private SerializedObject m_nodeGraphSO = null;
        private SerializedProperty m_graphPropertiesProp = null;
        private PropertyField m_propertyField = null;
        private IMGUIContainer m_imguiContainer = null;
        private BlackboardContainer m_blackboardContainer = null;

        public GraphInspector(NodeGraphView nodeGraphView)
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/GraphInspector");
            xmlAsset.CloneTree(this);

            m_graphPropertiesArea = this.Q<VisualElement>(GRAPH_PROPERTIES_AREA);
            m_propertyField = new PropertyField();
            m_imguiContainer = new IMGUIContainer();
            m_imguiContainer.onGUIHandler += OnIMGUIDraw;

            m_blackboardArea = this.Q<VisualElement>(BLACKBOARD_AREA);
            m_blackboardContainer = new BlackboardContainer(nodeGraphView);
            m_blackboardArea.Add(m_blackboardContainer);
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
                m_graphPropertiesArea.Add(m_imguiContainer);
            }
            else
            {
                m_propertyField = new PropertyField(m_graphPropertiesProp);
                m_propertyField.Bind(m_nodeGraphSO);
                m_graphPropertiesArea.Add(m_propertyField);
            }
        }

        public void Reset()
        {
            m_nodeGraphSO = null;
            m_graphPropertiesProp = null;
            if (m_propertyField != null && m_propertyField.parent == this)
            {
                m_graphPropertiesArea.Remove(m_propertyField);
                m_propertyField.Bind(null);
                m_propertyField = null;
            }
            if (m_imguiContainer.parent == this)
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

            for (int i = 0; i < blackboardElementImps.Count; i++)
            {
                BlackboardElementTypeAttribute attr = blackboardElementImps[i].GetCustomAttribute<BlackboardElementTypeAttribute>();

                if (attr == null)
                    continue;

                Type elementType = attr.ElementType;
                if (!m_blackboardElementLookup.ContainsKey(elementType))
                {
                    m_blackboardElementLookup.Add(elementType, blackboardElementImps[i]);
                    Debug.Log(elementType.ToString() + " => " + blackboardElementImps[i].ToString());
                }
            }
        }

        private void OnAddClicked(Blackboard blackboard)
        {
            GenericMenu menu = new GenericMenu();
            foreach (Type supportedType in m_blackboardElementLookup.Keys)
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