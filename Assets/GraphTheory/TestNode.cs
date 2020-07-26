using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory;
using System;

[Serializable]
[SupportedGraphTypes(typeof(TestGraph))]
public class TestNode : ANode
{
    public override string Name { get { return "TestNode"; } }


#if UNITY_EDITOR
    public override List<Type> CompatibleGraphs { get { return new List<Type> { typeof(TestGraph) }; } }

    public TestNode() : base()
    {
        CreateOutport();
    }
#endif
}
