using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Linq;
#endif

namespace Logical.BuiltInNodes
{
    public class BlackboardSetter : ANode
    {
        [SerializeField]
        private string m_blackboardElementId = "";
        public string BlackboardElementId { get { return m_blackboardElementId; } }

        [SerializeReference]
        private IBlackboardSetterElement m_setterValue = null;

        public override void OnNodeEnter(GraphControls graphControls)
        {
            base.OnNodeEnter(graphControls);

            if (string.IsNullOrEmpty(m_blackboardElementId))
            {
                Debug.LogError("BlackboardSetter: Blackboard element is not set!");
                return;
            }
            BlackboardElement element = graphControls.BlackboardProperties.GetElementById(m_blackboardElementId);
            m_setterValue.Evaluate(element);
            graphControls.TraverseEdge(0, this);
        }

#if UNITY_EDITOR
        public static readonly string BlackboardElementIdVarName = "m_blackboardElementId";
        public static readonly string SetterValueVarName = "m_setterValue";

        public override bool UseIMGUIPropertyDrawer { get { return true; } }
        public override int DefaultNumOutports { get { return 1; } }

        private static Dictionary<Type, Type> m_blackboardSetterElementTypes = null;
        public static Dictionary<Type, Type> BlackboardSetterElementTypes
        {
            get
            {
                if (m_blackboardSetterElementTypes == null)
                {
                    SetElementTypeDictionary();
                }
                return m_blackboardSetterElementTypes;
            }
        }

        private static Dictionary<Type, IBlackboardSetterElement> m_blankSetterElements = null;
        private static Dictionary<string, IBlackboardSetterElement> m_blankSettersByTypeString
            = new Dictionary<string, IBlackboardSetterElement>();

        public static string GetOutportLabel(SerializedProperty conditionalProp)
        {
            if (m_blankSetterElements == null)
            {
                SetElementTypeDictionary();
            }
            IBlackboardSetterElement blankSetter = null;
            string managedReferenceTypeStr = conditionalProp.managedReferenceFullTypename;
            m_blankSettersByTypeString.TryGetValue(managedReferenceTypeStr, out blankSetter);
            if (blankSetter == null)
            {
                GetTypeFromManagedReferenceFullTypeName(managedReferenceTypeStr, out Type conditionalType);
                if(conditionalType == null)
                {
                    return "";
                }

                m_blankSetterElements.TryGetValue(conditionalType, out blankSetter);
                if(blankSetter != null)
                {
                    m_blankSettersByTypeString.Add(managedReferenceTypeStr, blankSetter);
                }
            }
            return blankSetter != null ? blankSetter.GetOutportLabel(conditionalProp) : "";
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
            m_blackboardSetterElementTypes = new Dictionary<Type, Type>();
            m_blankSetterElements = new Dictionary<Type, IBlackboardSetterElement>();
            List<Type> setterElementTypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                setterElementTypes.AddRange(assemblies[i].GetTypes().Where(x => typeof(IBlackboardSetterElement).IsAssignableFrom(x)
                    && !x.IsAbstract
                    && x.GetCustomAttribute<BlackboardElementAttribute>() != null));
            }
            for (int i = 0; i < setterElementTypes.Count; i++)
            {
                Type relatedElementType = setterElementTypes[i].GetCustomAttribute<BlackboardElementAttribute>().ElementType;
                if (!m_blackboardSetterElementTypes.ContainsKey(relatedElementType))
                {
                    m_blackboardSetterElementTypes.Add(relatedElementType, setterElementTypes[i]);
                    m_blankSetterElements.Add(setterElementTypes[i], Activator.CreateInstance(setterElementTypes[i]) as IBlackboardSetterElement);
                }
            }
        }


#endif
    }
}