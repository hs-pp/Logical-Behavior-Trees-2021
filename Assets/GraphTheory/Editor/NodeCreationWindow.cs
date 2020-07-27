using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using System.Linq;
using GraphTheory;
using GraphTheory.BuiltInNodes;
using GraphTheory.Editor.UIElements;
using UnityEngine.UIElements;
using GraphTheory.Editor;
using UnityEditor;

public class NodeCreationWindow : ScriptableObject, ISearchWindowProvider
{
    private NodeGraphView m_nodeGraphView = null;
    private List<Type> m_universalNodeTypes = null;
    private List<Type> m_validNodeTypes = null;

    public void Setup(NodeGraphView nodeGraphView)
    {
        m_nodeGraphView = nodeGraphView;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        m_validNodeTypes = new List<Type>();
        for (int i = 0; i < assemblies.Length; i++)
        {
            m_validNodeTypes.AddRange(assemblies[i].GetTypes().Where(x => typeof(ANode).IsAssignableFrom(x)
                && !x.IsAbstract
                && x.GetCustomAttribute<SupportedGraphTypesAttribute>() != null));
            //TODO SORT THEM!
        }

        if (m_universalNodeTypes == null)
        {
            m_universalNodeTypes = new List<Type>();
            for (int i = 0; i < assemblies.Length; i++)
            {
                m_universalNodeTypes.AddRange(assemblies[i].GetTypes().Where(x => typeof(ANode).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<SupportedGraphTypesAttribute>() == null));
            }
            //TODO SORT THEM!!
        }
    }

    //private List<Type> m_allNodes
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
        tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));
        tree.Add(new SearchTreeGroupEntry(new GUIContent("Universal Nodes"), 1));
        for(int i = 0; i < m_universalNodeTypes.Count; i++)
        {
            tree.Add(new SearchTreeEntry(new GUIContent(m_universalNodeTypes[i].Name))
            {
                userData = m_universalNodeTypes[i],
                level = 2
            });
        }
        tree.Add(new SearchTreeGroupEntry(new GUIContent(m_nodeGraphView.GraphType.Name + " Nodes"), 1));
        for (int i = 0; i < m_validNodeTypes.Count; i++)
        {
            tree.Add(new SearchTreeEntry(new GUIContent(m_validNodeTypes[i].Name))
            {
                userData = m_validNodeTypes[i],
                level = 2
            });
        }
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        GraphTheoryWindow window = EditorWindow.GetWindow<GraphTheoryWindow>();

        Vector2 pos = m_nodeGraphView.contentViewContainer.WorldToLocal(
            window.rootVisualElement.ChangeCoordinatesTo(
                window.rootVisualElement.parent,
                context.screenMousePosition - window.position.position));

        //if (SearchTreeEntry.userData != null 
        //    && SearchTreeEntry.userData as Type != typeof(EntryNode))
        if(SearchTreeEntry.userData != null)
        {
            m_nodeGraphView.CreateNode(SearchTreeEntry.userData as Type, pos);
            return true;
        }
        return false;
    }
}
