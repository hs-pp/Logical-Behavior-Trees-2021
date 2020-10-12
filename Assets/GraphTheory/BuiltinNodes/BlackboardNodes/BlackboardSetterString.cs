using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[BlackboardElementType(typeof(StringBlackboardElement))]
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

    public bool Evaluate()
    {
        return true;
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
