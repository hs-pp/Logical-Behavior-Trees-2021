using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Logical.BuiltInNodes
{
    [CustomPropertyDrawer(typeof(BlackboardSetter))]
    public class BlackboardSetterDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.serializedObject.Update();
            bool blackboardEleIsSet = true;

            SerializedProperty setterValueProp = property.FindPropertyRelative(BlackboardSetter.SetterValueVarName);
            List<BlackboardElement> blackboardElements = (property.serializedObject.targetObject as NodeGraph).BlackboardProperties.GetAllElements();
            SerializedProperty blackboardElementIdProp = property.FindPropertyRelative(BlackboardSetter.BlackboardElementIdVarName);
            int selectedIndex = blackboardElements.FindIndex(x => x.GUID == blackboardElementIdProp.stringValue);
            if (selectedIndex == -1) // Trying to handle this here causes the apocalypse. Just rely on the NodeView.
            {
                blackboardEleIsSet = false;
            }

            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUILayout.Popup("Blackboard Element", selectedIndex, blackboardElements.Select(x => x.Name).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                int group = Undo.GetCurrentGroup();
                Undo.RecordObject(property.serializedObject.targetObject, "Switching BlackboardConditional element");
                blackboardElementIdProp.stringValue = blackboardElements[selectedIndex].GUID;

                BlackboardElement ele = blackboardElements[selectedIndex];
                BlackboardSetter.BlackboardSetterElementTypes.TryGetValue(ele.GetType(), out Type setterElementType);
                if(setterElementType == null)
                {
                    setterValueProp.managedReferenceValue = null;
                    blackboardEleIsSet = false;
                }
                else
                {
                    IBlackboardSetterElement newSetterEle = Activator.CreateInstance(setterElementType) as IBlackboardSetterElement;
                    setterValueProp.managedReferenceValue = newSetterEle;
                }

                property.serializedObject.ApplyModifiedProperties();
                Undo.CollapseUndoOperations(group);
            }

            if (blackboardEleIsSet)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(setterValueProp);
                GUILayout.EndVertical();
            }

            EditorGUI.EndProperty();
        }
    }
}