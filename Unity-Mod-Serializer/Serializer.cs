using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMS.Converters;
using UMS.Reflection;

namespace UMS
{
    public class Serializer
    {
        public Serializer()
        {
            _directConverters = new Dictionary<System.Type, IDirectConverter>();
            _cachedConverters = new Dictionary<System.Type, IBaseConverter>();
            _converters = new List<IBaseConverter>();

            _serializationQueue = new SerializationQueue<object>();
        }
        public static void Initialize()
        {
            UnityEngine.Debug.Log("Initializing Serializer");

            _instance = new Serializer();            
            AssemblyManager.Initialize();
            ObjectHandler.Initialize();
        }

        public static SerializationQueue<object> SerializationQueue { get { return _instance._serializationQueue; } }

        private static Dictionary<System.Type, IBaseConverter> CachedConverters { get { return _instance._cachedConverters; } }
        private static Dictionary<System.Type, IDirectConverter> DirectConverters { get { return _instance._directConverters; } }
        private static List<IBaseConverter> Converters { get { return _instance._converters; } }

        private static Serializer _instance;

        private SerializationQueue<object> _serializationQueue;

        private Dictionary<System.Type, IBaseConverter> _cachedConverters;
        private Dictionary<System.Type, IDirectConverter> _directConverters;
        private List<IBaseConverter> _converters;

        #region Public Deserialize Functions
        /// <summary>
        /// Deserializes a modfile and adds all its entries to ObjectHandler
        /// </summary>
        public static Result Deserialize(ModFile file)
        {
            Result result = Result.Success;

            ObjectHandler.RegisterData(file);

            foreach (string id in file.IDs)
            {
                if (file.ShouldDeserialize(id))
                {
                    ModFile.Entry entry = file[id];

                    UnityEngine.Object obj = null;
                    result += Deserialize(entry.Data, ref obj);

                    ObjectHandler.AddObject(obj, entry.Key);
                }
            }

            return result;
        }
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
        public static Result Deserialize<T>(Data data, System.Type objType, ref T obj)
        {
            Result result = Result.Success;

            object boxedObject = obj;
            result += InternalDeserialize(data, objType, ref boxedObject);
            obj = (T)boxedObject;

            return result;
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

            //We don't want to try and serialize null values
            if (value == null)
            {
                data = Data.Null;
                return Result.Error("Value is null!");
            }                

            Debugging.Verbose(DebuggingFlags.Serializer, string.Format("Trying to serialize {0} ({1})", value, value.GetType()));

            //Grab the type of our given value, and grab a converter that can serialize it.
            System.Type objType = value.GetType();
            IBaseConverter converter = GetConverter(objType);

            Debugging.Verbose(DebuggingFlags.Serializer, string.Format("Selected converter {0} ({1})", converter, converter.GetType()));

            //Serialize the objects members
            if(value == SerializationQueue.ActiveObject || !ReferenceManager.SupportsReferencing(objType))
            {
                //Perform serialization
                result += converter.Serialize(value, out data);

                //Add type metadata
                if (IDManager.CanGetID(objType) && data.IsDictionary)
                    result += MetaData.AddType(data, objType);
            }
            else //Serialize the object as a reference
            {
                data = new Data(new Dictionary<string, Data>());
                string id = IDManager.GetID(value);

                result += MetaData.AddReference(data, id);

                if (!SerializationQueue.HasBeenEnqueued(id) && ReferenceManager.SupportsReferencing(objType))
                    SerializationQueue.Enqueue(value);
            }            
            
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
            if (data == Data.Null)
                return Result.Error("Tried to deserialize data that is null", data);

            Result result = Result.Success;
            
            IBaseConverter converter = GetConverter(type);
            
            //Create an instance if one doesn't exist
            if(instance == null)
            {
                try
                {
                    instance = converter.CreateInstance(type);
                }
                catch (System.Exception e)
                {
                    return Result.Exception(e);
                }                

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
            //First we check if the type has been encountered before
            if (CachedConverters.ContainsKey(type))
                return CachedConverters[type];

            IBaseConverter converter = null;
            
            if (DirectConverters.ContainsKey(type))
            {
                converter = DirectConverters[type];
            }
            else
            {
                converter = TypeInheritanceTree.GetClosestType(Converters, type, x => x.ModelType);
            }

            if(converter == null)
            {
                throw new System.NotImplementedException("Couldn't find converter for " + type);
            }

            CachedConverters.Add(type, converter);

            return converter;
        }
        public static void AddConverter(IBaseConverter converter)
        {
            if(converter is IDirectConverter directConverter)
            {
                DirectConverters.Add(directConverter.ModelType, directConverter);
            }
            else
            {
                Converters.Add(converter);
            }
        }
        #endregion
    }
}
