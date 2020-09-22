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

        public Action<BlackboardElement> OnAddBlackboardElement = null;
        public Action<BlackboardElement> OnRemoveBlackboardElement = null;

        public virtual string DisplayName { get { return Target.GetType().Name; } }
        public virtual Vector2 NodeSize { get { return new Vector2(600, 300); } }
        public virtual Color NodeColor { get { return Color.gray; } }

        public void SetNodeView(NodeView nodeView, SerializedProperty serializedNode, NodeGraph nodeGraph)
        {
            TargetView = nodeView;
            TargetProperty = serializedNode;
            NodeGraph = nodeGraph;
        }

        public void DrawNodeView(NodeDisplayContainers nodeDisplayContainers)
        {
            nodeDisplayContainers.ClearDisplays();

            OnDrawHeader(nodeDisplayContainers.HeaderContainer);
            OnDrawTitle(nodeDisplayContainers.PreTitleContainer, nodeDisplayContainers.PostTitleContainer);
            OnDrawPrimaryBody(nodeDisplayContainers.PrimaryBodyContainer);
            OnDrawInport(nodeDisplayContainers.InportContainer);
            for (int i = 0; i < Target.NumOutports; i++)
            {
                OnDrawOutport(i, nodeDisplayContainers.OutportContainers[i]);
            }
            OnDrawSecondaryBody(nodeDisplayContainers.SecondaryBodyContainer);
            OnDrawFooter(nodeDisplayContainers.FooterContainer);
        }

        public virtual void OnDrawHeader(VisualElement headerContainer) { }
        public virtual void OnDrawTitle(VisualElement preTitleContainer, VisualElement postTitleContainer) { }
        public virtual void OnDrawPrimaryBody(VisualElement primaryBodyContainer) { }
        public virtual void OnDrawInport(InportContainer inportContainer) { }
        public virtual void OnDrawOutport(int outportIndex, OutportContainer outportContainer) { }
        public virtual void OnDrawSecondaryBody(VisualElement secondaryBodyContainer) { }
        public virtual void OnDrawFooter(VisualElement footerContainer) { }
    }
}