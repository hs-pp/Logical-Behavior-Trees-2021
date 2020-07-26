using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GraphTheory.Editor 
{
    public class NodeGraphEditor
    {
        [MenuItem("Graph/GraphTheory")]
        public static GraphTheoryWindow OpenWindow()
        {
            var window = EditorWindow.GetWindow<GraphTheoryWindow>();
            window.titleContent = new GUIContent("NodeGraph");
            window.Show();
            return window;
        }

        [OnOpenAsset(1)]
        public static bool OnGraphAssetOpen(int instanceID, int line)
        {
            if (typeof(NodeGraph).IsAssignableFrom(EditorUtility.InstanceIDToObject(instanceID).GetType()))
            {
                GraphTheoryWindow window = OpenWindow();
                window.OpenGraph(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID)));
                return true;
            }
            return false;
        }
    }
}
