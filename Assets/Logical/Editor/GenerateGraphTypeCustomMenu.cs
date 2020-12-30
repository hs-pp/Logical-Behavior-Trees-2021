using Logical.Editor;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateGraphTypeCustomMenu : CustomMenuElement
{
    private const string CREATE_BUTTON = "create-button";
    private const string CLOSE_BUTTON = "close-button";
    private const string GRAPH_TYPE_NAME = "graph-type-name-textfield";
    private const string INCLUDE_COMMENTS_TOGGLE = "include-comments-toggle";
    private const string PREVIEW_AREA = "preview-area-textfield";
    private const string ASSET_PATH_LABEL = "asset-path-label";

    private Button m_closeButton = null;
    private Button m_createButton = null;
    private TextField m_graphTypeName = null;
    private Toggle m_includeComments = null;
    private TextField m_previewArea = null;
    private Label m_assetPathLabel = null;

    private TextAsset m_template = null;
    private string m_pendingGeneratedCode = "";
    private string m_pendingAssetPath = "";

    public GenerateGraphTypeCustomMenu()
    {
        var uxmlAsset = Resources.Load<VisualTreeAsset>(ResourceAssetPaths.GenerateGraphClassCustomMenu_UXML);
        uxmlAsset.CloneTree(this);

        m_closeButton = this.Q<Button>(CLOSE_BUTTON);
        m_createButton = this.Q<Button>(CREATE_BUTTON);
        m_graphTypeName = this.Q<TextField>(GRAPH_TYPE_NAME);
        m_includeComments = this.Q<Toggle>(INCLUDE_COMMENTS_TOGGLE);
        m_previewArea = this.Q<TextField>(PREVIEW_AREA);
        m_assetPathLabel = this.Q<Label>(ASSET_PATH_LABEL);

        m_closeButton.clicked += OnCloseButtonPressed;
        m_createButton.clicked += OnCreateButtonPressed;


        m_includeComments.RegisterValueChangedCallback(ChangeTemplate);
        m_graphTypeName.RegisterValueChangedCallback(OnGraphTypeNameChanged);

        m_includeComments.value = true;
        ChangeTemplate(m_includeComments.value);

        UpdatePath();
        Selection.selectionChanged -= UpdatePath;
        Selection.selectionChanged += UpdatePath;
    }

    private void ChangeTemplate(ChangeEvent<bool> changeEvent) { ChangeTemplate(changeEvent.newValue); }
    private void ChangeTemplate(bool includeComments)
    {
        m_template = Resources.Load<TextAsset>(includeComments ? ResourceAssetPaths.NodeGraph_Template : ResourceAssetPaths.NodeGraph_Template_Simple);
        OnGraphTypeNameChanged(m_graphTypeName.value);
    }

    private void OnGraphTypeNameChanged(ChangeEvent<string> changeEvent) { OnGraphTypeNameChanged(changeEvent.newValue); }
    private void OnGraphTypeNameChanged(string newName)
    {
        bool nameIsEmpty = string.IsNullOrEmpty(newName);
        if(nameIsEmpty)
        {
            newName = "***NO*NAME***";
        }
        m_createButton.SetEnabled(!nameIsEmpty);

        m_pendingGeneratedCode = m_template.text.Replace("GRAPHNAME", newName);
        m_previewArea.value = m_pendingGeneratedCode;

        UpdatePath();
    }

    private void UpdatePath()
    {
        m_pendingAssetPath = GetFullAssetPath(m_graphTypeName.value);
        m_assetPathLabel.text = m_pendingAssetPath;
    }

    private string GetFullAssetPath(string name)
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (File.Exists(assetPath))
            assetPath = Path.GetDirectoryName(assetPath);
        if (string.IsNullOrEmpty(assetPath)) assetPath = "Assets";

        return $"{assetPath}\\{name}.cs";
    }

    private void OnCloseButtonPressed()
    {
        OnCloseClicked?.Invoke();
    }

    private void OnCreateButtonPressed()
    {
        if (string.IsNullOrEmpty(m_graphTypeName.value))
        {
            Debug.LogError("New graph type name not set!!!");
            return;
        }
        else if (File.Exists(m_pendingAssetPath))
        {
            Debug.LogError($"File at file path {m_pendingAssetPath} already exists!");
            return;
        }
        else
        {
            StreamWriter sw = File.CreateText(m_pendingAssetPath);
            sw.Write(m_pendingGeneratedCode);
            sw.Close();

            AssetDatabase.Refresh();
        }
    }
}
