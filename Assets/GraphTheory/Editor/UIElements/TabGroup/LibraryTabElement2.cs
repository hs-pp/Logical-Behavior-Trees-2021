using GraphTheory.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
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

    private VisualElement m_allGraphsGroup = null;
    private ObjectDisplayField m_openGraphDisplay = null;
    private GraphGroupFoldout m_favoritesFoldout = null;
    private GraphGroupFoldout m_recentsFoldout = null;
    private ToolbarSearchField m_searchField = null;


    public LibraryTabElement2()
    {
        var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/UIElements/TabGroup/LibraryTabElement");
        xmlAsset.CloneTree(this);

        m_openGraphDisplay = this.Q<ObjectDisplayField>(OPENED_GRAPH_FIELD);

        m_favoritesFoldout = this.Q<GraphGroupFoldout>(FAVORITES_FOLDOUT);
        m_recentsFoldout = this.Q<GraphGroupFoldout>(RECENTS_FOLDOUT);
        m_searchField = this.Q<ToolbarSearchField>(SEARCH_FIELD);
        m_allGraphsGroup = this.Q<VisualElement>(ALL_GRAPHS_GROUP);

    }


    public override void DeserializeData(string data)
    {
    }

    public override string GetSerializedData()
    {
        return "";
    }
}
