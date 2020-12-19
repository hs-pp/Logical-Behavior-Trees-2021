using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logical
{
    [Serializable]
    public abstract class ANode
    {
        [SerializeField, HideInInspector]
        private string m_id = Guid.NewGuid().ToString();
        public string Id { get { return m_id; } }
        [SerializeField, HideInInspector]
        private List<OutportEdge> m_outports = new List<OutportEdge>(0);

        public virtual void OnNodeEnter(GraphRunner graphRunner) { }
        public virtual void OnNodeUpdate(GraphRunner graphRunner) { }
        public virtual void OnNodeExit(GraphRunner graphRunner) { }

        public OutportEdge GetOutportEdge(int index)
        {
            return m_outports[index];
        }

        public bool ContainsOutport(OutportEdge outportEdge)
        {
            return m_outports.Exists(x => x.Id == outportEdge.Id);
        }

        [SerializeField, HideInInspector]
        private Vector2 m_position;
        [SerializeField, HideInInspector]
        private string m_comment;

#if UNITY_EDITOR
        public Vector2 Position { get { return m_position; } set { m_position = value; } }
        public int NumOutports { get { return m_outports.Count; } }

        public virtual int DefaultNumOutports { get { return 1; } }
        public virtual bool UseIMGUIPropertyDrawer { get { return false; } }

        public static readonly string OutportsVarName = "m_outports";
        public static readonly string IdVarname = "m_id";
        public static readonly string PositionVarName = "m_position";

        public ANode()
        {
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

        public bool OutportEdgeIsValid(int index)
        {
            if (index >= m_outports.Count || index < 0)
            {
                return false;
            }
            return m_outports[index].IsValid();
        }
#endif
    }
}