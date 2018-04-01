using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.EntryWriters;

namespace UMS.Reflection
{
    public static class EntryWriterLoader
    {
        [LoadTypes]
        public static void Poll(Type type)
        {
            if (typeof(EntryWriter).IsAssignableFrom(type))
            {
                if (type.IsAbstract)
                    return;

                EntryWriter.AddWriter((EntryWriter)Activator.CreateInstance(type));
            }
        }
    }
}
