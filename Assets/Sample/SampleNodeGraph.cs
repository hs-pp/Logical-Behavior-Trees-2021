using System;
using Logical;

public class SampleNodeGraph : NodeGraph
{
    [GraphProperties(typeof(SampleNodeGraph))]
    public class SampleNodeGraphProperties : AGraphProperties
    {
        public int thisint = 6;
        public string thisstring = "";
    }
}
