#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IBlackboardConditionalElement
{
    bool Evaluate();
#if UNITY_EDITOR
    string GetOutportLabel(SerializedProperty conditionalProp);
#endif
}
