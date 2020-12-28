using System;

namespace Logical
{
    /// <summary>
    /// Attribute to identify blackboard elements for BlackboardSetter and BlackboardConditional.
    /// </summary>
    public class BlackboardElementAttribute : Attribute
    {
        public Type ElementType { get; private set; }
        public BlackboardElementAttribute(Type type)
        {
            ElementType = type;
        }
    }
}