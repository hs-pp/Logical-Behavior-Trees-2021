using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.BuiltInNodes
{
    [CustomPropertyDrawer(typeof(BlackboardConditional))]
    public class BlackboardConditionalDrawer : PropertyDrawer
    {
        private class BlackboardElementPopupEle
        {
            public BlackboardElement Element;
            public string GetLabel()
            {
                return Element != null ? Element.Name : "NONE";
            }
            public string GetLabelWithType()
            {
                return Element != null ? $"{Element.Name} ({Element.Type.Name})" : "NONE (NONE)";
            }
            public string GetId()
            {
                return Element != null ? Element.GUID : "";
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement ve = new VisualElement();

            List<BlackboardElementPopupEle> elements = new List<BlackboardElementPopupEle>();
            elements.Add(new BlackboardElementPopupEle());
            
            foreach (BlackboardElement ele in (property.serializedObject.targetObject as NodeGraph).BlackboardData.GetAllElements())
            {
                elements.Add(new BlackboardElementPopupEle() { Element = ele });
            }
            SerializedProperty blackboardElementIdProp = property.FindPropertyRelative("m_blackboardElementId");

            int selectedIndex = elements.FindIndex(x => x.GetId() == blackboardElementIdProp.stringValue);
            if(selectedIndex == -1) // Currently selected id is invalid.
            {
                blackboardElementIdProp.serializedObject.Update();
                selectedIndex = 0;
                blackboardElementIdProp.stringValue = "";
                blackboardElementIdProp.serializedObject.ApplyModifiedProperties();
            }
            else if(string.IsNullOrEmpty(blackboardElementIdProp.stringValue)) // set to nothing
            {
            }

            PopupField<BlackboardElementPopupEle> popupField = new PopupField<BlackboardElementPopupEle>("BlackboardElement:", elements, selectedIndex, 
                (ele) => { return ele.GetLabel(); }, 
                (ele) => { return ele.GetLabelWithType(); });

            void OnBlackboardElementChanged(ChangeEvent<BlackboardElementPopupEle> evt)
            {
                blackboardElementIdProp.serializedObject.Update();
                blackboardElementIdProp.stringValue = evt.newValue.GetId();
                blackboardElementIdProp.serializedObject.ApplyModifiedProperties();
            }

            popupField.RegisterValueChangedCallback(OnBlackboardElementChanged);

            ve.Add(popupField);

            PropertyField propField = new PropertyField(blackboardElementIdProp, " ");
            propField.Bind(property.serializedObject);
            propField.SetEnabled(false);

            ve.Add(propField);
            return ve;

        }
    }
}