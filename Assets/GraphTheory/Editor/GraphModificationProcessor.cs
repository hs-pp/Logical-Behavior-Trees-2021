using System;
using UnityEditor;

namespace GraphTheory.Editor
{
    /// <summary>
    /// Currently Unity does not provide a way to track graphs that are duplicated :(
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