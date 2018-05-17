using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UMS
{
    public static class ProtobufWrapper
    {
        public static byte[] Serialize(object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, obj);

                return stream.ToArray();
            }
        }
        public static T Deserialize<T>(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return (T)ProtoBuf.Serializer.Deserialize(typeof(T), stream);
            }
        }
    }
}
