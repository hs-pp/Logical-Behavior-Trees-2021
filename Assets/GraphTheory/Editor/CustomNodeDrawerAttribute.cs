using System;

namespace GraphTheory.Editor
{
    public class CustomNodeDrawerAttribute : Attribute
    {
        public Type NodeType { get; private set; }
        public CustomNodeDrawerAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }
    }
}