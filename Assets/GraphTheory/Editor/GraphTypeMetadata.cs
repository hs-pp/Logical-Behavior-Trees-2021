using System;
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
        private Dictionary<Type, Type> m_universalNodeDrawers = new Dictionary<Type, Type>();
        private Dictionary<Type, Type> m_validNodeDrawers = new Dictionary<Type, Type>();


        public GraphTypeMetadata()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                UniversalNodeTypes.AddRange(assemblies[i].GetTypes().Where(x => typeof(ANode).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<SupportedGraphTypesAttribute>() == null));

                m_allNodeDrawers.AddRange(assemblies[i].GetTypes().Where(x => typeof(NodeDrawer).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<CustomNodeDrawerAttribute>() != null));
            }

            FindNodeDrawerTypes(UniversalNodeTypes, m_universalNodeDrawers);
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

            FindNodeDrawerTypes(ValidNodeTypes, m_validNodeDrawers);
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

        public Type GetNodeDrawerType(Type nodeType)
        {
            Type nodeDrawerType = typeof(NodeDrawer);
            if(m_universalNodeDrawers.ContainsKey(nodeType))
            {
                nodeDrawerType = m_universalNodeDrawers[nodeType];
            }
            else if (m_validNodeDrawers.ContainsKey(nodeType))
            {
                nodeDrawerType = m_validNodeDrawers[nodeType];
            }
            return nodeDrawerType;
        }
    }
}