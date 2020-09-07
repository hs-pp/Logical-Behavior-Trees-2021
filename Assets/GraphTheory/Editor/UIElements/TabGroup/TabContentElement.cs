using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    /// <summary>
    /// The content VisualElement to be displayed as the body when it's tab is selected.
    /// TODO: This element should have it's flexGrow set to 1.
    /// </summary>
    public abstract class TabContentElement : VisualElement
    {
        public abstract string GetSerializedData();
        public abstract void DeserializeData(string data);
    }
}