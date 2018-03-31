using System;
using System.Reflection;

namespace UMS.Reflection
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LoadTypesAttribute : Attribute
    {
        public static Result IsValid(MethodInfo method)
        {
            if (!method.IsStatic)
                return Result.Fail(method + " isn't static");

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length != 1)
                return Result.Fail(method + " doesn't contain 1 parameter, it contains " + parameters.Length);

            if (parameters[0].ParameterType != typeof(Type))
                return Result.Fail(method + " does contain 1 parameter, but it is not of type System.Type. Instead, it is of: " + parameters[0].ParameterType);

            if (method.ReturnType != typeof(void))
                return Result.Fail(method + " return type is not void, it is: " + method.ReturnType);

            return Result.Success;
        }
    }
}
