using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
#if DEBUG
using System.Reflection;
#endif

namespace UMS
{
    /// <summary>
    /// Handles all low-level serialization
    /// </summary>
    internal static class InternalSerializer
    {
        public static byte[] Serialize<T>(T obj)
        {
#if DEBUG
            EnsureIsSerializable(typeof(T));
#endif
            
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
        public static byte[] Serialize(object obj)
        {
#if DEBUG
            EnsureIsSerializable(obj.GetType());
#endif
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
        public static T Deserialize<T>(byte [] array)
        {
            using (MemoryStream stream = new MemoryStream(array))
            {
                return (T)ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }
#if DEBUG
        private static void EnsureIsSerializable(Type type)
        {
            if (type.GetCustomAttribute(typeof(ProtoContractAttribute)) == null)
                throw new System.NotImplementedException("Cannot serialize " + type + ", as it does not implement ProtoContract");                
        }
#endif
    }
}
