using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS
{
    /// <summary>
    /// Handles objects using their ID's during deserialization
    /// </summary>
    public static class ObjectHandler
    {
        public static IEnumerable<Data> AllData { get { return _allData.Values; } }

        private static Dictionary<string, Data> _allData;
        private static List<Object> _allObjects;
        private static Dictionary<string, Object> _keyLookup;

        public static void Initialize()
        {
            _allData = new Dictionary<string, Data>();
            _allObjects = new List<Object>();
            _keyLookup = new Dictionary<string, Object>();
        }
        public static T GetObject<T>(string key) where T : Object
        {
            if (!_keyLookup.ContainsKey(key))
                throw new System.NullReferenceException("No object has been loaded with the key " + key);

            return (T)_keyLookup[key];
        }
        /// <summary>
        /// Returns all objects which is of the exact type <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T> GetObjectsOfTypeExact<T>() where T : Object
        {
            return (IEnumerable<T>)_allObjects.Where(x => x.GetType() == typeof(T));
        }
        /// <summary>
        /// Returns all objects which can be cast to <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T> GetObjectsOfType<T> () where T : Object
        {
            return (IEnumerable<T>)_allObjects.Where(x => typeof(T).IsAssignableFrom(x.GetType()));
        }
        /// <summary>
        /// Returns the first object which is of the exact type <typeparamref name="T"/>
        /// </summary>
        public static T GetObjectOfTypeExact<T>() where T : Object
        {
            T obj = (T)_allObjects.FirstOrDefault(x => x.GetType() == typeof(T));

            if (obj == null)
                throw new System.ArgumentException("Couldn't find any instance of " + typeof(T));

            return obj;
        }
        /// <summary>
        /// Returns the first object which can be cast to <typeparamref name="T"/>
        /// </summary>
        public static T GetObjectOfType<T>() where T : Object
        {
            T obj = (T)_allObjects.FirstOrDefault(x => typeof(T).IsAssignableFrom(x.GetType()));

            if (obj == null)
                throw new System.ArgumentException("Couldn't find any instance of " + typeof(T));

            return obj;
        }        
        public static void AddObject(Object obj, string key)
        {
            if(key != string.Empty && key != null)
            {
                _keyLookup.Set(key, obj);
            }

            _allObjects.Add(obj);
        }
        public static void RegisterData(ModFile file)
        {
            foreach (string id in file.IDs)
            {
                ModFile.Entry entry = file[id];

                RegisterData(id, entry.Data);
            }
        }
        public static void RegisterData(string id, Data data)
        {
            _allData.Set(id, data);
        }
        public static Data GetData(string id)
        {
            if (!_allData.ContainsKey(id))
                throw new System.NullReferenceException("No data has been registered with the ID " + id);

            return _allData[id];
        }
    }
}
