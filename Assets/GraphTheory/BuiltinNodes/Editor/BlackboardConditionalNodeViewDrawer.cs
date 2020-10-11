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
            OnBlackboardElementChanged += (undoGroup) => { ValidateBlackboardElement(undoGroup); Repaint(); };
            OnSerializedPropertyChanged += () => { Repaint(); };
        }

        private void ValidateBlackboardElement(int undoGroup = -1)
        {
            TargetProperty.serializedObject.Update();

            if(undoGroup == -1)
            {
                undoGroup = Undo.GetCurrentGroup();
            }

            BlackboardConditional target = (Target as BlackboardConditional);
            if(BlackboardData.GetElementById(target.BlackboardElementId) == null)
            {
                TargetProperty.FindPropertyRelative(BlackboardConditional.BlackboardElementIdVarName).stringValue = "";
                SerializedProperty conditionalsList = TargetProperty.FindPropertyRelative(BlackboardConditional.ConditionalsVarName);
                NodeGraph.RemoveAllOutportsFromNode(TargetProperty);
                conditionalsList.arraySize = 0;
                TargetProperty.serializedObject.ApplyModifiedProperties();
            }
            Undo.CollapseUndoOperations(undoGroup);
            TargetProperty.serializedObject.Update();
        }

        public override void OnDrawPrimaryBody(VisualElement primaryBodyContainer)
        {
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

            SerializedProperty conditionalProp = TargetProperty
                .FindPropertyRelative(BlackboardConditional.ConditionalsVarName)
                .GetArrayElementAtIndex(outportIndex);

            string selectedEnum = ((BlackboardConditionalBool.BoolComparator)(conditionalProp.FindPropertyRelative("m_boolComparator").intValue)).ToString();
            string comparedVal = conditionalProp.FindPropertyRelative("m_comparedValue").boolValue.ToString();
            string desc = $"{selectedEnum} to {comparedVal}";
            outportContainer.OutportBody.Add(new Label(desc));
        }
    }
}