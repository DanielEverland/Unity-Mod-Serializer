using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.EntryWriters
{
    public abstract class EntryWriter
    {
        #region Static
        static EntryWriter()
        {
            _writers = new List<EntryWriter>();
            _isWritableCache = new Dictionary<Type, bool>();
        }

        private static List<EntryWriter> _writers;
        private static Dictionary<Type, bool> _isWritableCache;

        public static void AddWriter(EntryWriter writer)
        {
            if (!_writers.Contains(writer))
                _writers.Add(writer);
        }
        public static bool IsWritable(Type type)
        {
            if(!_isWritableCache.ContainsKey(type))
            {
                _isWritableCache.Add(type, GetWriter(type) != null);
            }

            return _isWritableCache[type];
        }
        public static EntryWriter GetWriter(Type type)
        {
            return TypeInheritanceTree.GetClosestType(_writers, type, x => x.WriterType);
        }
        #endregion

        /// <summary>
        /// Specifies which type the write can write.
        /// Note that this does work with inheritance
        /// </summary>
        public abstract Type WriterType { get; }

        /// <summary>
        /// Writes a manifest entry given an object
        /// It is guarenteed to be of type WriterTyp
        /// </summary>
        public abstract Manifest.Entry Write(object obj);
    }
}
