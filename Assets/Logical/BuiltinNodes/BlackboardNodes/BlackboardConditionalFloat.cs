using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    [BlackboardElementType(typeof(FloatBlackboardElement))]
    public class BlackboardConditionalFloat : IBlackboardConditionalElement
    {
        [Serializable]
        public enum FloatComparator
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
        private FloatComparator m_comparator = FloatComparator.Equals;
        [SerializeField]
        private float m_comparedValue = 0;

        public bool Evaluate(BlackboardElement element)
        {
            float floatVal = (float)element.Value;
            switch(m_comparator)
            {
                case FloatComparator.Equals:
                    if(floatVal == m_comparedValue)
                        return true;
                    break;
                case FloatComparator.Does_Not_Equal:
                    if (floatVal != m_comparedValue)
                        return true;
                    break;
                case FloatComparator.Less_Than:
                    if (floatVal < m_comparedValue)
                        return true;
                    break;
                case FloatComparator.Greater_Than:
                    if (floatVal > m_comparedValue)
                        return true;
                    break;
                case FloatComparator.Less_Than_or_Equal_To:
                    if (floatVal <= m_comparedValue)
                        return true;
                    break;
                case FloatComparator.Greater_Than_Or_Equal_To:
                    if (floatVal >= m_comparedValue)
                        return true;
                    break;
                case FloatComparator.Else:
                    return true;
            }
            return false;
        }

#if UNITY_EDITOR
        public static readonly string ComparatorVarName = "m_comparator";
        public static readonly string ComparedValVarName = "m_comparedValue";

        public string GetOutportLabel(SerializedProperty conditionalProp)
        {
            string selectedEnum = ((FloatComparator)(conditionalProp.FindPropertyRelative(ComparatorVarName).intValue)).ToString();
            string comparedVal = conditionalProp.FindPropertyRelative(ComparedValVarName).floatValue.ToString();
            return $"{selectedEnum} {comparedVal}";
        }
#endif
    }
}