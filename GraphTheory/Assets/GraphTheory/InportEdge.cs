using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTheory
{
    [System.Serializable]
    public struct InportEdge
    {
        public int InportIndex;

        public string ConnectedNodeId;
        public int ConnectedOutportIndex;

        public bool CheckMatchingOutport(OutportEdge outportEdge)
        {
            return true;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is InportEdge))
                return false;

            InportEdge other = (InportEdge)obj;

            return (InportIndex.Equals(other.InportIndex)) &&
                (ConnectedNodeId.Equals(other.ConnectedNodeId)) &&
                (ConnectedOutportIndex.Equals(other.ConnectedOutportIndex));
        }
    }
}