using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace GraphTheory
{
    public abstract class ANode
    {
        [SerializeField]
        private string m_id = "";
        [SerializeField]
        private List<InportEdge> m_inportEdges;
        [SerializeField]
        private List<OutportEdge> m_outportEdges;

        public string Id { get { return m_id; } }

        public abstract string Name { get; }
        public abstract int NumInports { get; }
        public abstract int NumOutports { get; }

        public ANode()
        {
            m_id = Guid.NewGuid().ToString();
            m_inportEdges = new List<InportEdge>(NumInports);
            m_outportEdges = new List<OutportEdge>(NumOutports);
        }

        [SerializeField]
        private Vector2 m_position;

        /// <summary>
        /// Only one outportEdge allowed per outportIndex. So we can safely return the first outportEdge with the correct outportIndex.
        /// </summary>
        public OutportEdge GetOutportEdge(int outportIndex)
        {
            return m_outportEdges.Find(x => x.OutportIndex == outportIndex);
        }
        public List<InportEdge> GetInportEdges(int inportIndex)
        {
            return m_inportEdges.FindAll(x => x.InportIndex == inportIndex);
        }

#if UNITY_EDITOR
        public Vector2 Position { get { return m_position; } }
        public virtual Vector2 Size { get { return new Vector2(600, 300); } }
        public virtual Color NodeColor { get { return Color.gray; } }
        public List<Type> CompatibleGraphs { get; }

        public virtual void DrawNodeView(Node nodeView)
        {

        }

        public bool CanAddInportEdge(InportEdge inportEdge, out GraphErrorMessage errorMessage)
        {
            errorMessage = default;
            return true;
        }
        public void AddInportEdge(InportEdge inportEdge)
        {
            m_inportEdges.Add(inportEdge);
        }

        /// <summary>
        /// Only one outportEdge allowed per outport index.
        /// </summary>
        public bool CanAddOutportEdge(OutportEdge outportEdge, out GraphErrorMessage errorMessage)
        {
            if(m_outportEdges.Exists(x => x.OutportIndex == outportEdge.OutportIndex))
            {
                errorMessage = new GraphErrorMessage("Output ports can only have one edge!");
                return false;
            }

            errorMessage = default;
            return true;
        }
        public void AddOutportEdge(OutportEdge outportEdge)
        {
            m_outportEdges.Add(outportEdge);
        }

        public void RemoveInportEdge(InportEdge inportEdge)
        {
            m_inportEdges.Remove(inportEdge);
        }
        public void RemoveOutportEdge()
        {

        }
#endif
    }
}