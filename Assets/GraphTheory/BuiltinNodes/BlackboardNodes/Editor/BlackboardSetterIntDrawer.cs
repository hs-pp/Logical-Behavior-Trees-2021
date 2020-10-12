using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BlackboardSetterInt))]
public class BlackboardSetterIntDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(property.FindPropertyRelative(BlackboardSetterInt.SetterCommandVarName), GUIContent.none);
        EditorGUILayout.PropertyField(property.FindPropertyRelative(BlackboardSetterInt.NewValueVarName), GUIContent.none);
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndProperty();
    }
}
