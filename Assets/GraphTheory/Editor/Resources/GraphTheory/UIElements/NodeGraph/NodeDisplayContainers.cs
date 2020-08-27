using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class NodeDisplayContainers
    {
        public NodeView NodeView { get; private set; }
        public VisualElement HeaderContainer { get; private set; }
        public VisualElement PreTitleContainer { get; private set; }
        public VisualElement PostTitleContainer { get; private set; }
        public InportContainer InportContainer { get; private set; }
        public List<OutportContainer> OutportContainers = new List<OutportContainer>();
        public VisualElement BodyContainer { get; private set; }
        public VisualElement FooterContainer { get; private set; }

        public NodeDisplayContainers(NodeView nodeView)
        {
            NodeView = nodeView;

            HeaderContainer = new VisualElement();
            HeaderContainer.name = "header-container";
            NodeView.Insert(0, HeaderContainer);
            PreTitleContainer = CreateBaseElement("pre-title-container");
            NodeView.titleContainer.Insert(0, PreTitleContainer);
            PostTitleContainer = CreateBaseElement("post-title-container");
            NodeView.titleContainer.Add(PostTitleContainer);
            BodyContainer = CreateBaseElement("body-container");
            NodeView.extensionContainer.Add(BodyContainer);
            FooterContainer = new VisualElement();
            FooterContainer.name = "footer-container";
            NodeView.Add(FooterContainer);
        }

        public VisualElement CreateBaseElement(string name)
        {
            VisualElement baseEle = new VisualElement();
            baseEle.name = name;
            baseEle.style.backgroundColor = new Color(0.234f, 0.234f, 0.234f);
            return baseEle;
        }

        public void AddNewOutport(PortView outport)
        {
            OutportContainer outportContainer = new OutportContainer(outport);
            OutportContainers.Add(outportContainer);
            NodeView.outputContainer.Add(outportContainer);
        }

        public void SetInport(PortView inport)
        {
            InportContainer = new InportContainer(inport);
            NodeView.inputContainer.Add(InportContainer);
            NodeView.inputContainer.style.flexGrow = 0;
        }

        public List<PortView> GetAllPorts()
        {
            List<PortView> portViews = new List<PortView>();
            portViews.Add(InportContainer.PortView);
            for (int i = 0; i < OutportContainers.Count; i++)
            {
                portViews.Add(OutportContainers[i].PortView);
            }
            return portViews;
        }

        public void ClearDisplays()
        {
            HeaderContainer.Clear();
            PreTitleContainer.Clear();
            PostTitleContainer.Clear();
            InportContainer.ClearContainers();
            for(int i = 0; i < OutportContainers.Count; i++)
            {
                OutportContainers[i].ClearContainers();
            }
            BodyContainer.Clear();
            FooterContainer.Clear();
        }
    }

    public class OutportContainer : VisualElement
    {
        private const string OUTPORT_HEADER = "outport-header";
        private const string OUTPORT_FOOTER = "outport-footer";
        private const string OUTPORT_BODY = "outport-body";
        private const string OUTPORT_AREA = "outport-area";

        public VisualElement OutportHeader { get; private set; }
        public VisualElement OutportFooter { get; private set; }
        public VisualElement OutportBody { get; private set; }
        private VisualElement OutportArea { get; set; }
        public PortView PortView { get; private set; }

        public OutportContainer(PortView portView)
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/UIElements/NodeGraph/OutportContainer");
            xmlAsset.CloneTree(this);

            OutportHeader = this.Q<VisualElement>(OUTPORT_HEADER);
            OutportFooter = this.Q<VisualElement>(OUTPORT_FOOTER);
            OutportBody = this.Q<VisualElement>(OUTPORT_BODY);
            OutportArea = this.Q<VisualElement>(OUTPORT_AREA);

            PortView = portView;
            OutportArea.Add(portView);
        }

        public void ClearContainers()
        {
            OutportHeader.Clear();
            OutportFooter.Clear();
            OutportBody.Clear();
        }
    }

    public class InportContainer : VisualElement
    {
        private const string INPORT_HEADER = "inport-header";
        private const string INPORT_FOOTER = "inport-footer";
        private const string INPORT_BODY = "inport-body";
        private const string INPORT_AREA = "inport-area";

        public VisualElement InportHeader { get; private set; }
        public VisualElement InportFooter { get; private set; }
        public VisualElement InportBody { get; private set; }
        private VisualElement InportArea { get; set; }
        public PortView PortView { get; private set; }

        public InportContainer(PortView portView)
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/UIElements/NodeGraph/InportContainer");
            xmlAsset.CloneTree(this);

            InportHeader = this.Q<VisualElement>(INPORT_HEADER);
            InportFooter = this.Q<VisualElement>(INPORT_FOOTER);
            InportBody = this.Q<VisualElement>(INPORT_BODY);
            InportArea = this.Q<VisualElement>(INPORT_AREA);

            PortView = portView;
            InportArea.Add(portView);
        }

        public void ClearContainers()
        {
            InportHeader.Clear();
            InportFooter.Clear();
            InportBody.Clear();
        }
    }
}