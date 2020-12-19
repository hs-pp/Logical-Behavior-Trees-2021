using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logical
{
    [Serializable]
    public class BlackboardData
    {
        [SerializeReference]
        private List<BlackboardElement> m_allElements = new List<BlackboardElement>();

        public BlackboardElement GetElementByName(string name)
        {
            return m_allElements.Find(x => x.Name == name);
        }
        public BlackboardElement GetElementById(string id)
        {
            return m_allElements.Find(x => x.GUID == id);
        }

        public BlackboardElement GetElementAt(int index)
        {
            return m_allElements[index];
        }

        public void AddElement(BlackboardElement element)
        {
            m_allElements.Add(element);
        }

        public void RemoveElement(string guid)
        {
            BlackboardElement element = m_allElements.Find(x => x.GUID == guid);
            m_allElements.Remove(element);
        }

        public List<BlackboardElement> GetAllElements()
        {
            return m_allElements;
        }

#if UNITY_EDITOR
        /// For SerializedProperties ///
        public static readonly string AllElements_VarName = "m_allElements";
#endif
    }
}