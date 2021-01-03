using Logical.Editor;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A bit of duplicate code here but not really worth cleaning up. Whatev
/// </summary>
public class GenerateNodeClassCustomMenu : CustomMenuElement
{
    private const string CREATE_BUTTON = "create-button";
    private const string CLOSE_BUTTON = "close-button";

    private const string GENERATE_NODE_TOGGLE = "generate-node-toggle";
    private const string NODE_CLASS_CONTAINER = "node-class-container";
    private const string NODE_NAME_TEXTFIELD = "node-name-textfield";
    private const string NODE_INCLUDE_COMMENTS_TOGGLE = "node-include-comments-toggle";
    private const string NODE_ASSET_PATH_LABEL = "node-asset-path-label";
    private const string NODE_PREVIEW_AREA = "node-preview-area-textfield";

    private const string GENERATE_NODEVIEWDRAWER_TOGGLE = "generate-nodeviewdrawer-toggle";
    private const string NODEVIEWDRAWER_CLASS_CONTAINER = "nodeviewdrawer-class-container";
    private const string NODEVIEWDRAWER_NAME_TEXTFIELD = "nodeviewdrawer-name-textfield";
    private const string NODEVIEWDRAWER_NODE_NAME_TEXTFIELD = "nodeviewdrawer-node-name-textfield";
    private const string NODEVIEWDRAWER_INCLUDE_COMMENTS_TOGGLE = "nodeviewdrawer-include-comments-toggle";
    private const string NODEVIEWDRAWER_PREVIEW_AREA = "nodeviewdrawer-preview-area-textfield";
    private const string NODEVIEWDRAWER_ASSET_PATH_LABEL = "nodeviewdrawer-asset-path-label";

    private const string NODENAME_PLACEHOLDER = "NODENAME";
    private const string NODEVIEWDRAWERNAME_PLACEHOLDER = "NODEVIEWDRAWERNAME";

    private Button m_closeButton = null;
    private Button m_createButton = null;

    private Toggle m_generateClass_Node = null;
    private VisualElement m_classContainer_Node = null;
    private TextField m_className_Node = null;
    private Toggle m_includeComments_Node = null;
    private TextField m_previewArea_Node = null;
    private Label m_assetPathLabel_Node = null;
    private TextAsset m_node_Template = null;
    private string m_pendingGeneratedCode_Node = "";
    private string m_pendingAssetPath_Node = "";

    private Toggle m_generateClass_NodeViewDrawer = null;
    private VisualElement m_classContainer_NodeViewDrawer = null;
    private TextField m_className_NodeViewDrawer = null;
    private TextField m_nodeClassName_NodeViewDrawer = null;
    private Toggle m_includeComments_NodeViewDrawer = null;
    private TextField m_previewArea_NodeViewDrawer = null;
    private Label m_assetPathLabel_NodeViewDrawer = null;
    private TextAsset m_nodeViewDrawer_Template = null;
    private string m_pendingGeneratedCode_NodeViewDrawer = "";
    private string m_pendingAssetPath_NodeViewDrawer = "";

