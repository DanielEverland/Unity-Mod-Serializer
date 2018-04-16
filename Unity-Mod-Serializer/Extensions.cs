using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UMS
{
    public static class Extensions
    {
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
