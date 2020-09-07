using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GraphTheory.BuiltInNodes;
using GraphTheory.Editor.UIElements;
using UnityEngine.UIElements;
using GraphTheory.Editor;
using UnityEditor;

namespace GraphTheory.Editor
{
    public class NodeCreationWindow : ScriptableObject, ISearchWindowProvider
    {
        private NodeGraphView m_nodeGraphView = null;
        private GraphTypeMetadata m_graphTypeMetadata = null;

        public void Setup(NodeGraphView nodeGraphView, GraphTypeMetadata graphTypeMetadata)
        {
            m_nodeGraphView = nodeGraphView;
            m_graphTypeMetadata = graphTypeMetadata;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Universal Nodes"), 1));
            for (int i = 0; i < m_graphTypeMetadata.UniversalNodeTypes.Count; i++)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(m_graphTypeMetadata.UniversalNodeTypes[i].Name))
                {
                    userData = m_graphTypeMetadata.UniversalNodeTypes[i],
                    level = 2
                });
            }
            tree.Add(new SearchTreeGroupEntry(new GUIContent(m_graphTypeMetadata.GraphType.Name + " Nodes"), 1));
            for (int i = 0; i < m_graphTypeMetadata.ValidNodeTypes.Count; i++)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(m_graphTypeMetadata.ValidNodeTypes[i].Name))
                {
                    userData = m_graphTypeMetadata.ValidNodeTypes[i],
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

            if (SearchTreeEntry.userData != null
                && !(typeof(EntryNode).IsAssignableFrom(SearchTreeEntry.userData as Type)))
            {
                m_nodeGraphView.CreateNode(SearchTreeEntry.userData as Type, pos);
                return true;
            }
            return false;
        }
    }
}