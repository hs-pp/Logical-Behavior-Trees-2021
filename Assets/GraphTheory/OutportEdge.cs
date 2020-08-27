using System;

namespace GraphTheory
{
    //TODO: encapsulation!@@!@!@
    [System.Serializable]
    public class OutportEdge
    {
        public string Id;
        public string ConnectedNodeId;

        public bool IsValid { get { return !string.IsNullOrEmpty(ConnectedNodeId); } }
        public void SetInvalid()
        {
            ConnectedNodeId = "";
        }
    }
}