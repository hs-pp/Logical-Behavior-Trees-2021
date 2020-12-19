using System;

namespace Logical.Editor
{
    public class CustomNodeViewDrawerAttribute : Attribute
    {
        public Type NodeType { get; private set; }
        public CustomNodeViewDrawerAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }
    }
}