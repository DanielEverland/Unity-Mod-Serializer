using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    /// <summary>
    /// Handles serialization of binary objects into a mod package
    /// </summary>
    public class BinarySerializer
    {
        public BinarySerializer()
        {
            _entries = new List<Entry>();
        }

        public IEnumerable<Entry> Entries { get { return _entries; } }

        private List<Entry> _entries;

        public void AddObject(object instance, byte[] data)
        {
            _entries.Add(new Entry(instance, data));
        }

        public struct Entry
        {
            public Entry(object instance, byte[] data)
            {
                this.data = data;
                this.instance = instance;
            }
            
            public object instance;
            public byte[] data;
        }
    }
}
