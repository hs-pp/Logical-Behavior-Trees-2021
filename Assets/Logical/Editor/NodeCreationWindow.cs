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
        private Vector2 m_mousePos = Vector2.zero;

        public void Setup(NodeGraphView nodeGraphView)
        {
            m_nodeGraphView = nodeGraphView;
        }
        
        public void SetMousePosition(Vector2 mousePos)
        {
            m_mousePos = mousePos;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            Type graphType = m_nodeGraphView.NodeGraph.GetType();
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"), 0));
            tree.Add(new SearchTreeGroupEntry(new GUIContent(graphType.Name + " Nodes"), 1));
            List<Type> nodeTypes = GraphTypeMetadata.GetNodeTypesFromGraphType(graphType);
            for (int i = 0; i < nodeTypes.Count; i++)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(nodeTypes[i].Name))
                {
                    userData = nodeTypes[i],
                    level = 2
                });
            }
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Universal Nodes"), 1));
            for (int i = 0; i < GraphTypeMetadata.UniversalNodeTypes.Count; i++)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(GraphTypeMetadata.UniversalNodeTypes[i].Name))
                {
                    userData = GraphTypeMetadata.UniversalNodeTypes[i],
                    level = 2
                });
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            // Vector2 pos = m_nodeGraphView.contentViewContainer.WorldToLocal(
            //     m_editorWindow.rootVisualElement.ChangeCoordinatesTo(
            //         m_editorWindow.rootVisualElement.parent,
            //         context.screenMousePosition - m_editorWindow.position.position));
            // VisualElement root = m_nodeGraphView.panel.visualTree;
            // Rect rect = GUIUtility.GUIToScreenRect(m_nodeGraphView.worldBound);
            // EditorWindow window = EditorWindow.GetWindow<LogicalTheoryWindow>();
            // Vector3 pos222 = window.position.position;
            // Vector2 pos = m_nodeGraphView.contentViewContainer.WorldToLocal(
            //     root.ChangeCoordinatesTo(root, context.screenMousePosition - rect.position));
            
            if (SearchTreeEntry.userData != null
                && !(typeof(EntryNode).IsAssignableFrom(SearchTreeEntry.userData as Type)))
            {
                m_nodeGraphView.CreateNode(SearchTreeEntry.userData as Type, m_mousePos);
                return true;
            }
            return false;
        }
    }
}