using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class ObjectDisplayField : VisualElement
    {
        private ObjectField m_objectField = null;
        private Object m_objectRef = null;

        public ObjectDisplayField()
        {
            m_objectField = new ObjectField();
            m_objectField.SetEnabled(false);
            this.Add(m_objectField);
            this.AddManipulator(new Clickable(OpenGraphInstance));
        }

        public void SetObject(Object objectInstance)
        {
            m_objectField.value = objectInstance;
        }

        private void OpenGraphInstance()
        {
            if (m_objectField.value != null)
            {
                EditorGUIUtility.PingObject(m_objectField.value);
            }
        }

        public new class UxmlFactory : UxmlFactory<ObjectDisplayField, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
    }
}