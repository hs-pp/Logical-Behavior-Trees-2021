﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GraphTheory.Editor
{
    public class GraphTypeMetadata
    {
        public Type GraphType { get; private set; } = null;
        public List<Type> UniversalNodeTypes { get; private set; } = new List<Type>();
        public List<Type> ValidNodeTypes { get; private set; } = new List<Type>();

        private List<Type> m_allNodeDrawers = new List<Type>();
        private Dictionary<Type, Type> m_universalNodeViewDrawers = new Dictionary<Type, Type>();
        private Dictionary<Type, Type> m_validNodeViewDrawers = new Dictionary<Type, Type>();


        public GraphTypeMetadata()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                UniversalNodeTypes.AddRange(assemblies[i].GetTypes().Where(x => typeof(ANode).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<SupportedGraphTypesAttribute>() == null));

                m_allNodeDrawers.AddRange(assemblies[i].GetTypes().Where(x => typeof(NodeViewDrawer).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<CustomNodeDrawerAttribute>() != null));
            }

            FindNodeDrawerTypes(UniversalNodeTypes, m_universalNodeViewDrawers);
            //TODO SORT THEM!!
        }

        public void SetNewGraphType(Type graphType)
        {
            if (graphType == GraphType)
            {
                return;
            }

            GraphType = graphType;
            ValidNodeTypes.Clear();

            if (graphType == null)
            {
                return;
            }
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                ValidNodeTypes.AddRange(assemblies[i].GetTypes().Where(x => typeof(ANode).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<SupportedGraphTypesAttribute>() != null
                    && x.GetCustomAttribute<SupportedGraphTypesAttribute>().SupportedTypes.Contains(graphType)));
                //TODO SORT THEM!
            }

            FindNodeDrawerTypes(ValidNodeTypes, m_validNodeViewDrawers);
        }

        private void FindNodeDrawerTypes(List<Type> nodeTypes, Dictionary<Type, Type> nodeDrawers)
        {
            nodeDrawers.Clear();
            for(int i = 0; i < nodeTypes.Count; i++)
            {
                Type nodeDrawer = m_allNodeDrawers.Find(x => x.GetCustomAttribute<CustomNodeDrawerAttribute>().NodeType == nodeTypes[i]);
                if (nodeDrawer != null)
                {
                    nodeDrawers.Add(nodeTypes[i], nodeDrawer);
                }
            }
        }

        public Type GetNodeViewDrawerType(Type nodeType)
        {
            Type nodeViewDrawerType = typeof(NodeViewDrawer);
            if(m_universalNodeViewDrawers.ContainsKey(nodeType))
            {
                nodeViewDrawerType = m_universalNodeViewDrawers[nodeType];
            }
            else if (m_validNodeViewDrawers.ContainsKey(nodeType))
            {
                nodeViewDrawerType = m_validNodeViewDrawers[nodeType];
            }
            return nodeViewDrawerType;
        }
    }
}