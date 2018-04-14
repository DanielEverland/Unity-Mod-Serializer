using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UMS.Reflection;

namespace UMS.Converters
{
    public class ComponentConverter : BaseConverter<Component>
    {
        public override Result DoSerialize(Component obj, out Data data)
        {
            return ReflectionHelper.SerializeObject(obj, out data);
        }
        public override Result DoDeserialize(Data data, ref Component obj)
        {
            Result result = Result.Success;

            object boxedObject = obj;
            result += ReflectionHelper.DeserializeObject(data, ref boxedObject);
            obj = (Component)boxedObject;

            return result;
        }
    }
}
