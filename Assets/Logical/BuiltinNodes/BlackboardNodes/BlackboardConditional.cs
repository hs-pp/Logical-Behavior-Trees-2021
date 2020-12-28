using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Linq;
#endif

namespace Logical.BuiltInNodes
{
    /// <summary>
    /// This node grabs the graph's blackboard elements and creates a conditional switch that lead to different outports.
    /// </summary>
    [Serializable]
    public class BlackboardConditional : ANode
    {
        [SerializeField]
        private string m_blackboardElementId = "";
        public string BlackboardElementId { get { return m_blackboardElementId; } }
        [SerializeReference]
        private List<IBlackboardConditionalElement> m_conditionals = new List<IBlackboardConditionalElement>();

        public override void OnNodeEnter(GraphRunner graphRunner)
        {
            base.OnNodeEnter(graphRunner);
            if(string.IsNullOrEmpty(m_blackboardElementId))
            {
                Debug.LogError("BlackboardConditional: Blackboard element is not set!");
                return;
            }
            BlackboardElement element = graphRunner.BlackboardProperties.GetElementById(m_blackboardElementId);
            for(int i = 0; i < m_conditionals.Count; i++)
            {
                if (m_conditionals[i].Evaluate(element))
                {
                    TraverseEdge(graphRunner, i);
                    return;
                }
            }
        }

#if UNITY_EDITOR
        public static string BlackboardElementIdVarName = "m_blackboardElementId";
        public static string ConditionalsVarName = "m_conditionals";

        public override bool UseIMGUIPropertyDrawer { get { return true; } }
        public override int DefaultNumOutports { get { return 0; } }

        private static Dictionary<Type, Type> m_blackboardConditionalElementTypes = null;
        public static Dictionary<Type, Type> BlackboardConditionalElementTypes
        {
            get
            {
                if(m_blackboardConditionalElementTypes == null)
                {
                    SetElementTypeDictionary();
                }
                return m_blackboardConditionalElementTypes;
            }
        }
        private static Dictionary<Type, IBlackboardConditionalElement> m_blankConditionalElements = null;
        private static Dictionary<string, IBlackboardConditionalElement> m_blankConditionalsByTypeString 
            = new Dictionary<string, IBlackboardConditionalElement>();

        public static string GetOutportLabel(SerializedProperty conditionalProp)
        {
            if(m_blankConditionalElements == null)
            {
                SetElementTypeDictionary();
            }
            IBlackboardConditionalElement blankConditional = null;
            string managedReferenceTypeStr = conditionalProp.managedReferenceFullTypename;
            m_blankConditionalsByTypeString.TryGetValue(managedReferenceTypeStr, out blankConditional);
            if(blankConditional == null)
            {
                GetTypeFromManagedReferenceFullTypeName(managedReferenceTypeStr, out Type conditionalType);
                blankConditional = m_blankConditionalElements[conditionalType];
                m_blankConditionalsByTypeString.Add(managedReferenceTypeStr, blankConditional);
            }
            return blankConditional.GetOutportLabel(conditionalProp);
        }

        private static bool GetTypeFromManagedReferenceFullTypeName(string managedReferenceFullTypename, out Type managedReferenceInstanceType)
        {
            managedReferenceInstanceType = null;

            var parts = managedReferenceFullTypename.Split(' ');
            if (parts.Length == 2)
            {
                var assemblyPart = parts[0];
                var nsClassnamePart = parts[1];
                managedReferenceInstanceType = Type.GetType($"{nsClassnamePart}, {assemblyPart}");
            }

            return managedReferenceInstanceType != null;
        }

        private static void SetElementTypeDictionary()
        {
            m_blackboardConditionalElementTypes = new Dictionary<Type, Type>();
            m_blankConditionalElements = new Dictionary<Type, IBlackboardConditionalElement>();
            List<Type> conditionalElementTypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                conditionalElementTypes.AddRange(assemblies[i].GetTypes().Where(x => typeof(IBlackboardConditionalElement).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<BlackboardElementAttribute>() != null));
            }
            for(int i = 0; i < conditionalElementTypes.Count; i ++)
            {
                Type relatedElementType = conditionalElementTypes[i].GetCustomAttribute<BlackboardElementAttribute>().ElementType;
                if(!m_blackboardConditionalElementTypes.ContainsKey(relatedElementType))
                {
                    m_blackboardConditionalElementTypes.Add(relatedElementType, conditionalElementTypes[i]);
                    m_blankConditionalElements.Add(conditionalElementTypes[i], Activator.CreateInstance(conditionalElementTypes[i]) as IBlackboardConditionalElement);
                }
            }
        }
#endif
    }
}