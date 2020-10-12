using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
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

    public string GetOutportLabel(SerializedProperty setterProp)
    {
        string selectedEnum = ((StringComparator)(setterProp.FindPropertyRelative(ComparatorVarName).intValue)).ToString();
        string comparedVal = setterProp.FindPropertyRelative(ComparedValVarName).stringValue;
        return $"{selectedEnum} to \"{comparedVal}\"";
    }
#endif
}
