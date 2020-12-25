using Logical.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphAxesController : VisualElement
{
    public static float AXIS_LINE_WIDTH = 4;

    private NodeGraphView m_nodeGraphView = null;
    private CustomContentDragger m_customContentDragger = null;
    private SecondarySelectionDragger m_secondarySelectionDragger = null;

    private GraphAxis m_xAxis = null;
    private GraphAxis m_yAxis = null;
    private List<CoordinateLabel> m_coordinateLabels = new List<CoordinateLabel>();

    private bool m_isEnabled = false;

    public GraphAxesController(NodeGraphView nodeGraphView, CustomContentDragger contentDragger, SecondarySelectionDragger secondarySelectionDragger)
    {
        this.name = "graph-axes-controller";
        m_nodeGraphView = nodeGraphView;
        m_customContentDragger = contentDragger;
        m_secondarySelectionDragger = secondarySelectionDragger;

        m_xAxis = new GraphAxis();
        m_yAxis = new GraphAxis();

        VisualElement graphViewContent = m_nodeGraphView.Q<VisualElement>("contentViewContainer");
        Add(m_xAxis);
        Add(m_yAxis);

        RefreshPositions();
        m_nodeGraphView.RegisterCallback<GeometryChangedEvent>(RefreshSizes);
        m_nodeGraphView.viewTransformChanged += RefreshPositions;
        m_customContentDragger.PositionChanged += RefreshPositions;
        m_secondarySelectionDragger.OnDragging += RefreshPositions; // TODO: This is a hack to get the axes to update when moving nodes around. But it still doesn't work perfectly.
    }

    ~GraphAxesController()
    {
        m_nodeGraphView?.UnregisterCallback<GeometryChangedEvent>(RefreshSizes);
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

        RefreshCoordinateLabels();
        //Debug.Log(m_nodeGraphView.viewTransform.position + " " + (m_nodeGraphView.contentViewContainer.worldBound));
    }

    private void RefreshPositions(GraphView graphView)
    {
        RefreshPositions();
    }

    private void RefreshPositions(Vector2 position)
    {
        RefreshPositions();
    }

    private void RefreshCoordinateLabels()
    {
        Rect worldBound = m_nodeGraphView.viewport.worldBound;
        worldBound.position = new Vector2(worldBound.position.x - m_nodeGraphView.viewTransform.position.x, worldBound.position.y - m_nodeGraphView.viewTransform.position.y);

        float zoom = m_nodeGraphView.viewTransform.scale.x;
        int increment = 100;

        List<CoordinateLabel> oldList = new List<CoordinateLabel>(m_coordinateLabels);
        m_coordinateLabels.Clear();
        CoordinateLabel GetCoordinateLabel()
        {
            CoordinateLabel coordinateLabel;
            if (oldList.Count > 0)
            {
                coordinateLabel = oldList[oldList.Count - 1];
                oldList.RemoveAt(oldList.Count - 1);
            }
            else
            {
                coordinateLabel = new CoordinateLabel();
                m_nodeGraphView.Add(coordinateLabel);
            }
            m_coordinateLabels.Add(coordinateLabel);
            return coordinateLabel;
        }

        //xAxis
        CoordinateLabel.Direction dir;
        Vector2 xAxisPos = m_xAxis.GetPosition().position;
        if (xAxisPos.x > worldBound.center.x)
        {
            dir = CoordinateLabel.Direction.LEFT;
        }
        else
        {
            dir = CoordinateLabel.Direction.RIGHT;
        }

        int upperBound = (int)((worldBound.y + worldBound.height) / increment) * increment;
        int lowerBound = (int)((worldBound.y) / increment) * increment;
        Debug.Log($"{lowerBound} to {upperBound}");
        for(int i = lowerBound; i <= upperBound; i += increment)
        {
            if (i - worldBound.y > 0)
            {
                CoordinateLabel coordinateLabel = GetCoordinateLabel();
                coordinateLabel.SetLabelXAxis(i, new Vector2(xAxisPos.x, i - worldBound.y), dir);
            }
        }

        //for (int i = 1; i < (worldBound.height / increment / 2) + 1; i++)
        //{
        //    int num = i * increment;
        //    CoordinateLabel coordinateLabel = GetCoordinateLabel();
        //    coordinateLabel.SetLabelXAxis(num, new Vector2(xAxisPos.x, num - worldBound.y), dir);
        //    CoordinateLabel coordinateLabel2 = GetCoordinateLabel();
        //    coordinateLabel2.SetLabelXAxis(-num, new Vector2(xAxisPos.x, -num - worldBound.y), dir);
        //}

        foreach(CoordinateLabel remaining in oldList)
        {
            m_nodeGraphView.Remove(remaining);
        }
    }

    private void RefreshSizes(GeometryChangedEvent geomChanged)
    {
        m_xAxis.SetSize(new Vector2(AXIS_LINE_WIDTH, geomChanged.newRect.height));
        m_yAxis.SetSize(new Vector2(geomChanged.newRect.width, AXIS_LINE_WIDTH));
    }

    /// <summary>
    /// This is a GraphElement because of the SetPosition() method which simply sets the VisualElement's layout.
    /// The default VisualElement's layout is readonly for some reason?
    /// </summary>
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

    public class CoordinateLabel : GraphElement, IDisposable
    {
        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
        }

        private Label m_label = null;
        public CoordinateLabel()
        {
            m_label = new Label();
            Add(m_label);
        }
        

        public void SetLabelXAxis(int num, Vector2 position, Direction direction)
        {
            m_label.text = num.ToString();
            SetPosition(new Rect(position, GetPosition().size));
        }

        public void Dispose()
        {
            Remove(m_label);

        }
    }
}
