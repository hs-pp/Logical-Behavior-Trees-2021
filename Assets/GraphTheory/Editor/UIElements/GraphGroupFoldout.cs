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
            public string GUID { get; } = "";
            public string Path { get; } = "";
            public string Name { get; } = "";
            public NodeGraph ObjectRef { get; } = null;
            public ObjectDisplayField DisplayField { get; } = new ObjectDisplayField();
            public static Comparer SortComparer = new Comparer();

            public GraphInstanceMetaContainer(string guid)
            {
                if(guid != null)
                {
                    Path = AssetDatabase.GUIDToAssetPath(guid);
                    ObjectRef = AssetDatabase.LoadAssetAtPath<NodeGraph>(Path);
                    Name = ObjectRef ? ObjectRef.name : "";
                }
                DisplayField.SetObject(ObjectRef);
            }
            public class Comparer : IComparer<GraphInstanceMetaContainer>
            {
                public int Compare(GraphInstanceMetaContainer x, GraphInstanceMetaContainer y)
                {
                    return x.Name.CompareTo(y.Name);
                }
            }
        }

        private bool m_sortByName = true;
        private Foldout m_foldout = null;
        private List<Func<Manipulator>> m_manipulators = new List<Func<Manipulator>>();
        private List<GraphInstanceMetaContainer> m_graphInstances = new List<GraphInstanceMetaContainer>();

        public bool IsToggledOn { get { return m_foldout.value; } }

        public GraphGroupFoldout()
        {
            m_foldout = new Foldout();
            Add(m_foldout);
            style.marginBottom = 6;
        }

        public void Setup(string name, bool sortByName)
        {
            m_foldout.text = name;
            m_sortByName = sortByName;
        }

        public void AddGraphByGUID(string graphGUID)
        {
            GraphInstanceMetaContainer newInstance = new GraphInstanceMetaContainer(graphGUID);
            if(m_sortByName)
            {
                int insertIndex = m_graphInstances.BinarySearch(newInstance, GraphInstanceMetaContainer.SortComparer);
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
            
            for(int i = 0; i < m_manipulators.Count; i++)
            {
                newInstance.DisplayField.AddManipulator(m_manipulators[i]());
            }
        }

        public void RemoveGraphByGUID(string graphGUID)
        {
            int index = m_graphInstances.FindIndex(x => x.GUID == graphGUID);
            if(index != -1)
            {
                m_foldout.Remove(m_graphInstances[index].DisplayField);
                m_graphInstances.RemoveAt(index);
            }
        }

        public void FilterByQuery(string searchQuery)
        {

        }

        public void AddDisplayFieldManipulator(Func<Manipulator> manipulatorFunc)
        {
            for(int i = 0; i < m_graphInstances.Count; i++)
            {
                m_graphInstances[i].DisplayField.AddManipulator(manipulatorFunc());
            }
            m_manipulators.Add(manipulatorFunc);
        }

        public void SetToggle(bool isOpen)
        {
            m_foldout.value = isOpen;
        }

        public void Reset()
        {
            for(int i = 0; i < m_graphInstances.Count; i++)
            {
                m_foldout.Remove(m_graphInstances[i].DisplayField);
            }
            m_graphInstances.Clear();
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