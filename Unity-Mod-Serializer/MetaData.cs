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
        public static Type GetMetaDataType(Data data)
        {
            if (!data.IsDictionary)
                throw new InvalidCastException();

            Data typeData = data.AsDictionary[Serializer.InstanceTypeKey];
            
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