using System;
using System.Collections;
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

    public void AddElement(BlackboardElement element)
    {
        m_allElements.Add(element);
    }

    public void RemoveElement(string guid)
    {
        BlackboardElement element = m_allElements.Find(x => x.GUID == guid);
        m_allElements.Remove(element);
    }
}

[BlackboardElementType(typeof(string))]
public class StringBlackboardElement : ABlackboardElement<string> { }
[BlackboardElementType(typeof(bool))]
public class BoolBlackboardElement : ABlackboardElement<bool> { }
[BlackboardElementType(typeof(float))]
public class FloatBlackboardElement : ABlackboardElement<float> { }