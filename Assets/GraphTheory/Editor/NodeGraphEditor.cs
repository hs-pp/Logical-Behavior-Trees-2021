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
                GraphTheoryWindow window = GraphTheoryWindow.OpenWindow();
                window.OpenGraph(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID)));
                return true;
            }
            return false;
        }
    }
}
