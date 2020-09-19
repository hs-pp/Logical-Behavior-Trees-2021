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
        private BlackboardView m_blackboardView = null;

        public GraphInspector(NodeGraphView nodeGraphView)
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/GraphInspector");
            xmlAsset.CloneTree(this);

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

    public class BlackboardView : Blackboard
    {
        private Dictionary<Type, Type> m_blackboardElementLookup = new Dictionary<Type, Type>();

        private BlackboardData m_blackboardData = null;
        private SerializedProperty m_serializedBlackboardData = null;
        private List<BlackboardRow> m_allElementRows = new List<BlackboardRow>();

        public BlackboardView(NodeGraphView nodeGraphView)
        {
            windowed = true;
            graphView = nodeGraphView;
            addItemRequested += OnAddClicked;
            editTextRequested += EditBlackboardFieldName;

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
                    //Debug.Log(elementType.ToString() + " => " + blackboardElementImps[i].ToString());
                }
            }
        }

        public void SetNodeGraph(NodeGraph nodeGraph)
        {
            Reset();

            if (nodeGraph == null)
                return;
            

            m_blackboardData = nodeGraph.BlackboardData;
            SerializedObject serializedGraph = new SerializedObject(nodeGraph);
            m_serializedBlackboardData = serializedGraph.FindProperty("m_blackboardData");
            LoadElements();
        }

        private void Reset()
        {
            m_blackboardData = null;
            m_serializedBlackboardData = null;
            ClearElements();
        }

        private void OnAddClicked(Blackboard blackboard)
        {
            if(m_serializedBlackboardData == null || m_blackboardData == null)
            {
                return;
            }

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

        private void LoadElements()
        {
            SerializedProperty allElements = m_serializedBlackboardData.FindPropertyRelative("m_allElements");
            for(int i = 0; i < allElements.arraySize; i++)
            {
                string name = allElements.GetArrayElementAtIndex(i).FindPropertyRelative("m_name").stringValue;
                BlackboardElement ele = m_blackboardData.GetElement(name);
                AddBlackboardRow(ele, allElements.GetArrayElementAtIndex(i));
            }
        }

        private void ClearElements()
        {
            for(int i = 0; i < m_allElementRows.Count; i++)
            {
                Remove(m_allElementRows[i]);
            }
            m_allElementRows.Clear();
        }

        private void AddBlackboardRow(BlackboardElement blackboardEle, SerializedProperty serializedBlackboardEle)
        {
            BlackboardFieldV2 bf = new BlackboardFieldV2(blackboardEle, serializedBlackboardEle);
            
            PropertyField propF = new PropertyField(serializedBlackboardEle.FindPropertyRelative("m_valueWrapper").FindPropertyRelative("value"));
            propF.Bind(serializedBlackboardEle.serializedObject);

            BlackboardRow br = new BlackboardRow(bf, propF);
            m_allElementRows.Add(br);
            Add(br);
        }

        private void AddNewElement(Type type)
        {
            BlackboardElement newElement = Activator.CreateInstance(m_blackboardElementLookup[type]) as BlackboardElement;
            m_blackboardData.AddElement(newElement);

            m_serializedBlackboardData.serializedObject.Update();
            SerializedProperty serializedBlackboardElement = m_serializedBlackboardData
                .FindPropertyRelative("m_allElements")
                .GetArrayElementAtIndex(m_serializedBlackboardData.FindPropertyRelative("m_allElements").arraySize -1);

            AddBlackboardRow(newElement, serializedBlackboardElement);
        }

        private void EditBlackboardFieldName(Blackboard blackboard, VisualElement blackboardField, string newName)
        {
            (blackboardField as BlackboardFieldV2).ChangeElementName(newName);
        }
    }

    public class BlackboardFieldV2 : BlackboardField
    {
        private BlackboardElement m_blackboardElement = null;
        private SerializedProperty m_serializedBlackboardElement = null;

        public BlackboardFieldV2(BlackboardElement blackboardElement, SerializedProperty serializedBlackboardElement)
        {
            m_blackboardElement = blackboardElement;
            m_serializedBlackboardElement = serializedBlackboardElement;

            text = m_blackboardElement.Name;
            typeText = m_blackboardElement.Type.Name;

            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
        }

        public void ChangeElementName(string newName)
        {
            m_serializedBlackboardElement.FindPropertyRelative("m_name").stringValue = newName;
            m_serializedBlackboardElement.serializedObject.ApplyModifiedProperties();
            text = newName;
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", (action) => { });
            evt.StopPropagation();
        }
    }
}