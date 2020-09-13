using System;
using UnityEngine;

[Serializable]
public abstract class ABlackboardElement<T> : BlackboardElement, ISerializationCallbackReceiver
{
    [Serializable]
    private class SerializeWrapper
    {
        public T value;
    }

    private SerializeWrapper m_serializeWrapper = null;

    public ABlackboardElement()
    {
        m_guid = Guid.NewGuid().ToString();
        Type = typeof(T);
        m_name = $"new {Type.Name}";

        m_serializedType = Type.AssemblyQualifiedName;

        if (Nullable.GetUnderlyingType(Type) != null)
        {
            Value = (T)Activator.CreateInstance(Type);
        }
        else
        {
            T defaultT = default;
            Value = defaultT;
        }

        m_serializeWrapper = new SerializeWrapper() { value = (T)Value };
    }

    public void OnBeforeSerialize()
    {
        m_serializedValue = JsonUtility.ToJson(m_serializeWrapper);
    }

    public void OnAfterDeserialize()
    {
        m_serializeWrapper = JsonUtility.FromJson<SerializeWrapper>(m_serializedValue);
        Value = m_serializeWrapper.value;
    }
}
