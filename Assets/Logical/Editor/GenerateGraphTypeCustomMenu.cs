using Logical.Editor;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateGraphTypeCustomMenu : CustomMenuElement
{
    public const string CREATE_BUTTON = "create-button";
    public const string CLOSE_BUTTON = "close-button";

    private Button m_closeButton = null;
    private Button m_createButton = null;

    public GenerateGraphTypeCustomMenu()
    {
        var uxmlAsset = Resources.Load<VisualTreeAsset>(ResourceAssetPaths.GenerateGraphTypeElement_UXML);
        uxmlAsset.CloneTree(this);

        m_closeButton = this.Q<Button>(CLOSE_BUTTON);
        m_createButton = this.Q<Button>(CREATE_BUTTON);

        m_closeButton.clicked += OnCloseButtonPressed;
    }

    private void OnCloseButtonPressed()
    {
        OnCloseClicked?.Invoke();
    }
}
