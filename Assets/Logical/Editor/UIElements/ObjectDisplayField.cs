using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Logical.Editor.UIElements
{
    public class ObjectDisplayField : VisualElement
    {
        private ObjectField m_objectField = null;
        public UnityEngine.Object ObjectRef { get; private set; } = null;

        public Action<UnityEngine.Object> OnDoubleClick = null;

        public ObjectDisplayField()
        {
            m_objectField = new ObjectField();
            m_objectField.SetEnabled(false);
            this.Add(m_objectField);
            //this.AddManipulator(new Clickable(OpenGraphInstance));
            this.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public void SetObject(UnityEngine.Object objectInstance)
        {
            m_objectField.value = objectInstance;
            ObjectRef = objectInstance;
        }

        private void OpenGraphInstance()
        {
            if (m_objectField.value != null)
            {
                EditorGUIUtility.PingObject(m_objectField.value);
            }
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;

            if(evt.clickCount == 1)
            {
                EditorGUIUtility.PingObject(ObjectRef);
            }
            else if(evt.clickCount == 2)
            {
                OnDoubleClick?.Invoke(ObjectRef);
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