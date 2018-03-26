using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using UMS.Zip;

namespace UMS
{
    public static class Extensions
    {
        public static string ToCamelCase(this string value)
        {
            if (value.Length == 1)
                return Char.ToLowerInvariant(value[0]).ToString();

            return (Char.ToLowerInvariant(value[0]) + value.Substring(1)).Replace("_", string.Empty);
        }
        public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
        public static T ToObject<T>(this ZipEntry entry)
        {
            return (T)ToObject(entry, typeof(T));
        }
        public static object ToObject(this ZipEntry entry, System.Type type)
        {
            return Mods.DeserializeString(ZipToJson(entry), type);
        }        
        public static string ZipToJson(this ZipEntry entry)
        {
            string toReturn = "";

            using (MemoryStream stream = new MemoryStream())
            {
                entry.Extract(stream);
                stream.Position = 0;

                StreamReader reader = new StreamReader(stream);
                toReturn = reader.ReadToEnd();
            }

            return toReturn;
        }
        public static string ToJson(this object obj)
        {
            return Mods.Serialize(obj);
        }
    }
}
