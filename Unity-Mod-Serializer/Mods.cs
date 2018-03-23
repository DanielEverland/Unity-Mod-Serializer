using UnityEngine;
using System.IO;
using UMS;
using UMS.Zip;
using Ionic.Zip;

/// <summary>
/// Front-end for calling UMS functions
/// </summary>
public static class Mods
{
    public static Serializer Serializer { get { return _serializer; } }

    private static Serializer _serializer = new Serializer();
    
    public static void EmptySerializationQueue()
    {
        while (Manifest.Instance.SerializationQueue.Count > 0)
        {
            object obj = Manifest.Instance.SerializationQueue.Dequeue();
            Serialize(obj);
        }
    }
    public static void CreateNewSession()
    {
        ObjectContainer.Initialize();
    }
    public static T GetObject<T>(string key)
    {
        return (T)ObjectContainer.Instance.GetFromKey(key).obj;
    }
    public static void Add(string json, System.Type type, string id, string key)
    {
        ObjectContainer.Instance.Add(json, type, id, key);
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

            foreach (Manifest.Entry entry in manifest.Entries)
            {
                string json = file[entry.path].ZipToJson();

                Add(json, entry.type, entry.id, entry.key);
            }
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
        Manifest.Instance.CurrentlySerializingObject = obj;
        _serializer.TrySerialize(obj.GetType(), obj, out Data data);

        return JsonPrinter.PrettyJson(data);
    }
    internal static void Create(object obj, string fullPath)
    {
        ZipSerializer.Create(obj, fullPath);

        Debug.Log("Serialized " + obj + " to " + fullPath);
    }
    internal static void Create(object obj, System.Type type, string fullPath)
    {
        Manifest.Instance.CurrentlySerializingObject = obj;

#if DEBUG
        _serializer.TrySerialize(type, obj, out Data data).AssertSuccessWithoutWarnings();
#else
        _serializer.TrySerialize(type, obj, out Data data);
#endif

        string json = JsonPrinter.PrettyJson(data);

        ZipSerializer.Create(json, fullPath, obj.ToString());

        Debug.Log("Serialized " + type + " to " + fullPath);
    }
    internal static object Deserialize(System.Type type, string fullPath)
    {
        string json = File.ReadAllText(fullPath);

        Data data = JsonParser.Parse(json);

        object deserialized = null;
#if DEBUG
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
#else
        _serializer.TryDeserialize(data, type, ref deserialized);
#endif

        Debug.Log("Deserialized " + type + " from " + fullPath);

        return deserialized;
    }
    public static T DeserializeString<T>(string content)
    {
        return (T)DeserializeString(content, typeof(T));
    }
    public static object DeserializeString(string content, System.Type type)
    {
        Data data = JsonParser.Parse(content);

        object deserialized = null;
#if DEBUG
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
#else
        _serializer.TryDeserialize(data, type, ref deserialized);
#endif

        return deserialized;
    }
}