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
        private static Dictionary<object, uint> _cachedIDs;
        private static HashSet<uint> _allIDs;

        private readonly static BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static void Initialize()
        {
            _cachedIDs = new Dictionary<object, uint>();
            _allIDs = new HashSet<uint>();
        }
        public static bool CanGetID(Type type)
        {
            return ReferenceManager.SupportsReferencing(type);
        }
        public static string GetID(object obj)
        {
            if (obj == null)
                throw new ArgumentException("Cannot return ID for null object");

            if (!CanGetID(obj.GetType()))
                throw new ArgumentException("Object " + obj + " isn't allowed to get an ID");

            if (!_cachedIDs.ContainsKey(obj))
            {
                Debugging.Info(DebuggingFlags.IDManager, "_____________Requesting ID for " + obj + "_____________");

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

                Debugging.Info(DebuggingFlags.IDManager, "Found existing ID " + id + " for " + obj);

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
            if (match != null)
            {
                Debugging.Verbose(DebuggingFlags.IDManager, "IEquatable pass matched for " + obj);

                id = _cachedIDs[match];
                return true;
            }

            match = DeepCompare(encounteredObjects, obj);
            if (match != null)
            {
                Debugging.Verbose(DebuggingFlags.IDManager, "Deep compare pass matched for " + obj);

                id = _cachedIDs[match];
                return true;
            }

            id = 0;
            return false;
        }
        private static object IEquatablePass(IEnumerable<object> objects, object target)
        {
            Debugging.Info(DebuggingFlags.IDManagerIEquatableComparer, "--------IEQUATABLE COMPARISON FOR " + target + "--------");

            foreach (object existingObject in objects)
            {
                Debugging.Info(DebuggingFlags.IDManagerIEquatableComparer, "Comparing to " + existingObject);

                if (existingObject == null)
                {
                    Debugging.Info(DebuggingFlags.IDManagerIEquatableComparer, "Quitting due to null");
                    continue;
                }

                if (existingObject.GetType().GetInterfaces().Any(x => x.GetType() == typeof(IEquatable<>)))
                {
                    Debugging.Verbose(DebuggingFlags.IDManagerIEquatableComparer, "Implements IEquatable");

                    if (existingObject.Equals(target))
                    {
                        Debugging.Info(DebuggingFlags.IDManagerIEquatableComparer, "Equals() call is true");
                        Debugging.Info(DebuggingFlags.IDManagerIEquatableComparer, "Returning");

                        return existingObject;
                    }
                    else
                    {
                        Debugging.Verbose(DebuggingFlags.IDManagerIEquatableComparer, "Equals() call is false");
                    }
                }
            }

            Debugging.Info(DebuggingFlags.IDManagerIEquatableComparer, "--------IEQUATABLE END--------");

            return null;
        }
        private static object DeepCompare(IEnumerable<object> objects, object target)
        {
            Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "Running deep compare for " + target + " (" + objects.Count() + ")");

            foreach (object existingObject in objects)
            {
                if (existingObject == null)
                {
                    Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "Quitting due to null");
                    continue;
                }

                if (ReferenceEquals(existingObject, target))
                {
                    Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "Reference equals true");
                    Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "Returns");
                    return existingObject;
                }

                if (existingObject.GetType() == target.GetType())
                {
                    Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "Matching types");

                    Type type = existingObject.GetType();

                    if (CompareProperties(existingObject, target, type))
                    {
                        Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "Property compare succeeded");

                        if (CompareFields(existingObject, target, type))
                        {
                            Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "Field compare succeeded");
                            Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "Returning");

                            return existingObject;
                        }
                        else
                        {
                            Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "Field compare failed");
                        }
                    }
                    else
                    {
                        Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "Property compare failed");
                    }
                }
            }

            return null;
        }
        private static bool CompareProperties(object firstObject, object secondObject, Type type)
        {
            Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "///Comparing " + firstObject + " to " + secondObject + "///");

            foreach (PropertyInfo property in type.GetProperties(_bindingFlags))
            {
                Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "Comparing " + property);

                if (property.GetMethod == null)
                {
                    Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "No getter for " + property + ". Returning");
                    continue;
                }

                if (!CompareValues(property.GetValue(firstObject), property.GetValue(secondObject)))
                {
                    Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "///Returning False Due To " + property + "///\n" + property.GetValue(firstObject) + ", " + property.GetValue(secondObject));

                    return false;
                }
            }

            Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "///Returning True///");

            return true;
        }
        private static bool CompareFields(object firstObject, object secondObject, Type type)
        {
            Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "//Comparing " + firstObject + " to " + secondObject + "//");

            foreach (FieldInfo field in type.GetFields(_bindingFlags))
            {
                Debugging.Verbose(DebuggingFlags.IDManagerDeepComparer, "Comparing " + field);

                if (!CompareValues(field.GetValue(firstObject), field.GetValue(secondObject)))
                {
                    Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "//Returning False Due To " + field + "//\n" + field.GetValue(firstObject) + ", " + field.GetValue(secondObject));

                    return false;
                }
            }

            Debugging.Info(DebuggingFlags.IDManagerDeepComparer, "///Returning True///");

            return true;
        }
        private static bool CompareValues(object a, object b)
        {
            if (a == null && b == null)
                return true;

            //This is valid since we just checked if both are null. If either of them are null now, the other one is not
            if (a == null || b == null)
                return false;

            if (a is IEnumerable aEnumerable && b is IEnumerable bEnumerable)
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
            uint id = Utility.GetRandomUnsignedInt();

            //This probably won't ever be necessary, but we need to check for collision nevertheless.
            while (_allIDs.Contains(id))
            {
                id = Utility.GetRandomUnsignedInt();
            }

            Debugging.Info(DebuggingFlags.IDManager, "Creating new ID " + id + " for " + obj);

            _allIDs.Add(id);
            _cachedIDs.Add(obj, id);
        }
    }
}
