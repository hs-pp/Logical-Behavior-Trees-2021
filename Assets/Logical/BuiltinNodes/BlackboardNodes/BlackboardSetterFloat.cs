using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    [BlackboardElementType(typeof(FloatBlackboardElement))]
    public class BlackboardSetterFloat : IBlackboardSetterElement
    {
        [Serializable]
        public enum FloatSetterCommand
        {
            Set_To,
            Increment,
            Decrement
        }

        [SerializeField]
        private FloatSetterCommand m_setterCommand = FloatSetterCommand.Set_To;
        [SerializeField]
        private float m_newValue = 0;

        public bool Evaluate()
        {
            return true;
        }
#if UNITY_EDITOR
        public static readonly string SetterCommandVarName = "m_setterCommand";
        public static readonly string NewValueVarName = "m_newValue";

        public string GetOutportLabel(SerializedProperty setterProp)
        {
            string selectedEnum = ((FloatSetterCommand)(setterProp.FindPropertyRelative(SetterCommandVarName).intValue)).ToString();
            string comparedVal = setterProp.FindPropertyRelative(NewValueVarName).floatValue.ToString();
            return $"{selectedEnum} {comparedVal}";
        }
#endif
    }
}