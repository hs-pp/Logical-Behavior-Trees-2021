using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

[BlackboardElementType(typeof(StringBlackboardElement))]
public class BlackboardConditionalString : IBlackboardConditionalElement
{
    [Serializable]
    public enum StringComparator
    {
        Equals,
        Does_Not_Equal
    }
    [SerializeField]
    private StringComparator m_comparator = StringComparator.Equals;
    [SerializeField]
    private string m_comparedValue = "";

    public bool Evaluate()
    {
        return true;
    }

#if UNITY_EDITOR
    public static readonly string ComparatorVarName = "m_comparator";
    public static readonly string ComparedValVarName = "m_comparedValue";

    public string GetOutportLabel(SerializedProperty conditionalProp)
    {
        string selectedEnum = ((StringComparator)(conditionalProp.FindPropertyRelative(ComparatorVarName).intValue)).ToString();
        string comparedVal = conditionalProp.FindPropertyRelative(ComparedValVarName).stringValue;
        return $"{selectedEnum} to {comparedVal}";
    }
#endif
}
