using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GraphTheory.Editor 
{
    public class NodeGraphEditor
    {
        [OnOpenAsset(1)]
        public static bool step1(int instanceID, int line)
        {
            if (typeof(NodeGraph).IsAssignableFrom(EditorUtility.InstanceIDToObject(instanceID).GetType()))
            {
                Debug.Log(EditorUtility.InstanceIDToObject(instanceID).GetType() + "yes");
                GraphTheoryWindow.OpenWindow(); 
                return true;
            }
            return false;
        }
    }
}
