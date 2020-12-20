using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logical
{
    /// <summary>
    /// The container that holds all of our blackboard elements.
    /// Blackboard elements can be added to a graph using the graph editor window.
    /// Elements are only intended to be accessable within the graph using the 
    /// built-in BlackboardSetter and BlackboardConditional nodes as well as custom
    /// nodes that may need them.
    /// 
    /// These elements are also accessable from code using GetElementByName(). 
    /// However, this is not recommended because blackboard elements are 
    /// graph instance-based and cannot be guaranteed to exist in a particular
    /// graph asset.
    /// </summary>
    [Serializable]
    public class BlackboardData
    {
        [SerializeReference]
        private List<BlackboardElement> m_allElements = new List<BlackboardElement>();

        public BlackboardElement GetElementByName(string name)
        {
            return m_allElements.Find(x => x.Name == name);
        }

#if UNITY_EDITOR

        public BlackboardElement GetElementById(string id)
        {
            return m_allElements.Find(x => x.GUID == id);
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

        /// For SerializedProperties ///
        public static readonly string AllElements_VarName = "m_allElements";
#endif
    }
}