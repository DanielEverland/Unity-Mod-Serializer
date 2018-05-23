using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UMS
{
    public static class Extensions
    {
        ///<returns>Whether the type is serializable natively by Protobuf</returns>
        public static bool IsSerializable(this object obj)
        {
            if (obj == null)
                return true;

            return IsSerializable(obj.GetType());
        }
        ///<returns>Whether the type is serializable natively by Protobuf</returns>
        public static bool IsSerializable(this System.Type type)
        {
            return ProtoBuf.Meta.RuntimeTypeModel.Default.CanSerialize(type);
        }
        public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                queue.Enqueue(item);
            }
        }
        public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
            else
            {
                dictionary[key] = value;
            }
        }
        public static T Deserialize<T>(this byte[] array)
        {
            return InternalSerializer.Deserialize<T>(array);
        }
        public static byte[] SerializeToBytes<T>(this T obj)
        {
            return InternalSerializer.Serialize(obj);
        }
    }
}
