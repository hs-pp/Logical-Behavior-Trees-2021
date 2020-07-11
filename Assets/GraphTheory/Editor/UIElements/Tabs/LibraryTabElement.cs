using GraphTheory.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    public class LibraryTabElement : TabContentElement
    {
        private const string OPENED_GRAPH_FIELD = "opened-graph-field";

        private ObjectDisplayField m_objectDisplayField = null;

        public LibraryTabElement() 
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/Tabs/LibraryTabElement");
            xmlAsset.CloneTree(this);

            m_objectDisplayField = this.Q<ObjectDisplayField>(OPENED_GRAPH_FIELD);
        }

        public void SetOpenNodeGraph(NodeGraph graph)
        {
            m_objectDisplayField.SetObject(graph);
        }


        public override void DeserializeData(string data)
        {
        }

        public override string GetSerializedData()
        {
            return "lib tab";
        }
    }
}