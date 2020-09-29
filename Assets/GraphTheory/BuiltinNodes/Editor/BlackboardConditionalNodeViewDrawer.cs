using GraphTheory.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.BuiltInNodes
{
    [CustomNodeViewDrawer(typeof(BlackboardConditional))]
    public class BlackboardConditionalNodeViewDrawer : NodeViewDrawer
    {
        public override string DisplayName { get { return "Conditional"; } }
        private PopupField<BlackboardElementPopupEle> m_popupField = null;

        public override void OnSetup()
        {
            OnBlackboardElementChanged += () => { Repaint(); };
        }

        private class BlackboardElementPopupEle
        {
            public BlackboardElement Element;
            public string GetLabel()
            {
                return Element != null ? Element.Name : "NONE";
            }
            public string GetLabelWithType()
            {
                return Element != null ? $"{Element.Name} ({Element.Type.Name})" : "NONE (NONE)";
            }
        }

        public override void OnRepaint()
        {
            List<BlackboardElementPopupEle> elements = new List<BlackboardElementPopupEle>();
            elements.Add(new BlackboardElementPopupEle());
            foreach(BlackboardElement ele in BlackboardData.GetAllElements())
            {
                elements.Add(new BlackboardElementPopupEle() { Element = ele });
            }

            m_popupField = new PopupField<BlackboardElementPopupEle>(elements, 0, (ele) => { return ele.GetLabel(); }, (ele) => { return ele.GetLabelWithType(); });
        }

        public override void OnDrawPrimaryBody(VisualElement primaryBodyContainer)
        {
            base.OnDrawPrimaryBody(primaryBodyContainer);
            int random = Random.Range(0, 100);
            primaryBodyContainer.Add(new Label(random.ToString()));
            primaryBodyContainer.Add(m_popupField);
        }

        public override void OnSerializedPropertyChanged()
        {
            Repaint();
        }

    }
}