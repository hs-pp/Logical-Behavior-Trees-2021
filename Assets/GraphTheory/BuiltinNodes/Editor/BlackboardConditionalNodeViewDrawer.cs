using GraphTheory.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.BuiltInNodes
{
    [CustomNodeViewDrawer(typeof(BlackboardConditional))]
    public class BlackboardConditionalNodeViewDrawer : NodeViewDrawer
    {
        public override string DisplayName { get { return "Blackboard Conditional"; } }

        public override void OnSetup()
        {
            OnBlackboardElementChanged += () => { ValidateBlackboardElement(); Repaint(); };
            OnSerializedPropertyChanged += () => { Repaint(); };
        }

        private void ValidateBlackboardElement()
        {
            BlackboardConditional target = (Target as BlackboardConditional);
            if(BlackboardData.GetElementById(target.BlackboardElementId) == null)
            {
                TargetProperty.FindPropertyRelative(BlackboardConditional.BlackboardElementIdVarName).stringValue = "";
                SerializedProperty conditionalsList = TargetProperty.FindPropertyRelative(BlackboardConditional.ConditionalsVarName);
                for (int i = conditionalsList.arraySize - 1; i >= 0; i--)
                {
                    conditionalsList.DeleteArrayElementAtIndex(i);
                    NodeGraph.RemoveOutportFromNode(TargetProperty, i);
                }
                TargetProperty.serializedObject.ApplyModifiedProperties();
            }
            TargetProperty.serializedObject.Update();
        }

        public override void OnDrawPrimaryBody(VisualElement primaryBodyContainer)
        {
            Debug.Log("repainting this guy");
            base.OnDrawPrimaryBody(primaryBodyContainer);
            string blackboardEleId = TargetProperty.FindPropertyRelative(BlackboardConditional.BlackboardElementIdVarName).stringValue;
            BlackboardElement blackboardElement = NodeGraph.BlackboardData.GetElementById(blackboardEleId);
            Label selectedEleLabel = new Label(blackboardElement == null ? "None" : blackboardElement.Name);
            selectedEleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            selectedEleLabel.style.fontSize = 18;
            primaryBodyContainer.Add(selectedEleLabel);
        }

        public override void OnDrawOutport(int outportIndex, OutportContainer outportContainer)
        {
            base.OnDrawOutport(outportIndex, outportContainer);
            outportContainer.OutportBody.Add((Target as BlackboardConditional).GetNodeViewOutportElement(outportIndex, 
                TargetProperty.FindPropertyRelative(BlackboardConditional.ConditionalsVarName).GetArrayElementAtIndex(outportIndex)));
        }
    }
}