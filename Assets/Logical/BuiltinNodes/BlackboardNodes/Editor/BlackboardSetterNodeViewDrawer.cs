using Logical.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Logical.BuiltInNodes
{
    [CustomNodeViewDrawer(typeof(BlackboardSetter))]
    public class BlackboardSetterNodeViewDrawer : NodeViewDrawer
    {
        public override string DisplayName { get { return "Blackboard Setter"; } }

        public override void OnSetup()
        {
            OnBlackboardElementChanged += (undoGroup) => { ValidateBlackboardElement(undoGroup); Repaint(); };
            OnSerializedPropertyChanged += () => { Repaint(); };
        }

        private void ValidateBlackboardElement(int undoGroup = -1)
        {
            TargetProperty.serializedObject.Update();

            if (undoGroup == -1)
            {
                undoGroup = Undo.GetCurrentGroup();
            }

            BlackboardSetter target = (Target as BlackboardSetter);
            if (BlackboardData.GetElementById(target.BlackboardElementId) == null)
            {
                TargetProperty.FindPropertyRelative(BlackboardSetter.BlackboardElementIdVarName).stringValue = "";
                SerializedProperty setterValueProp = TargetProperty.FindPropertyRelative(BlackboardSetter.SetterValueVarName);
                setterValueProp.managedReferenceValue = null;
                TargetProperty.serializedObject.ApplyModifiedProperties();
            }
            Undo.CollapseUndoOperations(undoGroup);
            TargetProperty.serializedObject.Update();
        }

        public override void OnDrawPrimaryBody(VisualElement primaryBodyContainer)
        {
            base.OnDrawPrimaryBody(primaryBodyContainer);
            string blackboardEleId = TargetProperty.FindPropertyRelative(BlackboardSetter.BlackboardElementIdVarName).stringValue;
            BlackboardElement blackboardElement = NodeGraph.BlackboardData.GetElementById(blackboardEleId);
            Label selectedEleLabel = new Label(blackboardElement == null ? "None" : blackboardElement.Name);
            selectedEleLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            selectedEleLabel.style.fontSize = 18;
            primaryBodyContainer.Add(selectedEleLabel);
        }

        public override void OnDrawOutport(int outportIndex, OutportContainer outportContainer)
        {
            base.OnDrawOutport(outportIndex, outportContainer);

            SerializedProperty setterValueProp = TargetProperty.FindPropertyRelative(BlackboardSetter.SetterValueVarName);

            outportContainer.OutportBody.Add(new Label(BlackboardSetter.GetOutportLabel(setterValueProp)));
        }
    }
}