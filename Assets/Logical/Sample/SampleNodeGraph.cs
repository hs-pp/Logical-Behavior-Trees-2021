using Logical;

public class SampleNodeGraph : NodeGraph
{
    /// <summary>
    /// We defined the GraphProperties class structure for this graph type here.
    /// </summary>
    [GraphProperties(typeof(SampleNodeGraph))]
    public class SampleNodeGraphProperties : AGraphProperties
    {
        public int thisint = 6;
        public string thisstring = "";
    }
}
