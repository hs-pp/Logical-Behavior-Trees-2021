using Logical.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This manipulator simply checks if you're dragging someeeething withing the graph view and then calls a callback.
/// Otherwise, we have no way of knowing when the graph is moving because a node is moving.
/// The GraphAxesController needs to know every time the graph's viewtransform has changed and this manip covers an edge case.
/// </summary>
public class SecondarySelectionDragger : MouseManipulator
{
    private NodeGraphView m_nodeGraphView = null;
    private bool m_isActive = false;
    public Action OnDragging = null;

    public SecondarySelectionDragger()
    {
    }

    protected override void RegisterCallbacksOnTarget()
    {
        m_nodeGraphView = target as NodeGraphView;
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected void OnMouseDown(MouseDownEvent e)
    {
        m_isActive = true;
    }
    protected void OnMouseMove(MouseMoveEvent e)
    {
        if (!m_isActive)
        {
            return;
        }
        if(m_nodeGraphView.selection.Count > 0)
        {
            OnDragging?.Invoke();
        }
    }
    protected void OnMouseUp(MouseUpEvent e)
    {
        m_isActive = false;
    }
}
