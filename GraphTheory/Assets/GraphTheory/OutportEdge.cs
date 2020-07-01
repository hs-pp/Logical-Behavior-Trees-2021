using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTheory
{
    [System.Serializable]
    public struct OutportEdge
    {
        public int OutportIndex;

        public string ConnectedNodeId;
        public int ConnectedInportIndex;

        public bool CheckMatchingInport(InportEdge inportEdge)
        {
            return true;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is OutportEdge))
                return false;

            OutportEdge other = (OutportEdge)obj;

            return (OutportIndex.Equals(other.OutportIndex)) &&
                (ConnectedNodeId.Equals(other.ConnectedNodeId)) &&
                (ConnectedInportIndex.Equals(other.ConnectedInportIndex));
        }
    }
}