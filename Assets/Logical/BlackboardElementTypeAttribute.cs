using System;

namespace Logical
{
    /// <summary>
    /// Attribute to identify blackboard elements and the types that they represent.
    /// Could this be replaced by some quick generic type identification through reflection???
    /// </summary>
    public class BlackboardElementTypeAttribute : Attribute
    {
        public Type ElementType { get; private set; }
        public BlackboardElementTypeAttribute(Type type)
        {
            ElementType = type;
        }
    }
}