using System;
using GraphTheory;

public class SampleNodeGraph : NodeGraph
{
    public override Type GraphPropertiesType => typeof(SampleNodeGraphProperties);

    public class SampleNodeGraphProperties : IGraphProperties
    {

    }
}
