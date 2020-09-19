using System;
using UnityEngine;

[Serializable]
public abstract class BlackboardElement
{
    [SerializeField]
    protected string m_guid;
    [SerializeField]
    protected string m_name;
    
    [SerializeField]
    protected string m_serializedType;

    public string GUID { get { return m_guid; } }
    public string Name { get { return m_name; } set { m_name = value; } }
    public Type Type { get; protected set; }
    public abstract object Value { get; set; }
}
