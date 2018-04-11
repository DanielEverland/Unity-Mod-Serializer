using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UMS.MemberBlockers;

namespace UMS.Reflection
{
    public static class MemberBlockerLoader
    {
        [LoadTypes]
        private static void Poll(Type type)
        {
            foreach (FieldInfo field in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                Result result = MemberBlockerAttribute.IsValid(field);

                if (result.Succeeded)
                {
                    IEnumerable<string> fieldValue = (IEnumerable<string>)field.GetValue(null);

                    MemberBlockerAttribute.AddBlockers(fieldValue);
                }
            }
        }
    }
}
