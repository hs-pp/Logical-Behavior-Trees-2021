using Logical.Editor.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Logical.Editor
{
    [Serializable]
    public class GraphClipboardData
    {
        [Serializable]
        private struct ClipboardElement
        {
            [SerializeField]
            private string ElementType;
            [SerializeField]
            private string Data;

            public ClipboardElement(ANode node)
            {
                ElementType = node.GetType().AssemblyQualifiedName;
                Data = JsonUtility.ToJson(node);
            }

            public ANode Deserialize()
            {
                return JsonUtility.FromJson(Data, Type.GetType(ElementType)) as ANode;
            }
        }

        [SerializeField]
        private string m_graphTypeName = "";
        public string GraphTypeName { get { return m_graphTypeName; } }
        [SerializeField]
        private List<ClipboardElement> m_serializedGraphElements = new List<ClipboardElement>();

        public GraphClipboardData(NodeGraph nodeGraph, IEnumerable<GraphElement> elements)
        {
            m_graphTypeName = nodeGraph.GetType().AssemblyQualifiedName;
            
            foreach (GraphElement element in elements)
            {
                NodeView nodeView = element as NodeView;
                if (nodeView != null)
                {
                    if(!(typeof(BuiltInNodes.EntryNode).IsAssignableFrom(nodeView.NodeType)))
                    {
                        m_serializedGraphElements.Add(new ClipboardElement(nodeView.Node));
                    }
                }
                //Edge data is in the nodes so we don't need to serialize edge views.
            }
        }

        public List<ANode> GetGraphElements()
        {
            List<ANode> nodes = new List<ANode>();
            for(int i = 0; i < m_serializedGraphElements.Count; i++)
            {
                nodes.Add(m_serializedGraphElements[i].Deserialize());
            }
            return nodes;
        }
    }
}