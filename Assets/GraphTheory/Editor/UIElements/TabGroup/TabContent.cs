using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    /// <summary>
    /// The content VisualElement to be displayed as the body when it's tab is selected.
    /// </summary>
    public abstract class TabContentElement : VisualElement
    {
        public abstract string GetSerializedData();
        public abstract void DeserializeData(string data);
    }

    public class TestContent : TabContentElement
    {
        public override string GetSerializedData()
        {
            return "This is test serialized data!";
        }
        public override void DeserializeData(string data)
        {
        }
    }

}