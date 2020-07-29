using GraphTheory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(NodeGraphRunner))]
public class NodeGraphRunnerDrawer : Editor
{
    private NodeGraphRunner m_nodeGraphRunner = null;
    private NodeGraph m_nodeGraph = null;
    void OnEnable()
    {
        m_nodeGraphRunner = target as NodeGraphRunner;
        m_nodeGraph = m_nodeGraphRunner.NodeGraph;
        if (m_nodeGraph != null)
        {
            LoadGraphParamTypes();
        }
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        GUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_nodeGraph"));
        if(EditorGUI.EndChangeCheck())
        {
            Debug.Log("Changed");
            ResetGraphParams();
        }
        for (int i = 0; i < m_nodeGraphRunner.m_graphParams.Count; i++)
        {
            GraphParam graphParam = m_nodeGraphRunner.m_graphParams[i];
            //SerializedProperty graphParamValueProp = serializedObject.FindProperty("m_graphParams").GetArrayElementAtIndex(i).FindPropertyRelative("ParamValue");

            GUILayout.BeginHorizontal();
            GUILayout.Label(graphParam.ParamName);
            //EditorGUILayout.ObjectField(graphParamValueProp, graphParam.ParamType);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private void ResetGraphParams()
    {
        m_nodeGraph = m_nodeGraphRunner.NodeGraph;
        m_nodeGraphRunner.m_graphParams.Clear();
        if (m_nodeGraph != null)
        {
            FieldInfo[] infos = m_nodeGraph.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach(FieldInfo info in infos)
            {
                Debug.Log(info.Name);
                GraphParam newParam = new GraphParam()
                {
                    ParamName = info.Name,
                    ParamType = info.FieldType,
                    ParamValue = null
                };
                m_nodeGraphRunner.m_graphParams.Add(newParam);
            }
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
    private void LoadGraphParamTypes()
    {
        List<FieldInfo> infos = m_nodeGraph.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();

        Type graphType = m_nodeGraph.GetType();
        foreach(GraphParam param in m_nodeGraphRunner.m_graphParams)
        {
            param.ParamType = infos.Find(x => x.Name == param.ParamName).FieldType;
        }
    }

}
