using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Reflection
{
    /// <summary>
    /// Manager that determines which types can be written
    /// into separate entries, thereby allowing them to support
    /// referencing
    /// </summary>
    public static class ReferenceManager
    {
        static ReferenceManager()
        {
            _referencableTypes = new List<Type>();
        }

        private static List<Type> _referencableTypes;
        
        /// <summary>
        /// Cache that contains info regarding whether a type can be referenced
        /// </summary>
        private static Dictionary<Type, bool> _cachedTypes;

        public static void AddType(Type type)
        {
            if (!_referencableTypes.Contains(type))
                _referencableTypes.Add(type);
        }
        public static bool SupportsReferencing(Type type)
        {
            if(!_cachedTypes.ContainsKey(type))
            {
                foreach (Type referencableType in _referencableTypes)
                {
                    if (referencableType.IsAssignableFrom(type))
                    {
                        _cachedTypes.Add(type, true);
                        return true;
                    }
                }

                _cachedTypes.Add(type, false);
            }

            return _cachedTypes[type];
        }
    }
}
