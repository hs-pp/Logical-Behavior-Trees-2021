using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTheory;
using System;
using System.Runtime.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public override void DrawNodeView(Node nodeView, SerializedProperty serializedNode)
    {
        base.DrawNodeView(nodeView, serializedNode);
        nodeView.contentContainer.Add(new Label("content"));
        nodeView.titleContainer.Add(new Label(serializedNode.FindPropertyRelative("m_id").stringValue));
        nodeView.inputContainer.Add(new Label("input"));
        nodeView.outputContainer.Add(new Label("output"));
        nodeView.extensionContainer.Add(new Label("extension"));
        nodeView.mainContainer.Add(new Label("main"));
        nodeView.RefreshExpandedState();
    }
#endif
}
