using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory;
using System;
using System.Runtime.Serialization;

[SupportedGraphTypes(typeof(TestGraph))]
public class TestNode : ANode
{
    public override string Name { get { return "TestNode"; } }
    
    private float m_maxTime = 0;
    private float elapsedTime = 0;

    /// <summary>
    /// Default constructed values to private variables are always ignored by Odin.
    /// https://bitbucket.org/sirenix/odin-inspector/issues/633/odin-is-not-initializing-private-non
    /// The workaround is to call an OnDeserializing method to init values.
    /// </summary>
    [OnDeserializing]
    private void OnDeserialize()
    {
        m_maxTime = 9;
    }

    public override void OnNodeEnter(NodeGraph nodeGraph)
    {
        Debug.Log("TestNode Enter");
        base.OnNodeEnter(nodeGraph);
        m_maxTime = (nodeGraph as TestGraph).waitTime;
        elapsedTime = 0;
    }

    public override void OnNodeUpdate()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("TestNode " + elapsedTime + "         " + m_maxTime);

        if (elapsedTime > m_maxTime)
        {
            ParentGraph.ChangeNode(this, GetOutportEdge(0));
        }
    }

    public override void OnNodeExit()
    {
        Debug.Log("TestNode Exit");
    }

#if UNITY_EDITOR
    public override List<Type> CompatibleGraphs { get { return new List<Type> { typeof(TestGraph) }; } }

    public TestNode() : base()
    {
        CreateOutport();
    }
#endif
}
