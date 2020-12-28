using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    [BlackboardElement(typeof(IntBlackboardElement))]
    public class BlackboardConditionalInt : IBlackboardConditionalElement
    {
        [Serializable]
        public enum IntComparator
        {
            Equals,
            Does_Not_Equal,
            Less_Than,
            Greater_Than,
            Greater_Than_Or_Equal_To,
            Less_Than_or_Equal_To,
            Else,
        }
        [SerializeField]
        private IntComparator m_comparator = IntComparator.Equals;
        [SerializeField]
        private int m_comparedValue = 0;

        public bool Evaluate(BlackboardElement element)
        {
            int intVal = (int)element.Value;
            switch (m_comparator)
            {
                case IntComparator.Equals:
                    if (intVal == m_comparedValue)
                        return true;
                    break;
                case IntComparator.Does_Not_Equal:
                    if (intVal != m_comparedValue)
                        return true;
                    break;
                case IntComparator.Less_Than:
                    if (intVal < m_comparedValue)
                        return true;
                    break;
                case IntComparator.Greater_Than:
                    if (intVal > m_comparedValue)
                        return true;
                    break;
                case IntComparator.Less_Than_or_Equal_To:
                    if (intVal <= m_comparedValue)
                        return true;
                    break;
                case IntComparator.Greater_Than_Or_Equal_To:
                    if (intVal >= m_comparedValue)
                        return true;
                    break;
                case IntComparator.Else:
                    return true;
            }
            return false;
        }

#if UNITY_EDITOR
        public static readonly string ComparatorVarName = "m_comparator";
        public static readonly string ComparedValVarName = "m_comparedValue";

        public string GetOutportLabel(SerializedProperty conditionalProp)
        {
            string selectedEnum = ((IntComparator)(conditionalProp.FindPropertyRelative(ComparatorVarName).intValue)).ToString();
            string comparedVal = conditionalProp.FindPropertyRelative(ComparedValVarName).intValue.ToString();
            return $"{selectedEnum} {comparedVal}";
        }
#endif
    }
}