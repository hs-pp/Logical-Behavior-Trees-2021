using UnityEngine;
using GraphTheory.BuiltInNodes;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GraphTheory
{
    [Serializable]
    public abstract class NodeGraph : ScriptableObject
    {
        public abstract Type GraphPropertiesType { get; }
        public virtual bool UseIMGUIPropertyDrawer { get { return false; } }

        [SerializeField]
        private NodeCollection m_nodeCollection;
        [SerializeReference, HideInInspector]
        public AGraphProperties GraphProperties;
        [SerializeField]
        private BlackboardData m_blackboardData;

        public void Awake()
        {
            //TODO: Register to runtime tracker here
        }

#if UNITY_EDITOR
        public NodeCollection NodeCollection { get { return m_nodeCollection; } }
        public BlackboardData BlackboardData { get { return m_blackboardData; } }
        public Action<string> OnNodeOutportAdded = null;
        public Action<string, int> OnNodeOutportRemoved = null;

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

        public static void AddOutportToNode(SerializedProperty serializedNode)
        {
            (serializedNode.serializedObject.targetObject as NodeGraph).AddOutportToNode_Internal(serializedNode);
        }

        public static void RemoveOutportFromNode(SerializedProperty serializedNode, int index = -1)
        {
            (serializedNode.serializedObject.targetObject as NodeGraph).RemoveOutportFromNode_Internal(serializedNode, index);
        }


        private void AddOutportToNode_Internal(SerializedProperty serializedNode)
        {
            string nodeId = serializedNode.FindPropertyRelative("m_id").stringValue;
            string newOutportId = Guid.NewGuid().ToString();

            Undo.RegisterCompleteObjectUndo(this, $"Added Outport to Node {nodeId}");

            SerializedProperty outportsProperty = serializedNode.FindPropertyRelative("m_outports");
            outportsProperty.InsertArrayElementAtIndex(outportsProperty.arraySize);
            SerializedProperty newOutportProperty = outportsProperty.GetArrayElementAtIndex(outportsProperty.arraySize - 1);
            newOutportProperty.FindPropertyRelative("Id").stringValue = newOutportId;
            newOutportProperty.FindPropertyRelative("ConnectedNodeId").stringValue = "";

            serializedNode.serializedObject.ApplyModifiedProperties();
            OnNodeOutportAdded?.Invoke(nodeId);
        }

        private void RemoveOutportFromNode_Internal(SerializedProperty serializedNode, int index)
        {
            if(index == -1)
            {
                index = serializedNode.FindPropertyRelative("m_outports").arraySize - 1;
            }

            string nodeId = serializedNode.FindPropertyRelative("m_id").stringValue;
            Undo.RegisterCompleteObjectUndo(this, $"Removed Outport {index} from Node {nodeId}");
            serializedNode.FindPropertyRelative("m_outports").DeleteArrayElementAtIndex(index);
            serializedNode.serializedObject.ApplyModifiedProperties();
            OnNodeOutportRemoved?.Invoke(nodeId, index);
        }
#endif
    }
}