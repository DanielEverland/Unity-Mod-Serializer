using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Reflection
{
    public static class ReferenceTypesDeclaration
    {
        [ReferenceTypes]
        private static List<System.Type> _referenceTypes = new List<System.Type>()
        {
            typeof(UnityEngine.Object),
        };
    }
}
