using Logical;
using Logical.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateGraphInstanceElement : VisualElement
{
    public const string GRAPH_TYPE_DROPDOWN = "graph-type-enum-area";
    public const string GRAPH_NAME_FIELD = "graph-name-textfield";
    public const string GRAPH_PATH_LABEL = "path-label";
    public const string CREATE_BUTTON = "create-button";
    public const string CLOSE_BUTTON = "close-button";

    private IMGUIContainer m_graphTypeDropdownDrawer = null;
    private TextField m_graphNameField = null;
    private Label m_pathDrawer = null;
    private Button m_createButton = null;
    private Button m_closeButton = null;

    private GraphTypeMetadata m_graphTypeMetaData = null;
    private Action m_onClosePressed = null;
    private Type[] m_allGraphTypes = new Type[0];
    private string[] m_allGraphTypeNames = new string[0];
    private int m_selectedIndex = -1;

    public CreateGraphInstanceElement(GraphTypeMetadata graphTypeMetadata, Action onClosePressed)
    {
        var uxmlAsset = Resources.Load<VisualTreeAsset>(ResourceAssetPaths.CreateGraphInstanceElement_UXML);
        uxmlAsset.CloneTree(this);

        m_graphTypeMetaData = graphTypeMetadata;

        m_graphTypeDropdownDrawer = this.Q<IMGUIContainer>(GRAPH_TYPE_DROPDOWN);
        m_graphNameField = this.Q<TextField>(GRAPH_NAME_FIELD);
        m_pathDrawer = this.Q<Label>(GRAPH_PATH_LABEL);
        m_createButton = this.Q<Button>(CREATE_BUTTON);
        m_closeButton = this.Q<Button>(CLOSE_BUTTON);

        m_graphNameField.value = "";
        m_graphTypeDropdownDrawer.onGUIHandler += DrawGraphTypeDropdown;
        m_createButton.clicked += OnCreateButtonPressed;
        m_closeButton.clicked += OnCloseButtonPressed;
        m_onClosePressed = onClosePressed;

        UpdatePath();
        Selection.selectionChanged -= UpdatePath;
        Selection.selectionChanged += UpdatePath;
        m_graphNameField.RegisterValueChangedCallback((newName) => { UpdatePath(); });

        m_allGraphTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(
            x => typeof(NodeGraph).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).ToArray();
            m_allGraphTypeNames = m_allGraphTypes.Select(x => x.Name).ToArray();
    }

    ~CreateGraphInstanceElement()
    {
        Selection.selectionChanged -= UpdatePath;
    }

    private void DrawGraphTypeDropdown()
    {
        GUILayout.BeginVertical(GUILayout.Width(300));
        m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_allGraphTypeNames);
        GUILayout.EndVertical();
    }

    private void UpdatePath()
    {
        m_pathDrawer.text = GetFullAssetPath(m_graphNameField.value);
    }

    private string GetFullAssetPath(string name)
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (File.Exists(assetPath))
            assetPath = Path.GetDirectoryName(assetPath);
        if (string.IsNullOrEmpty(assetPath)) assetPath = "Assets";

        return $"{assetPath}\\{name}.asset";
    }

    private void OnCreateButtonPressed()
    {
        if(m_selectedIndex == -1)
        {
            Debug.LogError("No graph type selected!");
            return;
        }
        if(m_graphNameField.value == "")
        {
            Debug.LogError("Graph name is empty!");
            return;
        }

        NodeGraph createdGraph = ScriptableObject.CreateInstance(m_allGraphTypes[m_selectedIndex]) as NodeGraph;
        createdGraph.GraphProperties = (AGraphProperties)Activator.CreateInstance(m_graphTypeMetaData.GetGraphPropertiesType(m_allGraphTypes[m_selectedIndex]));

        AssetDatabase.CreateAsset(createdGraph, GetFullAssetPath(m_graphNameField.value));
        GraphModificationProcessor.OnAssetCreated(createdGraph);
        OnCloseButtonPressed();
    }

    private void OnCloseButtonPressed()
    {
        m_onClosePressed?.Invoke();
    }
}
