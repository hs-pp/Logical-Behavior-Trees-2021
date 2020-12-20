using System;
using UnityEditor;

namespace Logical.Editor
{
    /// <summary>
    /// This class allows the LogicalGraphWindow to listen to graph asset modifications from outside the editor window.
    /// Currently, creating a graph, deleting a graph, and moving a graph from outside the editor window will notify the
    /// window to update itself accordingly.
    /// NOTE: Currently Unity does not provide a way to listen for assets that are duplicated :( Please dont fire me.
    /// </summary>
    public class GraphModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        public static Action<NodeGraph> OnGraphCreated = null;
        public static Action<NodeGraph> OnGraphWillDelete = null;
        public static Action<NodeGraph, string> OnGraphWillMove = null;

        public static void OnAssetCreated(NodeGraph nodeGraph)
        {
            OnGraphCreated?.Invoke(nodeGraph);
        }

        private static AssetDeleteResult OnWillDeleteAsset(string sourcePath, RemoveAssetOptions removeAssetOptions)
        {
            if (OnGraphWillDelete != null)
            {
                NodeGraph graphToDelete = AssetDatabase.LoadAssetAtPath<NodeGraph>(sourcePath);
                if (graphToDelete != null)
                {
                    OnGraphWillDelete?.Invoke(graphToDelete);
                }
            }
            return AssetDeleteResult.DidNotDelete;
        }

        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            if (OnGraphWillMove != null)
            {
                NodeGraph graphToDelete = AssetDatabase.LoadAssetAtPath<NodeGraph>(sourcePath);
                if (graphToDelete != null)
                {
                    OnGraphWillMove?.Invoke(graphToDelete, destinationPath);
                }
            }
            return AssetMoveResult.DidNotMove;
        }

        private static void OnGraphLoadedInRuntime(NodeGraph nodeGraph)
        {

        }
    }
}