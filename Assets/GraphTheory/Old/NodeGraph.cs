using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeGraphe : EditorWindow
{
    private NodeGraphView2 m_graphView;

    [MenuItem("Graph/NodeGraph")]
    public static void OpenNodeGraph()
    {
        var window = GetWindow<NodeGraphe>();
        window.titleContent = new GUIContent("NodeGraph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();
        var nodeCreateButton = new Button( ()=> 
        {
                m_graphView.CreateNode("Node from toolbar");
        });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        rootVisualElement.Add(toolbar);
    }

    private void ConstructGraphView()
    {
        m_graphView = new NodeGraphView2
        {
            name = "Main Graph"
        };
        m_graphView.StretchToParentSize();
        rootVisualElement.Add(m_graphView);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(m_graphView);
    }
}
