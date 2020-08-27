using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTheory
{
    [Serializable]
    public abstract class ANode
    {
        [SerializeField, HideInInspector]
        private string m_id = Guid.NewGuid().ToString();
        public string Id { get { return m_id; } }
        [SerializeField]
        private List<OutportEdge> m_outports = new List<OutportEdge>(0);

        protected NodeCollection ParentNodeCollection { get; private set; } = null;

        public virtual void OnNodeEnter(NodeCollection nodeCollection) { ParentNodeCollection = nodeCollection; }//TODO: auto set parent node collections before node enter
        public virtual void OnNodeUpdate() { }
        public virtual void OnNodeExit() { }

        public OutportEdge GetOutportEdge(int index)
        {
            return m_outports[index];
        }

        public bool OutportEdgeIsValid(int index)
        {
            if (index >= m_outports.Count || index < 0)
            {
                return false;
            }
            return m_outports[index].IsValid;
        }

        protected void TraverseEdge(int index)
        {
            if (index >= m_outports.Count || index < 0)
            {
                Debug.LogError("edge index out of range");
                return;
            }
            else
            {
                ParentNodeCollection.TraverseEdge(GetOutportEdge(index));
            }
        }

        [SerializeField, HideInInspector]
        private Vector2 m_position;
        [SerializeField, HideInInspector]
        private string m_comment;
        public virtual int DefaultNumOutports { get { return 1; } }

#if UNITY_EDITOR
        public Vector2 Position { get { return m_position; } set { m_position = value; } }

        public ANode()
        {
            // Instantiate the default number of ports
            for (int i = 0; i < DefaultNumOutports; i++)
            {
                m_outports.Add(new OutportEdge() { Id = Guid.NewGuid().ToString()});
            }
        }

        public void SanitizeNodeCopy(string newId, Vector2 position, Dictionary<string, string> oldToNewIdList)
        {
            m_id = newId;
            m_position = position;
            for (int i = m_outports.Count - 1; i >= 0; i--)
            {
                m_outports[i].Id = Guid.NewGuid().ToString();
                if (oldToNewIdList.TryGetValue(m_outports[i].ConnectedNodeId, out string foundId))
                {
                    m_outports[i].ConnectedNodeId = foundId;
                }
                else
                {
                    m_outports[i].SetInvalid();
                }
            }
        }

        public List<OutportEdge> GetAllEdges()
        {
            return m_outports;
        }

        public void AddOutportEdge(int outportIndex, string connectedEdge)
        {
            if (outportIndex > m_outports.Count - 1)
            {
                Debug.LogError("Error adding outport edge!");
                return;
            }
            m_outports[outportIndex].ConnectedNodeId = connectedEdge;
        }

        public void RemoveOutportEdge(int outportIndex)
        {
            if (outportIndex > m_outports.Count - 1)
            {
                Debug.LogError("Error removing outport edge!");
                return;
            }
            m_outports[outportIndex].SetInvalid();
        }
#endif
    }
}