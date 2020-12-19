using UnityEditor;
using UnityEngine;

namespace Logical.BuiltInNodes
{
    [CustomPropertyDrawer(typeof(BlackboardSetterFloat))]
    public class BlackboardSetterFloatDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property.FindPropertyRelative(BlackboardSetterFloat.SetterCommandVarName), GUIContent.none);
            EditorGUILayout.PropertyField(property.FindPropertyRelative(BlackboardSetterFloat.NewValueVarName), GUIContent.none);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndProperty();
        }
    }
}