using GraphTheory;
using GraphTheory.Editor.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LibraryTabElement2 : TabContentElement
{
    private const string ALL_GRAPHS_GROUP = "all-graphs-group";
    private const string OPENED_GRAPH_FIELD = "opened-graph-field";
    private const string FAVORITES_FOLDOUT = "favorites-foldout";
    private const string RECENTS_FOLDOUT = "recents-foldout";
    private const string SEARCH_FIELD = "graphs-search-bar";

    private ObjectDisplayField m_openGraphDisplay = null;

    private RecentsController m_recentsController = null;
    private FavoritesController m_favoritesController = null;
    private AllGraphsController m_allGraphsController = null;
    

    public LibraryTabElement2(Action<string> onObjectFieldDoubleClick)
    {
        var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/UIElements/TabGroup/LibraryTabElement");
        xmlAsset.CloneTree(this);

        m_openGraphDisplay = this.Q<ObjectDisplayField>(OPENED_GRAPH_FIELD);

        m_recentsController = new RecentsController(this.Q<GraphGroupFoldout>(RECENTS_FOLDOUT), onObjectFieldDoubleClick);
        m_favoritesController = new FavoritesController(this.Q<GraphGroupFoldout>(FAVORITES_FOLDOUT), onObjectFieldDoubleClick);
        m_allGraphsController = new AllGraphsController(this.Q<ToolbarSearchField>(SEARCH_FIELD), 
            this.Q<VisualElement>(ALL_GRAPHS_GROUP), onObjectFieldDoubleClick);

    }


    public override void DeserializeData(string data)
    {
    }

    public override string GetSerializedData()
    {
        return "";
    }

    private class RecentsController
    {
        private GraphGroupFoldout m_recentsGroup = null;
        private Action<string> m_onObjectFieldDoubleClick = null;

        public RecentsController(GraphGroupFoldout recentsGroup, Action<string> onObjectFieldDoubleClick)
        {
            m_recentsGroup = recentsGroup;
            m_onObjectFieldDoubleClick = onObjectFieldDoubleClick;
            m_recentsGroup.Setup("Recent", GraphGroupFoldout.SortRule.NONE, m_onObjectFieldDoubleClick);
        }
    }

    private class FavoritesController
    {
        private GraphGroupFoldout m_favoritesGroup = null;
        private Action<string> m_onObjectFieldDoubleClick = null;

        public FavoritesController(GraphGroupFoldout favoritesGroup, Action<string> onObjectFieldDoubleClick)
        {
            m_favoritesGroup = favoritesGroup;
            m_onObjectFieldDoubleClick = onObjectFieldDoubleClick;
            m_favoritesGroup.Setup("Favorites", GraphGroupFoldout.SortRule.TYPE_AND_NAME, m_onObjectFieldDoubleClick);
        }
    }

    private class AllGraphsController
    {
        private ToolbarSearchField m_searchField = null;
        private VisualElement m_allGraphsGroup = null;
        private Dictionary<Type, GraphGroupFoldout> m_allGraphsFoldouts = new Dictionary<Type, GraphGroupFoldout>();
        private Action<string> m_onObjectFieldDoubleClick = null;

        public AllGraphsController(ToolbarSearchField searchField, VisualElement allGraphsGroup, Action<string> onObjectFieldDoubleClick)
        {
            m_searchField = searchField;
            m_allGraphsGroup = allGraphsGroup;
            m_onObjectFieldDoubleClick = onObjectFieldDoubleClick;

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
                foldout.Setup(t.ToString(), GraphGroupFoldout.SortRule.NAME, m_onObjectFieldDoubleClick);

                foreach (string guid in foundGraphGUIDs)
                {
                    foldout.AddGraphByGUID(guid);
                }
                m_allGraphsGroup.Add(foldout);
                m_allGraphsFoldouts.Add(t, foldout);
                //foldout.AddDisplayFieldManipulator(AddToFavManipCreator);

            }
        }
    }
}
