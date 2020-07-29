using GraphTheory.Internal.OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTheory
{
    public class NodeGraphRunner : SerializedMonoBehaviour
    {
        [SerializeField]
        private NodeGraph m_nodeGraph = null;
        public NodeGraph NodeGraph { get { return m_nodeGraph; } }
        [SerializeField]
        public List<GraphParam> m_graphParams = new List<GraphParam>();
    }

    [Serializable]
    public class GraphParam
    {
        public string ParamName = "";
        public Type ParamType = null;
        public object ParamValue = null;
    }
}