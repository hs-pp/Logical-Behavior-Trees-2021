using System;
using System.Collections.Generic;
using System.Linq;

namespace Logical
{
    /// <summary>
    /// This attribute is to be used on ANode child classes to indicate which
    /// Graph types the node should be available for.
    /// </summary>
    public class SupportedGraphTypesAttribute : Attribute
    {
        public Type GraphType { get; private set; }
        public SupportedGraphTypesAttribute(Type graphType)
        {
            GraphType = graphType;
        }
    }
}