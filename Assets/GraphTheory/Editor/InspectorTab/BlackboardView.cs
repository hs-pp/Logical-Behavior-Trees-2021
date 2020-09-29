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
    public class BlackboardView : Blackboard
    {
        private Dictionary<Type, Type> m_blackboardElementLookup = new Dictionary<Type, Type>();

        private BlackboardData m_blackboardData = null;
        private SerializedProperty m_serializedBlackboardDataElements = null;
        private List<BlackboardRow> m_allElementRows = new List<BlackboardRow>();

        public Action<BlackboardElement> OnAddBlackboardElement = null;
        public Action<BlackboardElement> OnRemoveBlackboardElement = null;
        public Action OnBlackboardElementNameChanged = null;

        public BlackboardView(NodeGraphView nodeGraphView)
        {
            windowed = true;
            graphView = nodeGraphView;
            addItemRequested += OnAddClicked;
            editTextRequested += EditBlackboardFieldName;
            Undo.undoRedoPerformed += () =>
            {
                if (m_serializedBlackboardDataElements != null)
                {
                    m_serializedBlackboardDataElements.serializedObject.Update();
                }

                ClearElements();
                LoadElements();
            };

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

                //TODO: sort the element types!!!

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
            m_serializedBlackboardDataElements = serializedGraph.FindProperty(NodeGraph.BlackboardData_VarName)
                .FindPropertyRelative(BlackboardData.AllElements_VarName);
            LoadElements();
        }

        private void Reset()
        {
            m_blackboardData = null;
            m_serializedBlackboardDataElements = null;
            ClearElements();
        }

        private void OnAddClicked(Blackboard blackboard)
        {
            if (m_serializedBlackboardDataElements == null || m_blackboardData == null)
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
            for (int i = 0; i < m_serializedBlackboardDataElements.arraySize; i++)
            {
                string name = m_serializedBlackboardDataElements.GetArrayElementAtIndex(i).FindPropertyRelative(BlackboardElement.Name_VarName).stringValue;
                BlackboardElement ele = m_blackboardData.GetElementByName(name);
                AddBlackboardRow(ele, m_serializedBlackboardDataElements.GetArrayElementAtIndex(i), i);
            }
        }

        private void ClearElements()
        {
            for (int i = 0; i < m_allElementRows.Count; i++)
            {
                Remove(m_allElementRows[i]);
            }
            m_allElementRows.Clear();
        }

        /// <summary>
        /// Deleting an element while there's another element that is a serializable class throws an "Unsupported Type (type)" error.
        /// This seems like a harmless error and is already tracked by Unity.
        /// https://issuetracker.unity3d.com/issues/deleting-an-element-of-list-with-serializereference-attribute-causes-an-error
        /// and its related issue https://issuetracker.unity3d.com/issues/serializereference-non-serialized-initialized-fields-lose-their-values-when-entering-play-mode
        /// </summary>
        /// <param name="index"></param>
        private void DeleteElement(int index)
        {
            OnRemoveBlackboardElement?.Invoke(m_blackboardData.GetElementAt(index));

            m_serializedBlackboardDataElements.DeleteArrayElementAtIndex(index);
            m_serializedBlackboardDataElements.serializedObject.ApplyModifiedProperties();
            ClearElements();
            LoadElements();
            Debug.LogWarning("If you see an \"Unsupported type\" error, you can ignore it. It's a Unity bug!\nClick into this log to read more!");
        }

        private void AddBlackboardRow(BlackboardElement blackboardEle, SerializedProperty serializedBlackboardEle, int index)
        {
            BlackboardElementView elementView = new BlackboardElementView(blackboardEle, serializedBlackboardEle, () => { DeleteElement(index); });

            PropertyField propF = new PropertyField(serializedBlackboardEle
                .FindPropertyRelative(BlackboardElement.ValueWrapper_VarName)
                .FindPropertyRelative(BlackboardElement.Value_VarName));
            propF.Bind(serializedBlackboardEle.serializedObject);

            BlackboardRow br = new BlackboardRow(elementView, propF);
            m_allElementRows.Add(br);
            Add(br);
        }

        private void AddNewElement(Type type)
        {
            BlackboardElement newElement = Activator.CreateInstance(m_blackboardElementLookup[type]) as BlackboardElement;
            m_blackboardData.AddElement(newElement);

            m_serializedBlackboardDataElements.serializedObject.Update();

            int lastIndex = m_serializedBlackboardDataElements.arraySize - 1;
            SerializedProperty serializedBlackboardElement = m_serializedBlackboardDataElements.GetArrayElementAtIndex(lastIndex);

            AddBlackboardRow(newElement, serializedBlackboardElement, lastIndex);

            Debug.Log("should happen here");
            OnAddBlackboardElement?.Invoke(newElement);
        }

        private void EditBlackboardFieldName(Blackboard blackboard, VisualElement blackboardElementView, string newName)
        {
            (blackboardElementView as BlackboardElementView).ChangeElementName(newName);
            OnBlackboardElementNameChanged?.Invoke();
        }
    }
}