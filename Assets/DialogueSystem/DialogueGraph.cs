using GraphTheory;
using System;

namespace DialogueSystem
{
    public class DialogueGraph : NodeGraph
    {
        public override Type GraphPropertiesType => typeof(DialogueGraphProperties);

        public class DialogueGraphProperties : IGraphProperties
        {
            public bool hey;
        }
    }
}