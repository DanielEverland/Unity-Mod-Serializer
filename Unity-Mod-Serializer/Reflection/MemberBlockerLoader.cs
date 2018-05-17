using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UMS.Reflection
{
    public static class MemberBlockerLoader
    {
        [LoadTypes]
        private static void Poll(Type type)
        {
            foreach (FieldInfo field in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (MemberBlockerAttribute.IsValid(field))
                {
                    IEnumerable<string> fieldValue = (IEnumerable<string>)field.GetValue(null);

                    MemberBlockerAttribute.AddBlockers(fieldValue);
                }
            }
        }
    }
}
