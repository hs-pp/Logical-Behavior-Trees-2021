using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    /// <summary>
    /// The representation of a single tab.
    /// Keeps track of the tab's name, its contents, and whether or not it's currently selected.
    /// </summary>
    public class TabElement : VisualElement
    {
        private const string TAB_NAME_LABEL = "TabName";
        private const string NEUTRAL_DARK_STYLE = "neutral-tab-dark";
        private const string SELECTED_DARK_STYLE = "selected-tab-dark";

        public string Name { get; private set; }
        public TabContentElement Content { get; private set; }
        public bool IsSelected { get; private set; }
        public Action<bool> OnSelected = null;

        public TabElement(string tabName, TabContentElement content)
        {
            this.styleSheets.Add(Resources.Load<StyleSheet>("GraphTheory/TabGroup/TabElement"));
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/TabGroup/TabElement");
            xmlAsset.CloneTree(this);
            this.AddManipulator(new Clickable(OnClick));
            
            Name = tabName;
            Content = content;
            SetIsSelected(false);
            
            Label tabNameLabel = this.Q<Label>(TAB_NAME_LABEL);
            tabNameLabel.text = Name;
        }

        private void OnClick()
        {
            OnSelected?.Invoke(!IsSelected);
            SetIsSelected(!IsSelected);
        }

        public void SetIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
            if (IsSelected)
            {
                this.RemoveFromClassList(NEUTRAL_DARK_STYLE);
                this.AddToClassList(SELECTED_DARK_STYLE);
            }
            else
            {
                this.RemoveFromClassList(SELECTED_DARK_STYLE);
                this.AddToClassList(NEUTRAL_DARK_STYLE);
            }
            Content.style.display = isSelected ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public string GetSerializedData()
        {
            return Content.GetSerializedData();
        }

        public void DeserializeData(string data)
        {
            Content.DeserializeData(data);
        }
    }
}