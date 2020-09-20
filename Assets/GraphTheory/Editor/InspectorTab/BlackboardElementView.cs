using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    public class BlackboardElementView : BlackboardField
    {
        private BlackboardElement m_blackboardElement = null;
        private SerializedProperty m_serializedBlackboardElement = null;
        private Action m_onDeleteElement = null;

        public BlackboardElementView(BlackboardElement blackboardElement, SerializedProperty serializedBlackboardElement, Action onDeleteElement)
        {
            m_blackboardElement = blackboardElement;
            m_serializedBlackboardElement = serializedBlackboardElement;

            text = m_blackboardElement.Name;
            typeText = m_blackboardElement.Type.Name;

            m_onDeleteElement = onDeleteElement;

            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            capabilities &= ~Capabilities.Deletable;
        }

        public void ChangeElementName(string newName)
        {
            m_serializedBlackboardElement.FindPropertyRelative(BlackboardElement.Name_VarName).stringValue = newName;
            m_serializedBlackboardElement.serializedObject.ApplyModifiedProperties();
            text = newName;
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", (action) => m_onDeleteElement?.Invoke());
            evt.StopPropagation();
        }
    }
}