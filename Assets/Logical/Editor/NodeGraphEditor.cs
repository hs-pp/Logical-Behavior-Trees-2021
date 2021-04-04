using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Logical.Editor 
{
    [CustomEditor(typeof(NodeGraph), true)]
    public class NodeGraphEditor : UnityEditor.Editor
    {
        [MenuItem("Tools/Logical Graph")]
        public static LogicalTheoryWindow OpenWindow()
        {
            var window = EditorWindow.GetWindow<LogicalTheoryWindow>();
            window.titleContent = new GUIContent("Logical");
            window.Show();
            return window;
        }

        /// <summary>
        /// Temp method to clear graph data for testing.
        /// </summary>
        //[MenuItem("Tools/Clear Graph Data")]
        public static void ClearGraphData()
        {
            EditorPrefs.SetString("GraphWindowData", JsonUtility.ToJson(new GraphWindowData(), true));
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

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            Button openButton = new Button();
            openButton.text = "Open Graph";
            openButton.clicked += () =>
            {
                LogicalTheoryWindow window = OpenWindow();
                window.OpenGraph(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath((target as NodeGraph).GetInstanceID())));
            };
            container.Add(openButton);
            return container;
        }
    }
}
