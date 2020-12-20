using System;
using UnityEngine;

namespace Logical
{
    /// <summary>
    /// The base BlackboardElement type that the rest of the Logical system should be interfacing with.
    /// It's sole implementor ABlackboardElement handles serialization support for the actual value object
    /// of the blackboard element.
    /// 
    /// Custom blackboard elements should inherit from ABlackboardElement.
    /// </summary>
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

#if UNITY_EDITOR
        /// For SerializedProperties ///
        public static readonly string Name_VarName = "m_name";
        public static readonly string ValueWrapper_VarName = "m_valueWrapper";
        public static readonly string Value_VarName = "Value";
#endif
    }
}