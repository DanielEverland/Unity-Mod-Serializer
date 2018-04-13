using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UMS
{
    public static class Extensions
    {
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
