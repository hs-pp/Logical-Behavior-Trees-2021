using System;

namespace Logical.Editor
{
    /// <summary>
    /// Attribute to assign to a custom node view drawer to associate it to an ANode child class.
    /// </summary>
    public class CustomNodeViewDrawerAttribute : Attribute
    {
        public Type NodeType { get; private set; }
        public CustomNodeViewDrawerAttribute(Type nodeType)
        {
            NodeType = nodeType;
        }
    }
}