using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    [BlackboardElement(typeof(FloatBlackboardElement))]
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

        public void Evaluate(BlackboardElement element)
        {
            float floatValue = (float)element.Value;
            switch (m_setterCommand)
            {
                case FloatSetterCommand.Set_To:
                    floatValue = m_newValue;
                    break;
                case FloatSetterCommand.Increment:
                    floatValue += m_newValue;
                    break;
                case FloatSetterCommand.Decrement:
                    floatValue -= m_newValue;
                    break;
            }
            element.Value = floatValue;
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