using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters
{
    public interface IBaseConverter
    {
        System.Type ModelType { get; }
    }
    public interface IBaseConverter<T> : IBaseConverter
    {
        Result Serialize(T value, out Data data);
        Result Deserialize(Data data, out T obj);
    }
}
