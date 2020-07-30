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
        private ObjectDisplayFoldout m_recentsFoldout = null;
        private ObjectDisplayFoldout m_favoritesFoldout = null;
        private List<ObjectDisplayFoldout> m_allGraphsFoldouts = new List<ObjectDisplayFoldout>();

        public LibraryTabElement() 
        {
            var xmlAsset = Resources.Load<VisualTreeAsset>("GraphTheory/Tabs/LibraryTabElement");
            xmlAsset.CloneTree(this);

            m_allGraphsGroup = this.Q<VisualElement>(ALL_GRAPHS_GROUP);
            m_objectDisplayField = this.Q<ObjectDisplayField>(OPENED_GRAPH_FIELD);

            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            for(int i = 0; i < 3; i++)
            {
                objects.Add(null);
            }

            m_favoritesFoldout = this.Q<ObjectDisplayFoldout>(FAVORITES_FOLDOUT);
            m_favoritesFoldout.SetName("Favorites");
            m_favoritesFoldout.SetAssets(objects);

            m_recentsFoldout = this.Q<ObjectDisplayFoldout>(RECENTS_FOLDOUT);
            m_recentsFoldout.SetName("Recents");
            m_recentsFoldout.SetAssets(objects);

            m_recentsFoldout.AddDisplayFieldManipulator(AddToFavManipCreator);
            m_favoritesFoldout.AddDisplayFieldManipulator(RemoveFromFavManipCreator);

            Type[] m_allGraphTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(
                x => typeof(NodeGraph).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).ToArray();

            foreach(Type t in m_allGraphTypes)
            {
                List<UnityEngine.Object> graphs = new List<UnityEngine.Object>();

                Debug.Log(t.ToString());
                string[] foundGraphGUIDs = AssetDatabase.FindAssets("t: " + t.ToString());
                foreach(string guid in foundGraphGUIDs)
                {
                    NodeGraph graph = AssetDatabase.LoadAssetAtPath<NodeGraph>(AssetDatabase.GUIDToAssetPath(guid));
                    graphs.Add(graph);
                }

                ObjectDisplayFoldout foldout = new ObjectDisplayFoldout();
                foldout.SetName($"{t.ToString()}  ({graphs.Count})");
                foldout.SetAssets(graphs);
                m_allGraphsGroup.Add(foldout);
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

        private class GraphMetaData
        {
            public string GUID = "";
            public string Name = "";
            public UnityEngine.Object ObjectRef = null;
            public ObjectDisplayFoldout ParentFoldout = null;
        }
    }
}