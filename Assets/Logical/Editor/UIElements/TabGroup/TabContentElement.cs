using UnityEngine.UIElements;

namespace Logical.Editor.UIElements
{
    /// <summary>
    /// The content VisualElement to be displayed as the body when it's tab is selected.
    /// </summary>
    public abstract class TabContentElement : VisualElement
    {
        public abstract string GetSerializedData();
        public abstract void DeserializeData(string data);
    }
}