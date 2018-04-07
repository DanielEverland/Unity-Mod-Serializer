using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters
{
    public interface IBinaryConverter
    {
        Type ModelType { get; }
        BinarySerializer Serializer { get; set; }

        Result TrySerialize(object obj);
        Result TryDeserialize(byte[] data, out object obj);
    }

    /// <summary>
    /// Binary converters override regular converters, and allows you to serialize
    /// binary objects into a mod
    /// 
    /// IMPORTANT: Your converter will be ignored if the type you're trying to serialize
    /// doesn't have an entry writer
    /// </summary>
    public abstract class BinaryConverter<T> : IBinaryConverter
    {
        /// <summary>
        /// The type this converter can seriailize. Supports inheritance
        /// </summary>
        public Type ModelType { get { return typeof(T); } }

        public BinarySerializer Serializer { get; set; }

        public Result TrySerialize(object obj)
        {
            if (!ModelType.IsAssignableFrom(obj.GetType()))
                throw new ArgumentException("Type mismatch " + ModelType + " - " + obj.GetType());

            Result result = DoSerialize((T)obj, out byte[] data);

            if (result.Succeeded)
            {
                Serializer.AddObject(obj, data);
            }
            
            return result;
        }
        public Result TryDeserialize(byte[] data, out object obj)
        {
            return DoDeserialize(data, out obj);
        }

        public abstract Result DoSerialize(T obj, out byte[] data);
        public abstract Result DoDeserialize(byte[] data, out object obj);
    }
}
