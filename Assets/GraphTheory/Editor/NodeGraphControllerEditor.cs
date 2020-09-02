using GraphTheory;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeGraphController))]
public class NodeGraphControllerDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedProperty graphProp = serializedObject.FindProperty("m_nodeGraph");
        SerializedProperty overridePropertiesProp = serializedObject.FindProperty("m_overrideProperties");
        SerializedProperty useOverrides = serializedObject.FindProperty("m_useOverrides");

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(graphProp);
        if(EditorGUI.EndChangeCheck())
        {
            IGraphProperties properties = (graphProp.objectReferenceValue as NodeGraph)?.GraphProperties;
            if (properties != null)
            {
                IGraphProperties copiedProps = JsonUtility.FromJson(JsonUtility.ToJson(properties), properties.GetType()) as IGraphProperties;
                overridePropertiesProp.managedReferenceValue = copiedProps;
            }
            else
            {
                overridePropertiesProp.managedReferenceValue = null;
            }
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_createGraphInstance"));

        if (graphProp.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(useOverrides);
            if (useOverrides.boolValue)
            {
                EditorGUILayout.PropertyField(overridePropertiesProp, true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
