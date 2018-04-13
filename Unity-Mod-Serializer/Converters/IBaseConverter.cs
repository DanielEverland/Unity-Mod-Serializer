using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters
{
    public interface IBaseConverter
    {
        System.Type ModelType { get; }

        object CreateInstance(System.Type type);
        Result Serialize(object obj, out Data data);
        Result Deserialize(Data data, ref object obj);
    }
}
