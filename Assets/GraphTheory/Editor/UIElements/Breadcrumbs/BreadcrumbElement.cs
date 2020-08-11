using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class BreadcrumbElement : VisualElement
    {
        private const string BREADCRUMB_NAME_LABEL = "breadcrumb-label";

        public string Path { get; private set; }

        public BreadcrumbElement(string path, string crumbName, Action<string> callback)
        {
            Path = path;

            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/UIElements/Breadcrumbs/BreadcrumbElement");
            xmlAsset.CloneTree(this);

            Label label = this.Q<Label>(BREADCRUMB_NAME_LABEL);
            label.text = crumbName;

            label.AddManipulator(new Clickable(() => { callback?.Invoke(Path); }));
        }
    }
}