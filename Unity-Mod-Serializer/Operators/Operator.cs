using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UMS.Reflection;
using UMS.Operators;
using UnityEngine;

namespace UMS
{
    public static class Operator
    {
        private static readonly BindingFlags _bindingFlags = BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;

        private static Dictionary<Type, BaseOperator> _operators = new Dictionary<Type, BaseOperator>();

        public static object Serialize(object obj)
        {
            _operators.TryGetValue(obj.GetType(), out BaseOperator op);

            if (op == null)
                throw new NotImplementedException("Cannot convert " + obj.GetType());

            return op.Serialize(obj);
        }
        [LoadTypes]
        private static void Poll(Type type)
        {
            if(typeof(BaseOperator).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                BaseOperator @operator = (BaseOperator)Activator.CreateInstance(type);

                _operators.Set(@operator.ModelType, @operator);
            }
        }
    }
}
