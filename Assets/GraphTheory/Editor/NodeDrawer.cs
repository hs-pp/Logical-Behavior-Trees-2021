using GraphTheory.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    public class NodeDrawer
    {
        protected NodeView TargetView { get; private set; }
        protected ANode Target { get { return TargetView.Node; } }
        protected SerializedProperty TargetProperty { get; private set; }
        public virtual string DisplayName { get { return Target.GetType().Name; } }
        public virtual Vector2 NodeSize { get { return new Vector2(600, 300); } }
        public virtual Color NodeColor { get { return Color.gray; } }

        public void SetNodeView(NodeView nodeView, SerializedProperty serializedNode)
        {
            TargetView = nodeView;
            TargetProperty = serializedNode;
        }

        public virtual void OnDrawHeader(VisualElement headerContainer) { }
        /// <summary>
        /// Important Note: The parent "title" VisualElement's height is locked to 36.
        /// </summary>
        public virtual void OnDrawTitle(VisualElement preTitleContainer, VisualElement postTitleContainer) { }
        public virtual void OnDrawInport(InportContainer inportContainer) { }
        public virtual void OnDrawOutport(int outportIndex, OutportContainer outportContainer) { }
        public virtual void OnDrawBody(VisualElement bodyContainer) { }
        public virtual void OnDrawFooter(VisualElement footerContainer) { }
    }
}