using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory;
using System;
using System.Runtime.Serialization;

[Serializable]
[SupportedGraphTypes(typeof(TestGraph))]
public class TestNode : ANode
{
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

    public override void OnNodeEnter(NodeCollection nodeCollection)
    {
        Debug.Log("TestNode Enter");
        base.OnNodeEnter(nodeCollection);
        m_maxTime = (nodeCollection.ParentNodeGraph as TestGraph).waitTime;
        elapsedTime = 0;
    }

    public override void OnNodeUpdate()
    {
        elapsedTime += Time.deltaTime;
        Debug.Log("TestNode " + elapsedTime + "         " + m_maxTime);

        if (elapsedTime > m_maxTime)
        {
            TraverseEdge(0);
        }
    }

    public override void OnNodeExit()
    {
        Debug.Log("TestNode Exit");
    }

#if UNITY_EDITOR
    public TestNode() : base()
    {
        CreateOutport();
    }
#endif
}
