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
        public static bool IsWritable(Type type)
        {
            return GetWriter(type) != null;
        }
        public static EntryWriter GetWriter(Type type)
        {
            return TypeInheritanceTree.GetClosestType(EntryWriterRegistrar.Converters, type, x => x.WriterType);
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
