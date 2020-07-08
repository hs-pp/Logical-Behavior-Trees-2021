using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GraphTheory.Editor
{
    /// <summary>
    /// TODO: Redo this in UIElements PopupWindow
    /// </summary>
    public class CreateNewGraphPopup : EditorWindow
    {
        private GUIStyle m_titleStyle = new GUIStyle();

        private Type[] m_allGraphTypes = new Type[0];
        private string[] m_allGraphTypeNames = new string[0];
        private int m_selectedIndex = -1;
        private string m_assetName = "";

        public static void OpenWindow()
        {
            var window = GetWindow<CreateNewGraphPopup>();
            window.titleContent = new GUIContent("Create New Node Graph");
            window.minSize = new Vector2(300, 160);
            window.maxSize = window.minSize;
            window.Show();
        }

        private void OnEnable()
        {
            m_titleStyle.richText = true;
            m_titleStyle.fontSize = 20;
            m_titleStyle.fontStyle = FontStyle.Bold;

            m_allGraphTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(
                x => typeof(NodeGraph).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).ToArray();
            m_allGraphTypeNames = m_allGraphTypes.Select(x => x.Name).ToArray();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("<color=white>Create New Node Graph</color>", m_titleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            GUILayout.Label("Please select a graph type:");
            m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_allGraphTypeNames);

            GUILayout.Space(8);

            GUILayout.Label("Graph name:");
            m_assetName = GUILayout.TextField(m_assetName);

            GUILayout.Space(8);

            if (m_selectedIndex == -1 || string.IsNullOrEmpty(m_assetName))
            {
                GUI.enabled = false;
            }
            if(GUILayout.Button("Create Graph"))
            {
                AssetDatabase.CreateAsset(CreateInstance(m_allGraphTypes[m_selectedIndex]), GetFullAssetPath(m_assetName));
            }

            GUI.enabled = true;

            GUILayout.EndVertical();
        }

        private string GetFullAssetPath(string name)
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (File.Exists(assetPath))
                assetPath = Path.GetDirectoryName(assetPath) + "/";
            if (string.IsNullOrEmpty(assetPath)) assetPath = "Assets/";

            return $"{assetPath}{name}.asset";
        }
    }
}