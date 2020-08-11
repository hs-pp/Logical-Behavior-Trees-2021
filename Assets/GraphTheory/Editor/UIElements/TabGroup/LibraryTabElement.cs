using GraphTheory.Editor.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTheory.Editor.UIElements
{
    public class LibraryTabElement : TabContentElement
    {
        private const string ALL_GRAPHS_GROUP = "all-graphs-group";
        private const string OPENED_GRAPH_FIELD = "opened-graph-field";
        private const string FAVORITES_FOLDOUT = "favorites-foldout";
        private const string RECENTS_FOLDOUT = "recents-foldout";
        private const string SEARCH_FIELD = "graphs-search-bar";
        private const int NUM_RECENT = 3;

        private VisualElement m_allGraphsGroup = null;
        private ObjectDisplayField m_objectDisplayField = null;
        private string m_openGraphGUID = "";
        private GraphGroupFoldout m_favoritesFoldout = null;
        private GraphGroupFoldout m_recentsFoldout = null;
        private ToolbarSearchField m_searchField = null;
        private Dictionary<Type, GraphGroupFoldout> m_allGraphsFoldouts = new Dictionary<Type,GraphGroupFoldout>();
        private Action<string> m_onObjectFieldDoubleClick = null;

        private LibraryTabData m_libraryTabData = null;

        public LibraryTabElement(Action<string> onObjectFieldDoubleClick) 
        {
            m_onObjectFieldDoubleClick = onObjectFieldDoubleClick;

            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/UIElements/TabGroup/LibraryTabElement");
            xmlAsset.CloneTree(this);

            m_allGraphsGroup = this.Q<VisualElement>(ALL_GRAPHS_GROUP);
            m_objectDisplayField = this.Q<ObjectDisplayField>(OPENED_GRAPH_FIELD);

            m_favoritesFoldout = this.Q<GraphGroupFoldout>(FAVORITES_FOLDOUT);
            m_favoritesFoldout.Setup("Favorites", GraphGroupFoldout.SortRule.TYPE_AND_NAME, m_onObjectFieldDoubleClick);

            m_recentsFoldout = this.Q<GraphGroupFoldout>(RECENTS_FOLDOUT);
            m_recentsFoldout.Setup("Recents", GraphGroupFoldout.SortRule.NONE, m_onObjectFieldDoubleClick);

            m_searchField = this.Q<ToolbarSearchField>(SEARCH_FIELD);
            m_searchField.RegisterValueChangedCallback(x => { OnSearchQueryChanged(x.newValue); });

            m_recentsFoldout.AddDisplayFieldManipulator(AddToFavManipCreator);
            m_favoritesFoldout.AddDisplayFieldManipulator(RemoveFromFavManipCreator);

            Type[] m_allGraphTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(
                x => typeof(NodeGraph).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).ToArray();

            foreach(Type t in m_allGraphTypes)
            {
                string[] foundGraphGUIDs = AssetDatabase.FindAssets("t: " + t.ToString());

                GraphGroupFoldout foldout = new GraphGroupFoldout();
                foldout.Setup(t.ToString(), GraphGroupFoldout.SortRule.NAME, m_onObjectFieldDoubleClick);

                foreach (string guid in foundGraphGUIDs)
                {
                    foldout.AddGraphByGUID(guid);
                }
                m_allGraphsGroup.Add(foldout);
                m_allGraphsFoldouts.Add(t, foldout);
                foldout.AddDisplayFieldManipulator(AddToFavManipCreator);

            }
        }

        private Manipulator AddToFavManipCreator(string graphGUID)
        {
            return new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Add to favorites",
                    (a) =>
                    {
                        if(!string.IsNullOrEmpty(graphGUID) && !m_libraryTabData.FavoritesGUIDs.Contains(graphGUID))
                        {
                            m_libraryTabData.FavoritesGUIDs.Add(graphGUID);
                            m_favoritesFoldout.AddGraphByGUID(graphGUID);
                        }
                    },
                    (a) => DropdownMenuAction.Status.Normal,
                    "Added to favorites");
            });
        }

        private Manipulator RemoveFromFavManipCreator(string graphGUID)
        {
            return new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Remove from favorites",
                    (a) =>
                    {
                        if (!string.IsNullOrEmpty(graphGUID) && m_libraryTabData.FavoritesGUIDs.Contains(graphGUID))
                        {
                            m_libraryTabData.FavoritesGUIDs.Remove(graphGUID);
                            m_favoritesFoldout.RemoveGraphByGUID(graphGUID);
                        }
                    },
                    (a) => DropdownMenuAction.Status.Normal,
                    "Removed from favorites");
            });
        }

        public void SetOpenNodeGraph(NodeGraph graph, string graphGUID)
        {
            if(graphGUID == m_openGraphGUID)
            {
                return;
            }

            if(graph != null)
            {
                RegisterNewRecentGraph(m_openGraphGUID, graphGUID);
                m_openGraphGUID = graphGUID;
                m_objectDisplayField.SetObject(graph);
            }
            else
            {
                RegisterNewRecentGraph(m_openGraphGUID, null);
                m_openGraphGUID = null;
                m_objectDisplayField.SetObject(null);
            }
        }

        public void RegisterNewlyCreatedGraph(NodeGraph graph, string guid)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                m_allGraphsFoldouts[graph.GetType()].AddGraphByGUID(guid);
            }
        }

        public void HandleDeletedGraph(NodeGraph graph, string guid)
        {
            if(m_recentsFoldout.RemoveGraphByGUID(guid))
            {
                m_recentsFoldout.AddByIndex(NUM_RECENT - 1, "");
            }
            m_favoritesFoldout.RemoveGraphByGUID(guid);
            m_allGraphsFoldouts[graph.GetType()]?.RemoveGraphByGUID(guid);
        }

        private void RegisterNewRecentGraph(string oldGUID, string newGUID)
        {
            if (string.IsNullOrEmpty(oldGUID) || m_libraryTabData == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(newGUID))
            {
                int existingIndex = m_libraryTabData.RecentsGUIDs.FindIndex(x => x == newGUID);
                if (existingIndex != -1)
                {
                    m_libraryTabData.RecentsGUIDs.RemoveAt(existingIndex);
                    m_recentsFoldout.RemoveByIndex(existingIndex);
                    m_libraryTabData.RecentsGUIDs.Add("");
                    m_recentsFoldout.AddGraphByGUID("");
                }
            }

            if (m_recentsFoldout.AddByIndex(0, oldGUID))
            {
                m_recentsFoldout.RemoveByIndex(NUM_RECENT);

                m_libraryTabData.RecentsGUIDs.Insert(0, oldGUID);
                m_libraryTabData.RecentsGUIDs.RemoveAt(NUM_RECENT);
            }
        }

        private void OnSearchQueryChanged(string query)
        {
            foreach(GraphGroupFoldout foldout in m_allGraphsFoldouts.Values)
            {
                foldout.ApplySearchQuery(query);
            }
        }

        public override void DeserializeData(string data)
        {
            m_libraryTabData = JsonUtility.FromJson<LibraryTabData>(data);
            if(m_libraryTabData == null)
            {
                m_libraryTabData = new LibraryTabData();
                m_libraryTabData.RecentsGUIDs = new List<string>();
                for(int i = 0; i < NUM_RECENT; i++)
                {
                    m_libraryTabData.RecentsGUIDs.Add("");
                }
            }

            for (int i = 0; i < m_libraryTabData.FavoritesGUIDs.Count; i++)
            {
                m_favoritesFoldout.AddGraphByGUID(m_libraryTabData.FavoritesGUIDs[i]);
            }
            m_favoritesFoldout.SetToggle(m_libraryTabData.IsFavoritesFoldoutOpen);
            
            // Prune or grow recents count if necessary
            int recentsCount = m_libraryTabData.RecentsGUIDs.Count;
            if (m_libraryTabData.RecentsGUIDs.Count > NUM_RECENT)
            {
                for(int i = 0; i < (recentsCount - NUM_RECENT); i++)
                {
                    m_libraryTabData.RecentsGUIDs.RemoveAt(m_libraryTabData.RecentsGUIDs.Count - 1);
                }
            }
            else if(m_libraryTabData.RecentsGUIDs.Count < NUM_RECENT)
            {
                for(int i = 0; i < (NUM_RECENT - recentsCount); i++)
                {
                    m_libraryTabData.RecentsGUIDs.Add("");
                }
            }
            // Remove invalid guids in recents and replace with nulls
            int numInvalid = 0;
            for (int i = 0; i < m_libraryTabData.RecentsGUIDs.Count; i++)
            {
                bool isValid = m_recentsFoldout.AddGraphByGUID(m_libraryTabData.RecentsGUIDs[i]);
                if (!isValid)
                {
                    m_libraryTabData.RecentsGUIDs.RemoveAt(i);
                    i--;
                    numInvalid++;
                    Debug.Log("Found invalid");
                }
            }
            for (int i = 0; i < numInvalid; i++)
            {
                m_recentsFoldout.AddGraphByGUID("");
                m_libraryTabData.RecentsGUIDs.Add("");
            }
            m_recentsFoldout.SetToggle(m_libraryTabData.IsRecentsFoldoutOpen);

            m_searchField.value = m_libraryTabData.SearchQuery;
            OnSearchQueryChanged(m_libraryTabData.SearchQuery);
        }

        public override string GetSerializedData()
        {
            m_libraryTabData.IsFavoritesFoldoutOpen = m_favoritesFoldout.IsToggledOn;
            m_libraryTabData.IsRecentsFoldoutOpen = m_recentsFoldout.IsToggledOn;
            m_libraryTabData.SearchQuery = m_searchField.value;
            return JsonUtility.ToJson(m_libraryTabData);
        }
    }
}