using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTheory.BuiltInNodes
{
    [Serializable]
    public class BlackboardConditional : ANode
    {
        [SerializeField]
        private string m_blackboardElementId = "";
    }
}