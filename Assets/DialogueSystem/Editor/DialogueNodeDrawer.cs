using DialogueSystem;
using GraphTheory;
using GraphTheory.Editor;
using GraphTheory.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomNodeDrawer(typeof(DialogueNode))]
public class DialogueNodeDrawer : NodeDrawer
{
    public override string DisplayName { get { return "Dialogue"; } }

    public override void OnDrawHeader(VisualElement headerContainer)
    {
        base.OnDrawHeader(headerContainer);
        headerContainer.Add(new Label("Header"));
    }

    public override void OnDrawTitle(VisualElement preTitleContainer, VisualElement postTitleContainer)
    {
        base.OnDrawTitle(preTitleContainer, postTitleContainer);
        preTitleContainer.Add(GetPlaceholderElement("Pre-Title", 36));
        postTitleContainer.Add(GetPlaceholderElement("Post-Title", 36));
    }

    public override void OnDrawInport(InportContainer inportContainer)
    {
        base.OnDrawInport(inportContainer);
        inportContainer.InportBody.Add(GetPlaceholderElement("Inport body", 20));
        inportContainer.InportHeader.Add(GetPlaceholderElement("Inport header", 20));
        inportContainer.InportFooter.Add(GetPlaceholderElement("Inport footer", 20));
    }

    public override void OnDrawOutport(int outportIndex, OutportContainer outportContainer)
    {
        base.OnDrawOutport(outportIndex, outportContainer);
        outportContainer.OutportBody.Add(GetPlaceholderElement("Outport body", 20));
        outportContainer.OutportHeader.Add(GetPlaceholderElement("Outport header", 20));
        outportContainer.OutportFooter.Add(GetPlaceholderElement("Outport footer", 20));
    }

    public override void OnDrawBody(VisualElement bodyContainer)
    {
        base.OnDrawBody(bodyContainer);
        VisualElement body = GetPlaceholderElement("Body", -1);
        Button addOutportButton = new Button();
        addOutportButton.text = "Add Outport";
        addOutportButton.clickable.clicked += () => { NodeGraph.AddOutportToNode(TargetProperty); };
        body.Add(addOutportButton);
        Button removeOutportButton = new Button();
        removeOutportButton.text = "Remove Outport";
        removeOutportButton.clickable.clicked += () => { NodeGraph.RemoveOutportFromNode(TargetProperty); };
        body.Add(removeOutportButton);
        bodyContainer.Add(body);
    }

    public override void OnDrawFooter(VisualElement footerContainer)
    {
        base.OnDrawFooter(footerContainer);
        footerContainer.Add(new Label("Footer"));
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
