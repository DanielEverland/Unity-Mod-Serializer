using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    /// <summary>
    /// Helper class used to determine the inheritance level of types.
    /// Useful when trying to find Converters or EntryWriters,
    /// that are as close to a given type in the inheritance tree as possible
    /// </summary>
    public static class TypeInheritanceTree
    {
        private static Dictionary<Type, int> _cachedTreeIndexes = new Dictionary<Type, int>();

        /// <summary>
        /// Calculates the closest type in an inheritance tree to a target type
        /// </summary>
        public static Type GetClosestType(IEnumerable<Type> collection, Type targetType)
        {
            Type closestType = null;

            foreach (Type type in collection)
            {
                Process(ref closestType, type, targetType);
            }

            return closestType;
        }
        
        /// <summary>
        /// Calculates the closest type in an inheritance tree to a target type
        /// </summary>
        /// <typeparam name="T">Type of the collection elements</typeparam>
        /// <param name="typeConverter">Delegate that converts <typeparamref name="T"/> to System.Type</param>
        /// <returns></returns>
        public static T GetClosestType<T>(IEnumerable<T> collection, Type targetType, Func<T, Type> typeConverter)
        {
            T closestObject = default(T);

            foreach (T obj in collection)
            {
                Type objType = typeConverter(obj);
                Type currentType = closestObject != null ? typeConverter(closestObject) : null;

                Process(ref currentType, objType, targetType);

                if (currentType == objType)
                    closestObject = obj;
            }
            
            return closestObject;
        }

        /// <summary>
        /// Core function used to process types
        /// </summary>
        private static void Process(ref Type closestType, Type currentType, Type targetType)
        {
            if(InheritsType(targetType, currentType))
            {
                int currentIndex = GetInheritanceIndex(closestType);
                int potentialIndex = GetInheritanceIndex(currentType);
                
                if (potentialIndex > currentIndex)
                    closestType = currentType;
            }
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> inherits <paramref name="derivedType"/>
        /// </summary>
        public static bool InheritsType(Type type, Type derivedType)
        {
            if (derivedType == null)
                return false;

            return derivedType.IsAssignableFrom(type);
        }

        /// <summary>
        /// The inheritance index is the amount of types we have to navigate through in order to get to object
        /// </summary>
        public static int GetInheritanceIndex(Type type)
        {
            if (type == null)
                return -1;

            if (_cachedTreeIndexes.ContainsKey(type))
                return _cachedTreeIndexes[type];

            int index = CalculateInheritanceTreeIndex(type);

            _cachedTreeIndexes.Add(type, index);

            return index;
        }

        private static int CalculateInheritanceTreeIndex(Type type)
        {
            int index = 0;
            Type baseType = type;

            while (baseType != null)
            {
                index++;
                baseType = baseType.BaseType;
            }

            return index;
        }
    }
}
