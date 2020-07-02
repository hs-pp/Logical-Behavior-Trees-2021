using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTheory
{
    public struct GraphErrorMessage
    {
        private string m_errorMessage;
        public string ErrorMessage { get { return m_errorMessage; } }

        public GraphErrorMessage(string msg)
        {
            m_errorMessage = msg;
        }
    }
}