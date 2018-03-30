using UnityEngine;
using System.IO;
using System.Linq;
using UMS;
using UMS.Zip;
using Ionic.Zip;

namespace UMS
{
    /// <summary>
    /// Front-end for calling UMS functions
    /// </summary>
    public static class Mods
    {
        static Mods()
        {
            Serializer = new Serializer();
        }

        public static bool SessionInitiated { get; private set; } = false;
        public static Serializer Serializer { get; private set; }

        public static void CreateNewSession()
        {
            ObjectContainer.Initialize();
            Serializer = new Serializer();

            SessionInitiated = true;
        }

        /// <summary>
        /// Return a loaded object from a key
        /// </summary>
        public static T GetObject<T>(string key)
        {
            return (T)ObjectContainer.GetObjectFromKey(key);
        }

        /// <summary>
        /// Loads .mod file into memory
        /// </summary>
        /// <param name="fullPath">Full path of the .mod file</param>
        public static void Load(string fullPath)
        {
            using (ZipFile file = ZipFile.Read(fullPath))
            {
                if (!file.ContainsEntry(Utility.MANIFEST_NAME))
                    throw new System.NullReferenceException();

                Manifest manifest = file[Utility.MANIFEST_NAME].ToObject<Manifest>();

                //Deserialize all the data into memory, but don't try to create objects yet
                foreach (Manifest.Entry entry in manifest.Entries)
                {
                    string json = file[entry.path].ZipToJson();
                    Data data = JsonParser.Parse(json);

                    ObjectContainer.AddData(entry.id, data);
                }

                //Create instances
                foreach (Manifest.Entry entry in manifest.Entries)
                {
                    ObjectContainer.CreateObjectInstance(entry.id, entry.keys);
                }

                Debug.Log("Deserialized " + fullPath);
            }
        }

        /// <summary>
        /// Saves an object
        /// </summary>
        /// <typeparam name="T">Type to save</typeparam>
        /// <param name="obj">Object to save</param>
        /// <param name="fullPath">Full path with filename and extension</param>
        public static void Save<T>(T obj, string fullPath)
        {
            Create(obj, fullPath);
        }

        /// <summary>
        /// Saves an object
        /// </summary>
        /// <param name="obj">Object to save</param>
        /// <param name="fullPath">Full path with filename and extension</param>
        public static void Save(object obj, string fullPath)
        {
            Save(obj, obj.GetType(), fullPath);
        }

        /// <summary>
        /// Saves an object
        /// </summary>
        /// <param name="obj">Object to save</param>
        /// <param name="fullPath">Full path with filename and extension</param>
        /// <param name="type">Targeted storage type</param>
        public static void Save(object obj, System.Type type, string fullPath)
        {
            Create(obj, type, fullPath);
        }

        /// <summary>
        /// Serializes an object to a string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Json string</returns>
        public static string Serialize(object obj)
        {
            Serializer.ActiveObject = obj;
            Serializer.TrySerialize(obj.GetType(), obj, out Data data);

            return JsonPrinter.PrettyJson(data);
        }
        public static void Create(object obj, string fullPath)
        {
            ZipSerializer.Create(obj, fullPath);

            Debug.Log("Serialized " + obj + " to " + fullPath);
        }
        public static void Create(object obj, System.Type type, string fullPath)
        {
            Serializer.ActiveObject = obj;

#if DEBUG
            Serializer.TrySerialize(type, obj, out Data data).AssertSuccessWithoutWarnings();
#else
            Serializer.TrySerialize(type, obj, out Data data);
#endif

            string json = JsonPrinter.PrettyJson(data);

            ZipSerializer.Create(json, fullPath, obj.ToString());

            Debug.Log("Serialized " + type + " to " + fullPath);
        }

        public static object Deserialize(System.Type type, string fullPath)
        {
            string json = File.ReadAllText(fullPath);

            object deserialized = DeserializeString(json, type);

            Debug.Log("Deserialized " + type + " from " + fullPath);

            return deserialized;
        }
        public static T DeserializeString<T>(string content)
        {
            return (T)DeserializeString(content, typeof(T));
        }
        public static object DeserializeString(string content, System.Type type)
        {
            return DeserializeData(JsonParser.Parse(content), type);
        }
        public static object DeserializeData(Data data, System.Type type)
        {
            object deserialized = null;
#if DEBUG
            Serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
#else
            Serializer.TryDeserialize(data, type, ref deserialized);
#endif

            return deserialized;
        }
    }
}