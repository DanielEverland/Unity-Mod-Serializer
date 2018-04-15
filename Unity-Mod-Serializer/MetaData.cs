using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS
{
    public static class MetaData
    {
        public const string CHARACTER = "$";
        public const string KEY_TYPE = CHARACTER + "type";
        public const string KEY_REFERENCE = CHARACTER + "ref";

        public static Result AddReference(Data data, string ID)
        {
            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected Dictionary");

            Result result = Result.Success;
            
            data[KEY_REFERENCE] = new Data(ID);

            return result;
        }
        public static Result AddType(Data data, System.Type type)
        {
            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected Dictionary");

            Result result = Result.Success;

            result += Serializer.Serialize(type, out Data typeData);
            data[KEY_TYPE] = typeData;

            return result;
        }
        public static bool HasType(Data data)
        {
            if (data.IsDictionary)
            {
                return data.AsDictionary.ContainsKey(KEY_TYPE);
            }

            return false;
        }
        public static Result GetType(Data data, out System.Type type)
        {
            return GetMetaData(data, KEY_TYPE, out type);
        }
        private static Result GetMetaData<T>(Data data, string key, out T obj)
        {
            obj = default(T);

            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected Dictionary", data);

            Result result = Result.Success;

            Dictionary<string, Data> dictionary = data.AsDictionary;

            if (!dictionary.ContainsKey(key))
                return Result.Error("No metadata for " + key);

            object deserializedObject = null;
            result += Serializer.Deserialize(data[key], typeof(T), ref deserializedObject);

            obj = (T)deserializedObject;

            return result;
        }
    }
}
