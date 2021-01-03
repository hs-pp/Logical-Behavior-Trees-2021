using Logical.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomMenuController
{
    private VisualElement m_mainPanel = null;
    private NodeGraphView m_nodeGraphView = null;
    private Dictionary<string, CustomMenuElement> m_allCustomMenus = new Dictionary<string, CustomMenuElement>();
    private string m_activeMenu = "";

    public CustomMenuController(VisualElement mainPanel, NodeGraphView nodeGraphView)
    {
        m_mainPanel = mainPanel;
        m_nodeGraphView = nodeGraphView;
    }

    public void AddCustomMenu(string menuName, CustomMenuElement element)
    {
        m_mainPanel.Add(element);
        m_allCustomMenus.Add(menuName, element);
        element.style.display = DisplayStyle.None;
        element.OnCloseClicked += () => { HideCustomMenu(menuName); };
    }

    public void ShowCustomMenu(string menuName)
    {
        if(!string.IsNullOrEmpty(m_activeMenu))
        {
            HideCustomMenu(m_activeMenu);
            m_activeMenu = "";
        }

        m_allCustomMenus[menuName].style.display = DisplayStyle.Flex;
        m_nodeGraphView.style.display = DisplayStyle.None;
        m_activeMenu = menuName;
    }

    public void HideCustomMenu(string menuName)
    {
        m_allCustomMenus[menuName].style.display = DisplayStyle.None;
        m_nodeGraphView.style.display = DisplayStyle.Flex;
    }
}
