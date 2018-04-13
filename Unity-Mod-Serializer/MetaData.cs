using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS
{
    public static class MetaData
    {
        public const string KEY_TYPE = "$type";

        public static Result AddType(Data data, System.Type type)
        {
            if (!data.IsDictioanry)
                return Result.Error("Type mismatch. Expected Dictionary");

            Result result = Result.Success;

            result += Serializer.Serialize(type, out Data typeData);
            data[KEY_TYPE] = typeData;

            return result;
        }
        public static Result GetType(Data data, out System.Type type)
        {
            return GetMetaData(data, KEY_TYPE, out type);
        }
        private static Result GetMetaData<T>(Data data, string key, out T obj)
        {
            obj = default(T);

            if (!data.IsDictioanry)
                return Result.Error("Type mismatch. Expected Dictionary");

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