    public GenerateNodeClassCustomMenu()
    {
        var uxmlAsset = Resources.Load<VisualTreeAsset>(ResourceAssetPaths.GenerateNodeClassCustomMenu_UXML);
        uxmlAsset.CloneTree(this);

        m_createButton = this.Q<Button>(CREATE_BUTTON);
        m_closeButton = this.Q<Button>(CLOSE_BUTTON);

        m_generateClass_Node = this.Q<Toggle>(GENERATE_NODE_TOGGLE);
        m_classContainer_Node = this.Q<VisualElement>(NODE_CLASS_CONTAINER);
        m_className_Node = this.Q<TextField>(NODE_NAME_TEXTFIELD);
        m_includeComments_Node = this.Q<Toggle>(NODE_INCLUDE_COMMENTS_TOGGLE);
        m_previewArea_Node = this.Q<TextField>(NODE_PREVIEW_AREA);
        m_assetPathLabel_Node = this.Q<Label>(NODE_ASSET_PATH_LABEL);

        m_generateClass_NodeViewDrawer = this.Q<Toggle>(GENERATE_NODEVIEWDRAWER_TOGGLE);
        m_classContainer_NodeViewDrawer = this.Q<VisualElement>(NODEVIEWDRAWER_CLASS_CONTAINER);
        m_className_NodeViewDrawer = this.Q<TextField>(NODEVIEWDRAWER_NAME_TEXTFIELD);
        m_nodeClassName_NodeViewDrawer = this.Q<TextField>(NODEVIEWDRAWER_NODE_NAME_TEXTFIELD);
        m_includeComments_NodeViewDrawer = this.Q<Toggle>(NODEVIEWDRAWER_INCLUDE_COMMENTS_TOGGLE);
        m_previewArea_NodeViewDrawer = this.Q<TextField>(NODEVIEWDRAWER_PREVIEW_AREA);
        m_assetPathLabel_NodeViewDrawer = this.Q<Label>(NODEVIEWDRAWER_ASSET_PATH_LABEL);

        m_closeButton.clicked += OnCloseButtonPressed;
        m_createButton.clicked += OnCreateButtonPressed;

        m_generateClass_Node.RegisterValueChangedCallback(ToggleNodeContainer);
        m_generateClass_NodeViewDrawer.RegisterValueChangedCallback(ToggleNodeViewDrawerContainer);
        m_includeComments_Node.RegisterValueChangedCallback(ChangeNodeTemplate);
        m_includeComments_NodeViewDrawer.RegisterValueChangedCallback(ChangeNodeViewDrawerTemplate);
        m_className_Node.RegisterValueChangedCallback(OnNodeClassNameChanged);
        m_className_NodeViewDrawer.RegisterValueChangedCallback(OnNodeViewDrawerClassNameChanged);
        m_nodeClassName_NodeViewDrawer.RegisterValueChangedCallback(OnNodeViewDrawerNodeClassNameChanged);

        ToggleContainer(m_generateClass_Node.value, false);
        ToggleContainer(m_generateClass_NodeViewDrawer.value, true);

        m_includeComments_Node.value = true;
        ChangeTemplate(true, false);
        m_includeComments_NodeViewDrawer.value = true;
        ChangeTemplate(true, true);

    }

    private void ToggleNodeContainer(ChangeEvent<bool> changeEvent) 
    { 
        ToggleContainer(changeEvent.newValue, false);
        OnClassNameChanged(m_className_NodeViewDrawer.value, true);
    }
    private void ToggleNodeViewDrawerContainer(ChangeEvent<bool> changeEvent) { ToggleContainer(changeEvent.newValue, true); }
    private void ToggleContainer(bool toggle, bool isNodeViewDrawer)
    {
        if(isNodeViewDrawer)
        {
            m_classContainer_NodeViewDrawer.style.display = toggle ? DisplayStyle.Flex : DisplayStyle.None;
        }
        else
        {
            m_classContainer_Node.style.display = toggle ? DisplayStyle.Flex : DisplayStyle.None;
            m_nodeClassName_NodeViewDrawer.style.display = toggle ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }

    private void ChangeNodeTemplate(ChangeEvent<bool> changeEvent) { ChangeTemplate(changeEvent.newValue, false); }
    private void ChangeNodeViewDrawerTemplate(ChangeEvent<bool> changeEvent) { ChangeTemplate(changeEvent.newValue, true); }
    private void ChangeTemplate(bool includeComments, bool isNodeViewDrawer)
    {
        if (isNodeViewDrawer)
        {
            m_nodeViewDrawer_Template = Resources.Load<TextAsset>(includeComments
                ? ResourceAssetPaths.NodeViewDrawer_Template
                : ResourceAssetPaths.NodeViewDrawer_Template_Simple);
            OnClassNameChanged(m_className_NodeViewDrawer.value, isNodeViewDrawer);
        }
        else
        {
            m_node_Template = Resources.Load<TextAsset>(includeComments
                ? ResourceAssetPaths.Node_Template
                : ResourceAssetPaths.Node_Template_Simple);
            OnClassNameChanged(m_className_Node.value, isNodeViewDrawer);
        }
    }

    private void OnNodeClassNameChanged(ChangeEvent<string> changeEvent) { OnClassNameChanged(changeEvent.newValue, false); }
    private void OnNodeViewDrawerClassNameChanged(ChangeEvent<string> changeEvent) { OnClassNameChanged(changeEvent.newValue, true); }
    private void OnNodeViewDrawerNodeClassNameChanged(ChangeEvent<string> changeEvent) { OnClassNameChanged(m_className_NodeViewDrawer.value, true); }
    private void OnClassNameChanged(string newName, bool isNodeViewDrawer)
    {
        if (string.IsNullOrEmpty(newName))
        {
            newName = "***NO*NAME***";
        }
        if(isNodeViewDrawer)
        {
            string pendingCode = m_nodeViewDrawer_Template.text;
            pendingCode = pendingCode.Replace(NODEVIEWDRAWERNAME_PLACEHOLDER, newName);
            string nodeName = m_generateClass_Node.value ? m_className_Node.value : m_nodeClassName_NodeViewDrawer.value;
            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = "***NO*NAME***";
            }
            pendingCode = pendingCode.Replace(NODENAME_PLACEHOLDER, nodeName);
            m_pendingGeneratedCode_NodeViewDrawer = pendingCode;
            m_previewArea_NodeViewDrawer.value = m_pendingGeneratedCode_NodeViewDrawer;
            UpdatePath(isNodeViewDrawer);
        }
        else
        {
            string pendingCode = m_node_Template.text;
            pendingCode = pendingCode.Replace(NODENAME_PLACEHOLDER, newName);
            m_pendingGeneratedCode_Node = pendingCode;
            m_previewArea_Node.value = m_pendingGeneratedCode_Node;
            UpdatePath(isNodeViewDrawer);

            if(m_generateClass_NodeViewDrawer.value == true)
            {
                OnClassNameChanged(m_className_NodeViewDrawer.value, true);
            }
        }
    }

