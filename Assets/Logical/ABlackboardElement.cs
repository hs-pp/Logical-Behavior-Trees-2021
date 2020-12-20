using System;
using UnityEngine;

namespace Logical
{
    /// <summary>
    /// An intermediary blackboard element where T is the actual variable type.
    /// This mostly does serialization of the specified generic type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ABlackboardElement<T> : BlackboardElement, ISerializationCallbackReceiver
    {
        [Serializable]
        private class ValueWrapper
        {
            public T Value;
        }

        [SerializeField]
        private ValueWrapper m_valueWrapper = null;

        public override object Value
        {
            get { return m_valueWrapper.Value; }
            set { m_valueWrapper.Value = (T)value; }
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

            m_valueWrapper = new ValueWrapper() { Value = (T)newValue };
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            Type = Type.GetType(m_serializedType);
        }
    }

    // Default element types
    [BlackboardElementType(typeof(string))]
    public class StringBlackboardElement : ABlackboardElement<string> { }
    [BlackboardElementType(typeof(bool))]
    public class BoolBlackboardElement : ABlackboardElement<bool> { }
    [BlackboardElementType(typeof(int))]
    public class IntBlackboardElement : ABlackboardElement<int> { }
    [BlackboardElementType(typeof(float))]
    public class FloatBlackboardElement : ABlackboardElement<float> { }
    [BlackboardElementType(typeof(serializablecoolclass))]
    public class sccBlackboardElement : ABlackboardElement<serializablecoolclass> { }

    [Serializable]
    public class serializablecoolclass
    {
        public string stringone;
        public bool boolone;
        public float floatone;
        public serializablekindacoolclass kindacool;
    }

    [Serializable]
    public class serializablekindacoolclass
    {
        public string stringone;
        public bool boolone;
        public float floatone;

    }
}