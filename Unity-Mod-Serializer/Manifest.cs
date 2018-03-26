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
            _keys = new Dictionary<string, string>();

            Instance = this;
        }

        public static Manifest Instance { get; private set; }
        
        public IEnumerable<Entry> Entries { get { return _entries; } }
        
        [Property]
        private List<Entry> _entries;
        [Ignore]
        private Dictionary<string, string> _keys;
                
        public Entry this[int index]
        {
            get
            {
                return _entries[index];
            }
        }

        public string GetKey(string id)
        {
            if (!_keys.ContainsKey(id))
                return null;

            return _keys[id];
        }
        public void AddKey(string id, string key)
        {
            _keys.Set(id, key);
        }
        public void AddEntry(Entry entry)
        {
            _entries.Add(entry);
        }
        
        [Serializable]
        public struct Entry
        {
            [Property]
            public string id;
            [Property]
            public string path;
            [Property]
            public string key;
        }
    }
}
