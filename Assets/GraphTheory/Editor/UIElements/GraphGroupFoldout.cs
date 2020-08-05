using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class GraphGroupFoldout : VisualElement
    {
        private class GraphInstanceMetaContainer
        {
            public bool IsValid { get; } = false;
            public string GUID { get; } = "";
            public string Path { get; } = "";
            public string Name { get; } = "";
            public NodeGraph ObjectRef { get; } = null;
            public ObjectDisplayField DisplayField { get; } = new ObjectDisplayField();
            public static NameComparer NameSorter = new NameComparer();
            public static TypeAndNameComparer TypeAndNameSorter = new TypeAndNameComparer();

            public string TypeAndNameString { get { return ObjectRef.GetType().ToString() + Name; } }

            public GraphInstanceMetaContainer(string guid)
            {
                if(guid != null)
                {
                    GUID = guid;
                    Path = AssetDatabase.GUIDToAssetPath(guid);
                    ObjectRef = AssetDatabase.LoadAssetAtPath<NodeGraph>(Path);
                    Name = ObjectRef ? ObjectRef.name : "";
                    IsValid = ObjectRef != null;

                    if(!IsValid)
                    {
                        GUID = "";
                        Path = "";
                        ObjectRef = null;
                        Name = ""; 
                    }
                }
                DisplayField.SetObject(ObjectRef);
            }

            public class NameComparer : IComparer<GraphInstanceMetaContainer>
            {
                public int Compare(GraphInstanceMetaContainer x, GraphInstanceMetaContainer y)
                {
                    return x.Name.CompareTo(y.Name);
                }
            }
            public class TypeAndNameComparer : IComparer<GraphInstanceMetaContainer>
            {
                public int Compare(GraphInstanceMetaContainer x, GraphInstanceMetaContainer y)
                {
                    return x.TypeAndNameString.CompareTo(y.TypeAndNameString);
                }
            }
        }

        public enum SortRule
        {
            NONE,
            NAME,
            TYPE_AND_NAME,
        }

        private string m_foldoutName = "";
        private Foldout m_foldout = null;
        private SortRule m_sortRule = SortRule.NAME;
        private List<Func<string, Manipulator>> m_manipulators = new List<Func<string, Manipulator>>();
        private List<GraphInstanceMetaContainer> m_graphInstances = new List<GraphInstanceMetaContainer>();
        private Action<string> m_onElementDoubleClick = null;

        public bool IsToggledOn { get { return m_foldout.value; } }
        public int NumElements { get { return m_graphInstances.Count; } }
        public GraphGroupFoldout()
        {
            m_foldout = new Foldout();
            Add(m_foldout);
            style.marginBottom = 6;
        }

        public void Setup(string name, SortRule sortRule, Action<string> onElementDoubleClick)
        {
            m_foldoutName = name;
            SetFoldoutName(name);
            m_sortRule = sortRule;
            m_onElementDoubleClick = onElementDoubleClick;
        }

        public bool AddGraphByGUID(string graphGUID)
        {
            GraphInstanceMetaContainer newInstance = CreateNewGraphMetaContainer(graphGUID);
            if(newInstance == null)
            {
                return false;
            }


            if (m_sortRule != SortRule.NONE)
            {
                int insertIndex = 0;
                if (m_sortRule == SortRule.NAME)
                {
                    insertIndex = m_graphInstances.BinarySearch(newInstance, GraphInstanceMetaContainer.NameSorter);
                }
                else if(m_sortRule == SortRule.TYPE_AND_NAME)
                {
                    insertIndex = m_graphInstances.BinarySearch(newInstance, GraphInstanceMetaContainer.TypeAndNameSorter);
                }

                if(insertIndex > 0)
                {
                    m_foldout.Insert(insertIndex, newInstance.DisplayField);
                    m_graphInstances.Insert(insertIndex, newInstance);
                }
                else
                {
                    m_foldout.Insert(~insertIndex, newInstance.DisplayField);
                    m_graphInstances.Insert(~insertIndex, newInstance);
                }
            }
            else
            {
                m_foldout.Add(newInstance.DisplayField);
                m_graphInstances.Add(newInstance);
            }
            
            SetFoldoutName(m_foldoutName);

            return true;
        }

        private GraphInstanceMetaContainer CreateNewGraphMetaContainer(string guid)
        {
            GraphInstanceMetaContainer newInstance = new GraphInstanceMetaContainer(guid);
            if (!newInstance.IsValid && !string.IsNullOrEmpty(guid)) // The case where the graph SHOULD have existed but did not. Ignore this guid.
            {
                return null;
            }

            newInstance.DisplayField.OnDoubleClick += (UnityEngine.Object obj) =>
            {
                m_onElementDoubleClick?.Invoke(m_graphInstances.Find(x => x.ObjectRef == (obj as NodeGraph)).GUID);
            };
            for (int i = 0; i < m_manipulators.Count; i++)
            {
                newInstance.DisplayField.AddManipulator(m_manipulators[i](newInstance.GUID));
            }
            return newInstance;
        }

        public bool RemoveGraphByGUID(string graphGUID)
        {
            int index = m_graphInstances.FindIndex(x => x.GUID == graphGUID);
            if(index != -1)
            {
                m_foldout.Remove(m_graphInstances[index].DisplayField);
                m_graphInstances.RemoveAt(index);
            }
            SetFoldoutName(m_foldoutName);
            return (index != -1);
        }

        public bool AddByIndex(int index, string graphGUID)
        {
            if(m_sortRule != SortRule.NONE)
            {
                Debug.LogError("Cannot add by index when sort rule exists!");
            }

            GraphInstanceMetaContainer newInstance = CreateNewGraphMetaContainer(graphGUID);
            if(newInstance == null)
            {
                return false;
            }

            m_foldout.Insert(0, newInstance.DisplayField);
            m_graphInstances.Insert(0, newInstance);

            return true;
        }

        public void RemoveByIndex(int index)
        {
            m_foldout.Remove(m_graphInstances[index].DisplayField);
            m_graphInstances.RemoveAt(index);

            SetFoldoutName(m_foldoutName);
        }

        public void ApplySearchQuery(string query)
        {
            Debug.Log("applying query " + query);
            bool atLeastOneFound = false;
            for (int i = 0; i < m_graphInstances.Count; i++)
            {
                bool searchHit = NameContainsSearchQuery(m_graphInstances[i].Name, query);
                atLeastOneFound |= searchHit;
                m_graphInstances[i].DisplayField.style.display = searchHit ? DisplayStyle.Flex: DisplayStyle.None;
            }
            style.display = atLeastOneFound ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private bool NameContainsSearchQuery(string name, string query)
        {
            if(string.IsNullOrEmpty(query))
            {
                return true;
            }

            return name.Contains(query);
        }

        public void FilterByQuery(string searchQuery)
        {

        }

        private void SetFoldoutName(string name)
        {
            m_foldout.text = $"{name} ({m_graphInstances.Count})";
        }

        public void SetToggle(bool isOpen)
        {
            m_foldout.value = isOpen;
        }

        public void AddDisplayFieldManipulator(Func<string, Manipulator> manipulatorFunc)
        {
            for (int i = 0; i < m_graphInstances.Count; i++)
            {
                m_graphInstances[i].DisplayField.AddManipulator(manipulatorFunc(m_graphInstances[i].GUID));
            }
            m_manipulators.Add(manipulatorFunc);
        }

        public void Reset()
        {
            for(int i = 0; i < m_graphInstances.Count; i++)
            {
                m_foldout.Remove(m_graphInstances[i].DisplayField);
            }
            m_graphInstances.Clear();
            SetFoldoutName(m_foldoutName);
        }

        public new class UxmlFactory : UxmlFactory<GraphGroupFoldout, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
    }
}