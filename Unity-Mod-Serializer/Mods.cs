using UnityEngine;
using System.IO;
using UMS;
using UMS.Zip;

/// <summary>
/// Front-end for calling UMS functions
/// </summary>
public static class Mods
{
    private static Serializer _serializer = new Serializer();

    /// <summary>
    /// Loads an object
    /// </summary>
    /// <param name="T">The expected type to load</param>
    /// <param name="fullPath">Full path with filename and extension</param>
    /// <returns></returns>
    public static T Load<T>(string fullPath)
    {
        return (T)Deserialize(typeof(T), fullPath);
    }

    /// <summary>
    /// Loads an object
    /// </summary>
    /// <param name="type">The expected type to load</param>
    /// <param name="fullPath">Full path with filename and extension</param>
    /// <returns></returns>
    public static object Load(System.Type type, string fullPath)
    {
        return Deserialize(type, fullPath);
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
        _serializer.TrySerialize(obj.GetType(), obj, out Data data);

        return JsonPrinter.PrettyJson(data);
    }

#if DEBUG
    internal static void Create(object obj, string fullPath)
    {
        ZipSerializer.Create(obj, fullPath);

        Debug.Log("Serialized " + obj + " to " + fullPath);
    }
    internal static void Create(object obj, System.Type type, string fullPath)
    {
        _serializer.TrySerialize(type, obj, out Data data).AssertSuccessWithoutWarnings();

        string json = JsonPrinter.PrettyJson(data);

        ZipSerializer.Create(json, fullPath, obj.ToString());

        Debug.Log("Serialized " + type + " to " + fullPath);
    }
    internal static object Deserialize(System.Type type, string fullPath)
    {
        string json = File.ReadAllText(fullPath);

        Data data = JsonParser.Parse(json);

        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        Debug.Log("Deserialized " + type + " from " + fullPath);

        return deserialized;
    }
#endif
#if RELEASE
    internal static void Serialize(object obj, System.Type type, string fullPath)
    {
        _serializer.TrySerialize(type, obj, out Data data);

        string json = JsonPrinter.PrettyJson(data);

        File.WriteAllText(fullPath, json);
    }
    internal static object Deserialize(System.Type type, string fullPath)
    {
        string json = File.ReadAllText(fullPath);

        Data data = JsonParser.Parse(json);

        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized);

        return deserialized;
    }
#endif
}
