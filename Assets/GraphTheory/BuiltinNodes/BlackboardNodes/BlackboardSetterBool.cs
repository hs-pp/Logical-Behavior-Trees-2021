using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[BlackboardElementType(typeof(BoolBlackboardElement))]
public class BlackboardSetterBool : IBlackboardSetterElement
{
    [Serializable]
    public enum BoolSetterCommand
    {
        Set_To,
        Toggle
    }

    [SerializeField]
    private BoolSetterCommand m_setterCommand = BoolSetterCommand.Set_To;
    [SerializeField]
    private bool m_newValue = false;

    public bool Evaluate()
    {
        return true;
    }
#if UNITY_EDITOR
    public static readonly string SetterCommandVarName = "m_setterCommand";
    public static readonly string NewValueVarName = "m_newValue";

    public string GetOutportLabel(SerializedProperty setterProp)
    {
        BoolSetterCommand selectedEnum = ((BoolSetterCommand)(setterProp.FindPropertyRelative(SetterCommandVarName).intValue));
        if(selectedEnum == BoolSetterCommand.Toggle)
        {
            return selectedEnum.ToString();
        }
        else
        {
            string comparedVal = setterProp.FindPropertyRelative(NewValueVarName).boolValue.ToString();
            return $"{selectedEnum.ToString()} to {comparedVal}";
        }

    }
#endif
}
