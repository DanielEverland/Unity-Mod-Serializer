using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace UMS
{
    /// <summary>
    /// Handles ID's for objects within a mod package. There's a few things I have to point out
    /// 
    /// First of all, ID's are generated from a random int. There's absolutely no correlation
    /// between the hash of an object, a Unity objects InstanceID or it's GUID and the ID we'll
    /// be using within a mod package
    /// 
    /// Second of all, we generate items whenever we encounter a new object. We obviously cache
    /// objects and their ID's once we've determined it's new, but determining whether it is new
    /// or not is a slightly long process, since it has to work for _every_ object instance
    /// 
    /// 1) Check the cache layer for an existing ID
    /// 2) Check if IEquatable is implemented. Use Equals() if so
    /// 3) Use the deep compare function if not.
    /// 
    /// 
    /// The deep compare function works by iterating over all encountered objects, and running
    /// the following checks on the two of them
    /// 
    /// 1) Use ReferenceEquals to see if there's a shared reference
    /// 2) Check if either is null
    /// 3) Check if they're the same type
    /// 4) Iterate over their fields and properties
    /// 
    /// This process is slow during serialization-time, but that's okay to ensure performance
    /// during deserialization. 
    /// </summary>
    public static class IDManager
    {
        public static void Initialize()
        {
            _cachedIDs = new Dictionary<object, uint>();
            _allIDs = new HashSet<uint>();
        }

        private static Dictionary<object, uint> _cachedIDs;
        private static HashSet<uint> _allIDs;

        private readonly static BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static string GetID(object obj)
        {
            if (obj == null)
                throw new ArgumentException("Cannot return ID for null object");

            if (!_cachedIDs.ContainsKey(obj))
            {
                AddToCachingLayer(obj);
            }

            return _cachedIDs[obj].ToString();
        }
        private static void AddToCachingLayer(object obj)
        {
            bool exists = CheckForExistingID(obj, out uint id);
            
            if (exists)
            {
                if (id == 0)
                    throw new ArgumentException("We found an ID but it's somehow 0");

                _cachedIDs.Add(obj, id);
            }
            else
            {
                CreateNewID(obj);
            }
        }
        private static bool CheckForExistingID(object obj, out uint id)
        {
            List<object> encounteredObjects = new List<object>(_cachedIDs.Keys);
            object match = null;

            match = IEquatablePass(encounteredObjects, obj);
            if(match != null)
            {
                id = _cachedIDs[match];
                return true;
            }

            match = DeepCompare(encounteredObjects, obj);
            if (match != null)
            {
                id = _cachedIDs[match];
                return true;
            }
            
            id = 0;
            return false;
        }
        private static object IEquatablePass(IEnumerable<object> objects, object target)
        {
            foreach (object existingObject in objects)
            {
                if (existingObject == null)
                    continue;

                if (existingObject.GetType().GetInterfaces().Any(x => x.GetType() == typeof(IEquatable<>)))
                {
                    if (existingObject.Equals(target))
                    {
                        return existingObject;
                    }
                }

                if(existingObject.GetType() == target.GetType())
                {
                    Type type = existingObject.GetType();

                    if(CompareProperties(existingObject, target, type))
                    {
                        if(CompareFields(existingObject, target, type))
                        {
                            return existingObject;
                        }
                    }
                }
            }

            return null;
        }
        private static object DeepCompare(IEnumerable<object> objects, object target)
        {
            foreach (object existingObject in objects)
            {
                if(ReferenceEquals(existingObject, target))
                {
                    return existingObject;
                }
            }

            return null;
        }
        private static bool CompareProperties(object firstObject, object secondObject, Type type)
        {
            foreach (PropertyInfo property in type.GetProperties(_bindingFlags))
            {
                if (property.GetMethod == null)
                    continue;

                if (!CompareValues(firstObject, secondObject))
                    return false;
            }

            return true;
        }
        private static bool CompareFields(object firstOjbect, object secondObject, Type type)
        {
            foreach (FieldInfo field in type.GetFields(_bindingFlags))
            {
                if (!CompareValues(firstOjbect, secondObject))
                    return false;
            }

            return true;
        }
        private static bool CompareValues(object a, object b)
        {
            if(a is IEnumerable aEnumerable && b is IEnumerable bEnumerable)
            {
                return SequenceEqual(aEnumerable, bEnumerable);
            }

            return a.Equals(b);
        }
        private static bool SequenceEqual(IEnumerable a, IEnumerable b)
        {
            IEnumerator e1 = a.GetEnumerator();
            IEnumerator e2 = b.GetEnumerator();
            
            while (e1.MoveNext())
            {
                if (!(e2.MoveNext() && e1.Current.Equals(e2.Current)))
                    return false;
            }
            if (e2.MoveNext())
                return false;

            return true;
        }
        private static void CreateNewID(object obj)
        {
            uint id = Utility.GetRandomID();

            //This probably won't ever be necessary, but we need to check for collision nevertheless.
            while (_allIDs.Contains(id))
            {
                id = Utility.GetRandomID();
            }
            
            _allIDs.Add(id);
            _cachedIDs.Add(obj, id);
        }
    }
}
