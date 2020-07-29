using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGraphController : NodeGraphController
{
    public float m_newMaxTime = 0;
    private TestGraph TestGraph { get; set; }
    protected override void Awake()
    {
        base.Awake();
        TestGraph = (m_nodeGraph as TestGraph);

        // === Add any init code here ===//
        TestGraph.waitTime = m_newMaxTime;
        // ==========================//
    }

    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
}
