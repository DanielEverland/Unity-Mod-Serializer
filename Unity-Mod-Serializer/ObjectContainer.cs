using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS
{
    /// <summary>
    /// Handles data that shouldn't be serialized
    /// </summary>
    public static class ObjectContainer
    {
        static ObjectContainer()
        {
            Initialize();
        }

        public static IEnumerable<object> Objects { get { return _idToObjects.Values; } }
        public static IEnumerable<Data> Data { get { return _idToData.Values; } }
        public static IEnumerable<string> IDs { get { return _idToData.Keys.Union(_idToObjects.Keys); } }
        public static IEnumerable<string> Keys { get { return _keyToObjects.Keys; } }

        /// <summary>
        /// Index for getting data from an ID
        /// </summary>
        private static Dictionary<string, Data> _idToData;
        
        /// <summary>
        /// Index for getting deserialized objects from a key
        /// </summary>
        private static Dictionary<string, object> _keyToObjects;

        /// <summary>
        /// Index for getting deserialized objects from an index
        /// </summary>
        private static Dictionary<string, object> _idToObjects;

        private static Transform GameObjectContainer
        {
            get
            {
                if (Application.isEditor && !Application.isPlaying)
                    return null;

                if (_containerObject == null)
                    _containerObject = new GameObject("Mods Container");

                return _containerObject.transform;
            }
        }
        private static GameObject _containerObject;

        public static void Initialize()
        {
            _idToData = new Dictionary<string, Data>();
            _keyToObjects = new Dictionary<string, object>();
            _idToObjects = new Dictionary<string, object>();
        }
        public static bool ContainsKey(string key)
        {
            return _keyToObjects.ContainsKey(key);
        }
        public static bool ContainsData(string id)
        {
            return _idToData.ContainsKey(id);
        }
        public static bool ContainsObject(string index, IndexType type)
        {
            switch (type)
            {
                case IndexType.ID:
                    return _idToObjects.ContainsKey(index);
                case IndexType.Key:
                    return _keyToObjects.ContainsKey(index);
                default:
                    throw new System.NotImplementedException();
            }
        }
        public static void SetObject(string id, object obj)
        {
            _idToObjects.Set(id, obj);
        }
        public static void SetObject(string id, string key, object obj)
        {
            _idToObjects.Set(id, obj);
            
            if(key != null && key != "")
                _keyToObjects.Set(key, obj);
        }
        public static object GetObjectFromID(string id)
        {
            if (!_idToObjects.ContainsKey(id))
            {
                if (_idToData.ContainsKey(id))
                {
                    Data data = GetData(id);
                    System.Type type = MetaData.GetMetaDataType(data);

                    object obj = Mods.DeserializeData(data, type);

                    SetObject(id, obj);
                }
                else
                {
                    throw new System.NullReferenceException("No object using id " + id + " is in memory");
                }
            }                

            return _idToObjects[id];
        }
        public static object GetObjectFromKey(string key)
        {
            if (!_keyToObjects.ContainsKey(key))
                throw new System.NullReferenceException("No object using key " + key + " is in memory");

            return _keyToObjects[key];
        }
        public static Data GetData(string id)
        {
            if (!_idToData.ContainsKey(id))
                throw new System.NullReferenceException("No deserialized data from ID " + id);

            return _idToData[id];
        }
        public static void CreateObjectInstance(string id, IEnumerable<string> keys)
        {
            if (id == null)
                throw new System.ArgumentException("ID is null " + id);

            Data data = GetData(id);

            object deserialized = Mods.DeserializeData(data, MetaData.GetMetaDataType(data));

            if(deserialized is GameObject gameObject)
            {
                gameObject.SetActive(false);
                gameObject.transform.SetParent(GameObjectContainer);
            }
            
            _idToObjects.Set(id, deserialized);
            
            if(keys != null)
            {
                foreach (string key in keys)
                {
                    if (key != null)
                        _keyToObjects.Set(key, deserialized);
                }
            }                     
        }
        public static void AddData(string id, Data data)
        {
            _idToData.Set(id, data);           
        }
        public enum IndexType
        {
            None = 0,

            ID = 1,
            Key = 2,
        }
    }
}
