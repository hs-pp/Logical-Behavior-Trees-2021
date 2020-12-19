using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical
{
    [System.Serializable]
    public class OutportEdge
    {
        public string Id;
        public string ConnectedNodeId;

#if UNITY_EDITOR
        public static readonly string IdVarName = "Id";
        public static readonly string ConnectedNodeIdVarName = "ConnectedNodeId";

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
#endif
    }
}