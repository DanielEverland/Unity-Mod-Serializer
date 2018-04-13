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
            Result result = Result.Success;

            result += ReflectionHelper.SerializeObject(obj, out data);

            return result;
        }
        public override Result DoDeserialize(Data data, ref object obj)
        {
            Result result = Result.Success;

            result += ReflectionHelper.DeserializeObject(data, ref obj);

            return result;
        }
    }
}
