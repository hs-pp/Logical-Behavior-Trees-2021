using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class BreadcrumbsView : VisualElement
    {
        private const string BREADCRUMB_CONTAINER = "breadcrumb-container";
        private VisualElement m_breadcrumbContainer = null;

        private List<BreadcrumbElement> m_breadcrumbs = new List<BreadcrumbElement>();
        public Action<string> OnBreadcrumbChanged { get; set; }

        private string m_currentFullPath = "";

        public BreadcrumbsView()
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/Breadcrumbs/BreadcrumbsView");
            xmlAsset.CloneTree(this);

            m_breadcrumbContainer = this.Q<VisualElement>(BREADCRUMB_CONTAINER);
        }

        /// <summary>
        /// </summary>
        /// <param name="path"> Formatted as "Hello/this/is/a/path/" </param>
        public void SetBreadcrumbPath(string path)// This could be more efficient
        {
            m_currentFullPath = path;
            ClearCrumbs();
            string[] parsed = path.Split('/');
            string constructed = "";
            for (int i = 0; i < parsed.Length; i++)
            {
                if (i == parsed.Length - 1)
                {
                    if (string.IsNullOrEmpty(parsed[i]))
                    {
                        continue;
                    }
                    constructed += parsed[i];
                }
                else
                {
                    constructed += parsed[i] + "/";
                }
                AddCrumb(constructed, parsed[i]);
            }
        }

        private void AddCrumb(string path, string crumbName)
        {
            BreadcrumbElement newEle = new BreadcrumbElement(path, crumbName, OnBreadcrumbClick);
            m_breadcrumbContainer.Add(newEle);
            m_breadcrumbs.Add(newEle);
        }

        private void OnBreadcrumbClick(string path)
        {
            if(path == m_currentFullPath)
            {
                return;
            }
            OnBreadcrumbChanged?.Invoke(path);
        }

        private void ClearCrumbs()
        {
            for (int i = 0; i < m_breadcrumbs.Count; i++)
            {
                m_breadcrumbContainer.Remove(m_breadcrumbs[i]);
            }
            m_breadcrumbs.Clear();
        }
    }
}