using System;

namespace Logical
{
    /// <summary>
    /// This attribute is to be used on ANode child classes to indicate which
    /// Graph types the node should be available for.
    /// </summary>
    public class NodeAttribute : Attribute
    {
        public Type GraphType { get; private set; }
        public NodeAttribute(Type graphType)
        {
            GraphType = graphType;
        }
    }
}