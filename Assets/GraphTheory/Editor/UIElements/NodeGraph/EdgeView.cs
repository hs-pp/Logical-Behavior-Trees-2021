using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphTheory.Editor.UIElements
{
    public class EdgeView : Edge
    {
        public PortView FirstPort { get; private set; }
        public PortView SecondPort { get; private set; }

        public EdgeView() : base()
        {
            Setup();
        }
        public void Setup()
        {
            FirstPort = output as PortView;
            SecondPort = input as PortView;
        }
    }
}