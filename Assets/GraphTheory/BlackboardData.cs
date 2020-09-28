using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlackboardData
{
    [SerializeReference]
    private List<BlackboardElement> m_allElements = new List<BlackboardElement>();

    public BlackboardElement GetElement(string name)
    {
        return m_allElements.Find(x => x.Name == name);
    }

    public BlackboardElement GetElement(int index)
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