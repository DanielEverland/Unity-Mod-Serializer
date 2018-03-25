using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Reflection;

namespace UMS
{
    public static class MetaData
    {
        private static readonly Dictionary<MetaDataType, string> _keys = new Dictionary<MetaDataType, string>()
        {
            { MetaDataType.Ref, Serializer.ObjectReferenceKey },
            { MetaDataType.ID, Serializer.ObjectDefinitionKey },
            { MetaDataType.Type, Serializer.InstanceTypeKey },
            { MetaDataType.Version, Serializer.VersionKey }, 
            { MetaDataType.Content, Serializer.ContentKey },
        };

        public static bool ContainsMetaData(Data data, MetaDataType type)
        {
            if (!data.IsDictionary)
                return false;

            return data.AsDictionary.ContainsKey(_keys[type]);
        }
        public static bool ContainsMetaData(Data data, string key)
        {
            if (!data.IsDictionary)
                return false;
            
            return data.AsDictionary.ContainsKey(key);
        }
        public static string GetID(Data data)
        {
            if (!data.IsDictionary)
                throw new InvalidCastException();

            Data id = GetMetaData(data, MetaDataType.Ref);

            if (id.IsString == false)
            {
                throw new InvalidCastException();
            }

            return id.AsString;
        }
        public static Type GetMetaDataType(Data data)
        {
            if (!data.IsDictionary)
                throw new InvalidCastException();

            Data typeData = GetMetaData(data, MetaDataType.Type);

            if (typeData.IsString == false)
            {
                throw new InvalidCastException();
            }

            string typeName = typeData.AsString;

            return TypeCache.GetType(typeName);
        }
        public static Data GetMetaData(Data data, MetaDataType type)
        {
            if (!data.IsDictionary)
                throw new InvalidCastException();

            Dictionary<string, Data> dictionary = data.AsDictionary;

            if (!dictionary.ContainsKey(_keys[type]))
                throw new NullReferenceException("Data doesn't contain metadata " + _keys[type] + "\n" + data.ToString());

            return data.AsDictionary[_keys[type]];
        }
    }
    public enum MetaDataType
    {
        None = 0,

        Ref = 1,
        ID = 2,
        Type = 3,
        Version = 4,
        Content = 5,
    }
}