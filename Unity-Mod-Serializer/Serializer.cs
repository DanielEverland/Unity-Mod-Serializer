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
            _cachedConverters = new Dictionary<System.Type, IBaseConverter>();
            _converters = new List<IBaseConverter>();
        }

        private static Dictionary<System.Type, IBaseConverter> _cachedConverters;
        private static Dictionary<System.Type, IDirectConverter> _directConverters;
        private static List<IBaseConverter> _converters;

        #region Public Deserialize Functions
        /// <summary>
        /// Deserializes a byte array into memory
        /// </summary>
        public static Result Deserialize<T>(byte[] array, ref T obj)
        {
            return InternalDeserialize(array.Deserialize<Data>(), ref obj);
        }
        /// <summary>
        /// Deserializes a byte array into memory
        /// </summary>
        public static Result Deserialize(byte[] array, System.Type objType, ref object obj)
        {
            return InternalDeserialize(array.Deserialize<Data>(), objType, ref obj);
        }
        /// <summary>
        /// Deserializes a Data object into memory
        /// </summary>
        public static Result Deserialize<T>(Data data, ref T obj)
        {
            return InternalDeserialize(data, ref obj);
        }
        /// <summary>
        /// Deserializes a Data object into memory
        /// </summary>
        public static Result Deserialize(Data data, System.Type objType, ref object obj)
        {
            return InternalDeserialize(data, objType, ref obj);
        }
        #endregion

        #region Public Serialize Functions
        /// <summary>
        /// Serializes an object into a byte array
        /// </summary>
        public static Result Serialize<T>(T value, out byte[] array)
        {
            return InternalSerialize(value, out array);
        }
        /// <summary>
        /// Serializes an object into a byte array
        /// </summary>
        public static Result Serialize(object value, out byte[] array)
        {
            return InternalSerialize(value, out array);
        }
        /// <summary>
        /// Serializes an object into a Data object
        /// </summary>
        public static Result Serialize<T>(T value, out Data data)
        {
            return InternalSerialize(value, out data);
        }
        /// <summary>
        /// Serializes an object into a Data object
        /// </summary>
        public static Result Serialize(object value, out Data data)
        {
            return InternalSerialize(value, out data);
        }
        #endregion

        #region Internal Serialize Functions
        internal static Result InternalSerialize(object value, out byte[] array)
        {
            Result result = InternalSerialize(value, out Data data);
            array = data.SerializeToBytes();

            return result;
        }
        internal static Result InternalSerialize(object value, out Data data)
        {
            Result result = Result.Success;

            System.Type objType = value.GetType();
            IBaseConverter converter = GetConverter(objType);

            result += converter.Serialize(value, out data);

            if (result.Succeeded)
            {
                Debugging.Info(DebuggingFlags.Serializer, string.Format("Serialized value {0} ({1}) as {2} using converter {3}", value, value.GetType(), data, converter));
            }
            else
            {
                Debugging.Error(DebuggingFlags.Serializer, string.Format("Failed serializing value {0} ({1}) using converter {2}", value, value.GetType(), converter));
            }

            return result;
        }
        #endregion

        #region Internal Deserialize Functions
        internal static Result InternalDeserialize<T>(Data data, ref T instance)
        {
            object boxedObject = instance;
            Result result = InternalDeserialize(data, typeof(T), ref boxedObject);

            instance = (T)boxedObject;
            return result;
        }
        internal static Result InternalDeserialize(Data data, System.Type type, ref object instance)
        {
            Result result = Result.Success;
            
            IBaseConverter converter = GetConverter(type);            

            //Create an instance if one doesn't exist
            if(instance == null)
            {
                instance = converter.CreateInstance(type);

                if(instance == null)
                {
                    throw new System.NullReferenceException(string.Format("Converter ({0}) CreateInstance using ({1}) returned null!", converter, type.Name));
                }
            }

            //Perform deserialization
            result += converter.Deserialize(data, ref instance);

            if (result.Succeeded)
            {
                Debugging.Info(DebuggingFlags.Serializer, string.Format("Deserialized data {0} ({1}) to {2} ({3}) using {4}", data, type.Name, instance, instance.GetType().Name, converter));
            }
            else
            {
                Debugging.Error(DebuggingFlags.Serializer, string.Format("Failed deserializing data {0} ({1}) using {2}", data, type.Name, converter));
            }

            return result;
        }
        #endregion

        #region Converters
        private static IBaseConverter GetConverter(System.Type type)
        {
            if (!AssemblyManager.HasInitialized)
                AssemblyManager.Initialize();

            //First we check if the type has been encountered before
            if (_cachedConverters.ContainsKey(type))
                return _cachedConverters[type];

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

            _cachedConverters.Add(type, converter);

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
        #endregion
    }
}
