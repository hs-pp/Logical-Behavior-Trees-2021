using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Logical.Editor
{
    public class EdgeView : Edge
    {
        public SerializedProperty OutportEdgeProp { get; set; }
        public string EdgeId { get { return OutportEdgeProp.FindPropertyRelative(OutportEdge.IdVarName).stringValue; } }
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

        /// <summary>
        /// Unity's Edge OnCustomStyleResolved is bugged and will unselect itself if it was selected too soon after the graphview was created.
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/GraphViewEditor/Elements/Edge.cs
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/GraphViewEditor/Views/GraphView.cs
        /// OnCustomStyleResolved eventually takes you to GraphView.ChangeLayer where it's trying to prevent it from unselecting itself but 
        /// the result is it still does.
        /// Great way to see this is to override OnUnselected:
        //public override void OnUnselected()
        //{
        //    System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        //    for (int i = 1; i < 25; i++)
        //    {
        //        if (i >= 3 && i <= 10)
        //        {
        //            continue;
        //        }
        //        System.Reflection.MethodBase method = stackTrace.GetFrame(i).GetMethod();
        //        Debug.Log("unselected " + method.ReflectedType.ToString() + " " + method.Name);
        //    }

        //    base.OnUnselected();
        //}
        /// For now, there is a hacky solution in place that re-selects the edgeview if it was originally selected by
        /// the GraphTheoryWindow's Deserialize method.
        /// </summary>
        /// <param name="styles"></param>
        protected override void OnCustomStyleResolved(ICustomStyle styles)
        {
            bool retain = selected && RetainSelected;
            base.OnCustomStyleResolved(styles);
            if(retain)
            {
                RetainSelected = false;
                GetFirstAncestorOfType<NodeGraphView>().AddToSelection(this);
            }
        }
        public bool RetainSelected { get; set; } = false;
    }
}