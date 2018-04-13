using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMS.Converters;
using UMS.Reflection;

namespace UMS
{
    public static class Serializer
    {
        static Serializer()
        {
            _directConverters = new Dictionary<System.Type, IDirectConverter>();
            _converters = new List<IBaseConverter>();
        }

        private static Dictionary<System.Type, IDirectConverter> _directConverters;
        private static List<IBaseConverter> _converters;

        /// <summary>
        /// Deserializes a byte array into memory
        /// </summary>
        public static Result Deserialize<T>(byte[] array, ref T obj)
        {
            Result result = Result.Success;

            System.Type objType = typeof(T);
            IBaseConverter converter = GetConverter(objType);

            object boxedObject = null;
            result += converter.Deserialize(array.Deserialize<Data>(), ref boxedObject);

            obj = (T)boxedObject;

            return result;
        }
        /// <summary>
        /// Deserializes a byte array into memory
        /// </summary>
        public static Result Deserialize(byte[] array, System.Type objType, ref object obj)
        {
            Result result = Result.Success;

            Data data = array.Deserialize<Data>();
            Deserialize(data, objType, ref obj);

            return result;
        }
        /// <summary>
        /// Deserializes a Data object into memory
        /// </summary>
        public static Result Deserialize<T>(Data data, ref T obj)
        {
            Result result = Result.Success;

            System.Type objType = typeof(T);
            IBaseConverter converter = GetConverter(objType);

            object boxedObject = null;
            result += converter.Deserialize(data, ref boxedObject);

            obj = (T)boxedObject;

            return result;
        }
        /// <summary>
        /// Deserializes a Data object into memory
        /// </summary>
        public static Result Deserialize(Data data, System.Type objType, ref object obj)
        {
            Result result = Result.Success;
            
            IBaseConverter converter = GetConverter(objType);
            result += converter.Deserialize(data, ref obj);

            return result;
        }
        /// <summary>
        /// Serializes an object into a byte array
        /// </summary>
        public static Result Serialize(object value, out byte[] array)
        {
            Result result = Result.Success;

            result += Serialize(value, out Data data);
            array = data.SerializeToBytes();

            return result;
        }
        /// <summary>
        /// Serializes an object into a Data object
        /// </summary>
        public static Result Serialize(object value, out Data data)
        {
            Result result = Result.Success;

            System.Type objType = value.GetType();
            IBaseConverter converter = GetConverter(objType);

            result += converter.Serialize(value, out data);

            return result;
        }
        private static IBaseConverter GetConverter(System.Type type)
        {
            if (!AssemblyManager.HasInitialized)
                AssemblyManager.Initialize();

            IBaseConverter converter = null;

            if (_directConverters.ContainsKey(type))
            {
                converter = _directConverters[type];
            }
            else
            {
                converter = TypeInheritanceTree.GetClosestType(_converters, type, x => x.ModelType);
            }

            if(converter == null)
            {
                throw new System.NotImplementedException("Couldn't find converter for " + type);
            }

            return converter;
        }
        public static void AddConverter(IBaseConverter converter)
        {
            if(converter is IDirectConverter directConverter)
            {
                _directConverters.Add(directConverter.ModelType, directConverter);
            }
            else
            {
                _converters.Add(converter);
            }
        }
    }
}
