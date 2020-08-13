using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory;
using System;
using System.Runtime.Serialization;

[Serializable]
[SupportedGraphTypes(typeof(DialogueGraph))]
public class DialogueNode : ANode
{
    public override string Name { get { return "Dialogue"; } }

    [SerializeField]
    private string m_dialogue;
    
    [OnDeserializing]
    private void OnDeserialize()
    {

    }

    public override void OnNodeEnter(NodeCollection nodeCollection)
    {
        base.OnNodeEnter(nodeCollection);
    }

    public override void OnNodeUpdate()
    {
    }

    public override void OnNodeExit()
    {
    }

#if UNITY_EDITOR
    public override List<Type> CompatibleGraphs { get { return new List<Type> { typeof(TestGraph) }; } }

    public DialogueNode() : base()
    {
        CreateOutport();
    }
#endif
}
