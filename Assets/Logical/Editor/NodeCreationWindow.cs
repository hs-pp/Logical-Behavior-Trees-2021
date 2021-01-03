using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Logical.BuiltInNodes;
using UnityEngine.UIElements;
using UnityEditor;

namespace Logical.Editor
{
    /// <summary>
    /// This class is for the node creation window when right clicking on the graph zone and
    /// choosing the option to add a new node.
    /// </summary>
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
            tree.Add(new SearchTreeGroupEntry(new GUIContent(m_graphTypeMetadata.ActiveGraphType.Name + " Nodes"), 1));
            List<Type> nodeTypes = m_graphTypeMetadata.GetNodeTypesFromGraphType(m_graphTypeMetadata.ActiveGraphType);
            for (int i = 0; i < nodeTypes.Count; i++)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(nodeTypes[i].Name))
                {
                    userData = nodeTypes[i],
                    level = 2
                });
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            LogicalTheoryWindow window = EditorWindow.GetWindow<LogicalTheoryWindow>();

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