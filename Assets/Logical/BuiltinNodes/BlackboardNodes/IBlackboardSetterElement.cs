#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical.BuiltInNodes
{
    public interface IBlackboardSetterElement
    {
        void Evaluate(BlackboardElement element);
#if UNITY_EDITOR
        string GetOutportLabel(SerializedProperty setterProp);
#endif
    }
}