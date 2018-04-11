using System;
using System.Collections;
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
        private const int MAX_OUTPUT_LENGTH = 100;
        
        public static int[] GetLengths(this Array array)
        {
            int[] lengths = new int[array.Rank];

            for (int d = 0; d < array.Rank; d++)
            {
                lengths[d] = array.GetLength(d);
            }

            return lengths;
        }
        public static string CollectionToString(this ICollection collection)
        {
            StringWriter writer = new StringWriter();

            foreach (object obj in collection)
            {
                writer.Write(obj.ToString());
            }

            return writer.ToString();
        }
        public static void Output(this ICollection collection)
        {
            UnityEngine.Debug.Log("Outputting collection " + collection + "(" + collection.Count + ")");

            int i = 0;

            foreach (object obj in collection)
            {
                UnityEngine.Debug.Log(obj);

                i++;

                if(i >= MAX_OUTPUT_LENGTH)
                {
                    UnityEngine.Debug.LogWarning("Stopping output of collection. Too long");
                }
            }
        }
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
            return Mods.DeserializeString(ZipToText(entry), type);
        }
        public static byte[] ZipToByteArray(this ZipEntry entry)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                entry.Extract(stream);

                return stream.ToArray();
            }
        }
        public static string ZipToText(this ZipEntry entry)
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
