using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMS.Reflection;

namespace UMS.Converters
{
    /// <summary>
    /// Slow converter used as a last resort, of none else are found
    /// </summary>
    public class RootConverter : BaseConverter<object>
    {
        public override Result DoSerialize(object obj, out Data data)
        {
            UnityEngine.Debug.LogWarning(string.Format("Serializing {0} using reflection!", obj.GetType().Name));

            Result result = Result.Success;

            result += ReflectionHelper.SerializeObject(obj, out data);
            result += MetaData.AddType(data, obj.GetType());

            return result;
        }
        public override Result DoDeserialize(Data data, ref object obj)
        {
            Result result = Result.Success;

            System.Type objType = null;
            
            result += MetaData.GetType(data, out objType);
            result += ReflectionHelper.DeserializeObject(data, objType, ref obj);

            return result;
        }
    }
}
