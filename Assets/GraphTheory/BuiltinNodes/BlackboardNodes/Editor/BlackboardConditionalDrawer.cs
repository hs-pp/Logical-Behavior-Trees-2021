using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.BuiltInNodes
{
    [CustomPropertyDrawer(typeof(BlackboardConditional))]
    public class BlackboardConditionalDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.serializedObject.Update();
            List<BlackboardElement> blackboardElements = (property.serializedObject.targetObject as NodeGraph).BlackboardData.GetAllElements();
            SerializedProperty conditionalsList = property.FindPropertyRelative(BlackboardConditional.ConditionalsVarName);

            // Selected Blackboard Element dropdown
            SerializedProperty blackboardElementIdProp = property.FindPropertyRelative(BlackboardConditional.BlackboardElementIdVarName);
            int selectedIndex = blackboardElements.FindIndex(x => x.GUID == blackboardElementIdProp.stringValue);
            if(selectedIndex == -1) // Trying to handle this here causes the apocalypse. Just rely on the NodeView.
            {
            }
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup("Blackboard Element", selectedIndex, blackboardElements.Select(x => x.Name).ToArray());
            if(EditorGUI.EndChangeCheck())
            {
                int group = Undo.GetCurrentGroup();
                Undo.RecordObject(conditionalsList.serializedObject.targetObject, "Switching BlackboardConditional element");
                blackboardElementIdProp.stringValue = blackboardElements[selectedIndex].GUID;
                NodeGraph.RemoveAllOutportsFromNode(property);
                conditionalsList.arraySize = 0;
                conditionalsList.serializedObject.ApplyModifiedProperties();
                Undo.CollapseUndoOperations(group);
            }

            GUILayout.Space(8);

            // Conditional Elements
            for (int i = 0; i < conditionalsList.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical("box");
                GUILayout.Label("Condition:");
                EditorGUILayout.PropertyField(conditionalsList.GetArrayElementAtIndex(i), true);
                GUILayout.EndVertical();
                if(GUILayout.Button("X", GUILayout.ExpandHeight(true), GUILayout.Width(20))) // Delete
                {
                    int group = Undo.GetCurrentGroup();
                    Undo.RecordObject(conditionalsList.serializedObject.targetObject, "Delete blackboard conditional");
                    conditionalsList.DeleteArrayElementAtIndex(i);
                    conditionalsList.serializedObject.ApplyModifiedProperties();
                    NodeGraph.RemoveOutportFromNode(property, i);
                    Undo.CollapseUndoOperations(group);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }

            if(GUILayout.Button("Add Conditional"))
            {
                if(selectedIndex == -1)
                {
                    return;
                }

                BlackboardElement ele = blackboardElements[selectedIndex];

                Type conditionalElementType = null;
                BlackboardConditional.BlackboardConditionalElementTypes.TryGetValue(ele.GetType(), out conditionalElementType);
                if (conditionalElementType != null)
                {
                    conditionalsList.serializedObject.Update();
                    int group = Undo.GetCurrentGroup();
                    Undo.RecordObject(conditionalsList.serializedObject.targetObject, "Add blackboard conditional");
                    IBlackboardConditionalElement newConditionalEle = Activator.CreateInstance(conditionalElementType) as IBlackboardConditionalElement;
                    conditionalsList.InsertArrayElementAtIndex(conditionalsList.arraySize);
                    conditionalsList.GetArrayElementAtIndex(conditionalsList.arraySize - 1).managedReferenceValue = newConditionalEle;
                    conditionalsList.serializedObject.ApplyModifiedProperties();
                    NodeGraph.AddOutportToNode(property);
                    Undo.CollapseUndoOperations(group);
                }
                else
                {
                    Debug.LogError("There is no implementation of BlackboardConditional for type " + ele.GetType().Name);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}