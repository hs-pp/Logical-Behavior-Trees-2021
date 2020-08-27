using System;

namespace GraphTheory
{
    //TODO: encapsulation!@@!@!@
    [System.Serializable]
    public class OutportEdge
    {
        public string Id;
        public string SourceNodeId;
        public string ConnectedNodeId;

        public bool IsValid { get { return !string.IsNullOrEmpty(ConnectedNodeId); } }

        public OutportEdge()
        {
            //UnityEngine.Debug.Log("Genereated Noew id");
            //Id = Guid.NewGuid().ToString();
        }

        public void SetInvalid()
        {
            ConnectedNodeId = "";
        }
    }
}