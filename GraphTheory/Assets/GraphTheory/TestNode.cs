using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory;

public class TestNode : ANode
{
    public override string Name { get { return "TestNode"; } }

    public override int NumInports { get { return 1; } }
    public override int NumOutports { get { return 3; } }

#if UNITY_EDITOR

#endif
}
