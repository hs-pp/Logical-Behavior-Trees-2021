using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace GraphTheory
{
    [Serializable]
    public abstract class ANode
    {
        [SerializeField]
        private string m_id = "";
        [SerializeField]
        private List<OutportEdge> m_outports = new List<OutportEdge>(0);

        public string Id { get { return m_id; } }
        public int NumOutports { get { return m_outports.Count; } }

        public ANode()
        {
            m_id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Only one outportEdge allowed per outportIndex. So we can safely return the first outportEdge with the correct outportIndex.
        /// </summary>
        public OutportEdge GetOutportEdge(int outportIndex)
        {
            return m_outports[outportIndex];
        }

        [SerializeField]
        private Vector2 m_position;
#if UNITY_EDITOR
        public abstract string Name { get; }
        public abstract List<Type> CompatibleGraphs { get; }//Returning null = compatible to all. Returning empty list = compatible to none.
        public virtual bool HasInport { get { return true; } }

        public Vector2 Position { get { return m_position; } set { m_position = value; } }
        public virtual Vector2 Size { get { return new Vector2(600, 300); } }
        public virtual Color NodeColor { get { return Color.gray; } }

        public virtual void DrawNodeView(Node nodeView)
        {

        }

        public List<OutportEdge> GetAllEdges()
        {
            return m_outports;
        }

        protected int CreateOutport()
        {
            m_outports.Add(null);
            return m_outports.Count - 1;
        }
        protected void DestroyOutport(int index)
        {
            m_outports.RemoveAt(index);
        }
        /// <summary>
        /// Only one outportEdge allowed per outport index.
        /// </summary>
        public void AddOutportEdge(int outportIndex, OutportEdge outportEdge)
        {
            if(outportIndex > m_outports.Count - 1)
            {
                Debug.LogError("Error adding outport edge!");
                return;
            }
            m_outports[outportIndex] = outportEdge;
        }

        public void RemoveOutportEdge(int outportIndex)
        {
            if (outportIndex > m_outports.Count - 1)
            {
                Debug.LogError("Error removing outport edge!");
                return;
            }
            m_outports[outportIndex] = null;
        }
#endif
    }
}