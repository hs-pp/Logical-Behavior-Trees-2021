using UnityEngine;
using Logical.BuiltInNodes;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Logical
{
    /// <summary>
    /// The base graph asset.
    /// Actual graph classes should be inheriting from this.
    /// </summary>
    [Serializable]
    public abstract class NodeGraph : ScriptableObject
    {
        [SerializeField]
        private NodeCollection m_nodeCollection; // The nodes
        [SerializeReference, HideInInspector]
        public AGraphProperties GraphProperties; // Properties universally associated to the graph
        [SerializeField]
        private BlackboardData m_blackboardData; // Properties associated to a specific instance of the graph.

#if UNITY_EDITOR
        protected abstract Type GraphPropertiesType { get; } // Only necessary to determine what class to instantiate.
        public virtual bool UseIMGUIPropertyDrawer { get { return false; } } // Toggle on for LogicalGraphWindow to draw these using IMGUI. Defaulted to use UIToolkit.

        public NodeCollection NodeCollection { get { return m_nodeCollection; } }
        public BlackboardData BlackboardData { get { return m_blackboardData; } }
        public Action<string> OnNodeOutportAdded = null;
        public Action<string, int> OnNodeOutportRemoved = null;
        public Action<string> OnNodeAllOutportsRemoved = null;

        /// <summary>
        /// The constructor will only ever be called in editor.
        /// </summary>
        public NodeGraph()
        {
            m_nodeCollection = new NodeCollection();
            ANode entryNode = m_nodeCollection.CreateNode(typeof(EntryNode), Vector2.zero);
            m_nodeCollection.SetEntryNode(entryNode.Id);
            GraphProperties = Activator.CreateInstance(GraphPropertiesType) as AGraphProperties;
            m_blackboardData = new BlackboardData();
            m_blackboardData.AddElement(new StringBlackboardElement());
            m_blackboardData.AddElement(new BoolBlackboardElement());
        }

        public void Awake()
        {
            //TODO: Register to runtime tracker here
        }

        public static void AddOutportToNode(SerializedProperty serializedNode)
        {
            (serializedNode.serializedObject.targetObject as NodeGraph).AddOutportToNode_Internal(serializedNode);
        }

        public static void RemoveOutportFromNode(SerializedProperty serializedNode, int index = -1)
        {
            (serializedNode.serializedObject.targetObject as NodeGraph).RemoveOutportFromNode_Internal(serializedNode, index);
        }

        public static void RemoveAllOutportsFromNode(SerializedProperty serializedNode)
        {
            (serializedNode.serializedObject.targetObject as NodeGraph).RemoveAllOutportsFromNode_Internal(serializedNode);
        }

        private void AddOutportToNode_Internal(SerializedProperty serializedNode)
        {
            serializedNode.serializedObject.Update();
            string nodeId = serializedNode.FindPropertyRelative(ANode.IdVarname).stringValue;
            string newOutportId = Guid.NewGuid().ToString();

            SerializedProperty outportsProperty = serializedNode.FindPropertyRelative(ANode.OutportsVarName);
            outportsProperty.InsertArrayElementAtIndex(outportsProperty.arraySize);
            SerializedProperty newOutportProperty = outportsProperty.GetArrayElementAtIndex(outportsProperty.arraySize - 1);
            newOutportProperty.FindPropertyRelative(OutportEdge.IdVarName).stringValue = newOutportId;
            newOutportProperty.FindPropertyRelative(OutportEdge.ConnectedNodeIdVarName).stringValue = "";

            serializedNode.serializedObject.ApplyModifiedProperties();
            OnNodeOutportAdded?.Invoke(nodeId);
        }

        private void RemoveOutportFromNode_Internal(SerializedProperty serializedNode, int index)
        {
            SerializedProperty nodeOutports = serializedNode.FindPropertyRelative(ANode.OutportsVarName);
            if (index == -1)
            {
                index = nodeOutports.arraySize - 1;
            }

            serializedNode.serializedObject.Update();
            string nodeId = serializedNode.FindPropertyRelative(ANode.IdVarname).stringValue;
            nodeOutports.DeleteArrayElementAtIndex(index);
            serializedNode.serializedObject.ApplyModifiedProperties();
            OnNodeOutportRemoved?.Invoke(nodeId, index);
        }

        private void RemoveAllOutportsFromNode_Internal(SerializedProperty serializedNode)
        {
            string nodeId = serializedNode.FindPropertyRelative(ANode.IdVarname).stringValue;
            serializedNode.FindPropertyRelative(ANode.OutportsVarName).arraySize = 0;
            serializedNode.serializedObject.ApplyModifiedProperties();
            OnNodeAllOutportsRemoved?.Invoke(nodeId);
        }

        /// For SerializedProperties ///
        public static readonly string NodeCollection_VarName = "m_nodeCollection";
        public static readonly string GraphProperties_VarName = "GraphProperties";
        public static readonly string BlackboardData_VarName = "m_blackboardData";

#endif
    }
}