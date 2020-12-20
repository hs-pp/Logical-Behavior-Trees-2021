using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Logical.Editor
{
    /// <summary>
    /// This view displays the whole Blackboard as well as the blackboard elements.
    /// The desired UI features of the Blackboard view are already mostly done by Unity.
    /// This class mainly is functionality to wire our NodeGraph's BlackboardProperties into the 
    /// Blackboard UI with the assumption that the UI will just kinda work.
    /// </summary>
    public class BlackboardView : Blackboard
    {
        private Dictionary<Type, Type> m_blackboardElementLookup = new Dictionary<Type, Type>();

        private BlackboardProperties m_blackboardProperties = null;
        private SerializedProperty m_serializedBlackboardElements = null;
        private List<BlackboardRow> m_allElementRows = new List<BlackboardRow>();

        public Action<int> OnBlackboardElementChanged = null;

        public BlackboardView(NodeGraphView nodeGraphView)
        {
            windowed = true;
            graphView = nodeGraphView;
            addItemRequested += OnAddClicked;
            editTextRequested += EditBlackboardFieldName;
            Undo.undoRedoPerformed += () =>
            {
                if (m_serializedBlackboardElements != null)
                {
                    m_serializedBlackboardElements.serializedObject.Update();
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

            m_blackboardProperties = nodeGraph.BlackboardProperties;
            SerializedObject serializedGraph = new SerializedObject(nodeGraph);
            m_serializedBlackboardElements = serializedGraph.FindProperty(NodeGraph.BlackboardProperties_VarName)
                .FindPropertyRelative(BlackboardProperties.AllElements_VarName);
            LoadElements();
        }

        private void Reset()
        {
            m_blackboardProperties = null;
            m_serializedBlackboardElements = null;
            ClearElements();
        }

        private void OnAddClicked(Blackboard blackboard)
        {
            if (m_serializedBlackboardElements == null || m_blackboardProperties == null)
            {
                return;
            }

            GenericMenu menu = new GenericMenu();
            foreach (Type supportedType in m_blackboardElementLookup.Keys)
            {
                string name = supportedType.Name;
                if(name == "Single")
                {
                    name = "Float";
                }
                else if(name == "Int32")
                {
                    name = "Int";
                }
                menu.AddItem(new GUIContent(name), false, () =>
                {
                    AddNewElement(supportedType);
                });
            }
            menu.ShowAsContext();
        }

        private void LoadElements()
        {
            for (int i = 0; i < m_serializedBlackboardElements.arraySize; i++)
            {
                string name = m_serializedBlackboardElements.GetArrayElementAtIndex(i).FindPropertyRelative(BlackboardElement.Name_VarName).stringValue;
                BlackboardElement ele = m_blackboardProperties.GetElementByName(name);
                AddBlackboardRow(ele, m_serializedBlackboardElements.GetArrayElementAtIndex(i), i);
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
            int undoGroup = Undo.GetCurrentGroup();
            m_serializedBlackboardElements.serializedObject.Update();
            m_serializedBlackboardElements.DeleteArrayElementAtIndex(index);
            m_serializedBlackboardElements.serializedObject.ApplyModifiedProperties();
            OnBlackboardElementChanged?.Invoke(undoGroup);
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
            int undoGroup = Undo.GetCurrentGroup();
            BlackboardElement newElement = Activator.CreateInstance(m_blackboardElementLookup[type]) as BlackboardElement;
            m_blackboardProperties.AddElement(newElement);

            m_serializedBlackboardElements.serializedObject.Update();

            int lastIndex = m_serializedBlackboardElements.arraySize - 1;
            SerializedProperty serializedBlackboardElement = m_serializedBlackboardElements.GetArrayElementAtIndex(lastIndex);

            AddBlackboardRow(newElement, serializedBlackboardElement, lastIndex);
            
            OnBlackboardElementChanged?.Invoke(undoGroup);
        }

        private void EditBlackboardFieldName(Blackboard blackboard, VisualElement blackboardElementView, string newName)
        {
            int undoGroup = Undo.GetCurrentGroup();
            (blackboardElementView as BlackboardElementView).ChangeElementName(newName);
            OnBlackboardElementChanged?.Invoke(undoGroup);
        }
    }
}