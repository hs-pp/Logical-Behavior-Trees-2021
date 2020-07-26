using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class GraphAxisLabels : VisualElement // AxesController/Behaviour
    {
        private NodeGraphView m_nodeGraphView = null;
        private VisualElement m_verticalAxis = null;
        private VisualElement m_horizontalAxis = null;

        public GraphAxisLabels(NodeGraphView nodeGraphView, CustomContentDragger customContentDragger)
        {
            m_nodeGraphView = nodeGraphView;
            customContentDragger.PositionChanged += UpdateAxes;
            m_nodeGraphView.viewTransformChanged += OnViewTransformChange;
            this.StretchToParentSize();
        }

        private void OnViewTransformChange(GraphView graphView)
        {
            UpdateAxes(graphView.viewTransform.position);
            //Mesh generation!
            //MarkDirtyRepaint();
        }
        
        private void UpdateAxes(Vector2 graphPos)
        {
            //stubs
        }
    }
}