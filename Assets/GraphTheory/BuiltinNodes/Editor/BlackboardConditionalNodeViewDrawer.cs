using GraphTheory.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.BuiltInNodes
{
    [CustomNodeViewDrawer(typeof(BlackboardConditional))]
    public class BlackboardConditionalNodeViewDrawer : NodeViewDrawer
    {
        public override string DisplayName { get { return "Conditional"; } }

        public override void OnDrawPrimaryBody(VisualElement primaryBodyContainer)
        {
            base.OnDrawPrimaryBody(primaryBodyContainer);
            OnAddBlackboardElement += (ele) => { Debug.Log("Added ele hellow"); };
            OnRemoveBlackboardElement += (ele) => { Debug.Log("Removed ele hellow"); };

        }
    }
}