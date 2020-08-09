using GraphTheory.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace GraphTheory.Editor.UIElements
{
    public class InspectorTabElement : TabContentElement
    {
        private NodeGraph m_nodeGraph = null;
        private SerializedObject m_graphSO = null;
        private InspectorElement m_inspectorElement = null;
        private string m_selectedNodeId = "";

        public InspectorTabElement()
        {
        }

        public void Bind(string nodeId, string path)
        {
            m_graphSO.FindProperty("m_nodeCollection");
        }

        public override void DeserializeData(string data)
        {

        }

        public override string GetSerializedData()
        {
            return "";
        }
    }
}