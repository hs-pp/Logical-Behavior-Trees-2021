#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IBlackboardSetterElement
{
    bool Evaluate();
#if UNITY_EDITOR
    string GetOutportLabel(SerializedProperty setterProp);
#endif
}
