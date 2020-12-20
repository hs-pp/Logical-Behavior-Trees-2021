using System;
using Logical;

public class SampleNodeGraph : NodeGraph
{
    protected override Type GraphPropertiesType => typeof(SampleNodeGraphProperties);

    public class SampleNodeGraphProperties : AGraphProperties
    {
        public int thisint = 6;
        public string thisstring = "";
        public ThisSerializableClass serializedClass;
    }

    [Serializable]
    public class ThisSerializableClass
    {
        public int serializedInt = 2;
        public string serializedString = "";
    }
}
