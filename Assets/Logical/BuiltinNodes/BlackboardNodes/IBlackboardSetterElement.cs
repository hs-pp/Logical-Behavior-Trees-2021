#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    public interface IBlackboardSetterElement
    {
        bool Evaluate();
#if UNITY_EDITOR
        string GetOutportLabel(SerializedProperty setterProp);
#endif
    }
}