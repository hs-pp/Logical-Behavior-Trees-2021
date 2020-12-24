using Logical.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphAxesController
{
    public static float AXIS_LINE_WIDTH = 4;

    private NodeGraphView m_nodeGraphView = null;
    private CustomContentDragger m_customContentDragger = null;

    private GraphAxis m_xAxis = null;
    private GraphAxis m_yAxis = null;

    private bool m_isEnabled = false;

    public GraphAxesController(NodeGraphView nodeGraphView, CustomContentDragger contentDragger)
    {
        m_nodeGraphView = nodeGraphView;
        m_customContentDragger = contentDragger;

        m_xAxis = new GraphAxis();
        m_yAxis = new GraphAxis();

        nodeGraphView.Add(m_xAxis);
        nodeGraphView.Add(m_yAxis);
        m_xAxis.SendToBack();
        m_yAxis.SendToBack();

        m_nodeGraphView.RegisterCallback<GeometryChangedEvent>(UpdateSizes);
        m_nodeGraphView.viewTransformChanged += RefreshPositions;
        m_customContentDragger.PositionChanged += RefreshPositions;

    }

    ~GraphAxesController()
    {
        m_nodeGraphView?.UnregisterCallback<GeometryChangedEvent>(UpdateSizes);
        m_nodeGraphView.viewTransformChanged -= RefreshPositions;
        m_customContentDragger.PositionChanged -= RefreshPositions;

        m_nodeGraphView?.Remove(m_xAxis);
        m_nodeGraphView?.Remove(m_yAxis);
    }

    public void SetEnable(bool isEnabled)
    {
        m_isEnabled = isEnabled;
        m_xAxis.SetVisible(isEnabled);
        m_yAxis.SetVisible(isEnabled);
    }

    public void RefreshPositions()
    {
        if(!m_isEnabled)
        {
            return;
        }

        Vector2 topLeft = m_nodeGraphView.viewTransform.position;
        Vector2 xPos = new Vector2(Mathf.Clamp(topLeft.x, AXIS_LINE_WIDTH / 2, m_nodeGraphView.viewport.worldBound.width - AXIS_LINE_WIDTH), 0);
        Vector2 yPos = new Vector2(0, Mathf.Clamp(topLeft.y, AXIS_LINE_WIDTH / 2, m_nodeGraphView.viewport.worldBound.height - AXIS_LINE_WIDTH));
        m_xAxis.SetPos(xPos);
        m_yAxis.SetPos(yPos);
    }

    private void RefreshPositions(GraphView graphView)
    {
        RefreshPositions();
    }

    private void RefreshPositions(Vector2 position)
    {
        RefreshPositions();
    }

    private void UpdateSizes(GeometryChangedEvent geomChanged)
    {
        m_xAxis.SetSize(new Vector2(AXIS_LINE_WIDTH, geomChanged.newRect.height));
        m_yAxis.SetSize(new Vector2(geomChanged.newRect.width, AXIS_LINE_WIDTH));
    }

    public class GraphAxis : GraphElement
    {
        public GraphAxis()
        {
            capabilities &= ~Capabilities.Movable;
            style.borderLeftColor = Color.black;
            style.borderLeftWidth = AXIS_LINE_WIDTH;
            style.borderRightColor = Color.black;
            style.borderRightWidth = AXIS_LINE_WIDTH;
            style.borderTopColor = Color.black;
            style.borderTopWidth = AXIS_LINE_WIDTH;
            style.borderBottomColor = Color.black;
            style.borderBottomWidth = AXIS_LINE_WIDTH;
            SetVisible(false);
        }

        public void SetVisible(bool isVisible)
        {
            style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetPos(Vector2 position)
        {
            SetPosition(new Rect(position, GetPosition().size));
        }

        public void SetSize(Vector2 size)
        {
            SetPosition(new Rect(GetPosition().position, size));
        }
    }
}
