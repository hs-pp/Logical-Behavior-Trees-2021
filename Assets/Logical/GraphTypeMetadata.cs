using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Logical.Editor
{
    /// <summary>
    /// Class that fetches and stores all pertinent classes to the whole Logical system.
    /// Uses some cute one-time reflection to find stuff with the right attributes.
    /// Should only be used in editor scripts.
    /// </summary>
    public static class GraphTypeMetadata
    {
        public static Dictionary<Type, List<Type>> GraphToNodes = new Dictionary<Type, List<Type>>();
        public static List<Type> UniversalNodeTypes = new List<Type>();
        public static Dictionary<Type, Type> NodeToNodeViewDrawer = new Dictionary<Type, Type>();
        public static Dictionary<Type, Type> GraphToGraphProperties = new Dictionary<Type, Type>();

        static GraphTypeMetadata()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                foreach(Type type in assemblies[i].GetTypes())
                {
                    if (type.IsAbstract)
                        continue;

                    if(typeof(NodeGraph).IsAssignableFrom(type))
                    {
                        if (!GraphToNodes.ContainsKey(type))
                        {
                            GraphToNodes.Add(type, new List<Type>());
                        }
                    }
                    else if(typeof(ANode).IsAssignableFrom(type))
                    {
                        NodeAttribute nodeAttribute = type.GetCustomAttribute<NodeAttribute>();
                        if(nodeAttribute == null)
                        {
                            UniversalNodeTypes.Add(type);
                        }
                        else
                        {
                            if (!GraphToNodes.ContainsKey(nodeAttribute.GraphType))
                            {
                                GraphToNodes.Add(nodeAttribute.GraphType, new List<Type>());
                            }
                            GraphToNodes[nodeAttribute.GraphType].Add(type);
                        }
                    }
                    else if(typeof(NodeViewDrawer).IsAssignableFrom(type)
                        && type.GetCustomAttribute<CustomNodeViewDrawerAttribute>() != null)
                    {
                        NodeToNodeViewDrawer.Add(type.GetCustomAttribute<CustomNodeViewDrawerAttribute>().NodeType, type);
                    }
                    else if(typeof(AGraphProperties).IsAssignableFrom(type)
                        && type.GetCustomAttribute<GraphPropertiesAttribute>() != null)
                    {
                        GraphToGraphProperties.Add(type.GetCustomAttribute<GraphPropertiesAttribute>().GraphType, type);
                    }
                }
            }
            //DebugTypes();
        }

        private static void DebugTypes() 
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Debug Types");
            foreach(Type graphType in GraphToNodes.Keys)
            {
                stringBuilder.Append("\n" + graphType.Name);
                foreach(Type nodeType in GraphToNodes[graphType])
                {
                    if(NodeToNodeViewDrawer.ContainsKey(nodeType))
                    {
                        stringBuilder.Append("\n --" + nodeType.Name + " -- " + NodeToNodeViewDrawer[nodeType]);
                    }
                    else
                    {
                        stringBuilder.Append("\n --" + nodeType.Name);
                    }
                }
            }
            Debug.Log(stringBuilder.ToString());
        }

        public static List<Type> GetAllGraphTypes()
        {
            return new List<Type>(GraphToNodes.Keys);
        }

        public static List<Type> GetNodeTypesFromGraphType(Type graphType)
        {
            return GraphToNodes[graphType];
        }

        public static Type GetNodeViewDrawerType(Type nodeType) 
        {
            if(NodeToNodeViewDrawer.ContainsKey(nodeType))
            {
                return NodeToNodeViewDrawer[nodeType];
            }
            else
            {
                return typeof(NodeViewDrawer);
            }
        }

        public static Type GetGraphPropertiesType(Type graphType)
        {
            if(!GraphToGraphProperties.ContainsKey(graphType))
            {
                Debug.LogError("Problem creating graph instance! Could not find associated GraphProperties. Ensure that your GraphProperties class has the GraphProperties Attribute on it!");
                return null;
            }
            return GraphToGraphProperties[graphType];
        }
    }
}