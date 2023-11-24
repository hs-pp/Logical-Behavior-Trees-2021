using Logical.Editor.UIElements;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//https://docs.unity3d.com/Manual/UIE-Events-Reference.html

namespace Logical.Editor
{
    /// <summary>
    /// One big meaty boi
    /// </summary>
    public class LogicalTheoryWindow : EditorWindow, IHasCustomMenu
    {
        private const string DATA_STRING = "GraphWindowData";
        private const string TOOLBAR = "toolbar";
        private const string MAIN_SPLITVIEW = "main-TwoPanelSplit";
        private const string MAIN_PANEL_LEFT = "main-panel-left";
        private const string MAIN_PANEL_RIGHT = "main-panel-right";

        private GraphWindowData m_graphWindowData = null;
        private Toolbar m_toolbar = null;
        private UIElements.TwoPaneSplitView m_mainSplitView = null;
        private TabGroupElement m_mainTabGroup = null;
        private LibraryTabElement m_libraryTab = null;
        private InspectorTabElement m_inspectorTab = null;
        private NodeGraphView m_nodeGraphView = null;
        private ToolbarButton m_saveGraphButton = null;
        private CustomMenuController m_customMenuController = null;

        private NodeGraph m_openedGraphInstance = null;

        /// <summary>
        /// When the UI is enabled, it sets up all the VisualElement references and loads in the window data.
        /// </summary>
        private void OnEnable() 
        {
            //==================================Load Initial Data======================================//
            var uxmlAsset = Resources.Load<VisualTreeAsset>(ResourceAssetPaths.LogicalGraphWindow_UXML);
            uxmlAsset.CloneTree(rootVisualElement);
            m_mainSplitView = rootVisualElement.Q<UIElements.TwoPaneSplitView>(MAIN_SPLITVIEW);
            //=========================================================================================//=

            //==================================Register Toolbar=======================================//
            m_toolbar = rootVisualElement.Q<Toolbar>(TOOLBAR);

            // Save Button
            m_saveGraphButton = new ToolbarButton(() =>
            {
                if (m_openedGraphInstance != null)
                {
                    EditorUtility.SetDirty(m_openedGraphInstance);
                    AssetDatabase.SaveAssets();
                }
            });
            m_saveGraphButton.text = "Save";
            m_toolbar.Add(m_saveGraphButton);
            //=========================================================================================//

            //====================================Register Panels======================================//
            // Left panel is dependent on the right (NodeGraphView) so ordering is important!
            VisualElement mainPanelRight = rootVisualElement.Q<VisualElement>(MAIN_PANEL_RIGHT);
            VisualElement mainPanelLeft = rootVisualElement.Q<VisualElement>(MAIN_PANEL_LEFT);

            // Populate right panel
            m_nodeGraphView = new NodeGraphView();
            m_nodeGraphView.StretchToParentSize();
            m_nodeGraphView.OnAddToSelection += OnGraphElementSelectionAdded;
            m_nodeGraphView.OnRemoveFromSelection += OnGraphElementSelectionRemoved;
            m_nodeGraphView.OnClearSelection += OnGraphElementSelectionCleared;
            mainPanelRight.Add(m_nodeGraphView);

            m_customMenuController = new CustomMenuController(mainPanelRight, m_nodeGraphView);

            // Populate left panel
            List<(string, TabContentElement)> tabs = new List<(string, TabContentElement)>();
            tabs.Add(("Library", m_libraryTab = new LibraryTabElement((string guid) => { OpenGraph(guid); }, m_customMenuController)));
            tabs.Add(("Inspector", m_inspectorTab = new InspectorTabElement(m_nodeGraphView)));
            m_nodeGraphView.OnRemoveNode += (node) => { m_inspectorTab.SetNode(null, null); };
            m_mainTabGroup = new TabGroupElement(tabs);
            m_mainTabGroup.StretchToParentSize();
            m_nodeGraphView.OnMouseClick += () => { m_mainTabGroup.SelectTab(m_inspectorTab); };
            mainPanelLeft.Add(m_mainTabGroup);

            // Other setup
            m_inspectorTab.GraphInspector.OnBlackboardElementChanged += (undoGroup) => { m_nodeGraphView.CallAllNodeViewDrawerBlackboardElementChanged(undoGroup); };
            //=========================================================================================//

            //==================================Callback Listeners=====================================//
            GraphModificationProcessor.OnGraphCreated += OnNewGraphCreated;
            GraphModificationProcessor.OnGraphWillDelete += OnGraphWillDelete;
            //=========================================================================================//

            // Deserialize the editor window data.
            DeserializeData();
        }

        /// <summary>
        /// Before closing window, save the editor window state and break any listeners.
        /// </summary>
        private void OnDisable()
        {
            SerializeData();
            m_nodeGraphView.Reset();
            GraphModificationProcessor.OnGraphCreated -= OnNewGraphCreated;
            GraphModificationProcessor.OnGraphWillDelete -= OnGraphWillDelete;
        }

        private void Update()
        {
            UpdateSaveButtonState(); // There's no callback for when an asset is set to dirty so we have to check it ever frame :(
        }

