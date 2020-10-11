using GraphTheory.Editor.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    public class NodeViewDrawer
    {
        protected NodeView TargetView { get; private set; }
        protected ANode Target { get { return TargetView.Node; } }
        protected SerializedProperty TargetProperty { get; private set; }
        protected NodeGraph NodeGraph { get; private set; }
        protected AGraphProperties GraphProperties { get { return NodeGraph.GraphProperties; } }
        protected BlackboardData BlackboardData { get { return NodeGraph.BlackboardData; } }
        private NodeDisplayContainers m_nodeDisplayContainers = null;

        public Action<int> OnBlackboardElementChanged = null;
        public Action OnRepaint = null;
        public Action OnSerializedPropertyChanged = null;

        public virtual string DisplayName { get { return Target.GetType().Name; } }
        public virtual Vector2 NodeSize { get { return new Vector2(600, 300); } }
        public virtual Color NodeColor { get { return Color.gray; } }

        public void SetNodeView(NodeView nodeView, SerializedProperty serializedNode, NodeGraph nodeGraph, NodeDisplayContainers nodeDisplayContainers)
        {
            TargetView = nodeView;
            TargetProperty = serializedNode;
            NodeGraph = nodeGraph;
            m_nodeDisplayContainers = nodeDisplayContainers;
        }

        public void Repaint(List<PortView> portViews = null)
        {
            TargetProperty.serializedObject.Update();
            OnRepaint?.Invoke();

            m_nodeDisplayContainers.ClearDisplays(portViews != null);

            OnDrawHeader(m_nodeDisplayContainers.HeaderContainer);
            OnDrawTitle(m_nodeDisplayContainers.PreTitleContainer, m_nodeDisplayContainers.PostTitleContainer);
            OnDrawPrimaryBody(m_nodeDisplayContainers.PrimaryBodyContainer);
            OnDrawInport(m_nodeDisplayContainers.InportContainer);
            for (int i = 0; i < TargetProperty.FindPropertyRelative(ANode.OutportsVarName).arraySize; i++)
            {
                if (portViews != null)
                {
                    m_nodeDisplayContainers.AddNewOutport(portViews[i]);
                }
                OnDrawOutport(i, m_nodeDisplayContainers.OutportContainers[i]);
            }
            OnDrawSecondaryBody(m_nodeDisplayContainers.SecondaryBodyContainer);
            OnDrawFooter(m_nodeDisplayContainers.FooterContainer);
        }

        public virtual void OnSetup() { }
        public virtual void OnDrawHeader(VisualElement headerContainer) { }
        public virtual void OnDrawTitle(VisualElement preTitleContainer, VisualElement postTitleContainer) { }
        public virtual void OnDrawPrimaryBody(VisualElement primaryBodyContainer) { }
        public virtual void OnDrawInport(InportContainer inportContainer) { }
        public virtual void OnDrawOutport(int outportIndex, OutportContainer outportContainer) { }
        public virtual void OnDrawSecondaryBody(VisualElement secondaryBodyContainer) { }
        public virtual void OnDrawFooter(VisualElement footerContainer) { }
    }
}