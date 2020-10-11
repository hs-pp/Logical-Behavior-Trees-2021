using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Linq;
#endif

namespace GraphTheory.BuiltInNodes
{
    [Serializable]
    public class BlackboardConditional : ANode
    {
        [SerializeField]
        private string m_blackboardElementId = "";
        public string BlackboardElementId { get { return m_blackboardElementId; } }
        [SerializeReference]
        private List<IBlackboardConditionalElement> m_conditionals = new List<IBlackboardConditionalElement>();

#if UNITY_EDITOR
        public override bool UseIMGUIPropertyDrawer { get { return true; } }
        public override int DefaultNumOutports { get { return 0; } }

        public static string BlackboardElementIdVarName = "m_blackboardElementId";
        public static string ConditionalsVarName = "m_conditionals";

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

        private static void SetElementTypeDictionary()
        {
            m_blackboardConditionalElementTypes = new Dictionary<Type, Type>();
            List<Type> conditionalElementTypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                conditionalElementTypes.AddRange(assemblies[i].GetTypes().Where(x => typeof(IBlackboardConditionalElement).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<BlackboardElementTypeAttribute>() != null));
            }
            for(int i = 0; i < conditionalElementTypes.Count; i ++)
            {
                Type relatedElementType = conditionalElementTypes[i].GetCustomAttribute<BlackboardElementTypeAttribute>().ElementType;
                if(!m_blackboardConditionalElementTypes.ContainsKey(relatedElementType))
                {
                    m_blackboardConditionalElementTypes.Add(relatedElementType, conditionalElementTypes[i]);
                }
            }
        }
#endif
    }
}