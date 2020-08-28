using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleNodeGraphController : NodeGraphController
{
    public float m_newMaxTime = 0;
    private SampleNodeGraph SampleGraph { get; set; }
    protected override void Awake()
    {
        base.Awake();
        SampleGraph = (m_nodeGraph as SampleNodeGraph);

        // === Add any init code here ===//
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