    private void UpdatePath(bool isNodeViewDrawer)
    {
        if(isNodeViewDrawer)
        {
            m_pendingAssetPath_NodeViewDrawer = GetFullAssetPath(m_className_NodeViewDrawer.value, true);
            m_assetPathLabel_NodeViewDrawer.text = m_pendingAssetPath_NodeViewDrawer;
        }
        else
        {
            m_pendingAssetPath_Node = GetFullAssetPath(m_className_Node.value, false);
            m_assetPathLabel_Node.text = m_pendingAssetPath_Node;
        }
    }

    private void UpdateCreateButtonState()
    {
        if ((m_generateClass_Node.value == false && m_generateClass_NodeViewDrawer.value == false)
            || (m_generateClass_Node.value == true && string.IsNullOrEmpty(m_className_Node.value))
            || (m_generateClass_NodeViewDrawer.value == true && string.IsNullOrEmpty(m_className_NodeViewDrawer.value)))
        {
            m_createButton.SetEnabled(false);
        }
        else
        {
            m_createButton.SetEnabled(true);
        }
    }

    private string GetFullAssetPath(string name, bool inEditorFolder)
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (File.Exists(assetPath))
            assetPath = Path.GetDirectoryName(assetPath);
        if (string.IsNullOrEmpty(assetPath)) 
            assetPath = "Assets";
        if (inEditorFolder)
            assetPath += "\\Editor";

        return $"{assetPath}\\{name}.cs";
    }

    private void OnCloseButtonPressed()
    {
        OnCloseClicked?.Invoke();
    }

    private void OnCreateButtonPressed()
    {
        if(m_generateClass_Node.value)
        {
            CreateCodeFile(m_pendingGeneratedCode_Node, m_pendingAssetPath_Node);
        }
        if(m_generateClass_NodeViewDrawer.value)
        {
            CreateCodeFile(m_pendingGeneratedCode_NodeViewDrawer, m_pendingAssetPath_NodeViewDrawer);
        }
    }

    private void CreateCodeFile(string code, string path)
    {
        if (File.Exists(path))
        {
            Debug.LogError($"File at file path {path} already exists!");
            return;
        }
        else
        {
            StreamWriter sw = File.CreateText(path);
            sw.Write(code);
            sw.Close();

            AssetDatabase.Refresh();
        }
    }
}
