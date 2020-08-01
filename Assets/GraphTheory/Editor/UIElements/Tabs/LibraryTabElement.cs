using GraphTheory.Editor.UIElements;
using System;
using System.Collections;
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
        private const string RECENTS_FOLDOUT = "recents-foldout";
        private const string FAVORITES_FOLDOUT = "favorites-foldout";

        private VisualElement m_allGraphsGroup = null;
        private ObjectDisplayField m_objectDisplayField = null;
        private GraphGroupFoldout m_recentsFoldout = null;
        private GraphGroupFoldout m_favoritesFoldout = null;
        private Dictionary<Type, GraphGroupFoldout> m_allGraphsFoldouts = new Dictionary<Type,GraphGroupFoldout>();

        public LibraryTabElement() 
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/Tabs/LibraryTabElement");
            xmlAsset.CloneTree(this);

            m_allGraphsGroup = this.Q<VisualElement>(ALL_GRAPHS_GROUP);
            m_objectDisplayField = this.Q<ObjectDisplayField>(OPENED_GRAPH_FIELD);

            m_favoritesFoldout = this.Q<GraphGroupFoldout>(FAVORITES_FOLDOUT);
            m_favoritesFoldout.Setup("Favorites", false);

            m_recentsFoldout = this.Q<GraphGroupFoldout>(RECENTS_FOLDOUT);
            m_recentsFoldout.Setup("Recents", false);

            m_recentsFoldout.AddDisplayFieldManipulator(AddToFavManipCreator);
            m_favoritesFoldout.AddDisplayFieldManipulator(RemoveFromFavManipCreator);

            Type[] m_allGraphTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(
                x => typeof(NodeGraph).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).ToArray();

            foreach(Type t in m_allGraphTypes)
            {
                string[] foundGraphGUIDs = AssetDatabase.FindAssets("t: " + t.ToString());

                GraphGroupFoldout foldout = new GraphGroupFoldout();
                foldout.Setup($"{t.ToString()}  ({foundGraphGUIDs.Length})", true);

                foreach (string guid in foundGraphGUIDs)
                {
                    foldout.AddGraphByGUID(guid);
                }
                m_allGraphsGroup.Add(foldout);
                m_allGraphsFoldouts.Add(t, foldout);
            }
        }

        public Manipulator AddToFavManipCreator()
        {
            return new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Add to favorites",
                    (a) => Debug.Log("added to favorites"),
                    (a) => DropdownMenuAction.Status.Normal,
                    "ACTION 1");
            });
        }

        public Manipulator RemoveFromFavManipCreator()
        {
            return new ContextualMenuManipulator((evt) =>
            {
                evt.menu.AppendAction("Remove from favorites",
                    (a) => Debug.Log("removed to favorites"),
                    (a) => DropdownMenuAction.Status.Normal,
                    "ACTION 1");
            });
        }

        public void SetOpenNodeGraph(NodeGraph graph)
        {
            m_objectDisplayField.SetObject(graph);
        }

        public override void DeserializeData(string data)
        {
        }

        public override string GetSerializedData()
        {
            return "lib tab";
        }
    }
}