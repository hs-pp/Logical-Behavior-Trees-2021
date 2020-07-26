using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphTheory.Editor.UIElements
{
    public class EdgeView : Edge
    {
        private Action<EdgeView, bool> m_onEdgePortChanged = null;
        public EdgeView() : base()
        {

        }
        public EdgeView(Action<EdgeView, bool> onEdgePortChanged)
        {
            m_onEdgePortChanged = onEdgePortChanged;
        }

        public override void OnPortChanged(bool isInput)
        {
            m_onEdgePortChanged?.Invoke(this, isInput);
            base.OnPortChanged(isInput);
        }
    }
}