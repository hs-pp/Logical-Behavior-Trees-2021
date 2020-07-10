using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    /// <summary>
    /// The parent tab group element that manages the tabs.
    /// </summary>
    public class TabGroupElement : VisualElement
    {
        private const string TAB_AREA = "tab-area";
        private const string CONTENT_AREA = "content-area";

        private VisualElement m_tabArea = null;
        private VisualElement m_contentArea = null;
        private List<TabElement> m_allTabs = new List<TabElement>();

        private TabGroupData m_tabGroupData = new TabGroupData();

        public TabGroupElement(List<(string, TabContent)> tabs)
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/TabGroup/TabGroupElement");
            xmlAsset.CloneTree(this);
            this.styleSheets.Add(Resources.Load<StyleSheet>("GraphTheory/TabGroup/TabGroupElement"));
            m_tabArea = this.Q<VisualElement>(TAB_AREA);
            m_contentArea = this.Q<VisualElement>(CONTENT_AREA);

            for (int i = 0; i < tabs.Count; i++)
            {
                AddTab(tabs[i]);
            }
        }

        private void AddTab((string, TabContent) content)
        {
            TabElement tabElement = new TabElement(content.Item1, content.Item2);
            tabElement.OnSelected += (bool isSelected) =>
            {
                DeselectSelectedTabs();
            };
            m_tabArea.Add(tabElement);
            m_allTabs.Add(tabElement);
            content.Item2.visible = false;
            content.Item2.StretchToParentSize();
            m_contentArea.Add(content.Item2);
        }

        private void DeselectSelectedTabs()
        {
            for (int i = 0; i < m_allTabs.Count; i++)
            {
                if (m_allTabs[i].IsSelected)
                {
                    m_allTabs[i].SetIsSelected(false);
                }
            }
        }

        public TabGroupData GetSerializedData()
        {
            m_tabGroupData.SelectedTab = m_allTabs.FindIndex(x => x.IsSelected);
            m_tabGroupData.TabData.Clear();
            for (int i = 0; i < m_allTabs.Count; i++)
            {
                m_tabGroupData.TabData.Add(m_allTabs[i].Content.GetSerializedData());
            }
            return m_tabGroupData;
        }

        public void DeserializeData(TabGroupData data)
        {
            m_tabGroupData = data;

            for (int i = 0; i < m_tabGroupData.TabData.Count; i++)
            {
                m_allTabs[i].Content.DeserializeData(m_tabGroupData.TabData[i]);
            }

            if (m_tabGroupData.SelectedTab != -1)
            {
                DeselectSelectedTabs();
                m_allTabs[m_tabGroupData.SelectedTab].SetIsSelected(true);
            }
        }
    }
}