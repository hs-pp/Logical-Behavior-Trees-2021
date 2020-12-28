using System;

namespace Logical
{
    public class GraphPropertiesAttribute : Attribute
    {
        public Type GraphType { get; private set; }
        public GraphPropertiesAttribute(Type graphType)
        {
            GraphType = graphType;
        }
    }
}