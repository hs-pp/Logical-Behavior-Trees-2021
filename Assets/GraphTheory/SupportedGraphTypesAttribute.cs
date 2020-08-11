using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphTheory
{
    public class SupportedGraphTypesAttribute : Attribute
    {
        public List<Type> SupportedTypes { get; private set; }
        public SupportedGraphTypesAttribute(params Type[] supportedTypes)
        {
            SupportedTypes = supportedTypes.ToList();
        }
    }
}