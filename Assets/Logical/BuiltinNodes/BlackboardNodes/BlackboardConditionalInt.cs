using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    [BlackboardElementType(typeof(IntBlackboardElement))]
    public class BlackboardConditionalInt : IBlackboardConditionalElement
    {
        [Serializable]
        public enum IntComparator
        {
            Equals,
            Does_Not_Equal,
            Less_Than,
            Greater_Than
        }
        [SerializeField]
        private IntComparator m_comparator = IntComparator.Equals;
        [SerializeField]
        private int m_comparedValue = 0;

        public bool Evaluate()
        {
            return true;
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