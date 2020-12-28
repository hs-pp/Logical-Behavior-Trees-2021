using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    [BlackboardElement(typeof(StringBlackboardElement))]
    public class BlackboardSetterString : IBlackboardSetterElement
    {
        [Serializable]
        public enum StringSetterCommand
        {
            Set_To,
            Append,
            Prepend
        }

        [SerializeField]
        private StringSetterCommand m_setterCommand = StringSetterCommand.Set_To;
        [SerializeField]
        private string m_newValue = "";

        public void Evaluate(BlackboardElement element)
        {
            string strValue = (string)element.Value;
            switch(m_setterCommand)
            {
                case StringSetterCommand.Set_To:
                    strValue = m_newValue;
                    break;
                case StringSetterCommand.Append:
                    strValue += m_newValue;
                    break;
                case StringSetterCommand.Prepend:
                    strValue = m_newValue + strValue;
                    break;
            }
            element.Value = strValue;
        }

#if UNITY_EDITOR
        public static readonly string SetterCommandVarName = "m_setterCommand";
        public static readonly string NewValueVarName = "m_newValue";

        public string GetOutportLabel(SerializedProperty conditionalProp)
        {
            string selectedEnum = ((StringSetterCommand)(conditionalProp.FindPropertyRelative(SetterCommandVarName).intValue)).ToString();
            string comparedVal = conditionalProp.FindPropertyRelative(NewValueVarName).stringValue;
            return $"{selectedEnum} \"{comparedVal}\"";
        }
#endif
    }
}