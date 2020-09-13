using System;
using UnityEngine;

[Serializable]
public class BlackboardElement
{
    [SerializeField]
    protected string m_guid;
    [SerializeField]
    protected string m_name;
    
    [SerializeField]
    protected string m_serializedType;
    [SerializeField]
    protected string m_serializedValue;

    public string GUID { get { return m_guid; } }
    public string Name { get { return m_name; } set { m_name = value; } }
    public Type Type { get; protected set; }
    public object Value { get; protected set; }

    public T GetValue<T>()
    {
        return (T)Value;
    }
}
