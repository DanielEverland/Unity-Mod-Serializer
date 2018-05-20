using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Wrappers
{
    /// <summary>
    /// ProtoContract primitive wrapping. The reason why we need this is outlined here: https://stackoverflow.com/a/39215892/3834696
    /// </summary>
    public static class WrapperManager
    {
        private static Dictionary<Type, Func<object, object>> _wrappedTypes = new Dictionary<Type, Func<object, object>>()
        {
            { typeof(bool), x => BoolWrapper.Create((bool)x) },
            { typeof(byte), x => ByteWrapper.Create((byte)x) },
        };
        
        public static object Process(object obj)
        {
            if (obj == null)
                return null;

            if (_wrappedTypes.ContainsKey(obj.GetType()))
            {
                return _wrappedTypes[obj.GetType()].Invoke(obj);
            }
            else
            {
                return obj;
            }
        }
    }
}
