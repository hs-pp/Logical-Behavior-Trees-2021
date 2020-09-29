using GraphTheory.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.BuiltInNodes
{
    [CustomNodeViewDrawer(typeof(BlackboardConditional))]
    public class BlackboardConditionalNodeViewDrawer : NodeViewDrawer
    {
        public override string DisplayName { get { return "Conditional"; } }

        public override void OnSetup()
        {
            OnBlackboardElementChanged += () => { Repaint(); };
            OnSerializedPropertyChanged += () => { Repaint(); };
        }

        public override void OnDrawPrimaryBody(VisualElement primaryBodyContainer)
        {
            base.OnDrawPrimaryBody(primaryBodyContainer);
            int random = Random.Range(0, 100);
            primaryBodyContainer.Add(new Label("Selected:"));
            string blackboardEleId = TargetProperty.FindPropertyRelative("m_blackboardElementId").stringValue;
            BlackboardElement blackboardElement = NodeGraph.BlackboardData.GetElementById(blackboardEleId);

            primaryBodyContainer.Add(new Label(blackboardElement==null ? "None" : blackboardElement.Name));
        }
    }
}