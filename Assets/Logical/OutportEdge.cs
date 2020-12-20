using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical
{
    /// <summary>
    /// This class represents an outgoing port on a node.
    /// If the ConnectedNodeId is null, we can infer that the outport exists but a 
    /// connection out of it does not.
    /// 
    /// These node connections are singly linked. InportEdge as a class does not exist.
    /// </summary>
    [System.Serializable]
    public class OutportEdge
    {
        public string Id;
        public string ConnectedNodeId;

#if UNITY_EDITOR
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ConnectedNodeId);
        }

        public void SetInvalid()
        {
            ConnectedNodeId = "";
        }

        public static bool IsValid(SerializedProperty outportEdgeProp)
        {
            return !string.IsNullOrEmpty(outportEdgeProp.FindPropertyRelative(ConnectedNodeIdVarName).stringValue);
        }

        public static void SetInvalid(SerializedProperty outportEdgeProp)
        {
            outportEdgeProp.FindPropertyRelative(ConnectedNodeIdVarName).stringValue = "";
            outportEdgeProp.serializedObject.ApplyModifiedProperties();
        }

        public static readonly string IdVarName = "Id";
        public static readonly string ConnectedNodeIdVarName = "ConnectedNodeId";
#endif
    }
}