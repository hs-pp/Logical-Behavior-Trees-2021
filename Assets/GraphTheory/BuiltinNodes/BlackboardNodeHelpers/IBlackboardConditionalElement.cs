using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IBlackboardConditionalElement
{
#if UNITY_EDITOR
    VisualElement OnDrawNodeView(SerializedProperty property);
    bool Evaluate();
#endif
}
