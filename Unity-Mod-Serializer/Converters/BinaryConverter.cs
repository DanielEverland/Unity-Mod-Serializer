using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters
{
    /// <summary>
    /// Binary converters override regular converters, and allows you to serialize
    /// binary objects into a mod
    /// </summary>
    public abstract class BinaryConverter<T>
    {
        /// <summary>
        /// The type this converter can seriailize. Supports inheritance
        /// </summary>
        public Type ModelType { get { return typeof(T); } }

        public BinarySerializer Serializer;

        public Result TrySerialize(object obj)
        {
            if (!ModelType.IsAssignableFrom(obj.GetType()))
                throw new ArgumentException("Type mismatch " + ModelType + " - " + obj.GetType());

            Result result = DoSerialize((T)obj, out byte[] data);

            if (result.Succeeded)
            {
                Serializer.AddObject(ModelType, data);
            }
            
            return result;
        }

        public abstract Result DoSerialize(T obj, out byte[] data);
        //public abstract Result DoDeserialize()
    }
}
