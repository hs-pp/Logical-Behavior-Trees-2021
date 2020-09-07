using GraphTheory;
using GraphTheory.Editor;
using GraphTheory.Editor.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor
{
    public class LibraryTabElement : TabContentElement
    {
        private const string ALL_GRAPHS_GROUP = "all-graphs-group";
        private const string OPENED_GRAPH_FIELD = "opened-graph-field";
        private const string FAVORITES_FOLDOUT = "favorites-foldout";
        private const string RECENTS_FOLDOUT = "recents-foldout";
        private const string SEARCH_FIELD = "graphs-search-bar";

        private string m_currentGraphGuid = "";
        private ObjectDisplayField m_currentGraphDisplay = null;

        private RecentsController m_recentsController = null;
        private FavoritesController m_favoritesController = null;
        private AllGraphsController m_allGraphsController = null;

        private LibraryTabData LibraryTabData { get; set; }
        private Action<string> OnObjectFieldDoubleClick { get; } = null;

        public LibraryTabElement(Action<string> onObjectFieldDoubleClick)
        {
            OnObjectFieldDoubleClick = onObjectFieldDoubleClick;

            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/LibraryTabElement");
            xmlAsset.CloneTree(this);

            m_currentGraphDisplay = this.Q<ObjectDisplayField>(OPENED_GRAPH_FIELD);

            m_recentsController = new RecentsController(this);
            m_favoritesController = new FavoritesController(this);
            m_allGraphsController = new AllGraphsController(this);
        }

        private Manipulator GetAddToFavManip(string graphGUID)
        {
            return new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Add to favorites",
                    (a) =>
                    {
                        if (!string.IsNullOrEmpty(graphGUID) && !LibraryTabData.FavoritesGUIDs.Contains(graphGUID))
                        {
                            LibraryTabData.FavoritesGUIDs.Add(graphGUID);
                            m_favoritesController.AddGraphToFavorites(graphGUID);
                        }
                    },
                    (a) => DropdownMenuAction.Status.Normal,
                    "Added to favorites");
            });
        }

        public Manipulator GetRemoveFromFavManip(string graphGUID)
        {
            return new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Remove from favorites",
                    (a) =>
                    {
                        if (!string.IsNullOrEmpty(graphGUID) && LibraryTabData.FavoritesGUIDs.Contains(graphGUID))
                        {
                            LibraryTabData.FavoritesGUIDs.Remove(graphGUID);
                            m_favoritesController.RemoveGraphFromFavorites(graphGUID);
                        }
                    },
                    (a) => DropdownMenuAction.Status.Normal,
                    "Removed from favorites");
            });
        }

        public override void DeserializeData(string data)
        {
            LibraryTabData = JsonUtility.FromJson<LibraryTabData>(data);
            if (LibraryTabData == null)
            {
                LibraryTabData = new LibraryTabData();
            }

            m_recentsController.DeserializeData(LibraryTabData);
            m_favoritesController.DeserializeData(LibraryTabData);
            m_allGraphsController.DeserializeData(LibraryTabData);
        }

        public void SetCurrentNodeGraph(NodeGraph graph, string guid)
        {
            if (m_currentGraphGuid == guid)
            {
                return;
            }
            m_currentGraphDisplay.SetObject(graph);
            m_recentsController.OnOpenGraph(guid, m_currentGraphGuid);

            m_currentGraphGuid = guid;
        }

        public void OnGraphCreate(NodeGraph graph, string guid)
        {
            m_allGraphsController.OnGraphCreate(graph, guid);
        }

        public void OnGraphDelete(NodeGraph graph, string guid)
        {
            if (m_currentGraphGuid == guid)
            {
                m_currentGraphDisplay.SetObject(null);
                m_currentGraphGuid = "";
            }
            m_recentsController.OnGraphDelete(guid);
            m_favoritesController.OnGraphDelete(guid);
            m_allGraphsController.OnGraphDelete(graph, guid);

        }

        public override string GetSerializedData()
        {
            m_recentsController.SerializeData(LibraryTabData);
            m_favoritesController.SerializeData(LibraryTabData);
            m_allGraphsController.SerializeData(LibraryTabData);
            return JsonUtility.ToJson(LibraryTabData);
        }

        private class RecentsController
        {
            private static int NUM_RECENTS = 3;
            private GraphGroupFoldout m_recentsGroup = null;
            private LibraryTabElement m_libraryTab = null;

            public RecentsController(LibraryTabElement libraryTab)
            {
                m_libraryTab = libraryTab;
                m_recentsGroup = libraryTab.Q<GraphGroupFoldout>(RECENTS_FOLDOUT);
                m_recentsGroup.Setup("Recent", GraphGroupFoldout.SortRule.NONE, m_libraryTab.OnObjectFieldDoubleClick);
                m_recentsGroup.AddDisplayFieldManipulator(m_libraryTab.GetAddToFavManip);
            }

            public void DeserializeData(LibraryTabData libraryTabData)
            {
                for (int i = 0; i < libraryTabData.RecentsGUIDs.Count; i++)
                {
                    m_recentsGroup.AddGraphByGUID(libraryTabData.RecentsGUIDs[i]);
                }
                if (m_recentsGroup.NumElements > NUM_RECENTS)
                {
                    int diff = m_recentsGroup.NumElements - NUM_RECENTS;
                    for (int i = 0; i < diff; i++)
                    {
                        m_recentsGroup.RemoveByIndex(m_recentsGroup.NumElements - 1);
                    }
                }
                else if (m_recentsGroup.NumElements < NUM_RECENTS)
                {
                    int diff = NUM_RECENTS - m_recentsGroup.NumElements;
                    for (int i = 0; i < diff; i++)
                    {
                        m_recentsGroup.AddGraphByGUID(null);
                    }
                }

                m_recentsGroup.SetToggle(libraryTabData.IsRecentsFoldoutOpen);
            }

            public void OnOpenGraph(string newGraph, string oldGraph)
            {
                if (!string.IsNullOrEmpty(newGraph))
                {
                    m_recentsGroup.RemoveGraphByGUID(newGraph);
                }

                if (!string.IsNullOrEmpty(oldGraph) && !m_recentsGroup.ContainsGraph(oldGraph))
                {
                    m_recentsGroup.AddByIndex(0, oldGraph);
                    if (m_recentsGroup.NumElements > NUM_RECENTS)
                    {
                        int diff = m_recentsGroup.NumElements - NUM_RECENTS;
                        for (int i = 0; i < diff; i++)
                        {
                            m_recentsGroup.RemoveByIndex(m_recentsGroup.NumElements - 1);
                        }
                    }
                }
            }

            public void OnGraphDelete(string guid)
            {
                if (m_recentsGroup.ContainsGraph(guid))
                {
                    m_recentsGroup.RemoveGraphByGUID(guid);
                    m_recentsGroup.AddGraphByGUID(null);
                }
            }

            public void SerializeData(LibraryTabData libraryTabData)
            {
                libraryTabData.RecentsGUIDs.Clear();
                for (int i = 0; i < NUM_RECENTS; i++)
                {
                    libraryTabData.RecentsGUIDs.Add(m_recentsGroup.GetGraphGUIDAtIndex(i));
                }
                libraryTabData.IsRecentsFoldoutOpen = m_recentsGroup.IsToggledOn;
            }
        }

        private class FavoritesController
        {
            private GraphGroupFoldout m_favoritesGroup = null;
            private LibraryTabElement m_libraryTab = null;

            public FavoritesController(LibraryTabElement libraryTab)
            {
                m_libraryTab = libraryTab;
                m_favoritesGroup = m_libraryTab.Q<GraphGroupFoldout>(FAVORITES_FOLDOUT);
                m_favoritesGroup.Setup("Favorites", GraphGroupFoldout.SortRule.TYPE_AND_NAME, m_libraryTab.OnObjectFieldDoubleClick);
                m_favoritesGroup.AddDisplayFieldManipulator(m_libraryTab.GetRemoveFromFavManip);
            }

            public void AddGraphToFavorites(string guid)
            {
                m_favoritesGroup.AddGraphByGUID(guid);
            }

            public void RemoveGraphFromFavorites(string guid)
            {
                m_favoritesGroup.RemoveGraphByGUID(guid);
            }

            public void OnGraphDelete(string guid)
            {
                RemoveGraphFromFavorites(guid);
            }

            public void DeserializeData(LibraryTabData libraryTabData)
            {
                for (int i = 0; i < libraryTabData.FavoritesGUIDs.Count; i++)
                {
                    m_favoritesGroup.AddGraphByGUID(libraryTabData.FavoritesGUIDs[i]);
                }
                m_favoritesGroup.SetToggle(libraryTabData.IsFavoritesFoldoutOpen);
            }

            public void SerializeData(LibraryTabData libraryTabData)
            {
                libraryTabData.FavoritesGUIDs.Clear();
                for (int i = 0; i < m_favoritesGroup.NumElements; i++)
                {
                    libraryTabData.FavoritesGUIDs.Add(m_favoritesGroup.GetGraphGUIDAtIndex(i));
                }
                libraryTabData.IsFavoritesFoldoutOpen = m_favoritesGroup.IsToggledOn;
            }
        }

        private class AllGraphsController
        {
            private ToolbarSearchField m_searchField = null;
            private VisualElement m_allGraphsGroup = null;
            private Dictionary<Type, GraphGroupFoldout> m_allGraphsFoldouts = new Dictionary<Type, GraphGroupFoldout>();
            private LibraryTabElement m_libraryTab = null;

            public AllGraphsController(LibraryTabElement libraryTab)
            {
                m_searchField = libraryTab.Q<ToolbarSearchField>(SEARCH_FIELD);
                m_searchField.RegisterValueChangedCallback(x => { OnSearchQueryChanged(x.newValue); });
                m_allGraphsGroup = libraryTab.Q<VisualElement>(ALL_GRAPHS_GROUP);
                m_libraryTab = libraryTab;

                PopulateGroups();
            }

            private void PopulateGroups()
            {
                Type[] m_allGraphTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(
                    x => typeof(NodeGraph).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).ToArray();

                foreach (Type t in m_allGraphTypes)
                {
                    string[] foundGraphGUIDs = AssetDatabase.FindAssets("t: " + t.ToString());

                    GraphGroupFoldout foldout = new GraphGroupFoldout();
                    foldout.Setup(t.ToString(), GraphGroupFoldout.SortRule.NAME, m_libraryTab.OnObjectFieldDoubleClick);

                    foreach (string guid in foundGraphGUIDs)
                    {
                        foldout.AddGraphByGUID(guid);
                    }
                    m_allGraphsGroup.Add(foldout);
                    m_allGraphsFoldouts.Add(t, foldout);
                    foldout.AddDisplayFieldManipulator(m_libraryTab.GetAddToFavManip);
                }

                foreach (GraphGroupFoldout foldout in m_allGraphsFoldouts.Values)
                {
                    foldout.HideFoldoutIfNecessary();
                }
            }

            public void DeserializeData(LibraryTabData libraryTabData)
            {
                m_searchField.value = libraryTabData.SearchQuery;
                OnSearchQueryChanged(libraryTabData.SearchQuery);
            }

            private void OnSearchQueryChanged(string query)
            {
                foreach (GraphGroupFoldout foldout in m_allGraphsFoldouts.Values)
                {
                    foldout.ApplySearchQuery(query);
                }
            }

            public void OnGraphCreate(NodeGraph graph, string guid)
            {
                if (!string.IsNullOrEmpty(guid))
                {
                    m_allGraphsFoldouts[graph.GetType()].AddGraphByGUID(guid);
                    m_allGraphsFoldouts[graph.GetType()].HideFoldoutIfNecessary();
                }
            }

            public void OnGraphDelete(NodeGraph graph, string guid)
            {
                m_allGraphsFoldouts[graph.GetType()].RemoveGraphByGUID(guid);
                m_allGraphsFoldouts[graph.GetType()].HideFoldoutIfNecessary();
            }

            public void SerializeData(LibraryTabData libraryTabData)
            {
                libraryTabData.SearchQuery = m_searchField.value;
            }
        }
    }
}