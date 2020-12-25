using Logical.Editor;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphAxesController : VisualElement
{
    public static float AXIS_LINE_WIDTH = 4;
    public static float HALF_AXIS_LINE_WIDTH = 2;

    private NodeGraphView m_nodeGraphView = null;
    private CustomContentDragger m_customContentDragger = null;
    private SecondarySelectionDragger m_secondarySelectionDragger = null;

    private GraphAxis m_xAxis = null;
    private GraphAxis m_yAxis = null;
    private List<CoordinateLabel> m_coordinateLabels = new List<CoordinateLabel>();
    private List<CoordinateLabel> m_cachedCoordinateLabels = new List<CoordinateLabel>();
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

        foreach (CoordinateLabel label in m_coordinateLabels)
        {
            Remove(label); 
        }
        m_coordinateLabels.Clear();
        foreach (CoordinateLabel label in m_cachedCoordinateLabels)
        {
            Remove(label);
        }
        m_cachedCoordinateLabels.Clear();
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

        Vector2 size = m_nodeGraphView.viewport.worldBound.size;

        Vector2 center = new Vector2(
            m_nodeGraphView.viewTransform.position.x - m_nodeGraphView.contentViewContainer.layout.x,
            m_nodeGraphView.viewTransform.position.y - m_nodeGraphView.contentViewContainer.layout.y);

        center.x = Mathf.Clamp(center.x, 3, size.x - HALF_AXIS_LINE_WIDTH); // hard coded min val for aesthetic. HALF_AXIS_LINE_WIDTH doesnt look as good.
        center.y = Mathf.Clamp(center.y, HALF_AXIS_LINE_WIDTH, size.y - HALF_AXIS_LINE_WIDTH);

        m_xAxis.SetPos(new Vector2(center.x, 0));
        m_yAxis.SetPos(new Vector2(0, center.y));

        RefreshCoordinateLabels();
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
        m_cachedCoordinateLabels.AddRange(m_coordinateLabels);
        foreach (CoordinateLabel label in m_coordinateLabels)
        {
            Remove(label);
        }
        m_coordinateLabels.Clear();
        CoordinateLabel GetCoordinateLabel()
        {
            CoordinateLabel coordinateLabel;
            if (m_cachedCoordinateLabels.Count > 0)
            {
                coordinateLabel = m_cachedCoordinateLabels[m_cachedCoordinateLabels.Count - 1];
                m_cachedCoordinateLabels.RemoveAt(m_cachedCoordinateLabels.Count - 1); 
            }
            else
            {
                coordinateLabel = new CoordinateLabel();
            }
            Add(coordinateLabel);
            m_coordinateLabels.Add(coordinateLabel);
            return coordinateLabel;
        }

        Vector2 size = m_nodeGraphView.viewport.worldBound.size;
        Vector2 topRight_graphSpace = m_nodeGraphView.viewTransform.position;
        Vector2 offset_graphSpace = m_nodeGraphView.contentViewContainer.layout.position;
        Vector2 center_ScreenSpace = new Vector2(
            topRight_graphSpace.x - offset_graphSpace.x,
            topRight_graphSpace.y - offset_graphSpace.y);

        center_ScreenSpace.x = Mathf.Clamp(center_ScreenSpace.x, 0, size.x);
        center_ScreenSpace.y = Mathf.Clamp(center_ScreenSpace.y, 0, size.y);

        float zoom = m_nodeGraphView.viewTransform.scale.x;
        float increment = 100;
        float zoomedIncrement = increment * zoom;

        Vector2 topLeft_GraphSpace = new Vector2(offset_graphSpace.x - topRight_graphSpace.x,
            offset_graphSpace.y - topRight_graphSpace.y);
        Vector2 bottomRight_GraphSpace = size + new Vector2(offset_graphSpace.x - topRight_graphSpace.x,
            offset_graphSpace.y - topRight_graphSpace.y);

        float lowerBound = (int)(topLeft_GraphSpace.y / zoomedIncrement) * zoomedIncrement;
        float upperBound = (int)(bottomRight_GraphSpace.y / zoomedIncrement) * zoomedIncrement;

        for(float i = lowerBound; i <= upperBound; i += zoomedIncrement)
        {
            //if (i == 0) continue;

            CoordinateLabel coordinateLabel = GetCoordinateLabel();
            coordinateLabel.SetLabelXAxis((int)(i / zoom), new Vector2(center_ScreenSpace.x, i - offset_graphSpace.y + topRight_graphSpace.y), CoordinateLabel.Direction.RIGHT);
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
            Color color = new Color(0.098f, 0.098f, 0.098f);
            capabilities &= ~Capabilities.Movable;
            //style.borderLeftColor = color;
            //style.borderLeftWidth = AXIS_LINE_WIDTH;
            //style.borderRightColor = color;
            //style.borderRightWidth = AXIS_LINE_WIDTH;
            //style.borderTopColor = color;
            //style.borderTopWidth = AXIS_LINE_WIDTH;
            //style.borderBottomColor = color;
            //style.borderBottomWidth = AXIS_LINE_WIDTH;
            style.backgroundColor = color;
            SetVisible(false);
        }

        public void SetVisible(bool isVisible)
        {
            style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetPos(Vector2 position)
        {
            SetPosition(new Rect(position - new Vector2(HALF_AXIS_LINE_WIDTH, HALF_AXIS_LINE_WIDTH), GetPosition().size));
        }

        public void SetSize(Vector2 size)
        {
            SetPosition(new Rect(GetPosition().position, size));
        }
    }

    public class CoordinateLabel : GraphElement, IDisposable
    {
        private static readonly string COORDINATE_LABEL = "coordinate-label";

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
            var xmlAsset = Resources.Load<VisualTreeAsset>(ResourceAssetPaths.CoordinateLabel_UXML);
            xmlAsset.CloneTree(this);

            m_label = this.Q<Label>(COORDINATE_LABEL);
            //Add(m_label);
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
