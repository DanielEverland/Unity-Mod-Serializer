using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    [Serializable]
    public class Manifest
    {
        public Manifest()
        {
            _entries = new List<Entry>();
        }
        
        public IEnumerable<Entry> Entries { get { return _entries; } }
        
        [Property]
        private List<Entry> _entries;

        public void Add(string guid, string path, Type type)
        {
            _entries.Add(new Entry(guid, path, type));
        }
        public void Add(string guid, string path, Type type, string key)
        {
            _entries.Add(new Entry(guid, path, type, key));
        }

        public Entry this[int index]
        {
            get
            {
                return _entries[index];
            }
        }
        
        [Serializable]
        public struct Entry
        {
            public Entry(string guid, string path, Type type, string key)
            {
                this.guid = guid;
                this.path = path;
                this.type = type;
                this.key = key;
            }
            public Entry(string guid, string path, Type type)
            {
                this.guid = guid;
                this.path = path;
                this.type = type;

                key = null;
            }

            [Property]
            public string guid;
            [Property]
            public string path;
            [Property]
            public Type type;
            [Property]
            public string key;
        }
    }
}
