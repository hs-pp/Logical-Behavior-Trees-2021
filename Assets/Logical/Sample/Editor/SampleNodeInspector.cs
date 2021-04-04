using UnityEditor;
using UnityEngine;

/// <summary>
/// A simple property drawer for SampleNode. This will get drawn in the Graph Editor window when selecting a SampleNode.
/// </summary>
[CustomPropertyDrawer(typeof(SampleNode))]
public class SampleNodeInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.PropertyField(property.FindPropertyRelative("SampleString"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("SampleInt"));
    }
}
