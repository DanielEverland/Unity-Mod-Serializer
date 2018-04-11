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
            _keys = new Dictionary<string, List<string>>();

            Instance = this;
        }

        public static Manifest Instance { get; private set; }
        
        public IEnumerable<Entry> Entries { get { return _entries; } }
        
        [Property]
        private List<Entry> _entries;
        [Ignore]
        private Dictionary<string, List<string>> _keys;
                
        public Entry this[int index]
        {
            get
            {
                return _entries[index];
            }
        }

        public IEnumerable<string> GetKeys(string id)
        {
            if (!_keys.ContainsKey(id))
                return null;

            return _keys[id];
        }
        public void AddKey(string id, string key)
        {
            if(key != "" && key != null)
            {
                if (!_keys.ContainsKey(id))
                    _keys.Add(id, new List<string>());

                if(!_keys[id].Contains(key))
                    _keys[id].Add(key);
            }
        }
        public void AddEntry(Entry entry)
        {
            _entries.Add(entry);
        }
        
        [Serializable]
        public class Entry
        {
            private Entry() { }
            public Entry(string id, string path, IEnumerable<string> keys)
            {
                this.id = id;
                this.path = path;
                
                if(keys != null)
                {
                    List<string> keyList = new List<string>(keys.Where(x => x != "" && x != null));
                    
                    if (keyList.Count > 0)
                        this.keys = keyList;
                }
            }

            [Property]
            public string id;
            [Property]
            public string path;
            [Property]
            public List<string> keys;
            [Property]
            public Type type;
        }
    }
}
