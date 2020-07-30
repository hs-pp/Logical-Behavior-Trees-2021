using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class ObjectDisplayFoldout : VisualElement
    {
        private Foldout m_foldout = null;
        private List<ObjectDisplayField> m_objectDisplayFields = new List<ObjectDisplayField>();

        public ObjectDisplayFoldout()
        {
            m_foldout = new Foldout();
            Add(m_foldout);
            style.marginBottom = 6;
        }

        public void SetName(string name)
        {
            m_foldout.text = name;
        }

        public void SetAssets(List<UnityEngine.Object> assets)
        {
            Reset();
            for(int i = 0; i < assets.Count; i++)
            {
                ObjectDisplayField newDisplayField = new ObjectDisplayField();
                newDisplayField.SetObject(assets[i]);
                m_foldout.Add(newDisplayField);
                m_objectDisplayFields.Add(newDisplayField);
            }
        }

        public void AddDisplayFieldManipulator(Func<Manipulator> manipulatorCreator)
        {
            for(int i = 0; i < m_objectDisplayFields.Count; i++)
            {
                m_objectDisplayFields[i].AddManipulator(manipulatorCreator());
            }
        }

        public void SetToggle(bool isOpen)
        {
            m_foldout.value = isOpen;
        }

        public void Reset()
        {
            foreach(ObjectDisplayField odf in m_objectDisplayFields)
            {
                m_foldout.Remove(odf);
            }
            m_objectDisplayFields.Clear();
        }

        public new class UxmlFactory : UxmlFactory<ObjectDisplayFoldout, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
    }
}