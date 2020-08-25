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

namespace DialogueSystem
{
    [Serializable]
    [SupportedGraphTypes(typeof(DialogueGraph))]
    public class DialogueNode : ANode
    {
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