using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logical;
using System;
using System.Runtime.Serialization;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace DialogueSystem
{
    [Serializable]
    [Node(typeof(DialogueGraph))]
    public class DialogueNode : ANode
    {
        [SerializeField]
        private string m_dialogue;

        [OnDeserializing]
        private void OnDeserialize()
        {

        }

#if UNITY_EDITOR
        //public override void DrawNodeView(NodeDisplayContainers nodeDisplayContainers, SerializedProperty serializedNode)
        //{
        //    base.DrawNodeView(nodeDisplayContainers, serializedNode);
        //    //nodeView.contentContainer.Add(new Label("content"));
        //    ////nodeView.titleContainer.Add(new Label(serializedNode.FindPropertyRelative("m_id").stringValue));
        //    ////nodeView.inputContainer.Add(new Label("input"));
        //    //nodeView.outputContainer.Add(new Label("output"));
        //    nodeDisplayContainers.MainBody.Add(new Label("extension"));
        //    //nodeView.mainContainer.Add(new Label("main"));
        //    //nodeView.RefreshExpandedState();
        //}
#endif
    }
}