using System;
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

        [Property]
        private List<Entry> _entries;

        public void Add(string guid, string path)
        {
            _entries.Add(new Entry(guid, path));
        }
        public void Add(string guid, string path, string key)
        {
            _entries.Add(new Entry(guid, path, key));
        }

        [Serializable]
        private struct Entry
        {
            public Entry(string guid, string path, string key)
            {
                this.guid = guid;
                this.path = path;
                this.key = key;
            }
            public Entry(string guid, string path)
            {
                this.guid = guid;
                this.path = path;

                key = null;
            }

            [Property]
            public string guid;
            [Property]
            public string path;
            [Property]
            public string key;
        }
    }
}
