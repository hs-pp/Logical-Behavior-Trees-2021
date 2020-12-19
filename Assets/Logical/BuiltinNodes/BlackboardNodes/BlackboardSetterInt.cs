using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    [BlackboardElementType(typeof(IntBlackboardElement))]
    public class BlackboardSetterInt : IBlackboardSetterElement
    {
        [Serializable]
        public enum IntSetterCommand
        {
            Set_To,
            Increment,
            Decrement
        }

        [SerializeField]
        private IntSetterCommand m_setterCommand = IntSetterCommand.Set_To;
        [SerializeField]
        private int m_newValue = 0;

        public bool Evaluate()
        {
            return true;
        }
#if UNITY_EDITOR
        public static readonly string SetterCommandVarName = "m_setterCommand";
        public static readonly string NewValueVarName = "m_newValue";

        public string GetOutportLabel(SerializedProperty setterProp)
        {
            string selectedEnum = ((IntSetterCommand)(setterProp.FindPropertyRelative(SetterCommandVarName).intValue)).ToString();
            string comparedVal = setterProp.FindPropertyRelative(NewValueVarName).intValue.ToString();
            return $"{selectedEnum} {comparedVal}";
        }
#endif
    }
}