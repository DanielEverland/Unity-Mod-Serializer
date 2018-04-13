using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtoBuf;

namespace UMS
{
    public static class Extensions
    {
        public static T Deserialize<T>(this byte[] array)
        {
            using (MemoryStream stream = new MemoryStream(array))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }
        public static byte[] SerializeToBytes(this Data data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, data);
                return stream.ToArray();
            }
        }
    }
}
