using UnityEditor;
using UnityEngine;

namespace GraphTheory.Editor
{
    [CustomEditor(typeof(NodeGraphController))]
    public class NodeGraphControllerDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty graphProp = serializedObject.FindProperty("m_nodeGraph");
            SerializedProperty overridePropertiesProp = serializedObject.FindProperty("m_overrideProperties");
            SerializedProperty useOverrides = serializedObject.FindProperty("m_useOverrides");

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(graphProp);
            if (EditorGUI.EndChangeCheck())
            {
                AGraphProperties properties = (graphProp.objectReferenceValue as NodeGraph)?.GraphProperties;
                if (properties != null)
                {
                    AGraphProperties copiedProps = JsonUtility.FromJson(JsonUtility.ToJson(properties), properties.GetType()) as AGraphProperties;
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
}