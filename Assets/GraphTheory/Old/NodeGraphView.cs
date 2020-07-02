using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;

public class NodeGraphView : GraphView
{
    public NodeGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("NodeGraph"));  
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        var miniMap = new MiniMap { anchored = true };
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        Add(miniMap);

        AddElement(GenerateEntryPointNode());
    }

    private Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private Node GenerateEntryPointNode()
    {
        var node = new BasicNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            text = "ENTRYPOINT",
            EntryPoint = true
        };
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }
    public BasicNode CreateNewNode(string nodeName)
    {
        var newNode = new BasicNode
        {
            title = nodeName,
            text = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var inputPort = GeneratePort(newNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        newNode.outputContainer.Add(inputPort);

        var button = new Button(() => { AddChoicePort(newNode); });
        button.text = "New Choice";
        newNode.titleContainer.Add(button);

        newNode.RefreshExpandedState();
        newNode.RefreshPorts();
        newNode.SetPosition(new Rect(Vector2.zero, new Vector2(300,300)));

        return newNode;
    }

    private void AddChoicePort(BasicNode node)
    {
        var generatedPort = GeneratePort(node, Direction.Output);
        var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Choice {outputPortCount}";

        node.outputContainer.Add(generatedPort);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateNewNode(nodeName));
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port => 
        {
            if(startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }

        }));
        return compatiblePorts;
    }
}