﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS
{
    /// <summary>
    /// Contains deserialized objects
    /// </summary>
    public static class ObjectHandler
    {
        public static bool UseObjectHandler
        {
            get
            {
                if (!Application.isPlaying)
                    return false;

                return _useObjectHandler;
            }
            set
            {
                _useObjectHandler = value;
            }
        }
        private static bool _useObjectHandler = true;

        private static List<Object> _allObjects;
        private static Dictionary<string, Object> _keyLookup;
        private static GameObject _objectHandler;

        public static void Initialize()
        {
            _allObjects = new List<Object>();
            _keyLookup = new Dictionary<string, Object>();

            if (UseObjectHandler)
            {
                _objectHandler = new GameObject("Object Handler");
                _objectHandler.SetActive(false);
            }            
        }
        public static void InstantiateAllObjects()
        {
            foreach (Object obj in _keyLookup.Values)
            {
                GameObject instantiate = (GameObject)GameObject.Instantiate(obj);
                instantiate.name = obj.name;
            }
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
            return _allObjects.Where(x => x.GetType() == typeof(T)).Select(x => x as T);
        }
        /// <summary>
        /// Returns all objects which can be cast to <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T> GetObjectsOfType<T> () where T : Object
        {
            return _allObjects.Where(x => typeof(T).IsAssignableFrom(x.GetType())).Select(x => x as T);
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
            SetAsChild(obj);

            if(key != string.Empty && key != null)
            {
                _keyLookup.Set(key, obj);
            }

            _allObjects.Add(obj);
        }
        private static void SetAsChild(Object obj)
        {
            if (!UseObjectHandler)
                return;

            if(obj is GameObject gameObject)
            {
                gameObject.hideFlags = HideFlags.None;
                gameObject.transform.SetParent(_objectHandler.transform);
            }
        }
    }
}