        /// <summary>
        /// Retrieves editor window state from EditorPrefs and loads it
        /// </summary>
        private async void DeserializeData()
        {
            // Get the serialized data from EditorPrefs.
            string serializedData = EditorPrefs.GetString(DATA_STRING, "");
            if(string.IsNullOrEmpty(serializedData))
            {
                m_graphWindowData = new GraphWindowData(); 
            }
            else
            {
                m_graphWindowData = JsonUtility.FromJson<GraphWindowData>(serializedData);
            }
            //Debug.Log("Deserialized data: " + serializedData);
            
            // Load the data where necessary
            m_mainSplitView.SetSplitPosition(m_graphWindowData.MainSplitViewPosition);
            if(!string.IsNullOrEmpty(m_graphWindowData.OpenGraphGUID))
            {
                OpenGraph(m_graphWindowData.OpenGraphGUID);
                m_nodeGraphView.SetViewPosition(m_graphWindowData.GraphViewPosition);
            }
            m_mainTabGroup.DeserializeData(m_graphWindowData.MainTabGroup);
            m_nodeGraphView.ShowMinimap(m_graphWindowData.ShowMinimap);

            // Load graph element selection
            // Super sad that I need to wait a frame before I can add things to the selection and there's no clean way to
            // wait a frame. Otherwise, the elements will be technically selected but visually unselected.
            await Task.Delay(1);
            m_nodeGraphView.SetSelection(new List<string>(m_graphWindowData.SelectedGraphElements));
        }

        /// <summary>
        /// Update the editor window state class and serialize it into a string to be stored in EditorPrefs.
        /// </summary>
        private void SerializeData()
        {
            m_graphWindowData.MainSplitViewPosition = m_mainSplitView.SplitPosition;
            m_graphWindowData.MainTabGroup = m_mainTabGroup.GetSerializedData();
            m_graphWindowData.GraphViewPosition = m_nodeGraphView.GetViewPosition();
            m_graphWindowData.ShowMinimap = m_nodeGraphView.IsMinimapShowing();

            //Debug.Log("Serializing data: " + JsonUtility.ToJson(m_graphWindowData, true));
            EditorPrefs.SetString(DATA_STRING, JsonUtility.ToJson(m_graphWindowData, true));
        }

        /// <summary>
        /// Opens a specified graph and lets all the systems know.
        /// </summary>
        public void OpenGraph(string guid)
        {
            CloseCurrentGraph();

            m_openedGraphInstance = AssetDatabase.LoadAssetAtPath<NodeGraph>(AssetDatabase.GUIDToAssetPath(guid));
            if (m_openedGraphInstance != null)
            {
                m_graphWindowData.OpenGraphGUID = guid;
                m_libraryTab.SetCurrentNodeGraph(m_openedGraphInstance, guid);
                m_inspectorTab.SetOpenNodeGraph(m_openedGraphInstance);
                m_nodeGraphView.SetNodeCollection(m_openedGraphInstance);
            }
        }

        /// <summary>
        /// Closes currently opened graph and updates editor window state.
        /// </summary>
        private void CloseCurrentGraph()
        {
            m_graphWindowData.OpenGraphGUID = "";
            m_openedGraphInstance = null;
            m_inspectorTab.SetOpenNodeGraph(null);
            m_nodeGraphView.SetNodeCollection(null);
        }

        /// <summary>
        /// Callback to call when new graph is created outside of the editor window.
        /// </summary>
        private void OnNewGraphCreated(NodeGraph graph)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(graph, out string guid, out long localId);
            m_libraryTab.OnGraphCreate(graph, guid);
            OpenGraph(guid);
        }

        /// <summary>
        /// Callback to call when graph is deleted from outside of the editor window.
        /// </summary>
        private void OnGraphWillDelete(NodeGraph graph)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(graph, out string guid, out long localId);
            if (graph == m_openedGraphInstance)
            {
                CloseCurrentGraph();
            }
            m_libraryTab.OnGraphDelete(graph, guid);
        }

        /// <summary>
        /// Callback to call when an element is selected in the GraphView.
        /// </summary>
        private void OnGraphElementSelectionAdded(ISelectable addedElement)
        {
            if(m_nodeGraphView.selection.Count == 1)
            {
                NodeView nodeView = (addedElement as NodeView);
                if (nodeView != null)
                {
                    m_inspectorTab.SetNode(nodeView.Node, nodeView.SerializedNode);
                }
            }
            else
            {
                m_inspectorTab.SetNode(null, null);
            }

            if(addedElement is NodeView)
            {
                m_graphWindowData.SelectedGraphElements.Add((addedElement as NodeView).NodeId);
            }
            if(addedElement is EdgeView)
            {
                m_graphWindowData.SelectedGraphElements.Add((addedElement as EdgeView).EdgeId);
            }
        }

        private void OnGraphElementSelectionRemoved(ISelectable removedElement)
        {
            if (removedElement is NodeView)
            {
                m_graphWindowData.SelectedGraphElements.Remove((removedElement as NodeView).NodeId);
            }
            if (removedElement is EdgeView)
            {
                m_graphWindowData.SelectedGraphElements.Remove((removedElement as EdgeView).EdgeId);
            }
        }

        private void OnGraphElementSelectionCleared()
        {
            m_graphWindowData.SelectedGraphElements.Clear();
            m_inspectorTab.SetNode(null, null);
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reset Graph Position"), false, () => 
            {
                m_nodeGraphView.FrameAll();
            });
            menu.AddItem(new GUIContent("Reset Graph Position to origin"), false, () =>
            {
                m_nodeGraphView.FrameOrigin();
            });
            menu.AddItem(new GUIContent("Show Minimap"), m_graphWindowData.ShowMinimap, () => 
            {
                m_graphWindowData.ShowMinimap = !m_graphWindowData.ShowMinimap;
                m_nodeGraphView.ShowMinimap(m_graphWindowData.ShowMinimap);
            });
        }

        private void UpdateSaveButtonState()
        {
            if (EditorUtility.IsDirty(m_openedGraphInstance))
            {
                m_saveGraphButton.text = "*Save";
            }
            else
            {
                m_saveGraphButton.text = "Save";
            }
        }
    }
}