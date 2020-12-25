using Logical;
using Logical.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
[SupportedGraphTypes(typeof(SampleNodeGraph))]
public class AxisTesterNode : ANode
{
}

[CustomNodeViewDrawer(typeof(AxisTesterNode))]
public class AxisTesterNodeViewDrawer : NodeViewDrawer
{
    public override string DisplayName { get { return "AxisTester"; } }

    private class wtf : GraphElement
    {
        public wtf()
        {
            name = "wtfffffff";
            style.borderLeftColor = Color.black;
            style.borderLeftWidth = 4;
            style.borderRightColor = Color.black;
            style.borderRightWidth = 4;
            style.borderTopColor = Color.black;
            style.borderTopWidth = 4;
            style.borderBottomColor = Color.black;
            style.borderBottomWidth = 4;
        }
    }
    wtf ve = null;
    bool is_screenSpace = true;
    TextField xTF;
    TextField yTF;
    public override void OnDrawPrimaryBody(VisualElement primaryBodyContainer)
    {
        VisualElement vis = new VisualElement();
        vis.style.flexDirection = FlexDirection.Row;
        xTF = new TextField();xTF.value = "0";
        vis.Add(xTF);
        yTF = new TextField();yTF.value = "0";
        vis.Add(yTF);
        primaryBodyContainer.Add(vis);
        Button setScreenSpace = new Button();
        setScreenSpace.clicked += () =>
        {
            is_screenSpace = !is_screenSpace;
            setScreenSpace.text = is_screenSpace ? "screenSpace" : "graphSpace";
        };
        setScreenSpace.text = is_screenSpace ? "screenSpace" : "graphSpace";
        primaryBodyContainer.Add(setScreenSpace);
        Button convertButton = new Button() { text = "convert" };
        convertButton.clicked += () =>
        {
            GraphView graphView = TargetView.m_nodeGraphView;
            if (ve == null)
            {
                ve = new wtf();
            }
            is_screenSpace = !is_screenSpace;
            setScreenSpace.text = is_screenSpace ? "screenSpace" : "graphSpace";

            Vector2 orig = new Vector2(float.Parse(xTF.value), float.Parse(yTF.value));

            if (is_screenSpace)
            {
                graphView.Add(ve);
                // from graph space to screen space
                orig -= new Vector2(graphView.contentViewContainer.layout.x - graphView.viewTransform.position.x, graphView.contentViewContainer.layout.y - graphView.viewTransform.position.y);
            }
            else
            {
                graphView.AddElement(ve);
                // from screen space to graph space
                orig += new Vector2(graphView.contentViewContainer.layout.x - graphView.viewTransform.position.x, graphView.contentViewContainer.layout.y - graphView.viewTransform.position.y);

            }
            xTF.value = orig.x.ToString();
            yTF.value = orig.y.ToString();
            ve.SetPosition(new Rect(orig, new Vector2(10, 10)));
        };
        primaryBodyContainer.Add(convertButton);
        base.OnDrawPrimaryBody(primaryBodyContainer);
    }

    public override void OnDrawSecondaryBody(VisualElement secondaryBodyContainer)
    {
        Button butt = new Button();
        butt.clicked += () =>
        {
            GraphView graphView = TargetView.m_nodeGraphView;
            if (ve == null)
            {
                ve = new wtf();
            }

            if (is_screenSpace)
            {
                graphView.Add(ve);
            }
            else
            {
                graphView.AddElement(ve);
            }
            ve.SetPosition(new Rect(new Vector2(float.Parse(xTF.value), float.Parse(yTF.value)), new Vector2(10, 10)));
        };
        butt.text = "Display";
        secondaryBodyContainer.Add(butt);
    }

    private VisualElement GetPlaceholderElement(string name, float height)
    {
        VisualElement placeholder = new VisualElement();
        placeholder.style.borderLeftColor = Color.white;
        placeholder.style.borderRightColor = Color.white;
        placeholder.style.borderTopColor = Color.white;
        placeholder.style.borderBottomColor = Color.white;
        placeholder.style.borderLeftWidth = 1;
        placeholder.style.borderRightWidth = 1;
        placeholder.style.borderTopWidth = 1;
        placeholder.style.borderBottomWidth = 1;
        placeholder.style.borderTopLeftRadius = 5;
        placeholder.style.borderTopRightRadius = 5;
        placeholder.style.borderBottomLeftRadius = 5;
        placeholder.style.borderBottomRightRadius = 5;
        placeholder.style.marginLeft = 5;
        placeholder.style.marginRight = 5;
        placeholder.style.marginTop = 5;
        placeholder.style.marginBottom = 5;

        placeholder.style.justifyContent = Justify.Center;
        if (height != -1)
        {
            placeholder.style.height = height;
        }

        Label label = new Label(name);
        label.style.unityTextAlign = TextAnchor.MiddleCenter;
        placeholder.Add(label);

        return placeholder;
    }
}
