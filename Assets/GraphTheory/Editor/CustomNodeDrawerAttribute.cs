using System;

public class CustomNodeDrawerAttribute : Attribute
{
    public Type NodeType { get; private set; }
    public CustomNodeDrawerAttribute(Type nodeType)
    {
        NodeType = nodeType;
    }
}
