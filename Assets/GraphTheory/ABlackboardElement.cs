using System;
using UnityEngine;

[Serializable]
public abstract class ABlackboardElement<T> : BlackboardElement, ISerializationCallbackReceiver
{
    [Serializable]
    private class ValueWrapper
    {
        public T value;
    }

    [SerializeField]
    private ValueWrapper m_valueWrapper = null;

    public override object Value
    {
        get
        {
            return m_valueWrapper.value;
        }
        set
        {
            m_valueWrapper.value = (T)value;
        }
    }

    public ABlackboardElement()
    {
        m_guid = Guid.NewGuid().ToString();
        Type = typeof(T);
        m_name = $"new {Type.Name}";

        m_serializedType = Type.AssemblyQualifiedName;

        object newValue;
        if (Nullable.GetUnderlyingType(Type) != null)
        {
            newValue = (T)Activator.CreateInstance(Type);
        }
        else
        {
            T defaultT = default;
            newValue = defaultT;
        }

        m_valueWrapper = new ValueWrapper() { value = (T)newValue };
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        Type = Type.GetType(m_serializedType);
    }
}
