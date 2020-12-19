using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Logical.Editor 
{
    public class NodeGraphEditor
    {
        [MenuItem("Graph/Logical Graph")]
        public static LogicalTheoryWindow OpenWindow()
        {
            var window = EditorWindow.GetWindow<LogicalTheoryWindow>();
            window.titleContent = new GUIContent("Logical");
            window.Show();
            return window;
        }

        [OnOpenAsset(1)]
        public static bool OnGraphAssetOpen(int instanceID, int line)
        {
            if (typeof(NodeGraph).IsAssignableFrom(EditorUtility.InstanceIDToObject(instanceID).GetType()))
            {
                LogicalTheoryWindow window = OpenWindow();
                window.OpenGraph(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID)));
                return true;
            }
            return false;
        }
    }
}
