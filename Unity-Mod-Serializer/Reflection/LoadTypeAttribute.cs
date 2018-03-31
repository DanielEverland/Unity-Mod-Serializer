using System;
using System.Reflection;

namespace UMS.Reflection
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LoadTypeAttribute : Attribute
    {
        public static bool IsValid(MethodInfo method)
        {
            if (!method.IsStatic)
                return false;

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length != 1)
                return false;

            if (parameters[0].ParameterType != typeof(Type))
                return false;

            if (method.ReturnType != null)
                return false;

            return true;
        }
    }
}
