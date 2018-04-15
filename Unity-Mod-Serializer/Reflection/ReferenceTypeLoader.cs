using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Reflection
{
    public static class ReferenceTypeLoader
    {
        private static readonly BindingFlags _bindingFlags = BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;

        [LoadTypes]
        public static void Poll(Type type)
        {
            foreach (MemberInfo member in type.GetMembers(_bindingFlags))
            {
                foreach (ReferenceTypesAttribute attribute in member.GetCustomAttributes<ReferenceTypesAttribute>())
                {
                    if (attribute.IsValid(member))
                    {
                        attribute.AddToManager(member);
                    }
                    else
                    {
                        Debugging.Warning(DebuggingFlags.Reflection, "Found invalid use of ReferenceTypesAttribute on " + member);
                    }
                }
            }
        }
    }
}
