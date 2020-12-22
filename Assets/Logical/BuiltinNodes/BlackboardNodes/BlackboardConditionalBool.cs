using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace Logical.BuiltInNodes
{
    [BlackboardElementType(typeof(BoolBlackboardElement))]
    public class BlackboardConditionalBool : IBlackboardConditionalElement
    {
        [Serializable]
        public enum BoolComparator
        {
            Equals,
            Does_Not_Equal
        }
        [SerializeField]
        private BoolComparator m_boolComparator = BoolComparator.Equals;
        [SerializeField]
        private bool m_comparedValue = false;

        public bool Evaluate(BlackboardElement element)
        {
            bool boolValue = (bool)element.Value;
            switch (m_boolComparator)
            {
                case BoolComparator.Equals:
                    if (m_comparedValue == boolValue)
                        return true;
                    break;
                case BoolComparator.Does_Not_Equal:
                    if (m_comparedValue != boolValue)
                        return true;
                    break;
            }
            return false;
        }

#if UNITY_EDITOR
        public static readonly string ComparatorVarName = "m_boolComparator";
        public static readonly string ComparedValVarName = "m_comparedValue";

        public string GetOutportLabel(SerializedProperty conditionalProp)
        {
            string selectedEnum = ((BoolComparator)(conditionalProp.FindPropertyRelative(ComparatorVarName).intValue)).ToString();
            string comparedVal = conditionalProp.FindPropertyRelative(ComparedValVarName).boolValue.ToString();
            return $"{selectedEnum} to {comparedVal}";
        }
#endif
    }
}