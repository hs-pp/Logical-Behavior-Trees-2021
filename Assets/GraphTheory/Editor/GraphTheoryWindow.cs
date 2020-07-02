using GraphTheory.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    public class GraphTheoryWindow : EditorWindow
    {
        private const string DATA_STRING = "GraphWindowData";
        private const string TOOLBAR = "toolbar";
        private const string MAIN_SPLITVIEW = "main-TwoPanelSplit";
        private const string MAIN_PANEL_RIGHT = "main-panel-right";


        private GraphWindowData m_graphWindowData = null;
        private NodeGraphView m_nodeGraphView = null;
        private Toolbar m_toolbar = null;
        private TwoPaneSplitView m_mainSplitView = null;

        [MenuItem("Graph/GraphTheory")]
        public static void OpenWindow()
        {
            var window = GetWindow<GraphTheoryWindow>();
            window.titleContent = new GUIContent("NodeGraph");
        }

        private void Awake()
        {
            Debug.Log("Awake");
        }

        private void OnEnable() 
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheoryWindow");
            xmlAsset.CloneTree(rootVisualElement);
            m_mainSplitView = rootVisualElement.Q<TwoPaneSplitView>(MAIN_SPLITVIEW);
            VisualElement mainPanelRight = rootVisualElement.Q<VisualElement>(MAIN_PANEL_RIGHT);
            m_toolbar = rootVisualElement.Q<Toolbar>(TOOLBAR);
            RegisterNodeGraphView(mainPanelRight);

            RegisterToolbarButton_CreateNewGraph();

            DeserializeData();
        }

        private void DeserializeData()
        {
            string serializedData = EditorPrefs.GetString(DATA_STRING, "");
            if(string.IsNullOrEmpty(serializedData))
            {
                m_graphWindowData = new GraphWindowData(); 
            }
            else
            {
                m_graphWindowData = JsonUtility.FromJson<GraphWindowData>(serializedData);
            }
            Debug.Log("Deserialized data: " + serializedData);

            // Window size
            Rect window = position;
            window.size = m_graphWindowData.WindowDimensions; 
            position = window;

            // Main split view position
            m_mainSplitView.SetSplitPosition(m_graphWindowData.MainSplitViewPosition);
        }

        private void SerializeData()
        {
            m_graphWindowData.WindowDimensions = position.size;
            m_graphWindowData.MainSplitViewPosition = m_mainSplitView.SplitPosition;

            Debug.Log("Serializing data: " + JsonUtility.ToJson(m_graphWindowData, true));
            EditorPrefs.SetString(DATA_STRING, JsonUtility.ToJson(m_graphWindowData, true));
        }

        private void OnDisable()
        {
            SerializeData();
        }

        private void RegisterToolbarButton_CreateNewGraph()
        { 
            var graphCreateButton = new Button(() =>
            {
                CreateNewGraphPopup.OpenWindow();
            });
            graphCreateButton.text = "Create Graph";
            m_toolbar.Add(graphCreateButton);
        }

        private void RegisterNodeGraphView(VisualElement parent)
        {
            m_nodeGraphView = new NodeGraphView
            {
                name = "NodeGraphView" 
            };
            m_nodeGraphView.StretchToParentSize();
            parent.Add(m_nodeGraphView);
        }
    }
}