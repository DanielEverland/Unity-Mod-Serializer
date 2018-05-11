using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
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
        private static BinaryFormatter _formatter = new BinaryFormatter();

        public static byte[] Serialize<T>(T obj)
        {
#if DEBUG
            EnsureIsSerializable(typeof(T));
#endif
            
            using (MemoryStream stream = new MemoryStream())
            {
                _formatter.Serialize(stream, obj);
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
                _formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
        public static object Deserialize(byte[] array)
        {
            using (MemoryStream stream = new MemoryStream(array))
            {
                return _formatter.Deserialize(stream);
            }
        }
        public static T Deserialize<T>(byte [] array)
        {
            using (MemoryStream stream = new MemoryStream(array))
            {
                return (T)_formatter.Deserialize(stream);
            }
        }
#if DEBUG
        private static void EnsureIsSerializable(Type type)
        {
            if (!type.GetCustomAttributes().Any(x => x.GetType() == typeof(SerializableAttribute)))
                throw new System.NotImplementedException("Cannot serialize " + type + ", as it does not implement SerializableAttribute");                
        }
#endif
    }
}
