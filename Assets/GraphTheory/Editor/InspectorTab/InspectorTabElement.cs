using GraphTheory.Editor.UIElements;
using UnityEditor;

namespace GraphTheory.Editor
{
    public class InspectorTabElement : TabContentElement
    {
        private NodeGraph m_nodeGraph = null;
        private NodeInspector m_nodeInspector = null;
        private GraphInspector m_graphInspector = null;

        public InspectorTabElement(NodeGraphView nodeGraphView)
        {
            Add(m_graphInspector = new GraphInspector(nodeGraphView));
            Add(m_nodeInspector = new NodeInspector());
            m_nodeInspector.SetVisible(false);
        }

        public void SetOpenNodeGraph(NodeGraph nodeGraph)
        {
            if (nodeGraph == null)
            {
                Reset();
                return;
            }

            m_nodeGraph = nodeGraph;
            m_graphInspector.SetNodeGraph(nodeGraph);
        }

        private void Reset()
        {
            m_nodeGraph = null;
            m_nodeInspector.Reset();
            m_graphInspector.Reset();
        }

        public void SetNode(ANode node, SerializedProperty serializedNode)
        {
            m_graphInspector.SetVisible(node == null);
            m_nodeInspector.SetVisible(node != null);
            if (node != null)
            {
                m_nodeInspector.SetNode(node, serializedNode);
            }
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